﻿using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Utils
{
    public class InvalidEncodeTlvTag : TlvTag
    {
        public InvalidEncodeTlvTag(uint type, bool nonCritical, bool forward) : base(type, nonCritical, forward)
        {
        }

        public InvalidEncodeTlvTag(ITlvTag tag) : base(tag)
        {
        }

        public override byte[] EncodeValue()
        {
            return null;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}