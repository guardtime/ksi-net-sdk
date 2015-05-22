﻿using System.Text;

namespace Guardtime.KSI.Parser
{
    public class IntegerTag : TlvTag
    {
        public ulong Value;

        public IntegerTag(byte[] bytes) : base(bytes)
        {
            DecodeValue(ValueBytes);
        }

        public IntegerTag(TlvTag tag) : base(tag)
        {
            DecodeValue(tag.EncodeValue());
        }

        public IntegerTag(uint type, bool nonCritical, bool forward, ulong value)
            : base(type, nonCritical, forward, Util.Util.EncodeUnsignedLong(value))
        {
            Value = value;
        }

        private void DecodeValue(byte[] bytes)
        {
            Value = Util.Util.DecodeUnsignedLong(bytes, 0, bytes.Length);
        }

        public override byte[] EncodeValue()
        {
            return Util.Util.EncodeUnsignedLong(Value);
        }

        public override bool Equals(object obj)
        {
            var tag = obj as IntegerTag;
            if (tag == null)
            {
                return false;
            }

            return tag.Type == Type &&
                   tag.Forward == Forward &&
                   tag.NonCritical == NonCritical &&
                   tag.Value == Value;
        }

        public sealed override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("TLV[0x").Append(Type.ToString("X"));

            if (NonCritical)
            {
                builder.Append(",N");
            }

            if (Forward)
            {
                builder.Append(",F");
            }

            builder.Append("]:");
            builder.Append("i").Append(Value);
            return builder.ToString();
        }

    }

}
