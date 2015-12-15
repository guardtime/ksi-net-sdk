﻿using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Signature
{
    /// <summary>
    ///     Signature data TLV element
    /// </summary>
    public sealed class SignatureData : CompositeTag
    {
        private readonly RawTag _certificateId;
        private readonly StringTag _certificateRepositoryUri;

        private readonly StringTag _signatureType;
        private readonly RawTag _signatureValue;

        /// <summary>
        ///     Create new signature data TLV element from TLV element
        /// </summary>
        /// <param name="tag">TLV element</param>
        public SignatureData(ITlvTag tag) : base(tag)
        {
            if (Type != Constants.SignatureData.TagType)
            {
                throw new TlvException("Invalid signature data type(" + Type + ").");
            }

            int signatureTypeCount = 0;
            int signatureValueCount = 0;
            int certificateIdCount = 0;
            int certificateRepositoryUriCount = 0;

            foreach (ITlvTag childTag in this)
            {
                switch (childTag.Type)
                {
                    case Constants.SignatureData.SignatureTypeTagType:
                        _signatureType = new StringTag(childTag);
                        signatureTypeCount++;
                        break;
                    case Constants.SignatureData.SignatureValueTagType:
                        _signatureValue = new RawTag(childTag);
                        signatureValueCount++;
                        break;
                    case Constants.SignatureData.CertificateIdTagType:
                        _certificateId = new RawTag(childTag);
                        certificateIdCount++;
                        break;
                    case Constants.SignatureData.CertificateRepositoryUriTagType:
                        _certificateRepositoryUri = new StringTag(childTag);
                        certificateRepositoryUriCount++;
                        break;
                    default:
                        VerifyUnknownTag(childTag);
                        break;
                }
            }

            if (signatureTypeCount != 1)
            {
                throw new TlvException("Only one signature type must exist in signature data.");
            }

            if (signatureValueCount != 1)
            {
                throw new TlvException("Only one signature value must exist in signature data.");
            }

            if (certificateIdCount != 1)
            {
                throw new TlvException("Only one certificate id must exist in signature data.");
            }

            if (certificateRepositoryUriCount > 1)
            {
                throw new TlvException(
                    "Only one certificate repository uri is allowed in signature data.");
            }
        }

        /// <summary>
        ///     Get certificate ID.
        /// </summary>
        public byte[] GetCertificateId()
        {
            return _certificateId.Value;
        }

        /// <summary>
        ///     Get signature value.
        /// </summary>
        public byte[] GetSignatureValue()
        {
            return _signatureValue.Value;
        }

        /// <summary>
        ///     Get signature type.
        /// </summary>
        public string SignatureType => _signatureType.Value;

        /// <summary>
        ///     Get certificate repository URI if it exists.
        /// </summary>
        public string CertificateRepositoryUri => _certificateRepositoryUri?.Value;
    }
}