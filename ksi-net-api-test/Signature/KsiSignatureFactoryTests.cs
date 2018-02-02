﻿/*
 * Copyright 2013-2017 Guardtime, Inc.
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
using System.IO;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Signature.Verification;
using Guardtime.KSI.Signature.Verification.Policy;
using Guardtime.KSI.Test.Properties;
using Guardtime.KSI.Test.Signature.Verification;
using Guardtime.KSI.Test.Signature.Verification.Rule;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Signature
{
    [TestFixture]
    public class KsiSignatureFactoryTests
    {
        [Test]
        public void CreateFromStreamFromNullInvalidTest()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new KsiSignatureFactory().Create((Stream)null);
            });
            Assert.AreEqual("stream", ex.ParamName);
        }

        [Test]
        public void CreateFromStreamTest()
        {
            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Ok), FileMode.Open))
            {
                new KsiSignatureFactory().Create(stream);
            }
        }

        [Test]
        public void CreateFromStreamAndVerifyInvalidSignatureTest()
        {
            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Invalid_Aggregation_Chain_Input_Hash), FileMode.Open))
            {
                KsiSignatureInvalidContentException ex = Assert.Throws<KsiSignatureInvalidContentException>(delegate
                {
                    new KsiSignatureFactory().Create(stream);
                });

                Assert.That(ex.Message.StartsWith("Signature verification failed"), "Unexpected exception message: " + ex.Message);
                Assert.IsNotNull(ex.Signature);
            }
        }

        [Test]
        public void CreateFromStreamAndVerifyWithPolicyInvalidTest()
        {
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory(new PublicationBasedVerificationPolicy(),
                new TestVerificationContext()
                {
                    UserPublication = new PublicationData("AAAAAA-CVZ2AQ-AANGVK-SV7GJL-36LN65-AVJYZR-6XRZSL-HIMRH3-6GU7WR-YNRY7C-X2XECY-WFQXRB")
                });

            KsiSignatureInvalidContentException ex = Assert.Throws<KsiSignatureInvalidContentException>(delegate
            {
                // Check invalid signature
                using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Ok_With_Publication_Record), FileMode.Open))
                {
                    signatureFactory.Create(stream);
                }
            });

            Assert.AreEqual(VerificationError.Pub04.Code, ex.VerificationResult.VerificationError.Code, "Unexpected result code");
        }

        [Test]
        public void CreateFromStreamAndDoNotVerifyInvalidSignatureTest()
        {
            // Do not verify invalid signature
            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Invalid_Aggregation_Chain_Index_Mismatch), FileMode.Open))
            {
                new KsiSignatureFactory(new EmptyVerificationPolicy()).Create(stream);
            }
        }

        [Test]
        public void CreateFromAggregationResponsePayloadTest()
        {
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory();

            // corrseponding hash: "01D439459856BEF5ED25772646F73A70A841FC078D3CBBC24AB7F47C464683768D"
            AggregationResponsePayload aggregationResponsePayload =
                new AggregationResponsePayload(new RawTag(0x2, false, false,
                    Base16.Decode(
                        "01048BEAA4F5040005094E6F206572726F720088010067020457EBC32A03010B030206FD030203EF030103030103052101D439459856BEF5ED25772646F73A70A841FC078D3CBBC24AB7F47C464683768D060104072804267E0101610A616E6F6E206874747000620A616E6F6E3A687474700063006407053D913449EE6788010074020457EBC32A03010B030206FD030203EF0301030531046DF9DD7721D40C66006D89EAC90E25B97C4FD9B4B08D29E583110CD257A68C80F46FECE13F3C27C5F5591184CEEBEA3A060101072804267E0101610974616176695F616C00620B746573743A74616176690063006407053D91344F716688010186020457EBC32A03010B030206FD030203EF0521012D9B46B0D304DE23054347ED9851614BF14C868DB0870BECB9BE584A506309D0060101072201011B031D03000A73746167696E6720415300000000000000000000000000000000072601010702210100000000000000000000000000000000000000000000000000000000000000000723022101000000000000000000000000000000000000000000000000000000000000000007230221010000000000000000000000000000000000000000000000000000000000000000082302210126B310372170090DEB38DCE88367B8064A7D88C265EF6AAADE765EDF2D576D4A0723022101000000000000000000000000000000000000000000000000000000000000000007230221015C71338639B0B208B40475C7FF7C9F21F1FD3E758B345DB730A2537D8C9758FE07230221015F5F44AE22506BCCEBCDCFA4AE93689FE80BC571DAD43ADBC172404C146E30D007230221010000000000000000000000000000000000000000000000000000000000000000880101A7020457EBC32A03010B030206FD05210136B0066278D7C3D2FDEC4F7F517F4B16E66CA055AAC499DD1D9A7FD21605C8250601010722010105031D0300024754000000000000000000000000000000000000000000000000082601010702210182FF351DE0856A77845D580306AA1BBC500EF8223B61C03D8F8F275B7AA261E107230221010CBD37B5B45378082EB14D097CC9C403CCB89A1D9728FF9FC5496439206ABBAE0723022101000000000000000000000000000000000000000000000000000000000000000007230221018AF735B2A92BA3CC25269A35EA456C911F2DBF6C2B8DE730342E784318EF1362072302210138FFF691B2797C32F4ABC598F6EAE177FDD7778E14196EEDDEEF0C67F3D48E1C07230221010000000000000000000000000000000000000000000000000000000000000000072302210100000000000000000000000000000000000000000000000000000000000000000823022101000000000000000000000000000000000000000000000000000000000000000007230221013E7844503DD1B7976867CB40A55E3164AFF3042A872ACE06122134F927FDAE2A880100A4020457EBC32A03010B0521010B391E1BA3936CAD575F41B2CC025C4759BE011529B21680CA00A237FE38CC7B060101072601010F022101FBD0152F46E4C3ABDECAE417E92CE92DA99DC6FB636DBA23F0E56B1FD7B27E32072601012C02210137C1DEF4BD109E8F72FF4861ECFA134CC37B72623D4AD7F05F4025941EA07CF4082302210165A50BF7000E4F21108351A98C0C56D7C3213CDC4C5B345B1805C691988B2244880202A5010457EBC32A020457EBC32A0521019A96779C533C41659D5E9E783263A7633DEEC1404259B7A24EFC949E8B278B1A082101B889794E8768629FB66A22DEB05BEA7B37BF0ACB79A453462CE35E93BEC5FC8E0821011EB614D293CE2CF75161057FCF97AE1F37217150C6A90F68BB0161EC096BD2FE082101109323AFF6789E0ADE4F702AA08B671A52BE6044F4D1993E601D5328B7323CCD082101583CA8A1582CE31621C353FBC8BFCFAA09B7497474BCCD3AEBB1925FBF3DFB94082101518696A269EAB9FF1A32708F08AED4C9F3132F031F2DB7DEF9784A61E405E6FE08210192FEA847A3CD0E379A1CFB644082A55DD3702AA9A0671D373793C3163798DD920821011134FA5E508AA43C41BC5A99FA7D72880FF3E42BAB859727ED3C8ECA888F742B08210188B4845368AC24366CB24CF9BD1B14466AA4E196D39F17D63EA1B22203F51F060821019B81E3AECABEBC50A6B4DD597E5C1B56EEF1CE272E0C8F261ED14C0E9D709872082101293EF4708AEAE8D8CF6D384F1F07CD27D327F4AA2DDD530DD086E992A033C84F082101F0367ECE12995DF333C6CE54CD72B1644E6F24FAB6195EA990A2D1E6B015CF56082101D3CB30C04CEBF26EF00FA468F69443A29F5B238369CFB8280DA3394FA385C7EF0821014C23C4855510FAE15EFEBE246A01260D96C470DEBB50C63F88B2508FD6DC343708210129CB7DB7FE0C51D0F1E8B9663413AED76D7D89AC832F0D21981AE6687138CB21082101A0698E6B45EDEEAF9037E49F668114617CA60124F0FC416D017D06D78CA4295A082101A6F082B82280F3A6AFB14C8E39B7F57860B857B70CA57AFD35F40395EEB32458082101496FC0120D854E7534B992AB32EC3045B20D4BEE1BFBE4564FD092CEAFA08B72082101BB44FD36A5F3CDEE7B5C6DF3A6098A09E353335B6029F1477502588A7E37BE00880501513029020457EBC32A042101CD4BCCC347C0B07C0BAE7C9143D96B4F312E312ABB96225E89E939313863F0D4800B01220116312E322E3834302E3131333534392E312E312E31310080020100926FE3A4D7A4A2FFFF72D7A6BDE7872B4309DBCBD13CA35A3511FFF19DFC655005F55701BE52A15AEC7BEC629C5E7BA88AAFA7EAFED5D86869C2C21277DB9E65067E798378275795F5008CED21E1E527A1FC50DD90B2E03566A2E15745D406643FF6A33FCE4380CB27BA336125CF84BEE4C95D5A079266D2916DBED9AF38A4868B1E5C2E37C0355D50F3CEA37FC56CC4CB42E0709D0046C87F2D9936CB31F9D924647F4D184F64922664EC8D04096CE1AAD997DE98102B6499F31240A76081C6D7EA14819F1594502F3B4326D657C90969AC16A2EF722455E656C3EC6C0BF81EE7CB4EDF017F9585ECCF3046D72CEF855E2AF068BC92D2C96E73D9B73E21DB1C0304644C740D")));
            signatureFactory.Create(aggregationResponsePayload, new DataHash(Base16.Decode("01D439459856BEF5ED25772646F73A70A841FC078D3CBBC24AB7F47C464683768D")));
        }

        [Test]
        public void CreateFromPartsTest()
        {
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory();
            IKsiSignature signature;

            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Ok), FileMode.Open))
            {
                signature = new KsiSignatureFactory().Create(stream);
            }

            IKsiSignature newSignature = signatureFactory.Create(signature.GetAggregationHashChains(), signature.CalendarHashChain, signature.CalendarAuthenticationRecord,
                signature.PublicationRecord,
                signature.Rfc3161Record, signature.InputHash);

            Assert.AreEqual(signature.EncodeValue(), newSignature.EncodeValue(), "Signatures should be equal.");
        }

        [Test]
        public void CreateFromPartsWithoutCalendarHashChainFailTest()
        {
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory();
            IKsiSignature signature;

            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignature_Ok), FileMode.Open))
            {
                signature = new KsiSignatureFactory().Create(stream);
            }

            // create signature without calendar hash chain but with calendar auth record.
            TlvException ex = Assert.Throws<TlvException>(delegate
            {
                signatureFactory.Create(signature.GetAggregationHashChains(), null, signature.CalendarAuthenticationRecord, signature.PublicationRecord,
                    signature.Rfc3161Record, signature.InputHash);
            });

            Assert.That(ex.Message, Does.StartWith("No publication record or calendar authentication record is allowed in KSI signature if there is no calendar hash chain"));
        }

        [Test]
        public void CreateSignatureWithAggregationChainUsingMetadataAndLevelsTest()
        {
            // Base signature input hash: {SHA-256:[017238818675AC5DD8879E292BF8C6A11AF5B5F68EF61661580288E50EEA40F2C8]} with level of 5

            /*                                5A848EE
                                    /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\
                                 014024AA 1                     \
                          /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\              \
                       0178BAFB 1              01979985           \
                     /‾‾‾‾‾‾‾‾‾‾\           /‾‾‾‾‾‾‾‾‾‾‾‾\         \
                04000000       test3   02000000 1       test2    05000000 4
            */
            IKsiSignature signature = TestUtil.GetSignature(Resources.KsiSignature_Ok_LevelCorrection5);

            CreateSignatureWithAggregationChainAndVerify(
                signature,
                new DataHash(Base16.Decode("04000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")),
                new AggregationHashChain.Link[]
                {
                    new AggregationHashChain.Link(LinkDirection.Left, null, new AggregationHashChain.Metadata("test3"), 0),
                    new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("01979985EED807EC9E036D679D327B7BEFF0CA0D127524B0AD6EC37414EBE96258")), null, 1),
                    new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("0500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")), null, 1)
                });

            CreateSignatureWithAggregationChainAndVerify(
                signature,
                new DataHash(Base16.Decode("020000000000000000000000000000000000000000")),
                new AggregationHashChain.Link[]
                {
                    new AggregationHashChain.Link(LinkDirection.Left, null, new AggregationHashChain.Metadata("test2", "machine-id-1", 1, 1517236554764), 1),
                    new AggregationHashChain.Link(LinkDirection.Right, new DataHash(Base16.Decode("0178BAFB1F3AF73B661F9C7B4ADC30DE5A7B715184A4543B200694101C1A8C0E02"))),
                    new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("0500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")), null, 1)
                });

            CreateSignatureWithAggregationChainAndVerify(
                signature,
                new DataHash(Base16.Decode("0500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")),
                new AggregationHashChain.Link[]
                {
                    new AggregationHashChain.Link(LinkDirection.Right, new DataHash(Base16.Decode("014024AA0B2367EBBC3E27B3C498B800C48AFD9A2623C1458A07D2D0ABA4387B3B")), null, 4)
                });

            CreateSignatureWithAggregationChainAndVerify(
                signature,
                new DataHash(Base16.Decode("0500000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")),
                new AggregationHashChain.Link[]
                {
                    new AggregationHashChain.Link(LinkDirection.Right, new DataHash(Base16.Decode("014024aa0b2367ebbc3e27b3c498b800c48afd9a2623c1458a07d2d0aba4387b3b")), null, 4)
                });
        }

        [Test]
        public void CreateSignatureWithAggregationChainFailWrongLevelCorrectionTest()
        {
            IKsiSignature signature = TestUtil.GetSignature(Resources.KsiSignature_Ok_LevelCorrection3);

            // new chain does not fit, base signature first level correction is not big enough
            KsiException ex1 = Assert.Throws<KsiException>(delegate
            {
                CreateSignatureWithAggregationChainAndVerify(
                    signature,
                    new DataHash(Base16.Decode("01580192B0D06E48884432DFFC26A67C6C685BEAF0252B9DD2A0B4B05D1724C5F2")),
                    new AggregationHashChain.Link[]
                    {
                        new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("018D982C6911831201C5CF15E937514686A2169E2AD57BA36FD92CBEBD99A67E34")), null, 1),
                        new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("0114F9189A45A30D856029F9537FD20C9C7342B82A2D949072AB195D95D7B32ECB"))),
                        new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("01D4F6E36871BA12449CA773F2A36F9C0112FC74EBE164C8278D213042C772E3AB"))),
                    });
            });

            Assert.That(ex1.Message.StartsWith(
                "The aggregation hash chain cannot be added as lowest level chain. It's output level (4) is bigger than level correction of the first link of the first aggregation hash chain of the base signature (3)"),
                "Unexpected exception message: " + ex1.Message);

            // new chain does not fit, base signature first level correction is not big enough
            KsiException ex2 = Assert.Throws<KsiException>(delegate
            {
                CreateSignatureWithAggregationChainAndVerify(
                    signature,
                    new DataHash(Base16.Decode("019D982C6911831201C5CF15E937514686A2169E2AD57BA36FD92CBEBD99A67E32")),
                    new AggregationHashChain.Link[]
                    {
                        new AggregationHashChain.Link(LinkDirection.Right, new DataHash(Base16.Decode("01680192B0D06E48884432DFFC26A67C6C685BEAF0252B9DD2A0B4B05D1724C5F1")), null,
                            2),
                        new AggregationHashChain.Link(LinkDirection.Right, new DataHash(Base16.Decode("015950DCA0E23E65EF56D68AF94718951567EBC2EF1F54357732530FC25D925340"))),
                    });
            });

            Assert.That(ex2.Message.StartsWith(
                "The aggregation hash chain cannot be added as lowest level chain. It's output level (4) is bigger than level correction of the first link of the first aggregation hash chain of the base signature (3)"),
                "Unexpected exception message: " + ex1.Message);
        }

        [Test]
        public void CreateSignatureWithAggregationChainFailHashMismatchTest()
        {
            // cannot add new aggregation hash chain, it's output hash and base signature input hash mismatch
            KsiException ex = Assert.Throws<KsiException>(delegate
            {
                CreateSignatureWithAggregationChainAndVerify(
                    TestUtil.GetSignature(Resources.KsiSignature_Ok_LevelCorrection3),
                    new DataHash(Base16.Decode("01580192B0D06E48884432DFFC26A67C6C685BEAF0252B9DD2A0B4B05D1724C5F2")),
                    new AggregationHashChain.Link[]
                    {
                        new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("0114F9189A45A30D856029F9537FD20C9C7342B82A2D949072AB195D95D7B32ECB"))),
                        new AggregationHashChain.Link(LinkDirection.Left, new DataHash(Base16.Decode("01D4F6E36871BA12449CA773F2A36F9C0112FC74EBE164C8278D213042C772E3AB"))),
                    });
            });

            Assert.That(ex.Message.StartsWith("The aggregation hash chain cannot be added as lowest level chain. It's output hash does not match base signature input hash"),
                "Unexpected exception message: " + ex.Message);
        }

        private static void CreateSignatureWithAggregationChainAndVerify(IKsiSignature signature, DataHash inputHash, AggregationHashChain.Link[] links,
                                                                         string expectedVerificationErrorCode = null)
        {
            IKsiSignature newSignature = new KsiSignatureFactory(
                new EmptyVerificationPolicy()).CreateSignatureWithAggregationChain(signature, inputHash, HashAlgorithm.Sha2256, links);
            VerificationResult result = new InternalVerificationPolicy().Verify(new VerificationContext(newSignature) { DocumentHash = inputHash });

            if (string.IsNullOrEmpty(expectedVerificationErrorCode))
            {
                Assert.AreEqual(VerificationResultCode.Ok, result.ResultCode, "Unexpected verification result");
            }
            else
            {
                Assert.AreEqual(VerificationResultCode.Fail, result.ResultCode, "Unexpected verification result");
                Assert.AreEqual(expectedVerificationErrorCode, result.VerificationError.Code, "Unexpected verification error code");
            }
        }
    }
}