﻿using System;
using System.Collections.Generic;

namespace Rasa.Managers
{
    using Data;
    using Database.Tables.Character;
    using Game;
    using Packets.MapChannel.Server;
    using Structures;
    using Timer;

    public class MapChannelManager
    {

        private static MapChannelManager _instance;
        private static readonly object InstanceLock = new object();
        public const int MapChannel_PlayerQueue = 32;
        private readonly Dictionary<int,MapChannel> MapChannelsByContextId = new Dictionary<int, MapChannel>();     // list of maps that need to be loaded
        public readonly Dictionary<int, MapChannel> MapChannelArray = new Dictionary<int, MapChannel>();           // list of loaded maps
        public readonly Timer Timer = new Timer();

        public static MapChannelManager Instance
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                            _instance = new MapChannelManager();
                    }
                }

                return _instance;
            }
        }

        private MapChannelManager()
        {
        }

        public void CharacterLogout(Client client)
        {
            if (client.MapClient.LogoutActive == false)
                return;
            client.MapClient.RemoveFromMap = true;
        }

        public MapChannelClient CreateNewMapClient(MapChannel mapChannel, Client client)
        {
            var mapClient = new MapChannelClient();
            mapClient.Client = client;
            mapClient.MapChannel = mapChannel;
            // register mapChannelClient
            EntityManager.Instance.RegisterEntity(mapClient.ClientEntityId, EntityType.MapClient);
            EntityManager.Instance.RegisterMapClient(mapClient.ClientEntityId, mapClient);
            return mapClient;
        }

        public void CreatePlayerCharacter(MapChannelClient mapClient)
        {
            var data = CharacterTable.GetCharacterData(mapClient.Client.Entry.Id, mapClient.Client.LoadingSlot);
            var tempAppearanceData = new Dictionary<int, AppearanceData>();
            var appearance = CharacterAppearanceTable.GetAppearance(data.CharacterId);
            foreach (var t in appearance)
                tempAppearanceData.Add(t.SlotId, new AppearanceData { SlotId = t.SlotId, ClassId = t.ClassId, Color = new Color(t.Color) });

            var player = new PlayerData
            {
                Actor = new Actor
                {
                    EntityClassId = data.Gender == 0 ? (uint)692 : 691,
                    Name = data.Name,
                    FamilyName = data.FamilyName,
                    Position = new Position
                    {
                        PosX = data.PosX,
                        PosY = data.PosY,
                        PosZ = data.PosZ
                    },
                    Rotation = data.Rotation,
                    MapContextId = data.MapContextId,
                    IsRunning = true,
                    InCombatMode = false,
                    Stats = new ActorStats(),
                },
                ControllerUser = mapClient,
                AppearanceData = tempAppearanceData,
                CharacterId = data.CharacterId,
                AccountId = data.AccountId,
                SlotId = data.SlotId,
                Gender = data.Gender,
                Scale = data.Scale,
                RaceId = data.RaceId,
                ClassId = data.ClassId,
                Experience = data.Experience,
                Level = data.Level,
                Body = data.Body,
                Mind = data.Mind,
                Spirit = data.Spirit,
                CloneCredits = data.CloneCredits,
                NumLogins = data.NumLogins + 1,
                TotalTimePlayed = data.TotalTimePlayed,
                TimeSinceLastPlayed = data.TimeSinceLastPlayed,
                ClanId = data.ClanId,
                ClanName = data.ClanName,
                Credits = data.Credits,
                Prestige = data.Prestige,
                Skills = GetPlayerSkills(data.CharacterId),
                Titles = CharacterTitlesTable.GetCharacterTitles(data.CharacterId),
                Abilities = GetPlayerAbilities(data.CharacterId),
                CurrentAbilityDrawer = data.CurrentAbilityDrawer,
                MissionStateCount = 0,
                // MissionStateData = new CharacterMissionData(),
                LoginTime = 0,
                Logos = CharacterLogosTable.GetLogos(data.CharacterId)
            };
            // register new Player
            EntityManager.Instance.RegisterEntity(player.Actor.EntityId, EntityType.Player);
            EntityManager.Instance.RegisterActor(player.Actor.EntityId, player.Actor);
            mapClient.Player = player;
            mapClient.Player.Client = mapClient.Client;
            PlayerManager.Instance.UpdateStatsValues(mapClient, true);
            CommunicatorManager.Instance.LoginOk(mapClient, mapClient.MapChannel);
            CellManager.Instance.AddToWorld(mapClient); // will introduce the player to all clients, including the current owner
            PlayerManager.Instance.AssignPlayer(mapClient);
        }
        
        public MapChannel FindByContextId(int contextId)
        {
            return MapChannelArray[contextId];
        }

        public MapChannelClient GetMapClientFromMapChannel(Client client)
        {
            var mapChannel = FindByContextId(client.LoadingMap);
            foreach (var mapClient in mapChannel.PlayerList)
                if (mapClient.Client == client)
                    return mapClient;

            return null;
        }

        public Dictionary<int, AbilityDrawerData> GetPlayerAbilities(uint characterId)
        {
            var abilities = new Dictionary<int, AbilityDrawerData>();
            var abilitiesData = CharacterAbilityDrawerTable.GetCharacterAbilities(characterId);
            for (var i = 0; i < 25; i++)
            {
                if (abilitiesData[i * 3 + 1] > 0)   // don't insert if there is no ablility in slot
                    abilities.Add(abilitiesData[i * 3], new AbilityDrawerData { AbilitySlotId = abilitiesData[i * 3],  AbilityId = abilitiesData[i * 3 + 1], AbilityLevel = abilitiesData[i * 3 + 2] });
            }
            return abilities;
        }

        public Dictionary<int, SkillsData> GetPlayerSkills(uint characterId)
        {
            var skills = new Dictionary<int, SkillsData>();
            var skillsData = CharacterSkillsTable.GetCharacterSkills(characterId);

            foreach (var skill in skillsData)
                skills.Add(skill.SkillId, new SkillsData { SkillId = skill.SkillId, AbilityId = skill.AbilityId, SkillLevel = skill.SkillLevel});

            return skills;
        }

        public void MapChannelInit()
        {
            MapChannelsByContextId.Add(1220, new MapChannel { MapInfo = new MapInfo { BaseRegionId = 0, MapId = 1220, MapName = "adv_foreas_concordia_wilderness", MapVersion = 1556 } });
            MapChannelsByContextId.Add(1148, new MapChannel { MapInfo = new MapInfo { BaseRegionId = 10, MapId = 1148, MapName = "adv_foreas_concordia_divide", MapVersion = 1584 } });
            
            foreach (var t in MapChannelsByContextId)
            {  
                var id = t.Key;
                // load all maps
                var newMapChannel = new MapChannel
                {
                    MapInfo = MapChannelsByContextId[id].MapInfo,
                    SocketToClient = new Dictionary<int, MapChannelClient>(),
                    //TimerClientEffectUpdate = Environment.TickCount,
                    //TimerMissileUpdate = Environment.TickCount,
                    //TimerDynObjUpdate = Environment.TickCount,
                    //TimerGeneralTimer = Environment.TickCount,
                    //TimerController = Environment.TickCount,
                    //TimerPlayerUpdate = Environment.TickCount,
                    //PlayerCount = 0,
                    PlayerLimit = 128,
                    PlayerList = new List<MapChannelClient>()
                };
                // register mapChannel
                MapChannelArray.Add(id, newMapChannel);
            }

            foreach (var t in MapChannelArray)
            {
                var mapChannel = t.Value;
                //navmesh_initForMapChannel(mapChannel);
                //dynamicObject_init(mapChannel);
                //mission_initForChannel(mapChannel);
                //missile_initForMapchannel(mapChannel);
                //spawnPool_initForMapChannel(mapChannel);
                //controller_initForMapChannel(mapChannel);
                //teleporter_initForMapChannel(mapChannel); //---load teleporters
                //logos_initForMapChannel(mapChannel); // logos world objects
                Console.WriteLine("Map: {0} loaded", mapChannel.MapInfo.MapName);
            }

            Console.WriteLine("\nMapChannels Started...");

            Timer.Add("CheckForLogingClients", 1000, true, null);
            Timer.Add("PerformAbilities", 500, true, null);
            Timer.Add("ClientEffectUpdate", 500, true, null);
            Timer.Add("CellUpdateVisibility", 300, true, null);
        }

        public void MapChannelWorker(long delta)
        {
            Timer.Update(delta);

            foreach (var t in MapChannelArray)
            {
                var mapChannel = t.Value;
                mapChannel.MapChannelElapsed += delta;
                if (Timer.IsTriggered("CheckForLogingClients"))
                    if (mapChannel.QueuedClients.Count > 0)
                    {
                        // create new player
                        var newMapCLient = CreateNewMapClient(mapChannel, mapChannel.QueuedClients.Dequeue());
                        // add it to list, but dont increase list count yet
                        mapChannel.PlayerList.Add(newMapCLient);
                    }

                if (mapChannel.PlayerList.Count > 0)
                {
                    ActorActionManager.Instance.DoWork(delta);

                    // CellManager worker
                    if (Timer.IsTriggered("CellUpdateVisibility"))
                        CellManager.Instance.DoWork(mapChannel);
                    
                    // check for abilities
                    if (Timer.IsTriggered("PerformAbilities"))
                        if (mapChannel.QueuedPerformAbilities.Count > 0)
                            PlayerManager.Instance.PerformAbilitie(mapChannel, mapChannel.QueuedPerformAbilities.Dequeue());
                    
                    // check for effects (buffs)
                    if (Timer.IsTriggered("ClientEffectUpdate"))
                        GameEffectManager.Instance.DoWork(mapChannel, delta);

                    // chack for player LogOut
                    foreach (var mapClient in mapChannel.PlayerList)
                        if (mapClient != null)
                            if (mapClient.RemoveFromMap == true)
                            {
                                RemovePlayer(mapClient, true);
                                break;
                            }

                    // ToDo check for timers
                    //missile_check(mapChannel, 100);
                    // do other work

                    // check timers

                    //gameEffect_checkForPlayers(mapChannel->playerList, mapChannel->playerCount, 500);
                    //Debugger.Break();
                    //if (Timer.IsTriggered("MissileCheck"))
                    //missile_check(mapChannel, 100);
                    /*if (currentTime - mapChannel.TimerMissileUpdate >= 100)
                    {
                        //missile_check(mapChannel, 100);
                        mapChannel.TimerMissileUpdate += 100;
                    }
                    if (currentTime - mapChannel.TimerDynObjUpdate >= 100)
                    {
                        //dynamicObject_check(mapChannel, 100);
                        mapChannel.TimerDynObjUpdate += 100;
                    }
                    if (currentTime - mapChannel.TimerController >= 250)
                    {
                        //mapteleporter_checkForEntityInRange(mapChannel);
                        //controller_mapChannelThink(mapChannel);
                        mapChannel.TimerController += 250;
                    }
                    if ((currentTime - mapChannel.TimerPlayerUpdate) >= 1000)
                    {
                        var playerUpdateTick = currentTime - mapChannel.TimerPlayerUpdate;
                        mapChannel.TimerPlayerUpdate = currentTime;
                        for (var i = 0; i < mapChannel.PlayerCount; i++)
                        {
                            //manifestation_updatePlayer(mapChannel->playerList[i], playerUpdateTick);
                        }
                    }*/
                    /*if (currentTime - mapChannel.TimerGeneralTimer >= 100)
                    {
                        var timePassed = 100;
                        // queue for deleting map timers
                        std::vector<mapChannelTimer_t*> queue_timerDeletion;
                        // parse through all timers
                        mapChannel_check_AutoFireTimers(mapChannel);
                        std::vector<mapChannelTimer_t*>::iterator timer = mapChannel->timerList.begin();
                        while (timer != mapChannel->timerList.end())
                        {
                            (*timer)->timeLeft -= timePassed;
                            if ((*timer)->timeLeft <= 0)
                            {
                                sint32 objTimePassed = (*timer)->period - (*timer)->timeLeft;
                                (*timer)->timeLeft += (*timer)->period;
                                // trigger object
                                bool remove = (*timer)->cb(mapChannel, (*timer)->param, objTimePassed);
                                if (remove == false)
                                    queue_timerDeletion.push_back(*timer);
                            }
                            timer++;
                        }
                        // parse deletion queue
                        if (queue_timerDeletion.empty() != true)
                        {
                            mapChannelTimer_t** timerList = &queue_timerDeletion[0];
                            sint32 timerCount = queue_timerDeletion.size();
                            for (sint32 f = 0; f < timerCount; f++)
                            {
                                mapChannelTimer_t* toBeDeletedTimer = timerList[f];
                                // remove from timer list
                                std::vector<mapChannelTimer_t*>::iterator itr = mapChannel->timerList.begin();
                                while (itr != mapChannel->timerList.end())
                                {
                                    if ((*itr) == toBeDeletedTimer)
                                    {
                                        mapChannel.TimerList.erase(itr);
                                        break;
                                    }
                                    ++itr;
                                }
                            }
                        }
                        mapChannel.TimerGeneralTimer += 100;
                    }*/
                }
            }
        }

        public void MapLoaded(Client client)
        {
            var mapClient = GetMapClientFromMapChannel(client);
            client.MapClient = mapClient;

            CreatePlayerCharacter(mapClient);
            CommunicatorManager.Instance.RegisterPlayer(mapClient);
            CommunicatorManager.Instance.PlayerEnterMap(mapClient);
            InventoryManager.Instance.InitForClient(mapClient);
            //mission_initForClient(cm);
        }

        public void PassClientToMapChannel(Client client, MapChannel mapChannel)
        {
            mapChannel.QueuedClients.Enqueue(client);
        }

        public void PassClientToCharacterSelection(Client client)
        {
            // ToDo
            /*if (ClientsGameMainCount >= MAX_GAMEMAIN_CLIENTS)
            {
                // force disconnect
                closesocket(cgm->socket);
                //free(cgm);
                return;
            }*/
            CharacterManager.Instance.StartCharacterSelection(client);
            //Increase count and return struct
            //ClientsGameMainCount++;
        }

        public void Ping(Client client)
        {
            // ToDo
        }

        public void RemovePlayer(MapChannelClient mapClient, bool logout)
        {
            // unregister Communicator
            CommunicatorManager.Instance.PlayerExitMap(mapClient);
            // unregister mapChannelClient
            EntityManager.Instance.UnregisterEntity(mapClient.ClientEntityId);
            EntityManager.Instance.UnregisterMapClient(mapClient.ClientEntityId);
            // unregister Actor
            EntityManager.Instance.UnregisterEntity(mapClient.Player.Actor.EntityId);
            EntityManager.Instance.UnregisterActor(mapClient.Player.Actor.EntityId);
            // unregister character Inventory
            for (var i = 0; i < 250; i++)
                if (mapClient.Inventory.PersonalInventory[i] != 0)
                {
                    EntityManager.Instance.DestroyPhysicalEntity(mapClient, mapClient.Inventory.PersonalInventory[i], EntityType.Item);
                    mapClient.Inventory.PersonalInventory[i] = 0;
                }

            for (var i = 0; i < 22; i++)
                if (mapClient.Inventory.EquippedInventory[i] != 0)
                {
                    EntityManager.Instance.DestroyPhysicalEntity(mapClient, mapClient.Inventory.EquippedInventory[i], EntityType.Item);
                    mapClient.Inventory.EquippedInventory[i] = 0;
                }

            for (var i = 0; i < 5; i++)
                if (mapClient.Inventory.WeaponDrawer[i] != 0)
                {
                    EntityManager.Instance.DestroyPhysicalEntity(mapClient, mapClient.Inventory.WeaponDrawer[i], EntityType.Item);
                    mapClient.Inventory.WeaponDrawer[i] = 0;
                }

            // unregister from chat
            CommunicatorManager.Instance.UnregisterPlayer(mapClient);
            CellManager.Instance.RemoveFromWorld(mapClient);
            PlayerManager.Instance.RemovePlayerCharacter(mapClient.MapChannel, mapClient);
            if (logout)
                if (mapClient.Disconected == false)
                    PassClientToCharacterSelection(mapClient.Client);
            // remove from list
            for (var i = 0; i < mapClient.MapChannel.PlayerList.Count; i++)
            {
                if (mapClient == mapClient.MapChannel.PlayerList[i])
                {
                    mapClient.MapChannel.PlayerList.RemoveAt(i);
                    //mapClient.MapChannel.PlayerCount--;
                    break;
                }
            }
        
        }

        public void RequestLogout(Client client)
        {           
            client.SendPacket(5, new LogoutTimeRemainingPacket());
            client.MapClient.LogoutRequestedLast = Environment.TickCount;
            client.MapClient.LogoutActive = true;
        }
    }
}