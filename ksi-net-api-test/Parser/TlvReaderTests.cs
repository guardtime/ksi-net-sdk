﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Guardtime.KSI.Parser
{
    [TestClass()]
    public class TlvReaderTests
    {
        [TestMethod()]
        public void TestReadTagShort()
        {
            using (var reader = new TlvReader(new MemoryStream(new byte[] { 0x21, 0x4, 0x0, 0x1, 0x2, 0x3 })))
            {
                var tag = reader.ReadTag();
                Assert.AreEqual(new RawTag(0x1, false, true, new byte[] { 0x0, 0x1, 0x2, 0x3 }), tag, "Reader should output correct tag");
            }
        }

        [TestMethod()]
        public void ReadTagShortWithLongTypeTest()
        {
            using (var reader = new TlvReader(new MemoryStream(new byte[] { 0xa0, 0x33, 0x0, 0x4, 0x0, 0x1, 0x2, 0x3 })))
            {
                Assert.AreEqual(new RawTag(0x33, false, true, new byte[] { 0x0, 0x1, 0x2, 0x3 }), reader.ReadTag(), "Reader should output correct byte array");
            }
        }

        [TestMethod()]
        public void TestReadTagLongWithShortType()
        {

            var data = new byte[260];
            data[0] = 0xe0;
            data[1] = 0x1;
            data[2] = 0x1;
            data[3] = 0x0;
            Array.Copy(new byte[256], 0, data, 4, 256);
            using (var reader = new TlvReader(new MemoryStream(data)))
            {
                Assert.AreEqual(new RawTag(0x1, true, true, new byte[256]), reader.ReadTag(), "Reader should output correct byte array");
            }
        }

        [TestMethod()]
        public void TestReadTagLongWithLongType()
        {
            var data = new byte[260];
            data[0] = 0xe2;
            data[1] = 0x57;
            data[2] = 0x1;
            data[3] = 0x0;
            Array.Copy(new byte[256], 0, data, 4, 256);

            using (var reader = new TlvReader(new MemoryStream(data)))
            {
                Assert.AreEqual(new RawTag(0x257, true, true, new byte[256]), reader.ReadTag(), "Reader should output correct byte array");
            }
        }
    }
}