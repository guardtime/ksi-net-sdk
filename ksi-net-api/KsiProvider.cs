using System.Security.Cryptography.X509Certificates;
using Guardtime.KSI.Crypto;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Hashing;

namespace Guardtime.KSI
{
    /// <summary>
    /// KSI provider.
    /// </summary>
    public class KsiProvider
    {
        private static ICryptoProvider _cryptoProvider;

        /// <summary>
        /// Set crypto provider.
        /// </summary>
        /// <param name="provider"></param>
        public static void SetCryptoProvider(ICryptoProvider provider)
        {
            _cryptoProvider = provider;
        }

        /// <summary>
        /// Get PKCS#7 crypto signature verifier.
        /// </summary>
        /// <returns>PKCS#7 verifier</returns>
        public static ICryptoSignatureVerifier GetPkcs7CryptoSignatureVerifier(X509Store trustStore, ICertificateSubjectRdnSelector certificateRdnSelector)
        {
            CheckCryptoProvider();
            return _cryptoProvider.GetPkcs7CryptoSignatureVerifier(trustStore, certificateRdnSelector);
        }

        /// <summary>
        /// Get RSA signature verifier.
        /// </summary>
        /// <param name="algorithm">hash algorithm</param>
        /// <returns>RSA signature verifier</returns>
        public static ICryptoSignatureVerifier GetRsaCryptoSignatureVerifier(string algorithm)
        {
            CheckCryptoProvider();
            return _cryptoProvider.GetRsaCryptoSignatureVerifier(algorithm);
        }

        /// <summary>
        /// Get HMAC hasher.
        /// </summary>
        /// <returns></returns>
        public static IHmacHasher GetHmacHasher()
        {
            CheckCryptoProvider();
            return _cryptoProvider.GetHmacHasher();
        }

        /// <summary>
        /// Get data hasher.
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public static IDataHasher GetDataHasher(HashAlgorithm algorithm)
        {
            CheckCryptoProvider();
            return _cryptoProvider.GetDataHasher(algorithm);
        }

        /// <summary>
        /// Check if crypto provider exists
        /// </summary>
        private static void CheckCryptoProvider()
        {
            if (_cryptoProvider == null)
            {
                throw new KsiException("Crypto provider not set. Please use SetCryptoProvider.");
            }
        }
    }
}