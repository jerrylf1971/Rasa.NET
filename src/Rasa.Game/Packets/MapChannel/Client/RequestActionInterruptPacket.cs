﻿namespace Rasa.Packets.MapChannel.Client
{
    using Data;
    using Memory;

    public class RequestActionInterruptPacket : PythonPacket
    {
        public override GameOpcode Opcode { get; } = GameOpcode.RequestActionInterrupt;

        public ActionId ActionId { get; set; }
        public int ActionArgId { get; set; }

        public override void Read(PythonReader pr)
        {
            pr.ReadTuple();
            ActionId = (ActionId)pr.ReadInt();
            ActionArgId = pr.ReadInt();
        }

        public override void Write(PythonWriter pw)
        {
        }
    }
}

