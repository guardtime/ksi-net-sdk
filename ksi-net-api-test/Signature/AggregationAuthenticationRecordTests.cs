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
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Signature;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Signature
{
    [TestFixture]
    public class AggregationAuthenticationRecordTests
    {
        [Test]
        public void TestAggregationAuthenticationRecordOk()
        {
            AggregationAuthenticationRecord aggregationAuthenticationRecord = GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Ok);
            Assert.AreEqual(5, aggregationAuthenticationRecord.Count, "Invalid amount of child TLV objects");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidExtraTag()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Extra_Tag);
            }, "Invalid tag");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMissingAggregationTime()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Missing_Aggregation_Time);
            }, "Only one aggregation time must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMissingChainIndex()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Missing_Chain_Index);
            }, "Chain indexes must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMissingInputHash()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Missing_Input_Hash);
            }, "Only one input hash must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMissingSignatureData()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Missing_Signature_Data);
            }, "Only one signature data must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMultipleAggregationTime()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Multiple_Aggregation_Time);
            }, "Only one aggregation time must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMultipleInputHash()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Multiple_Input_Hash);
            }, "Only one input hash must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidMultipleSignatureData()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Multiple_Signature_Data);
            }, "Only one signature data must exist in aggregation authentication record");
        }

        [Test]
        public void TestAggregationAuthenticationRecordInvalidType()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetAggregationAuthenticationRecordFromFile(Properties.Resources.AggregationAuthenticationRecord_Invalid_Type);
            }, "Invalid aggregation authentication record type: 2053");
        }

        [Test]
        public void ToStringTest()
        {
            AggregationAuthenticationRecord tag = TestUtil.GetCompositeTag<AggregationAuthenticationRecord>(Constants.AggregationAuthenticationRecord.TagType,
                new ITlvTag[]
                {
                    new IntegerTag(Constants.AggregationAuthenticationRecord.AggregationTimeTagType, false, false, 1),
                    new IntegerTag(Constants.AggregationAuthenticationRecord.ChainIndexTagType, false, false, 0),
                    new ImprintTag(Constants.AggregationAuthenticationRecord.InputHashTagType, false, false,
                        new DataHash(HashAlgorithm.Sha2256,
                            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 })),
                    TestUtil.GetCompositeTag<SignatureData>(Constants.SignatureData.TagType,
                        new ITlvTag[]
                        {
                            new StringTag(Constants.SignatureData.SignatureTypeTagType, false, false, "Test SignatureType"),
                            new RawTag(Constants.SignatureData.SignatureValueTagType, false, false, new byte[] { 0x2 }),
                            new RawTag(Constants.SignatureData.CertificateIdTagType, false, false, new byte[] { 0x3 }),
                            new StringTag(Constants.SignatureData.CertificateRepositoryUriTagType, false, false, "Test CertificateRepositoryUri")
                        })
                });

            AggregationAuthenticationRecord tag2 = new AggregationAuthenticationRecord(tag);

            Assert.AreEqual(tag.ToString(), tag2.ToString());
        }

        private static AggregationAuthenticationRecord GetAggregationAuthenticationRecordFromFile(string file)
        {
            using (TlvReader reader = new TlvReader(new FileStream(Path.Combine(TestSetup.LocalPath, file), FileMode.Open)))
            {
                AggregationAuthenticationRecord aggregationAuthenticationRecord = new AggregationAuthenticationRecord(reader.ReadTag());

                return aggregationAuthenticationRecord;
            }
        }
    }
}