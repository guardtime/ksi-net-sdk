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
using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Signature
{
    /// <summary>
    ///     Aggregation hash chain TLV element.
    /// </summary>
    public sealed partial class AggregationHashChain
    {
        /// <summary>
        /// Aggregation hash chain link metadata TLV element.
        /// </summary>
        public class MetaData : CompositeTag
        {
            private readonly StringTag _clientId;
            private readonly StringTag _machineId;

            // Please do keep in mind that request time is in milliseconds!
            private readonly IntegerTag _requestTime;
            private readonly IntegerTag _sequenceNumber;

            /// <summary>
            /// Create new aggregation hash chain link metadata TLV elment from TLV element
            /// </summary>
            /// <param name="tag">TLV element</param>
            public MetaData(ITlvTag tag) : base(tag)
            {
                if (Type != Constants.AggregationHashChain.MetaData.TagType)
                {
                    throw new TlvException("Invalid aggregation hash chain link metadata type(" + Type + ").");
                }

                int clientIdCount = 0;
                int machineIdCount = 0;
                int sequenceNumberCount = 0;
                int requestTimeCount = 0;

                for (int i = 0; i < Count; i++)
                {
                    ITlvTag childTag = this[i];

                    switch (childTag.Type)
                    {
                        case Constants.AggregationHashChain.MetaData.PaddingTagType:
                            this[i] = Padding = childTag as RawTag ?? new RawTag(childTag);
                            PaddingTagIndex = i;
                            break;
                        case Constants.AggregationHashChain.MetaData.ClientIdTagType:
                            this[i] = _clientId = new StringTag(childTag);
                            clientIdCount++;
                            break;
                        case Constants.AggregationHashChain.MetaData.MachineIdTagType:
                            this[i] = _machineId = new StringTag(childTag);
                            machineIdCount++;
                            break;
                        case Constants.AggregationHashChain.MetaData.SequenceNumberTagType:
                            this[i] = _sequenceNumber = new IntegerTag(childTag);
                            sequenceNumberCount++;
                            break;
                        case Constants.AggregationHashChain.MetaData.RequestTimeTagType:
                            this[i] = _requestTime = new IntegerTag(childTag);
                            requestTimeCount++;
                            break;
                        default:
                            VerifyUnknownTag(childTag);
                            break;
                    }
                }

                if (clientIdCount != 1)
                {
                    throw new TlvException("Exactly one client id must exist in aggregation hash chain link metadata.");
                }

                if (machineIdCount > 1)
                {
                    throw new TlvException("Only one machine id is allowed in aggregation hash chain link metadata.");
                }

                if (sequenceNumberCount > 1)
                {
                    throw new TlvException("Only one sequence number is allowed in aggregation hash chain link metadata.");
                }

                if (requestTimeCount > 1)
                {
                    throw new TlvException("Only one request time is allowed in aggregation hash chain link metadata.");
                }
            }

            /// <summary>
            /// Padding element
            /// </summary>
            public RawTag Padding { get; }

            /// <summary>
            /// Padding element index inside the metadata element
            /// </summary>       
            public int PaddingTagIndex { get; }

            /// <summary>
            /// Client identifier
            /// </summary>
            public string ClientId => _clientId.Value;

            /// <summary>
            /// Machine identifier
            /// </summary>
            public string MachineId => _machineId.Value;

            /// <summary>
            /// The time when the server received the request from the client (in milliseconds)
            /// </summary>
            public ulong RequestTime => _requestTime.Value;

            /// <summary>
            /// A local sequence number of a request assigned by the machine that created the link
            /// </summary>
            public ulong SequenceNumber => _sequenceNumber.Value;
        }
    }
}