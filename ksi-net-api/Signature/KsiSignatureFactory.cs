﻿using System.IO;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Service;
using NLog;

namespace Guardtime.KSI.Signature
{
    public class KsiSignatureFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Get KSI signature instance from stream.
        /// </summary>
        /// <param name="stream">signature data stream</param>
        /// <returns>KSI signature</returns>
        public IKsiSignature Create(Stream stream)
        {
            if (stream == null)
            {
                throw new KsiException("Invalid input stream: null.");
            }

            using (TlvReader reader = new TlvReader(stream))
            {
                try
                {
                    Logger.Debug("Creating KSI signature from stream.");
                    KsiSignature signature = new KsiSignature(reader.ReadTag());
                    Logger.Debug("Creating KSI signature from stream successful.");
                    return signature;
                }
                catch (TlvException e)
                {
                    Logger.Warn("Creating KSI signature from stream failed: {0}", e);
                    throw;
                }
            }
        }

        /// <summary>
        ///     Get KSI signature instance from aggregation response payload.
        /// </summary>
        /// <param name="payload">aggregation response payload</param>
        /// <returns>KSI signature</returns>
        public IKsiSignature Create(AggregationResponsePayload payload)
        {
            if (payload == null)
            {
                throw new KsiException("Invalid aggregation response payload: null.");
            }

            using (TlvWriter writer = new TlvWriter(new MemoryStream()))
            {
                foreach (ITlvTag childTag in payload)
                {
                    if (childTag.Type > 0x800 && childTag.Type < 0x900)
                    {
                        writer.WriteTag(childTag);
                    }
                }

                try
                {
                    Logger.Debug("Creating KSI signature from aggregation response.");
                    KsiSignature signature = new KsiSignature(new RawTag(Constants.KsiSignature.TagType, false, false, ((MemoryStream)writer.BaseStream).ToArray()));
                    Logger.Debug("Creating KSI signature from aggregation response successful.");
                    return signature;
                }
                catch (TlvException e)
                {
                    Logger.Warn("Creating KSI signature from aggregation response failed: {0}", e);
                    throw;
                }
            }
        }
    }
}