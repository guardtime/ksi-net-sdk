﻿using System;
using System.Collections.Generic;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Utils;

namespace Guardtime.KSI.Service
{
    public sealed class AggregationRequestPayload : AggregationPduPayload
    {
        // TODO: Better name
        public const uint TagType = 0x201;
        private const uint RequestIdTagType = 0x1;
        private const uint RequestHashTagType = 0x2;

        private readonly IntegerTag _requestId;
        private readonly ImprintTag _requestHash;
        private readonly IntegerTag _requestLevel;
        private readonly RawTag _config;

        // Create correct constructor
        public AggregationRequestPayload(DataHash hash) : base(TagType, false, false, new List<TlvTag>())
        {
            if (hash == null)
            {
                throw new ArgumentNullException("hash");
            }

            _requestId = new IntegerTag(RequestIdTagType, false, false, Util.GetRandomUnsignedLong());
            AddTag(_requestId);

            _requestHash = new ImprintTag(RequestHashTagType, false, false, hash);
            AddTag(_requestHash);
        }

    }
}
