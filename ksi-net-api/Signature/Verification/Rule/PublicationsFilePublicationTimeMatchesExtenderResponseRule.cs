﻿using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Publication;

namespace Guardtime.KSI.Signature.Verification.Rule
{
    /// <summary>
    ///     Rule checks that publications file publication time matches with extender response calendar hash chain registration
    ///     time.
    /// </summary>
    public sealed class PublicationsFilePublicationTimeMatchesExtenderResponseRule : VerificationRule
    {
        /// <see cref="VerificationRule.Verify" />
        /// <exception cref="KsiException">thrown if verification context is missing</exception>
        /// <exception cref="KsiVerificationException">thrown if verification cannot occur</exception>
        public override VerificationResult Verify(IVerificationContext context)
        {
            IKsiSignature signature = GetSignature(context);
            IPublicationsFile publicationsFile = GetPublicationsFile(context);
            CalendarHashChain signatureCalendarHashChain = GetCalendarHashChain(signature);
            PublicationRecord publicationRecord = publicationsFile.GetNearestPublicationRecord(signatureCalendarHashChain.RegistrationTime);

            if (publicationRecord == null)
            {
                throw new KsiVerificationException("No publication record found in publications file after registration time: " + signatureCalendarHashChain.RegistrationTime + ".");
            }

            CalendarHashChain extendedTimeCalendarHashChain = GetExtendedTimeCalendarHashChain(context, publicationRecord.PublicationData.PublicationTime);

            if (publicationRecord.PublicationData.PublicationTime != extendedTimeCalendarHashChain.PublicationTime)
            {
                return new VerificationResult(GetRuleName(), VerificationResultCode.Fail, VerificationError.Pub02);
            }

            return signature.AggregationTime != extendedTimeCalendarHashChain.RegistrationTime
                ? new VerificationResult(GetRuleName(), VerificationResultCode.Fail, VerificationError.Pub02)
                : new VerificationResult(GetRuleName(), VerificationResultCode.Ok);
        }
    }
}