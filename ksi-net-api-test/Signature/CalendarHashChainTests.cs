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
using Guardtime.KSI.Parser;
using NUnit.Framework;

namespace Guardtime.KSI.Signature
{
    [TestFixture]
    public class CalendarHashChainTests
    {
        [Test]
        public void TestCalendarHashChainOk()
        {
            CalendarHashChain calendarHashChain = GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Ok);
            Assert.AreEqual(26, calendarHashChain.Count, "Invalid amount of child TLV objects");
        }

        [Test]
        public void TestCalendarHashChainOkMissingOptionals()
        {
            CalendarHashChain calendarHashChain = GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Ok_Missing_Optionals);
            Assert.AreEqual(25, calendarHashChain.Count, "Invalid amount of child TLV objects");
        }

        [Test]
        public void TestCalendarHashChainInvalidType()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Type);
            }, "Invalid calendar hash chain type: 2051");
        }

        [Test]
        public void TestCalendarHashChainInvalidExtraTag()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Extra_Tag);
            }, "Invalid tag");
        }

        [Test]
        public void TestCalendarHashChainInvalidMissingInputHash()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Missing_Input_Hash);
            }, "Only one input hash must exist in calendar hash chain");
        }

        [Test]
        public void TestCalendarHashChainInvalidMissingLinks()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Missing_Links);
            }, "Links are missing in calendar hash chain");
        }

        [Test]
        public void TestCalendarHashChainInvalidMissingPublicationTime()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Missing_Publication_Time);
            }, "Only one publication time must exist in calendar hash chain");
        }

        [Test]
        public void TestCalendarHashChainInvalidMultipleAggregationTime()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Multiple_Aggregation_Time);
            }, "Only one aggregation time is allowed in calendar hash chain");
        }

        [Test]
        public void TestCalendarHashChainInvalidMultipleInputHash()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Multiple_Input_Hash);
            }, "Only one input hash must exist in calendar hash chain");
        }

        [Test]
        public void TestCalendarHashChainInvalidMultiplePublicationTime()
        {
            Assert.Throws<TlvException>(delegate
            {
                GetCalendarHashChainFromFile(Properties.Resources.CalendarHashChain_Invalid_Multiple_Publication_Time);
            }, "Only one publication time must exist in calendar hash chain");
        }

        private static CalendarHashChain GetCalendarHashChainFromFile(string file)
        {
            using (TlvReader reader = new TlvReader(new FileStream(file, FileMode.Open)))
            {
                CalendarHashChain calendarHashChain = new CalendarHashChain(reader.ReadTag());

                return calendarHashChain;
            }
        }
    }
}