﻿namespace Rasa.Packets.MapChannel.Client
{
    using Data;
    using Memory;

    public class RequestWeaponAttackPacket : PythonPacket
    {
        public override GameOpcode Opcode { get; } = GameOpcode.RequestWeaponAttack;

        public int ActionId { get; set; }
        public int ActionArgId { get; set; }
        public long TargetId { get; set; }
        public bool IsAltAction { get; set; }       // ToDo

        public override void Read(PythonReader pr)
        {
            Logger.WriteLog(LogType.Debug, $"RequestWeaponAttack:\n {pr.ToString()}");
            pr.ReadTuple();
            ActionId = pr.ReadInt();
            ActionArgId = pr.ReadInt();
            if (pr.PeekType() == PythonType.Long)   // has target
                TargetId = pr.ReadLong();
            else
                pr.ReadNoneStruct();                // no target
            pr.ReadZeroStruct();
        }

        public override void Write(PythonWriter pw)
        {
        }
    }
}
