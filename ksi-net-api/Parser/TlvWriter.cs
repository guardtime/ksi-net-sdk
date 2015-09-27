﻿using System;
using System.IO;

namespace Guardtime.KSI.Parser
{
    /// <summary>
    ///     TLV object writer for stream.
    /// </summary>
    public class TlvWriter : BinaryWriter
    {
        /// <summary>
        ///     Create TLV object writer instance.
        /// </summary>
        /// <param name="input">Output stream</param>
        public TlvWriter(Stream input) : base(input)
        {
        }

        /// <summary>
        ///     Write TLV object to given stream.
        /// </summary>
        /// <param name="tag">TLV object</param>
        public void WriteTag(TlvTag tag)
        {
            if (tag == null)
            {
                return;
            }

            if (tag.Type > TlvReader.MaxType)
            {
                throw new ArgumentOutOfRangeException("tag");
            }

            byte[] data = tag.EncodeValue();
            bool tlv16 = tag.Type > TlvReader.TypeMask
                         || (data != null && data.Length > byte.MaxValue);
            byte firstByte = (byte) ((tlv16 ? TlvReader.Tlv16Flag : 0)
                                     + (tag.NonCritical ? TlvReader.NonCriticalFlag : 0)
                                     + (tag.Forward ? TlvReader.ForwardFlag : 0));

            if (tlv16)
            {
                firstByte = (byte) (firstByte
                                    | (tag.Type >> TlvReader.ByteBits) & TlvReader.TypeMask);
                Write(firstByte);
                Write((byte) tag.Type);
                if (data == null)
                {
                    Write((byte) 0);
                }
                else
                {
                    if (data.Length > ushort.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException("tag");
                    }
                    Write((byte) (data.Length >> TlvReader.ByteBits));
                    Write((byte) data.Length);
                    Write(data);
                }
            }
            else
            {
                firstByte = (byte) (firstByte | tag.Type & TlvReader.TypeMask);
                Write(firstByte);
                if (data == null)
                {
                    Write((byte) 0);
                }
                else
                {
                    Write((byte) data.Length);
                    Write(data);
                }
            }
        }
    }
}