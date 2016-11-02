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

using System.IO;
using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature.Verification;
using Guardtime.KSI.Test.Crypto;
using Guardtime.KSI.Test.Properties;
using Guardtime.KSI.Trust;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Service
{
    /// <summary>
    /// Signing tests with static response
    /// </summary>
    [TestFixture]
    public class SigningStaticTests
    {
        /// <summary>
        /// Test signing.
        /// </summary>
        [Test]
        public void SignStaticTest()
        {
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregationResponsePdu)), 1584727637);

            ksi.Sign(new DataHash(Base16.Decode("019f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")));
        }

        /// <summary>
        /// Test signing. PDU v2 response is returned to PDU v1 request.
        /// </summary>
        [Test]
        public void SignStaticInvalidPduResponseVersionTest()
        {
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregationResponsePdu)), 1584727637, PduVersion.v1);

            InvalidRequestFormatException ex = Assert.Throws<InvalidRequestFormatException>(delegate
            {
                ksi.Sign(new DataHash(Base16.Decode("0111A700B0C8066C47ECBA05ED37BC14DCADB238552D86C659342D1D7E87B8772D")));
            });

            Assert.That(ex.Message.StartsWith("Received PDU v2 response to PDU v1 request."), "Unexpected exception message: " + ex.Message);
        }

        /// <summary>
        /// Test signing. Response has multiple payloads
        /// </summary>
        [Test]
        public void SignStaticWithMultiPayloadsResponseTest()
        {
            // Response has multiple payloads (including a payload containing invalid signature, a configuration payload and an acknowledgment payload)
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregationResponsePdu_Multi_Payloads)), 1584727637);

            ksi.Sign(new DataHash(Base16.Decode("019f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")));
        }

        /// <summary>
        /// Test signing and verification fail.
        /// </summary>
        [Test]
        public void SignStaticInvalidSignatureTest()
        {
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregationResponsePdu_Invalid_Signature)), 2);

            KsiSignatureInvalidContentException ex = Assert.Throws<KsiSignatureInvalidContentException>(delegate
            {
                ksi.Sign(new DataHash(Base16.Decode("0111A700B0C8066C47ECBA05ED37BC14DCADB238552D86C659342D1D7E87B8772D")));
            });

            Assert.That(ex.Message.StartsWith("Signature verification failed"), "Unexpected exception message: " + ex.Message);
            Assert.IsNotNull(ex.Signature);
            Assert.AreEqual(VerificationError.Int01.Code, ex.VerificationResult.VerificationError.Code);
        }

        /// <summary>
        /// Test signing with PDU containing multiple payloads including an error payload.
        /// </summary>
        [Test]
        public void SignStaticMultiPayloadsResponseIncludingErrorPayloadTest()
        {
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregationResponsePdu_Multi_Payloads_Including_ErrorPayload)), 2);

            KsiServiceException ex = Assert.Throws<KsiServiceException>(delegate
            {
                ksi.Sign(new DataHash(Base16.Decode("0111A700B0C8066C47ECBA05ED37BC14DCADB238552D86C659342D1D7E87B8772D")));
            });

            Assert.That(ex.Message.StartsWith("Error occured during aggregation. Status: 418464624128; Message: anon"), "Unexpected inner exception message: " + ex.Message);
        }

        /// <summary>
        /// Test signing with PDU containing only an error payload.
        /// </summary>
        [Test]
        public void SignStaticErrorPayloadTest()
        {
            Ksi ksi = GetKsi(File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, Resources.KsiService_AggregatorResponsePdu_ErrorPayload)), 2);

            KsiServiceException ex = Assert.Throws<KsiServiceException>(delegate
            {
                ksi.Sign(new DataHash(Base16.Decode("0111A700B0C8066C47ECBA05ED37BC14DCADB238552D86C659342D1D7E87B8772D")));
            });

            Assert.That(ex.Message.StartsWith("Error occured during aggregation. Status: 258; Message: The request could not be authenticated."),
                "Unexpected exception message: " + ex.Message);
        }

        private static Ksi GetKsi(byte[] requestResult, ulong requestId, PduVersion pduVersion = PduVersion.v2)
        {
            TestKsiServiceProtocol protocol = new TestKsiServiceProtocol
            {
                RequestResult = requestResult
            };

            return new Ksi(new TestKsiService(protocol, new ServiceCredentials("anon", "anon"), protocol, new ServiceCredentials("anon", "anon"), protocol,
                new PublicationsFileFactory(
                    new PkiTrustStoreProvider(new X509Store(StoreName.Root),
                        CryptoTestFactory.CreateCertificateSubjectRdnSelector("E=publications@guardtime.com"))), requestId, pduVersion));
        }
    }
}