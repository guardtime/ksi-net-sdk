﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Publication;

namespace Guardtime.KSI.Signature
{

    public partial class KsiSignatureFactory
    {

        /// <summary>
        ///     KSI Signature implementation.
        /// </summary>
        private sealed class KsiSignature : CompositeTag, IKsiSignature
        {
            /// <summary>
            ///     KSI signature tag type
            /// </summary>
            public const uint TagType = 0x800;

            private readonly AggregationAuthenticationRecord _aggregationAuthenticationRecord;

            private readonly List<AggregationHashChain> _aggregationHashChainCollection =
                new List<AggregationHashChain>();

            private readonly CalendarAuthenticationRecord _calendarAuthenticationRecord;
            private readonly CalendarHashChain _calendarChain;
            private readonly PublicationRecord _publicationRecord;
            private readonly Rfc3161Record _rfc3161Record;

            /// <summary>
            ///     Create new KSI signature TLV element from TLV element.
            /// </summary>
            /// <param name="tag">TLV element</param>
            public KsiSignature(TlvTag tag) : base(tag)
            {
                if (Type != TagType)
                {
                    throw new InvalidTlvStructureException("Invalid signature type: " + Type);
                }

                int calendarChainCount = 0;
                int publicationRecordCount = 0;
                int aggregationAuthenticationRecordCount = 0;
                int calendarAuthenticationRecordCount = 0;
                int rfc3161RecordCount = 0;

                for (int i = 0; i < Count; i++)
                {
                    switch (this[i].Type)
                    {
                        case AggregationHashChain.TagType:
                            AggregationHashChain aggregationChainTag = new AggregationHashChain(this[i]);
                            _aggregationHashChainCollection.Add(aggregationChainTag);
                            this[i] = aggregationChainTag;
                            break;
                        case CalendarHashChain.TagType:
                            _calendarChain = new CalendarHashChain(this[i]);
                            this[i] = _calendarChain;
                            calendarChainCount++;
                            break;
                        case PublicationRecord.TagTypeSignature:
                            _publicationRecord = new PublicationRecord(this[i]);
                            this[i] = _publicationRecord;
                            publicationRecordCount++;
                            break;
                        case AggregationAuthenticationRecord.TagType:
                            _aggregationAuthenticationRecord = new AggregationAuthenticationRecord(this[i]);
                            this[i] = _aggregationAuthenticationRecord;
                            aggregationAuthenticationRecordCount++;
                            break;
                        case CalendarAuthenticationRecord.TagType:
                            _calendarAuthenticationRecord = new CalendarAuthenticationRecord(this[i]);
                            this[i] = _calendarAuthenticationRecord;
                            calendarAuthenticationRecordCount++;
                            break;
                        case Rfc3161Record.TagType:
                            _rfc3161Record = new Rfc3161Record(this[i]);
                            this[i] = _rfc3161Record;
                            rfc3161RecordCount++;
                            break;
                        default:
                            VerifyCriticalFlag(this[i]);
                            break;
                    }
                }

                if (_aggregationHashChainCollection.Count == 0)
                {
                    throw new InvalidTlvStructureException("Aggregation hash chains must exist in signature data object");
                }

                if (calendarChainCount > 1)
                {
                    throw new InvalidTlvStructureException(
                        "Only one calendar hash chain is allowed in signature data object");
                }

                if (calendarChainCount == 0 && (publicationRecordCount != 0 || calendarAuthenticationRecordCount != 0))
                {
                    throw new InvalidTlvStructureException(
                        "No publication record or calendar authentication record is allowed in signature data object if there is no calendar hash chain");
                }

                if ((publicationRecordCount == 1 && calendarAuthenticationRecordCount == 1) ||
                    publicationRecordCount > 1 ||
                    calendarAuthenticationRecordCount > 1)
                {
                    throw new InvalidTlvStructureException(
                        "Only one from publication record or calendar authentication record is allowed in signature data object");
                }

                if (aggregationAuthenticationRecordCount > 1)
                {
                    throw new InvalidTlvStructureException(
                        "Only one aggregation authentication record is allowed in signature data object");
                }

                if (rfc3161RecordCount > 1)
                {
                    throw new InvalidTlvStructureException(
                        "Only one RFC 3161 record is allowed in signature data object");
                }

                // TODO: Aggregation authentication record

                _aggregationHashChainCollection.Sort(new AggregationHashChain.ChainIndexOrdering());
            }

            /// <summary>
            ///     Get RFC 3161 record
            /// </summary>
            public Rfc3161Record Rfc3161Record
            {
                get { return _rfc3161Record; }
            }

            /// <summary>
            ///     Is signature RFC 3161 format
            /// </summary>
            public bool IsRfc3161Signature
            {
                get { return _rfc3161Record != null; }
            }

            /// <summary>
            ///     Get calendar hash chain.
            /// </summary>
            public CalendarHashChain CalendarHashChain
            {
                get { return _calendarChain; }
            }

            /// <summary>
            ///     Get calendar authentication record.
            /// </summary>
            public CalendarAuthenticationRecord CalendarAuthenticationRecord
            {
                get { return _calendarAuthenticationRecord; }
            }

            /// <summary>
            ///     Get publication record.
            /// </summary>
            public PublicationRecord PublicationRecord
            {
                get { return _publicationRecord; }
            }

            /// <summary>
            ///     Get aggregation hash chains list.
            /// </summary>
            /// <returns>aggregations hash chains list</returns>
            public ReadOnlyCollection<AggregationHashChain> GetAggregationHashChains()
            {
                return _aggregationHashChainCollection.AsReadOnly();
            }

            /// <summary>
            ///     Get aggregation hash chain output hash.
            /// </summary>
            /// <returns>output hash</returns>
            public DataHash GetAggregationHashChainRootHash()
            {
                // Store result
                AggregationHashChain.ChainResult lastResult = new AggregationHashChain.ChainResult(0,
                    _aggregationHashChainCollection[0].InputHash);
                for (int i = 0; i < _aggregationHashChainCollection.Count; i++)
                {
                    lastResult = _aggregationHashChainCollection[i].GetOutputHash(lastResult);
                }

                return lastResult.Hash;
            }

            /// <summary>
            /// Get aggregation time.
            /// </summary>
            public ulong AggregationTime
            {
                get { return _aggregationHashChainCollection[0].AggregationTime; }
            }

            /// <summary>
            /// Extend KSI signature with given calendar hash chain.
            /// </summary>
            /// <param name="calendarHashChain">calendar hash chain</param>
            /// <returns>extended KSI signature</returns>
            public IKsiSignature Extend(CalendarHashChain calendarHashChain)
            {
                if (calendarHashChain == null)
                {
                    throw new ArgumentNullException("calendarHashChain");
                }

                using (MemoryStream stream = new MemoryStream())
                using (TlvWriter writer = new TlvWriter(stream))
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (this[i].Type == CalendarHashChain.TagType)
                        {
                            writer.WriteTag(calendarHashChain);
                            continue;
                        }

                        writer.WriteTag(this[i]);
                    }

                    return new KsiSignature(new RawTag(TagType, false, false, stream.ToArray()));
                }
            }
        }
    }
}