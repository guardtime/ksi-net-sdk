﻿using System;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Parser
{
    [TestFixture]
    public class ImprintTagTests
    {
        [Test]
        public void TestImprintTagCreateFromTag()
        {
            var rawTag = new RawTag(0x1, false, false,
                new byte[]
                {
                    0x1, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,
                    0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x30, 0x31, 0x32
                });
            var tag = new ImprintTag(rawTag);
            Assert.AreEqual((uint)0x1, tag.Type, "Tag type should be correct");
            Assert.IsFalse(tag.NonCritical, "Tag non critical flag should be correct");
            Assert.IsFalse(tag.Forward, "Tag forward flag should be correct");
            Assert.AreEqual(
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }), tag.Value, "Tag value should be decoded correctly");
            Assert.AreEqual("TLV[0x1]:0x010102030405060708091011121314151617181920212223242526272829303132", tag.ToString(), "Tag string representation should be correct");

            var newTag = new ImprintTag(rawTag);
            Assert.AreEqual(newTag, tag, "Value should be equal");
        }

        [Test]
        public void TestImprintTagEquals()
        {
            var tag = new ImprintTag(0x1, false, false,
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }));
            Assert.AreEqual(
                new ImprintTag(0x1, false, false,
                    new DataHash(HashAlgorithm.Sha2256,
                        new byte[]
                        {
                            0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                            0x28, 0x29, 0x30, 0x31, 0x32
                        })), tag, "Tag Equals function should compare correctly");
            Assert.IsTrue(tag.Equals(tag), "Tags should be equal");
            Assert.IsTrue(
                tag ==
                new ImprintTag(0x1, false, false,
                    new DataHash(HashAlgorithm.Sha2256,
                        new byte[]
                        {
                            0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                            0x28, 0x29, 0x30, 0x31, 0x32
                        })), "Tag should compare correctly with other objects");
            Assert.IsTrue(
                tag !=
                new ChildImprintTag(0x1, false, false,
                    new DataHash(HashAlgorithm.Sha2256,
                        new byte[]
                        {
                            0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                            0x28, 0x29, 0x30, 0x31, 0x32
                        })), "Tag should compare correctly with other objects");
            Assert.IsFalse(tag.Equals(new StringTag(0x1, false, false, "test")), "Tag Equals function should compare correctly with other objects");
            Assert.IsFalse(tag.Equals(new object()), "Tag Equals function should compare correctly with other objects");
        }

        [Test]
        public void TestImprintTagHashCode()
        {
            var tag = new ImprintTag(0x1, false, false,
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }));
            Assert.AreEqual(-1500617533, tag.GetHashCode(), "Hash code should be correct");
        }

        [Test]
        public void TestImprintTagToString()
        {
            var tag = new ImprintTag(0x1, false, false,
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }));
            Assert.AreEqual("TLV[0x1]:0x010102030405060708091011121314151617181920212223242526272829303132", tag.ToString(), "Tag imprint representation should be correct");

            tag = new ImprintTag(0x1, true, true,
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }));
            Assert.AreEqual("TLV[0x1,N,F]:0x010102030405060708091011121314151617181920212223242526272829303132", tag.ToString(), "Tag imprint representation should be correct");
        }

        [Test]
        public void TestStringTagCastToString()
        {
            var tag = new ImprintTag(0x1, false, false,
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }));
            Assert.AreEqual(
                new DataHash(HashAlgorithm.Sha2256,
                    new byte[]
                    {
                        0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
                        0x28,
                        0x29, 0x30, 0x31, 0x32
                    }), tag.Value, "Tag should cast correctly to DataHash");
        }

        [Test]
        public void TestTlvTagCreateFromInvalidEncodeTlvTag()
        {
            Assert.Throws<TlvException>(delegate
            {
                new ImprintTag(new InvalidEncodeTlvTag(0x0, false, false));
            });
        }

        [Test]
        public void TestImprintTagCreateFromNullTag()
        {
            Assert.Throws<TlvException>(delegate
            {
                new ImprintTag((TlvTag)null);
            });
        }

        [Test]
        public void TestImprintTagCreateWithNullValue()
        {
            Assert.Throws<TlvException>(delegate
            {
                new ImprintTag(0x1, true, true, null);
            });
        }

        private class ChildImprintTag : ImprintTag
        {
            public ChildImprintTag(TlvTag tag) : base(tag)
            {
            }

            public ChildImprintTag(uint type, bool nonCritical, bool forward, DataHash value) : base(type, nonCritical, forward, value)
            {
            }
        }
    }
}