﻿using System.Collections.ObjectModel;
using Guardtime.KSI.Exceptions;
using NLog;

namespace Guardtime.KSI.Signature.Verification.Rule
{
    /// <summary>
    ///     Rule verifies if all aggregation hash chains are consistent. e.g. previous aggregation hash chain output hash
    ///     equals to current aggregation hash chain input hash.
    /// </summary>
    public sealed class AggregationHashChainConsistencyRule : VerificationRule
    {
        /// <see cref="VerificationRule.Verify" />
        /// <exception cref="KsiException">thrown if verification context is missing</exception>
        /// <exception cref="KsiVerificationException">thrown if verification cannot occur</exception>
        public override VerificationResult Verify(IVerificationContext context)
        {
            ReadOnlyCollection<AggregationHashChain> aggregationHashChains = GetAggregationHashChains(GetSignature(context), true);
            AggregationHashChainResult chainResult = null;

            foreach (AggregationHashChain aggregationHashChain in aggregationHashChains)
            {
                if (chainResult == null)
                {
                    chainResult = new AggregationHashChainResult(0, aggregationHashChains[0].InputHash);
                }

                if (aggregationHashChain.InputHash != chainResult.Hash)
                {
                    Logger.Error("Previous aggregation hash chain output hash {0} does not match current input hash {1}", chainResult.Hash, aggregationHashChain.InputHash);
                    return new VerificationResult(GetRuleName(), VerificationResultCode.Fail, VerificationError.Int01);
                }

                chainResult = aggregationHashChain.GetOutputHash(chainResult);
            }

            return new VerificationResult(GetRuleName(), VerificationResultCode.Ok);
        }
    }
}