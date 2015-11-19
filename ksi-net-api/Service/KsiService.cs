﻿using System;
using System.IO;
using System.Threading;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Publication;
using Guardtime.KSI.Signature;
using Guardtime.KSI.Utils;

namespace Guardtime.KSI.Service
{
    /// <summary>
    ///     KSI service.
    /// </summary>
    public class KsiService : IKsiService
    {
        private readonly IKsiExtendingServiceProtocol _extendingServiceProtocol;
        private readonly KsiSignatureFactory _ksiSignatureFactory;
        private readonly PublicationsFileFactory _publicationsFileFactory;
        private readonly IKsiPublicationsFileServiceProtocol _publicationsFileServiceProtocol;
        private readonly IKsiServiceSettings _serviceSettings;
        private readonly IKsiSigningServiceProtocol _sigingServiceProtocol;

        /// <summary>
        ///     Create KSI service with service protocol and service settings.
        /// </summary>
        /// <param name="signingServiceProtocol">signing service protocol</param>
        /// <param name="extendingServiceProtocol">extending service protocol</param>
        /// <param name="publicationsFileServiceProtocol">publications file protocol</param>
        /// <param name="serviceSettings">service settings</param>
        /// <param name="publicationsFileFactory">publications file factory</param>
        /// <param name="ksiSignatureFactory">ksi signature factory</param>
        public KsiService(IKsiSigningServiceProtocol signingServiceProtocol,
                          IKsiExtendingServiceProtocol extendingServiceProtocol,
                          IKsiPublicationsFileServiceProtocol publicationsFileServiceProtocol,
                          IKsiServiceSettings serviceSettings,
                          PublicationsFileFactory publicationsFileFactory,
                          KsiSignatureFactory ksiSignatureFactory)
        {
            if (serviceSettings == null)
            {
                throw new KsiException("Invalid service settings: null.");
            }

            if (publicationsFileFactory == null)
            {
                throw new KsiException("Invalid publications file factory: null.");
            }

            if (ksiSignatureFactory == null)
            {
                throw new KsiException("Invalid KSI signature factory: null.");
            }

            _sigingServiceProtocol = signingServiceProtocol;
            _extendingServiceProtocol = extendingServiceProtocol;
            _publicationsFileServiceProtocol = publicationsFileServiceProtocol;
            _serviceSettings = serviceSettings;
            _publicationsFileFactory = publicationsFileFactory;
            _ksiSignatureFactory = ksiSignatureFactory;
        }

        /// <summary>
        ///     Sync create signature with given data hash.
        /// </summary>
        /// <param name="hash">data hash</param>
        /// <returns>KSI signature</returns>
        public IKsiSignature Sign(DataHash hash)
        {
            return EndSign(BeginSign(hash, null, null));
        }

        /// <summary>
        ///     Async begin create signature with given data hash.
        /// </summary>
        /// <param name="hash">data hash</param>
        /// <param name="callback">callback when creating signature is finished</param>
        /// <param name="asyncState">async state object</param>
        /// <returns>async result</returns>
        public IAsyncResult BeginSign(DataHash hash, AsyncCallback callback, object asyncState)
        {
            if (_sigingServiceProtocol == null)
            {
                throw new KsiServiceException("Signing service protocol is missing from service.");
            }

            KsiPduHeader header = new KsiPduHeader(_serviceSettings.LoginId);
            AggregationRequestPayload payload = new AggregationRequestPayload(hash);
            AggregationPdu pdu = new AggregationPdu(header, payload, KsiPdu.GetHashMacTag(_serviceSettings.LoginKey, header, payload));
            IAsyncResult serviceProtocolAsyncResult = _sigingServiceProtocol.BeginSign(pdu.Encode(), callback,
                asyncState);
            return new CreateSignatureKsiServiceAsyncResult(serviceProtocolAsyncResult, asyncState);
        }

        /// <summary>
        ///     Async end create signature.
        /// </summary>
        /// <param name="asyncResult">async result status</param>
        /// <returns>KSI signature</returns>
        public IKsiSignature EndSign(IAsyncResult asyncResult)
        {
            if (_sigingServiceProtocol == null)
            {
                throw new KsiServiceException("Signing service protocol is missing from service.");
            }

            if (asyncResult == null)
            {
                throw new KsiException("Invalid IAsyncResult: null.");
            }

            KsiServiceAsyncResult serviceAsyncResult = asyncResult as CreateSignatureKsiServiceAsyncResult;
            if (serviceAsyncResult == null)
            {
                throw new KsiServiceException("Invalid IAsyncResult, could not cast to correct object.");
            }

            if (!serviceAsyncResult.IsCompleted)
            {
                serviceAsyncResult.AsyncWaitHandle.WaitOne();
            }

            byte[] data = _sigingServiceProtocol.EndSign(serviceAsyncResult.ServiceProtocolAsyncResult);
            if (data == null)
            {
                throw new KsiException("Invalid sign response payload: null.");
            }

            try
            {
                using (TlvReader reader = new TlvReader(new MemoryStream(data)))
                {
                    AggregationPdu pdu = new AggregationPdu(reader.ReadTag());
                    AggregationResponsePayload payload = pdu.Payload as AggregationResponsePayload;
                    AggregationErrorPayload errorPayload = pdu.Payload as AggregationErrorPayload;

                    if (payload == null && errorPayload == null)
                    {
                        throw new KsiException("Invalid aggregation response payload: null.");
                    }

                    if (payload == null || payload.Status != 0)
                    {
                        string errorMessage = payload == null ? errorPayload.ErrorMessage : payload.ErrorMessage;
                        throw new KsiException("Error occured during aggregation: " + errorMessage + ".");
                    }

                    if (!pdu.ValidateMac(_serviceSettings.LoginKey))
                    {
                        throw new KsiServiceException("Invalid HMAC in aggregation response payload");
                    }

                    return _ksiSignatureFactory.Create(payload);
                }
            }
            catch (TlvException e)
            {
                throw new KsiException("Could not parse response message: " + Base16.Encode(data), e);
            }
        }

        /// <summary>
        ///     Sync extend signature to latest publication.
        /// </summary>
        /// <param name="aggregationTime">aggregation time</param>
        /// <returns>extended calendar hash chain</returns>
        public CalendarHashChain Extend(ulong aggregationTime)
        {
            return EndExtend(BeginExtend(aggregationTime, null, null));
        }

        /// <summary>
        ///     Sync extend signature to given publication.
        /// </summary>
        /// <param name="aggregationTime">aggregation time</param>
        /// <param name="publicationTime">publication time</param>
        /// <returns>extended calendar hash chain</returns>
        public CalendarHashChain Extend(ulong aggregationTime, ulong publicationTime)
        {
            return EndExtend(BeginExtend(aggregationTime, publicationTime, null, null));
        }

        /// <summary>
        ///     Async begin extend signature to latest publication.
        /// </summary>
        /// <param name="aggregationTime">aggregation time</param>
        /// <param name="callback">callback when extending signature is finished</param>
        /// <param name="asyncState">async state object</param>
        /// <returns>async result</returns>
        public IAsyncResult BeginExtend(ulong aggregationTime, AsyncCallback callback, object asyncState)
        {
            return BeginExtend(new ExtendRequestPayload(aggregationTime), callback, asyncState);
        }

        /// <summary>
        ///     Async begin extend signature to given publication.
        /// </summary>
        /// <param name="aggregationTime">aggregation time</param>
        /// <param name="publicationTime">publication time</param>
        /// <param name="callback">callback when extending signature is finished</param>
        /// <param name="asyncState">async state object</param>
        /// <returns>async result</returns>
        public IAsyncResult BeginExtend(ulong aggregationTime, ulong publicationTime, AsyncCallback callback,
                                        object asyncState)
        {
            return BeginExtend(new ExtendRequestPayload(aggregationTime, publicationTime), callback, asyncState);
        }

        /// <summary>
        ///     Async end extend signature.
        /// </summary>
        /// <param name="asyncResult">async result</param>
        /// <returns>extended calendar hash chain</returns>
        public CalendarHashChain EndExtend(IAsyncResult asyncResult)
        {
            if (_extendingServiceProtocol == null)
            {
                throw new KsiServiceException("Extending service protocol is missing from service.");
            }

            if (asyncResult == null)
            {
                throw new KsiException("Invalid IAsyncResult: null.");
            }

            ExtendSignatureKsiServiceAsyncResult serviceAsyncResult =
                asyncResult as ExtendSignatureKsiServiceAsyncResult;
            if (serviceAsyncResult == null)
            {
                throw new KsiServiceException("Invalid IAsyncResult, could not cast to correct object.");
            }

            if (!serviceAsyncResult.IsCompleted)
            {
                serviceAsyncResult.AsyncWaitHandle.WaitOne();
            }

            byte[] data = _extendingServiceProtocol.EndExtend(serviceAsyncResult.ServiceProtocolAsyncResult);
            if (data == null)
            {
                throw new KsiException("Invalid extend response payload: null.");
            }

            try
            {
                using (TlvReader reader = new TlvReader(new MemoryStream(data)))
                {
                    ExtendPdu pdu = new ExtendPdu(reader.ReadTag());
                    ExtendResponsePayload payload = pdu.Payload as ExtendResponsePayload;
                    ExtendErrorPayload errorPayloadPayload = pdu.Payload as ExtendErrorPayload;

                    if (payload == null && errorPayloadPayload == null)
                    {
                        throw new KsiException("Invalid extension response payload: null.");
                    }

                    if (payload == null || payload.Status != 0)
                    {
                        string errorMessage = payload == null ? errorPayloadPayload.ErrorMessage : payload.ErrorMessage;
                        throw new KsiException("Error occured during extending: " + errorMessage + ".");
                    }

                    if (!pdu.ValidateMac(_serviceSettings.LoginKey))
                    {
                        throw new KsiServiceException("Invalid HMAC in aggregation response payload");
                    }

                    if (payload.CalendarHashChain == null)
                    {
                        throw new KsiServiceException("No calendar hash chain in payload.");
                    }

                    return payload.CalendarHashChain;
                }
            }
            catch (TlvException e)
            {
                throw new KsiException("Could not parse response message: " + Base16.Encode(data), e);
            }
        }

        /// <summary>
        ///     Sync get publications file.
        /// </summary>
        /// <returns>Publications file</returns>
        public IPublicationsFile GetPublicationsFile()
        {
            return EndGetPublicationsFile(BeginGetPublicationsFile(null, null));
        }

        /// <summary>
        ///     Async begin get publications file.
        /// </summary>
        /// <param name="callback">callback when publications file is downloaded</param>
        /// <param name="asyncState">async state object</param>
        /// <returns>async result</returns>
        public IAsyncResult BeginGetPublicationsFile(AsyncCallback callback, object asyncState)
        {
            if (_publicationsFileServiceProtocol == null)
            {
                throw new KsiServiceException("Publications file service protocol is missing from service.");
            }

            IAsyncResult serviceProtocolAsyncResult = _publicationsFileServiceProtocol.BeginGetPublicationsFile(
                callback, asyncState);
            return new PublicationKsiServiceAsyncResult(serviceProtocolAsyncResult, asyncState);
        }

        /// <summary>
        ///     Async end get publications file.
        /// </summary>
        /// <param name="asyncResult">async result</param>
        /// <returns>publications file</returns>
        public IPublicationsFile EndGetPublicationsFile(IAsyncResult asyncResult)
        {
            if (_publicationsFileServiceProtocol == null)
            {
                throw new KsiServiceException("Publications file service protocol is missing from service.");
            }

            if (asyncResult == null)
            {
                throw new KsiException("Invalid IAsyncResult: null.");
            }

            KsiServiceAsyncResult serviceAsyncResult = asyncResult as PublicationKsiServiceAsyncResult;
            if (serviceAsyncResult == null)
            {
                throw new KsiServiceException("Invalid IAsyncResult, could not cast to correct object.");
            }

            if (!serviceAsyncResult.IsCompleted)
            {
                serviceAsyncResult.AsyncWaitHandle.WaitOne();
            }

            byte[] data =
                _publicationsFileServiceProtocol.EndGetPublicationsFile(serviceAsyncResult.ServiceProtocolAsyncResult);
            return _publicationsFileFactory.Create(data);
        }

        /// <summary>
        ///     Begin extend with payload.
        /// </summary>
        /// <param name="payload">extend request payload</param>
        /// <param name="callback">callback when extending signature is finished</param>
        /// <param name="asyncState">async state object</param>
        /// <returns></returns>
        private IAsyncResult BeginExtend(KsiPduPayload payload, AsyncCallback callback, object asyncState)
        {
            if (_extendingServiceProtocol == null)
            {
                throw new KsiServiceException("Extending service protocol is missing from service.");
            }

            KsiPduHeader header = new KsiPduHeader(_serviceSettings.LoginId);
            ExtendPdu pdu = new ExtendPdu(header, payload, KsiPdu.GetHashMacTag(_serviceSettings.LoginKey, header, payload));
            IAsyncResult serviceProtocolAsyncResult = _extendingServiceProtocol.BeginExtend(pdu.Encode(), callback,
                asyncState);
            return new ExtendSignatureKsiServiceAsyncResult(serviceProtocolAsyncResult, asyncState);
        }

        /// <summary>
        ///     Create signature KSI service async result.
        /// </summary>
        private class CreateSignatureKsiServiceAsyncResult : KsiServiceAsyncResult
        {
            public CreateSignatureKsiServiceAsyncResult(IAsyncResult serviceProtocolAsyncResult, object asyncState)
                : base(serviceProtocolAsyncResult, asyncState)
            {
            }
        }

        /// <summary>
        ///     Extend signature KSI service async result.
        /// </summary>
        private class ExtendSignatureKsiServiceAsyncResult : KsiServiceAsyncResult
        {
            public ExtendSignatureKsiServiceAsyncResult(IAsyncResult serviceProtocolAsyncResult, object asyncState)
                : base(serviceProtocolAsyncResult, asyncState)
            {
            }
        }

        /// <summary>
        ///     Publications file KSI service async result.
        /// </summary>
        private class PublicationKsiServiceAsyncResult : KsiServiceAsyncResult
        {
            public PublicationKsiServiceAsyncResult(IAsyncResult serviceProtocolAsyncResult, object asyncState)
                : base(serviceProtocolAsyncResult, asyncState)
            {
            }
        }

        /// <summary>
        ///     KSI service async result.
        /// </summary>
        private abstract class KsiServiceAsyncResult : IAsyncResult
        {
            protected KsiServiceAsyncResult(IAsyncResult serviceProtocolAsyncResult, object asyncState)
            {
                if (serviceProtocolAsyncResult == null)
                {
                    throw new KsiException("Invalid service protocol IAsyncResult: null.");
                }

                ServiceProtocolAsyncResult = serviceProtocolAsyncResult;
                AsyncState = asyncState;
            }

            public IAsyncResult ServiceProtocolAsyncResult { get; }

            public object AsyncState { get; }

            public WaitHandle AsyncWaitHandle => ServiceProtocolAsyncResult.AsyncWaitHandle;

            public bool CompletedSynchronously => ServiceProtocolAsyncResult.CompletedSynchronously;

            public bool IsCompleted => ServiceProtocolAsyncResult.IsCompleted;
        }
    }
}