﻿using System;
using Guardtime.KSI.Exceptions;

namespace Guardtime.KSI.Signature.Verification.Rule
{
 //* This rule is used to check if keyless signature contains publication record or not.
 
     
    /// <summary>
    /// Rule checks if KSI signature contains publication record.
    /// </summary>
    public sealed class SignaturePublicationRecordExistenceRule : VerificationRule
    {
        /// <see cref="VerificationRule.Verify"/>
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
                throw new KsiVerificationException("Invalid KSI signature: null");
            }

            return context.Signature.PublicationRecord == null ? VerificationResult.Na : VerificationResult.Ok;
        }
    }
}