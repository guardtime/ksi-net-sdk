﻿using System.Collections.Generic;
using Guardtime.KSI.Crypto;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Publication;

namespace Guardtime.KSI.Signature
{
    /// <summary>
    ///     Calendar hash chain TLV element
    /// </summary>
    public sealed class CalendarHashChain : CompositeTag
    {
        private readonly IntegerTag _aggregationTime;
        private readonly List<Link> _chain = new List<Link>();
        private readonly ImprintTag _inputHash;
        private readonly IntegerTag _publicationTime;

        /// <summary>
        ///     Create new calendar hash chain TLV element from TLV element
        /// </summary>
        /// <param name="tag">TLV element</param>
        /// <exception cref="TlvException">thrown when TLV parsing fails</exception>
        public CalendarHashChain(ITlvTag tag) : base(tag)
        {
            if (Type != Constants.CalendarHashChain.TagType)
            {
                throw new TlvException("Invalid calendar hash chain type(" + Type + ").");
            }

            int publicationTimeCount = 0;
            int aggregationTimeCount = 0;
            int inputHashCount = 0;

            for (int i = 0; i < Count; i++)
            {
                switch (this[i].Type)
                {
                    case Constants.CalendarHashChain.PublicationTimeTagType:
                        _publicationTime = new IntegerTag(this[i]);
                        publicationTimeCount++;
                        break;
                    case Constants.CalendarHashChain.AggregationTimeTagType:
                        _aggregationTime = new IntegerTag(this[i]);
                        aggregationTimeCount++;
                        break;
                    case Constants.CalendarHashChain.InputHashTagType:
                        _inputHash = new ImprintTag(this[i]);
                        inputHashCount++;
                        break;
                    case (uint)LinkDirection.Left:
                    case (uint)LinkDirection.Right:
                        Link chainTag = new Link(this[i]);
                        _chain.Add(chainTag);
                        break;
                    default:
                        VerifyUnknownTag(this[i]);
                        break;
                }
            }

            if (publicationTimeCount != 1)
            {
                throw new TlvException("Only one publication time must exist in calendar hash chain.");
            }

            if (aggregationTimeCount > 1)
            {
                throw new TlvException("Only one aggregation time is allowed in calendar hash chain.");
            }

            if (inputHashCount != 1)
            {
                throw new TlvException("Only one input hash must exist in calendar hash chain.");
            }

            if (_chain.Count == 0)
            {
                throw new TlvException("Links are missing in calendar hash chain.");
            }

            RegistrationTime = CalculateRegistrationTime();
            OutputHash = CalculateOutputHash();
            PublicationData = new PublicationData(_publicationTime.Value, OutputHash);
        }

        /// <summary>
        ///     Get aggregation time
        /// </summary>
        public ulong AggregationTime => _aggregationTime == null ? _publicationTime.Value : _aggregationTime.Value;

        /// <summary>
        ///     Get publication time.
        /// </summary>
        public ulong PublicationTime => _publicationTime.Value;

        /// <summary>
        ///     Get registration time.
        /// </summary>
        public ulong RegistrationTime { get; }

        /// <summary>
        ///     Get input hash.
        /// </summary>
        public DataHash InputHash => _inputHash.Value;

        /// <summary>
        ///     Get output hash.
        /// </summary>
        public DataHash OutputHash { get; }

        /// <summary>
        ///     Get publication data.
        /// </summary>
        public PublicationData PublicationData { get; }

        /// <summary>
        ///     Compare right links if they are equal.
        /// </summary>
        /// <param name="calendarHashChain">calendar hash chain to compare to</param>
        /// <returns>true if right links are equal and on same position</returns>
        public bool AreRightLinksEqual(CalendarHashChain calendarHashChain)
        {
            if (_chain.Count != calendarHashChain?._chain.Count)
            {
                return false;
            }

            for (int i = 0; i < _chain.Count; i++)
            {
                if (calendarHashChain._chain[i].Direction != LinkDirection.Right)
                {
                    continue;
                }

                if (_chain[i] != calendarHashChain._chain[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Calculate output hash.
        /// </summary>
        /// <returns>output hash</returns>
        private DataHash CalculateOutputHash()
        {
            DataHash inputHash = InputHash;
            for (int i = 0; i < _chain.Count; i++)
            {
                DataHash siblingHash = _chain[i].Value;
                if (_chain[i].Direction == LinkDirection.Left)
                {
                    inputHash = GetStepHash(siblingHash.Algorithm, inputHash.Imprint, siblingHash.Imprint);
                }

                if (_chain[i].Direction == LinkDirection.Right)
                {
                    inputHash = GetStepHash(inputHash.Algorithm, siblingHash.Imprint, inputHash.Imprint);
                }
            }

            return inputHash;
        }

        /// <summary>
        ///     Hash two hashes together with algorithm.
        /// </summary>
        /// <param name="algorithm">hash algorithm</param>
        /// <param name="hashA">hash a</param>
        /// <param name="hashB">hash b</param>
        /// <returns>result hash</returns>
        private static DataHash GetStepHash(HashAlgorithm algorithm, byte[] hashA, byte[] hashB)
        {
            IDataHasher hasher = CryptoProvider.GetDataHasher(algorithm);
            hasher.AddData(hashA);
            hasher.AddData(hashB);
            hasher.AddData(new byte[] {0xFF});
            return hasher.GetHash();
        }

        /// <summary>
        ///     Calculate registration time.
        /// </summary>
        /// <returns>registration time</returns>
        /// <exception cref="TlvException">thrown when registration time calculation fails.</exception>
        private ulong CalculateRegistrationTime()
        {
            ulong r = _publicationTime.Value;
            ulong t = 0;
            // iterate over the chain in reverse

            for (int i = _chain.Count - 1; i >= 0; i--)
            {
                if (r <= 0)
                {
                    throw new TlvException("Invalid calendar hash chain shape for publication time.");
                }

                if (_chain[i].Direction == LinkDirection.Left)
                {
                    r = HighBit(r) - 1;
                }
                else
                {
                    t = t + HighBit(r);
                    r = r - HighBit(r);
                }
            }

            if (r != 0)
            {
                throw new TlvException("Calendar hash chain shape inconsistent with publication time.");
            }

            return t;
        }

        /// <summary>
        ///     Calculate highest bit.
        /// </summary>
        /// <param name="n">number to get highest bit from.</param>
        /// <returns>highest bit</returns>
        private static ulong HighBit(ulong n)
        {
            n |= (n >> 1);
            n |= (n >> 2);
            n |= (n >> 4);
            n |= (n >> 8);
            n |= (n >> 16);
            n |= (n >> 32);
            return n - (n >> 1);
        }

        /// <summary>
        ///     Calendar hash chain link object which is imprint containing link direction
        /// </summary>
        private class Link : ImprintTag
        {
            public Link(ITlvTag tag) : base(tag)
            {
                switch (tag.Type)
                {
                    case (int)LinkDirection.Left:
                        Direction = LinkDirection.Left;
                        break;
                    case (int)LinkDirection.Right:
                        Direction = LinkDirection.Right;
                        break;
                    default:
                        if (Direction == 0)
                        {
                            throw new TlvException("Invalid calendar hash chain link type(" + Type + ").");
                        }
                        break;
                }
            }

            public LinkDirection Direction { get; }
        }
    }
}