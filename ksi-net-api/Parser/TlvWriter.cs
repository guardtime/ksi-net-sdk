﻿using System;
using System.IO;
using System.Text;

namespace Guardtime.KSI.Parser
{
    public class TlvWriter : BinaryWriter
    {
        public TlvWriter(Stream input) : base(input)
        {
        }

        public TlvWriter(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        public void WriteTag(ITlvTag tag) {
            var data = tag.EncodeValue();
            var tlv16 = tag.Type > TlvReader.TypeMask
                    || (data != null && data.Length > byte.MaxValue);
            var firstByte = (byte) ((tlv16 ? TlvReader.Tlv16Flag : 0)
                                     + (tag.NonCritical ? TlvReader.NonCriticalFlag : 0)
                                     + (tag.Forward ? TlvReader.ForwardFlag : 0));
            
            if (tlv16) {
                firstByte = (byte) (firstByte
                                    | (tag.Type >> TlvReader.ByteBits) & TlvReader.TypeMask);
                Write(firstByte);
                Write((byte)tag.Type);
                if (data == null) {
                    Write(0);
                } else {
                    // TODO: If data is longer than 13 bits then throw exception
                    Write((ushort)data.Length);
                    Write(data);
                }
            } else {
                firstByte = (byte) (firstByte | tag.Type & TlvReader.TypeMask);
                Write(firstByte);
                if (data == null) {
                    Write(0);
                } else {
                    Write((byte)data.Length);
                    Write(data);
                }
            }
        }
    }
}
