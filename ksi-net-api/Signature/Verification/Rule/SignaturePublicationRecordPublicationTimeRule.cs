﻿using System;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Publication;

namespace Guardtime.KSI.Signature.Verification.Rule
{
    public sealed class SignaturePublicationRecordPublicationTimeRule : VerificationRule
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

            if (context.Signature == null)
            {
                throw new KsiVerificationException("Invalid KSI signature in context: null");
            }

            if (context.Signature.PublicationRecord == null)
            {
                return VerificationResult.Ok;
            }

            if (context.Signature.CalendarHashChain == null)
            {
                throw new KsiVerificationException("Calendar hash chain missing in KSI signature");
            }

            PublicationData publicationRecordPublicationData = context.Signature.PublicationRecord.PublicationData;
            PublicationData calendarHashChainPublicationData = context.Signature.CalendarHashChain.PublicationData;

            if (publicationRecordPublicationData.PublicationTime != calendarHashChainPublicationData.PublicationTime)
            {
                return VerificationResult.Fail;
            }

            return VerificationResult.Ok;
        }
    }
}