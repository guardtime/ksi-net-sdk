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
using Guardtime.KSI.Service;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Test.Properties;
using Guardtime.KSI.Utils;
using NUnit.Framework;

namespace Guardtime.KSI.Test.Signature
{
    [TestFixture]
    public class KsiSignatureFactoryTests
    {
        [Test]
        public void CreateFromStreamTest()
        {
            // Test stream
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory();

            Assert.Throws<KsiException>(delegate
            {
                signatureFactory.Create((Stream)null);
            });

            using (FileStream stream = new FileStream(Path.Combine(TestSetup.LocalPath, Resources.KsiSignatureDo_Ok), FileMode.Open))
            {
                signatureFactory.Create(stream);
            }
        }

        [Test]
        public void CreateFromAggregationResponsePayloadTest()
        {
            // Test stream
            KsiSignatureFactory signatureFactory = new KsiSignatureFactory();

            Assert.Throws<KsiException>(delegate
            {
                signatureFactory.Create((AggregationResponsePayload)null);
            });

            AggregationResponsePayload aggregationResponsePayload =
                new AggregationResponsePayload(new RawTag(0x202, false, false,
                    Base16.Decode(
                        "010796314aaea3651e8805015130290204551dd80004210100423db0da0737c5ab475549ca0446493cca7e6cb2ce71d6f92756f697f41b6d800b01220116312e322e3834302e3131333534392e312e312e31310080020100255300542c92f1063d1144d0c708fa9ae195e42f344c102eeffcd37a15880fac1ff4c73613c8115166b15295cccd506bef7451ee9caf8ba59a374968cbe990fd707cb7931837f39c8708b18d91c1c5c7cc3dee862de6b64427ac22a648995722c1bb631d3e615144ec8d30c5be14934e1733cb1d69988807b48d983575ca69af12c9d67d0eabc10695a35e6c1c7abdd1bdfa804c12ad36202f858714176487e0b069d2eab1d7b24a039586f52c337396e45f7bc82e88e1573a3a271c9334c7ff2b86814ae010cb855bc20a03038c74ae4236c9f7ceea3894cbc2be88d5799d95d909ac3ea57cea2bef778f75ad9fd29a08593eee8b9f0f36ebe99b2ae2e122bb0304644c740d880203770104551dd8000204551d05c9052101ad56fc341dfb5779335a3412c0abee8a67a5035325ff01f8d393559286f77872082101684c7d2c10c34a055e36723b490c091dde1a571e724c9c491e3c9a9dda479cbe07210100000000000000000000000000000000000000000000000000000000000000000721010000000000000000000000000000000000000000000000000000000000000000082101180375079e61e3ca83119c12022caff856e5c8867289c01c302860c72a51e6bd07210100000000000000000000000000000000000000000000000000000000000000000721010000000000000000000000000000000000000000000000000000000000000000082101a2df35383735bd86c551278b9eb4f5fb2d42304243a2cf06b1c778430b0a2988082101b37d04573f17ef2455d543ba3505774d19052db54b75e554fe3a7d1c4e662537082101211bfb6abc58911e9021a8a186d6e5684bfb34f23b535e0803f47d7ea5dd647c072101000000000000000000000000000000000000000000000000000000000000000008210133d65d2d7dae86c9885621cd5a195f667b7d5900dedba1874ee760d244a5843d0721010000000000000000000000000000000000000000000000000000000000000000072101000000000000000000000000000000000000000000000000000000000000000007210100000000000000000000000000000000000000000000000000000000000000000721010000000000000000000000000000000000000000000000000000000000000000072101000000000000000000000000000000000000000000000000000000000000000008210122a06d8898f2aad8bd0b98bbbfc609d4cdd35c5ef5c230e2d01413d41886f2900821011c17776c798926af7aa5fd3cef3e37694db2c00f65bed7e56e71e8698b7e6ef2082101519c6c1c080d8098cf33c93219a6f625d5102e2384534f96365f67ce82438cb9082101f9211f49fdecf554be19bf1abb8b640951596096c57201f444244311582ff205082101b16ff759f8a8094777e6b9759a282f5513b8b476c1ad8a6b196364d4aeceff63082101a6f082b82280f3a6afb14c8e39b7f57860b857b70ca57afd35f40395eeb32458082101496fc0120d854e7534b992ab32ec3045b20d4bee1bfbe4564fd092ceafa08b72082101bb44fd36a5f3cdee7b5c6df3a6098a09e353335b6029f1477502588a7e37be00880100a70204551d05c903010b03041bfedf6f03010905210111a700b0c8066c47ecba05ed37bc14dcadb238552d86c659342d1d7e87b8772d060101072304210105616e6f6e00020d3139322e3136382e312e383000030004070512ba198244cd082302210100000000000000000000000000000000000000000000000000000000000000000823022101f5061003c85ac1f8a9968d2f315be2140c8a2d7031b77fe5723fa87fd3d90d32880104470204551d05c903010b03041bfedf6f05210158aae24d2ce0512e205ada1b53571bbaf79535cd68ae89b5e085149c6ae9de73060101072201010c031d03000c72656c656173652074657374000000000000000000000000000007260101010221010000000000000000000000000000000000000000000000000000000000000000072302210100000000000000000000000000000000000000000000000000000000000000000723022101000000000000000000000000000000000000000000000000000000000000000008230221010000000000000000000000000000000000000000000000000000000000000000072302210100000000000000000000000000000000000000000000000000000000000000000723022101358824e2f8d5afe759f117799e1795a270c682f3d18dfcc7a8c3fc3dfac98f0108230221010100af5726805d259b02b41baa74bd08642607dbc3accedb354db0c43dc4ddbb07230221013c8872bf2abdb873e2efd53c7a6105cc3bf237765e45526bbc3a49d3ff3adb8c07230221010c4d4edcac2739fd333241144f8f079b79824a787c20fec3a6f717f51518aef70722010103031d030002475400000000000000000000000000000000000000000000000007260101070221010000000000000000000000000000000000000000000000000000000000000000072302210100000000000000000000000000000000000000000000000000000000000000000823022101b67af3a0d94ec6fc95db56a805b147f9ee4d016331a41cc659f5a8c94da4cb6507230221013ae01b387351defa1b090dc62059d0344501e9c06c14abd08ccfefac771045b3072302210100000000000000000000000000000000000000000000000000000000000000000823022101e24cd4bb3c92c3f491b5a76c99ec5081d81c9028066f64557ec957c29b3a3852072302210100000000000000000000000000000000000000000000000000000000000000000722010106031d030002475400000000000000000000000000000000000000000000000007260101070221010c42e483667750e454e3f7885d042c1f8055fb2841bc607c787e7721685d71ef07230221015d588faaa29acc69f144c8e045d77912daffe95c2b9cfb7a7a5fcfe3caf60e2b072302210100000000000000000000000000000000000000000000000000000000000000000723022101a960c8446503ea05140163cb163a6c665b8564d8c4801d3a0f4c2516e98f043807230221018e84b1c6edada17d7bc1572eaa5edcb9f966169fec98b1998313d13b59a66751072302210100000000000000000000000000000000000000000000000000000000000000000723022101d0b56b1102ffe6150c0c104779a77403334390792aac3841f47ea99d764c879d0823022101837718d352a0cd14cecfd1fb714b815f2b4b568674966d48b30ff68c54c794450723022101778566762d0eaf4d6e00ef7811087d3092de2be078c294acc743f2666e5bb1cf880100a40204551d05c903010b0521010526482ea1f9f1ab0c0ba9a735ad7740b8e9a5072e7fdb4d38cfc7fc0eb6f299060101072601010f0221010000000000000000000000000000000000000000000000000000000000000000072601012c02210138f5972c32cb7c0b2841871d230ea26fefa576da2ef98b784eb6504131db08ec0823022101674c62f59994bfae32b82e53a3071e2f7377e8472db57846771e99e85b24db05")));
            signatureFactory.Create(aggregationResponsePayload);
        }
    }
}