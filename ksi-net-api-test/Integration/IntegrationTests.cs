﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI.Crypto;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Trust;

namespace Guardtime.KSI.Integration
{
    public class IntegrationTests
    {
        private static readonly HttpKsiServiceProtocol KsiServiceProtocol =
            new HttpKsiServiceProtocol("http://ksigw.test.guardtime.com:3333/gt-signingservice", "http://ksigw.test.guardtime.com:8010/gt-extendingservice",
                "http://verify.guardtime.com/ksi-publications.bin");

        protected static object[] TestCases =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        KsiServiceProtocol,
                        KsiServiceProtocol,
                        KsiServiceProtocol,
                        new ServiceCredentials("anon", "anon"),
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(TrustStoreUtilities.GetTrustAnchorCollection(), new CertificateRdnSubjectSelector("E=publications@guardtime.com"))),
                        new KsiSignatureFactory()))
            }
        };
    }
}