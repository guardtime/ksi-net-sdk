﻿/*
 * Copyright 2013-2016 Guardtime, Inc.
 *
 * This file is part of the Guardtime client SDK.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *     http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES, CONDITIONS, OR OTHER LICENSES OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 * "Guardtime" and "KSI" are trademarks or registered trademarks of
 * Guardtime, Inc., and no license to trademarks is granted; Guardtime
 * reserves and retains all trademark rights.
 */

using System;
using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Test.Crypto;
using Guardtime.KSI.Test.Properties;
using Guardtime.KSI.Trust;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Integration
{
    public class IntegrationTests
    {
        private static readonly HttpKsiServiceProtocol HttpKsiServiceProtocol =
            new HttpKsiServiceProtocol(Settings.Default.HttpSigningServiceUrl, Settings.Default.HttpExtendingServiceUrl, Settings.Default.HttpPublicationsFileUrl, 100000);

        private static readonly HttpKsiServiceProtocol HttpKsiServiceProtocolInvalidUrls =
            new HttpKsiServiceProtocol("http://invalid.signing.service.url", "http://invalid.extending.service.url",
                "http://invalid.publications.file.url", 100000);

        private static readonly TcpKsiServiceProtocol TcpKsiServiceProtocol = new TcpKsiServiceProtocol(Settings.Default.TcpSigningServiceUrl,
            Settings.Default.TcpSigningServicePort, 100000);

        private static readonly TcpKsiServiceProtocol TcpKsiServiceProtocolInvalidUrl = new TcpKsiServiceProtocol("tcp.invalid.service.url",
            Settings.Default.TcpSigningServicePort, 100000);

        private static readonly TcpKsiServiceProtocol TcpKsiServiceProtocolInvalidPort = new TcpKsiServiceProtocol(Settings.Default.TcpSigningServiceUrl, 2847, 100000);

        public static object[] HttpTestCases =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com"))),
                        HashAlgorithm.Sha2256))
            }
        };

        protected static object[] HttpTestCasesInvalidSigningPass =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, "anonx"),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, "anon"),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com"))),
                        new KsiSignatureFactory()))
            }
        };

        protected static object[] HttpTestCasesInvalidExtendingPass =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, "anon"),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, "anonx"),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com"))),
                        new KsiSignatureFactory(),
                        HashAlgorithm.Sha2256))
            }
        };

        protected static object[] HttpTestCasesInvalidSigningUrl =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocolInvalidUrls,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] HttpTestCasesInvalidExtendingUrl =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] HttpTestCasesInvalidPublicationsFilegUrl =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] TcpTestCases =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        TcpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] TcpTestCasesInvalidPass =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        TcpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, "anonx"),
                        HttpKsiServiceProtocol,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, "anonx"),
                        HttpKsiServiceProtocol,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] TcpTestCasesInvalidUrl =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        TcpKsiServiceProtocolInvalidUrl,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        protected static object[] TcpTestCasesInvalidPort =
        {
            new object[]
            {
                new Ksi(
                    new KsiService(
                        TcpKsiServiceProtocolInvalidPort,
                        new ServiceCredentials(Settings.Default.HttpSigningServiceUser, Settings.Default.HttpSigningServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new ServiceCredentials(Settings.Default.HttpExtendingServiceUser, Settings.Default.HttpExtendingServicePass),
                        HttpKsiServiceProtocolInvalidUrls,
                        new PublicationsFileFactory(
                            new PkiTrustStoreProvider(new X509Store(StoreName.Root), CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com")))))
            }
        };

        public static KsiService GetHttpKsiService()
        {
            return new KsiService(
                HttpKsiServiceProtocol,
                new ServiceCredentials("anon", "anon"),
                HttpKsiServiceProtocol,
                new ServiceCredentials("anon", "anon"),
                HttpKsiServiceProtocol,
                new PublicationsFileFactory(new PkiTrustStoreProvider(new X509Store(StoreName.Root),
                    CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com"))));
        }

        [Test, TestCaseSource(typeof(IntegrationTests), nameof(HttpTestCases))]
        public void GetPublicationsFileTest(Ksi ksi)
        {
            IPublicationsFile publicationsFile = ksi.GetPublicationsFile();

            PublicationRecordInPublicationFile latest = publicationsFile.GetLatestPublication();
            if (latest == null)
            {
                Assert.True(true);
                return;
            }

            PublicationRecordInPublicationFile prev = publicationsFile.GetNearestPublicationRecord(latest.PublicationData.PublicationTime - 35 * 24 * 3600);

            Assert.True(latest.PublicationData.PublicationTime > prev.PublicationData.PublicationTime);

            prev = publicationsFile.GetNearestPublicationRecord(Util.ConvertUnixTimeToDateTime(latest.PublicationData.PublicationTime).AddDays(-35));

            Assert.True(latest.PublicationData.PublicationTime > prev.PublicationData.PublicationTime);
        }
    }
}