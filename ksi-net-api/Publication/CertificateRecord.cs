﻿using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Parser;

namespace Guardtime.KSI.Publication
{
    public sealed class CertificateRecord : CompositeTag
    {
        // TODO: Better name
        public const uint TagType = 0x702;
        private const uint CertificateIdTagType = 0x1;
        private const uint X509CertificateTagType = 0x2;

        private readonly RawTag _certificateId;
        private readonly RawTag _x509Certificate;

        public RawTag CertificateId
        {
            get
            {
                return _certificateId;
            }
        }

        public RawTag X509Certificate
        {
            get
            {
                return _x509Certificate;
            }
        }

        public CertificateRecord(TlvTag tag) : base(tag)
        {
            if (Type != TagType)
            {
                throw new InvalidTlvStructureException("Invalid certificate record type: " + Type);
            }

            int certificateIdCount = 0;
            int x509CertificateCount = 0;

            for (int i = 0; i < Count; i++)
            {
                switch (this[i].Type)
                {
                    case CertificateIdTagType:
                        _certificateId = new RawTag(this[i]);
                        this[i] = _certificateId;
                        certificateIdCount++;
                        break;
                    case X509CertificateTagType:
                        _x509Certificate = new RawTag(this[i]);
                        this[i] = _x509Certificate;
                        x509CertificateCount++;
                        break;
                    default:
                        VerifyCriticalTag(this[i]);
                        break;
                }
            }

            if (certificateIdCount != 1)
            {
                throw new InvalidTlvStructureException("Only one certificate id must exist in certificate record");
            }

            if (x509CertificateCount != 1)
            {
                throw new InvalidTlvStructureException("Only one certificate must exist in certificate record");
            }
        }
    }
}