﻿using NUnit.Framework;
using Guardtime.KSI.Signature.Verification.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Service;

namespace Guardtime.KSI.Signature.Verification.Rule
{
    [TestFixture()]
    public class PublicationsFilePublicationHashMatchesExtenderResponseRuleTests
    {
        [Test()]
        public void TestVerify()
        {
            var rule = new PublicationsFilePublicationHashMatchesExtenderResponseRule();

            // Argument null exception when no context
            Assert.Throws<ArgumentNullException>(delegate
            {
                rule.Verify(null);
            });

            // Verification exception on missing KSI signature
            Assert.Throws<KsiVerificationException>(delegate
            {
                var context = new TestVerificationContext();

                rule.Verify(context);
            });

            // Invalid publications file in context: null
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok, FileMode.Open))
            {
                var context = new TestVerificationContext()
                {
                    Signature = KsiSignature.GetInstance(stream)
                };

                Assert.Throws<KsiVerificationException>(delegate
                {
                    rule.Verify(context);
                });
            }

            PublicationsFile publicationsFile;
            using (var stream = new FileStream("resources/publication/publicationsfile/ksi-publications.bin", FileMode.Open))
            {
                publicationsFile = PublicationsFile.GetInstance(stream);
            }

            // Check invalid publications recordin signature: null
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok_Missing_Calendar_Hash_Chain, FileMode.Open))
            {
                var context = new TestVerificationContextFaultyFunctions()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile
                };

                Assert.Throws<KsiVerificationException>(delegate
                {
                    rule.Verify(context);
                });
            }

            var serviceProtocol = new TestKsiServiceProtocol();
            var ksiService = new KsiService(serviceProtocol, serviceProtocol, serviceProtocol, new ServiceCredentials("anon", "anon"), new PublicationsFileFactory());

            // Check invalid extended calendar chain from context function: null
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok, FileMode.Open))
            {
                var context = new TestVerificationContextFaultyFunctions()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile,
                    KsiService = ksiService
                };

                Assert.Throws<KsiVerificationException>(delegate
                {
                    rule.Verify(context);
                });
            }

            // Check no publication found after current signature
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok_New, FileMode.Open))
            {
                var context = new TestVerificationContext()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile,
                    KsiService = ksiService
                };

                Assert.Throws<KsiVerificationException>(delegate
                {
                    rule.Verify(context);
                });
            }

            // Check legacy signature
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Legacy_Ok_With_Publication_Record, FileMode.Open))
            {
                var context = new TestVerificationContext()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile,
                    KsiService = ksiService
                };

                Assert.AreEqual(VerificationResult.Ok, rule.Verify(context));
            }

            // Check signature
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok_With_Publication_Record, FileMode.Open))
            {
                var context = new TestVerificationContext()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile,
                    KsiService = ksiService
                };

                Assert.AreEqual(VerificationResult.Ok, rule.Verify(context));
            }

            // Check invalid signature for hash found from extender message
            using (var stream = new FileStream(Properties.Resources.KsiSignatureDo_Ok_With_Publication_Record, FileMode.Open))
            {
                var context = new TestVerificationContext()
                {
                    Signature = KsiSignature.GetInstance(stream),
                    PublicationsFile = publicationsFile,
                    KsiService = ksiService
                };

                serviceProtocol.FailNext = true;
                Assert.AreEqual(VerificationResult.Fail, rule.Verify(context));
            }
        }
    }
}