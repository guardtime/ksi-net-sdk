﻿/*
 * Copyright 2013-2016 Guardtime, Inc.
 *
 * This file is part of the Guardtime client SDK.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *     http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES, CONDITIONS, OR OTHER LICENSES OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 * "Guardtime" and "KSI" are trademarks or registered trademarks of
 * Guardtime, Inc., and no license to trademarks is granted; Guardtime
 * reserves and retains all trademark rights.
 */

using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Service
{
    /// <summary>
    ///     Extendx PDU.
    /// </summary>
    public sealed class ExtendPdu : KsiPdu
    {
        /// <summary>
        ///     Get PDU payload.
        /// </summary>
        public override KsiPduPayload Payload { get; }

        /// <summary>
        ///     Create extend PDU from TLV element.
        /// </summary>
        /// <param name="tag">TLV element</param>
        public ExtendPdu(ITlvTag tag) : base(tag)
        {
            if (Type != Constants.ExtendPdu.TagType)
            {
                throw new TlvException("Invalid extend PDU type(" + Type + ").");
            }

            for (int i = 0; i < Count; i++)
            {
                ITlvTag childTag = this[i];

                switch (childTag.Type)
                {
                    case Constants.ExtendRequestPayload.TagType:
                        this[i] = Payload = new ExtendRequestPayload(childTag);
                        break;
                    case Constants.ExtendResponsePayload.TagType:
                        this[i] = Payload = new ExtendResponsePayload(childTag);
                        break;
                    case Constants.ExtendErrorPayload.TagType:
                        this[i] = Payload = new ExtendErrorPayload(childTag);
                        break;
                    case Constants.KsiPduHeader.TagType:
                    case Constants.KsiPdu.MacTagType:
                        break;
                    default:
                        VerifyUnknownTag(childTag);
                        break;
                }
            }
        }

        /// <summary>
        ///     Create extend pdu from KSI header and extend pdu payload.
        /// </summary>
        /// <param name="header">KSI header</param>
        /// <param name="payload">Extend pdu payload</param>
        /// <param name="hmacAlgorithm">HMAC algorithm</param>
        /// <param name="key">hmac key</param>
        public ExtendPdu(KsiPduHeader header, KsiPduPayload payload, HashAlgorithm hmacAlgorithm, byte[] key)
            : base(Constants.ExtendPdu.TagType, header, payload, hmacAlgorithm, key)
        {
            Payload = payload;
        }
    }
}