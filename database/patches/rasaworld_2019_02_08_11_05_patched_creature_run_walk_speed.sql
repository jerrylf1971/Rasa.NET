UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=1;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=2;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=3;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=4;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=5;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=6;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=7;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=8;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=10;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=11;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=12;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=13;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=14;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=15;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=16;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=17;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=18;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=19;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=20;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=21;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=22;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=23;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=24;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=25;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=26;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=27;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=28;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=29;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=100;
UPDATE `rasaworld`.`creatures` SET `run_speed`='9' WHERE  `dbId`=101;

UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=1;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=2;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=3;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=4;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=5;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=6;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=7;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=8;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=10;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=11;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=12;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=13;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=14;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=15;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=16;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=17;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=18;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=19;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=20;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=21;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=22;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=23;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=24;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=25;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=26;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=27;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=28;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=29;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=100;
UPDATE `rasaworld`.`creatures` SET `walk_speed`='4.5' WHERE  `dbId`=101;

INSERT INTO `rasaworld`.`creatures` (`dbId`, `comment`, `classId`, `faction`, `level`, `maxHitPoints`, `nameId`, `action1`) VALUES ('9', 'AFS_Turret_Mini', '11302', '1', '4', '1500', '0', '8');

INSERT INTO `rasaworld`.`spawnpool` (`dbId`, `respawnTime`, `posX`, `posY`, `posZ`, `orientation`, `contextId`, `creatureId1`, `creatureMinCount1`, `creatureMaxCount1`) VALUES ('9', '500', '851.688', '295.422', '396.75', '0.28', '1220', '9', '1', '1');

INSERT INTO `rasaworld`.`applied_patches` (`patch_name`) VALUES ('rasaworld_2019_02_08_11_05_patched_creature_run_walk_speed');