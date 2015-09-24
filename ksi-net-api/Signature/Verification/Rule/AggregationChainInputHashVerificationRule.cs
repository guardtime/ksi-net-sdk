﻿using System;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;

namespace Guardtime.KSI.Signature.Verification.Rule
{
    /// <summary>
    ///     This rule verifies input hash for aggregation chain. If RFC3161 record is present then document hash is first
    ///     hashed to aggregation input hash and is then compared. Otherwise document hash is compared directly to input hash.
    /// </summary>
    public sealed class AggregationChainInputHashVerificationRule : VerificationRule
    {
        /// <see cref="VerificationRule.Verify" />
        /// <exception cref="ArgumentNullException">thrown if context is missing</exception>
        /// <exception cref="KsiVerificationException">thrown if verification cannot occur</exception>
        public override VerificationResult Verify(IVerificationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            KsiSignature signature = context.Signature;
            DataHash inputHash = context.DocumentHash;
            if (signature == null)
            {
                throw new KsiVerificationException("Invalid KSI signature: null");
            }

            DataHash aggregationHashChainInputHash = signature.GetAggregationHashChains()[0].InputHash;

            if (signature.IsRfc3161Signature)
            {
                DataHasher hasher = new DataHasher(aggregationHashChainInputHash.Algorithm);
                hasher.AddData(signature.Rfc3161Record.GetOutputHash(inputHash).Imprint);
                inputHash = hasher.GetHash();
            }

            return inputHash != aggregationHashChainInputHash ? VerificationResult.Fail : VerificationResult.Ok;
        }
    }
}