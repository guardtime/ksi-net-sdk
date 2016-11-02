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

using System;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Service
{
    /// <summary>
    ///     Extend PDU.
    /// </summary>
    [Obsolete]
    public sealed class LegacyExtendPdu : LegacyKsiPdu
    {
        /// <summary>
        ///     Get PDU payload.
        /// </summary>
        public override KsiPduPayload Payload { get; }

        /// <summary>
        ///     Create extend PDU from TLV element.
        /// </summary>
        /// <param name="tag">TLV element</param>
        [Obsolete]
        public LegacyExtendPdu(ITlvTag tag) : base(tag)
        {
            CheckTagType(Constants.LegacyExtendPdu.TagType);

            int headerCount = 0;
            int payloadCount = 0;
            int macCount = 0;

            for (int i = 0; i < Count; i++)
            {
                ITlvTag childTag = this[i];

                switch (childTag.Type)
                {
                    case Constants.ExtendRequestPayload.LegacyTagType:
                        this[i] = Payload = new LegacyExtendRequestPayload(childTag);
                        payloadCount++;
                        break;
                    case Constants.ExtendResponsePayload.LegacyTagType:
                        this[i] = Payload = new LegacyExtendResponsePayload(childTag);
                        payloadCount++;
                        break;
                    case Constants.ExtendErrorPayload.LegacyTagType:
                        this[i] = Payload = new LegacyExtendErrorPayload(childTag);
                        payloadCount++;
                        break;
                    case Constants.KsiPduHeader.TagType:
                        headerCount++;
                        break;
                    case Constants.KsiPdu.MacTagType:
                        macCount++;
                        break;
                    default:
                        VerifyUnknownTag(childTag);
                        break;
                }
            }

            if (payloadCount != 1)
            {
                throw new TlvException("Exactly one payload must exist in KSI PDU.");
            }

            if (Payload.Type != Constants.ExtendErrorPayload.TagType && Payload.Type != Constants.ExtendErrorPayload.LegacyTagType && headerCount != 1)
            {
                throw new TlvException("Exactly one header must exist in KSI PDU.");
            }

            if (Payload.Type != Constants.ExtendErrorPayload.TagType && Payload.Type != Constants.ExtendErrorPayload.LegacyTagType && macCount != 1)
            {
                throw new TlvException("Exactly one mac must exist in KSI PDU.");
            }
        }

        /// <summary>
        ///     Create extend pdu from KSI header and extend pdu payload.
        /// </summary>
        /// <param name="header">KSI header</param>
        /// <param name="payload">Extend pdu payload</param>
        /// <param name="mac">Extend pdu hmac</param>
        [Obsolete]
        public LegacyExtendPdu(KsiPduHeader header, KsiPduPayload payload, ImprintTag mac)
            : base(header, mac, Constants.LegacyExtendPdu.TagType, false, false, new ITlvTag[] { header, payload, mac })
        {
            Payload = payload;
        }
    }
}