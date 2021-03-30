﻿using System;

namespace Rasa.Packets.Game.Both
{
    using Data;
    using Memory;

    public class SetDesiredCrouchStatePacket : PythonPacket
    {
        public override GameOpcode Opcode { get; } = GameOpcode.SetDesiredCrouchState;

        public CharacterState DesiredCrouchState { get; set; }

        public SetDesiredCrouchStatePacket()
        {
        }

        public SetDesiredCrouchStatePacket(CharacterState desiredCrouchState)
        {
            if (desiredCrouchState != CharacterState.Standing || desiredCrouchState != CharacterState.Crouched)
                throw new ArgumentOutOfRangeException(nameof(desiredCrouchState));

            DesiredCrouchState = desiredCrouchState;
        }

        public override void Read(PythonReader pr)
        {
            pr.ReadTuple();
            DesiredCrouchState = (CharacterState)pr.ReadUInt();
        }

        public override void Write(PythonWriter pw)
        {
            pw.WriteTuple(1);
            pw.WriteUInt((uint)DesiredCrouchState);
        }
    }
}