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
using Guardtime.KSI.Crypto;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Test.Properties;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Trust
{
    [TestFixture]
    public class Pkcs7CryptoSignatureVerifierTests
    {
        [Test]
        public void SignedBytesNullTest()
        {
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                verifier.Verify(null, Base16.Decode(encodedSignature), new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
            });

            Assert.That(ex.ParamName == "signedBytes", "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void SignatureBytesNullTest()
        {
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                verifier.Verify(Base16.Decode("01"), null, new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
            });

            Assert.That(ex.ParamName == "signatureBytes", "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void NoTrustAnchorsTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                verifier.Verify(Base16.Decode(encodedSignedBytes), Base16.Decode(encodedSignature), null);
            });

            Assert.That(ex.Message.StartsWith("No trust anchors given."), "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            verifier.Verify(Base16.Decode(encodedSignedBytes),
                Base16.Decode(encodedSignature),
                new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
        }

        [Test]
        public void VerifyWithMultiSignerInfosTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            verifier.Verify(Base16.Decode(encodedSignedBytes),
                Base16.Decode(encodedSignature),
                new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
        }

        [Test]
        public void VerifyWithRandomSignatureBytesTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature = "0102030405";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            PkiVerificationErrorException ex = Assert.Throws<PkiVerificationErrorException>(delegate
            {
                verifier.Verify(Base16.Decode(encodedSignedBytes),
                    Base16.Decode(encodedSignature),
                    new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
            });

            // separate error messages for Microsoft and Bouncy Castle
            Assert.That(ex.Message.StartsWith("Error when verifying PKCS#7 signature") || ex.Message.StartsWith("Cannot create signature from signatureBytes"),
                "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyWithRandomCertBytesTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            string encodedCert = "0102030405";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            PkiVerificationErrorException ex = Assert.Throws<PkiVerificationErrorException>(delegate
            {
                verifier.Verify(Base16.Decode(encodedSignedBytes),
                    Base16.Decode(encodedSignature),
                    new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
            });

            Assert.That(ex.Message.StartsWith("Cannot create trust anchor certificate from CertificateBytes"), "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyWithTimeTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
            verifier.Verify(Base16.Decode(encodedSignedBytes),
                Base16.Decode(encodedSignature),
                new CryptoSignatureVerificationData(Base16.Decode(encodedCert), Util.ConvertDateTimeToUnixTime(new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc))));
        }

        [Test]
        public void VerifyWithTimeExpiredTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            PkiVerificationFailedCertNotValidException ex = Assert.Throws<PkiVerificationFailedCertNotValidException>(delegate
            {
                ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
                verifier.Verify(Base16.Decode(encodedSignedBytes),
                    Base16.Decode(encodedSignature),
                    // test with time 2015.12.31
                    new CryptoSignatureVerificationData(Base16.Decode(encodedCert), Util.ConvertDateTimeToUnixTime(new DateTime(2015, 12, 31))));
            });

            Assert.That(ex.Message.StartsWith("PKCS#7 signature certificate is not valid"), "Unexpected exception message: " + ex.Message);
            Assert.That(ex.InnerException.Message.StartsWith("Certificate not valid at"), "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyWithModifiedSignedBytesTest()
        {
            string encodedModifiedSignedBytes = "0029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            // valid from 2016.01.01 - 2026.01.01
            string encodedCert =
                "308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C6420";

            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000FA0A522D88F59EFD6B8D79217B94C7300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B0500038181000E2B265923ED37069FEA637264A3A9D8CBFD14A6732D380B54C61FADAF4F94DBE99E51409E25535896D9EB77328AF92B572E3E037AA0E052E89A067A77F448A0DFCDB432A45EED103B0182835638C048BE0668AD659E2E93DA056E44BC8180F9637D797E1072B4F99684EC5BEBF47E7194FB2A33B95CD08B6D2932303B8EFD2100003182013B30820137020101302630123110300E06035504030C0774657374696E67021000FA0A522D88F59EFD6B8D79217B94C7300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313131333133313831335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D0101010500048180990B1C530CAED23935CB286774C6089AEA1E5593AEFCE56284517B1BEB00AE2F5C90212CD9ACD95A974F11F8FC68A53E16E2471682E61CDB461BA59037F56CBB6E55CA6B1A8A49B5A316A2B45274DB36E75CB1E0403A7C0B6DBEA52F0EBAA843E64CD97B878B216E1F0656B747A925E337EA375A3885623205351FC3721D5F1D000000000000";

            PkiVerificationFailedException ex = Assert.Throws<PkiVerificationFailedException>(delegate
            {
                ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
                verifier.Verify(Base16.Decode(encodedModifiedSignedBytes),
                    Base16.Decode(encodedSignature),
                    new CryptoSignatureVerificationData(Base16.Decode(encodedCert)));
            });

            Assert.That(
                ex.Message.StartsWith("Failed to verify PKCS#7 signature")
                && (
                    // separate error messages for Microsoft and Bouncy Castle
                    ex.InnerException.Message.StartsWith("The hash value is not correct") ||
                    ex.InnerException.Message.StartsWith("message-digest attribute value does not match calculated value")
                    ),
                "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyWithInvalidCertTest()
        {
            string encodedSignedBytes = "3029020456C0D6A904210187EBC5594827DC2B3F87918DC7AFE8A528E844D65918CD525984D65981C2C79A";
            string encodedSignature =
                "308006092A864886F70D010702A0803080020101310F300D06096086480165030402010500308006092A864886F70D0107010000A080308201A730820110A003020102021000BAA25764984826073CEF1BEEAA7B6F300D06092A864886F70D01010B050030123110300E06035504030C0774657374696E67301E170D3136303130313030303030305A170D3236303130313030303030305A30123110300E06035504030C0774657374696E6730819F300D06092A864886F70D010101050003818D0030818902818100E66DC137E4F856EADB0D47C280BED297D70191287919FD6EBF1195DF5E821EA867F861E551A37762E3CAEBB32B1DE7E0143529F1678A87BCE2C8E5D5185F25EEC3ABC7E295EEBC64EFE4BC8ADB412A99D3F9125D30C45F887632DE4B95AA169B79D1A6FD4E735255632341ED41B5BFA828975A4F1501B02C2277CA15BD470DAB0203010001300D06092A864886F70D01010B05000381810057B6B2BED11E7DC06E7A7AD77D1922C33FE8BDEBDC48B20A9444F807AA6017890E10B21C6FA5C4795AF32B9A45E9C560580115C8DF09E4ACA8E69C2757F9CAAB3007B548A567DBE8FB228EFF4F3D7995B3B2F008F3D62BECB3D2E80827A17B42B9C8C8C0280565868D572BBE7E4916E4888D8F503BEC1561CF4A1495C43C642000003182013B30820137020101302630123110300E06035504030C0774657374696E67021000BAA25764984826073CEF1BEEAA7B6F300D06096086480165030402010500A069301806092A864886F70D010903310B06092A864886F70D010701301C06092A864886F70D010905310F170D3137313130393133333235335A302F06092A864886F70D01090431220420F083925226E0D857E5320AB3544F754B2FA74B364268BC755987C518083D2940300D06092A864886F70D01010105000481809BC20BF93B9190207D585A4638AFBB3BCDB740E6030F1B56A922E7E1B25024F3DCE9F2B63B693281E5056D7BA4553814B1FF8E4A38955DAF0FA9B26D468B29D8137E1EE0FAF3A9D8FD1E5EBFF0560C51684B93735E3D1A2313484AF62EC2FD39BA7BA2781818BB38BFE2D33C2C23F74AB4F43824D4209E3DA211F1DB990DCD0D000000000000";

            ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();

            PkiVerificationFailedException ex = Assert.Throws<PkiVerificationFailedException>(delegate
            {
                verifier.Verify(Base16.Decode(encodedSignedBytes),
                    Base16.Decode(encodedSignature),
                    new CryptoSignatureVerificationData(GetFileBytes(Resources.PkiTrustProvider_CustomCertInvalid)));
            });

            // separate error messages for Microsoft and Bouncy Castle
            Assert.That(ex.Message.StartsWith("Trust chain did not complete to the known authority anchor. Thumbprints did not match.") ||
                        (ex.Message.StartsWith("Could not build certificate path") &&
                         ex.InnerException.Message.StartsWith("Unable to find certificate chain.")),
                "Unexpected exception message: " + ex.Message);
        }

        [Test]
        public void VerifyWithPublicationsFileCertTest()
        {
            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.PkiTrustProvider_PubsFileCustomCert), FileMode.Open, FileAccess.Read))
            {
                PublicationsFile pubsFile = new PublicationsFileFactory(new TestPkiTrustProvider()).Create(stream) as PublicationsFile;
                ICryptoSignatureVerifier verifier = KsiProvider.CreatePkcs7CryptoSignatureVerifier();
                verifier.Verify(pubsFile.GetSignedBytes(), pubsFile.GetSignatureValue(), new CryptoSignatureVerificationData(GetFileBytes(Resources.PkiTrustProvider_CustomCert)));
            }
        }

        private byte[] GetFileBytes(string path)
        {
            return File.ReadAllBytes(Path.Combine(TestSetup.LocalPath, path));
        }
    }
}