﻿using System.IO;
using Guardtime.KSI.Exceptions;

namespace Guardtime.KSI.Hashing
{
    /// <summary>
    ///     This class provides functionality for hashing data.
    /// </summary>
    public class DataHasher : IDataHasher
    {
        private const int DefaultStreamBufferSize = 8192;

        private readonly HashAlgorithm _algorithm;
        private System.Security.Cryptography.HashAlgorithm _messageHasher;
        private DataHash _outputHash;

        /// <summary>
        ///     Create new Datahasher with given algorithm
        /// </summary>
        /// <param name="algorithm">Hash algorithm</param>
        public DataHasher(HashAlgorithm algorithm)
        {
            if (algorithm == null)
            {
                throw new HashingException("Invalid hash algorithm: null.");
            }

            /*
                If an algorithm is given which is not implemented, an illegal argument exception is thrown
                The developer must ensure that only implemented algorithms are used.
             */
            if (algorithm.Status == HashAlgorithm.AlgorithmStatus.NotImplemented)
            {
                throw new HashingException("Hash algorithm is not implemented.");
            }

            _algorithm = algorithm;

            _messageHasher = System.Security.Cryptography.HashAlgorithm.Create(algorithm.Name);
            if (_messageHasher == null)
            {
                throw new HashingException("Hash algorithm(" + algorithm.Name + ") is not supported.");
            }

            _messageHasher.Initialize();
        }

        /// <summary>
        ///     Create new data hasher for the default algorithm.
        /// </summary>
        public DataHasher() : this(HashAlgorithm.GetByName("DEFAULT"))
        {
        }

        /// <summary>
        ///     Updates the digest using the specified array of bytes, starting at the specified offset.
        /// </summary>
        /// <param name="data">the list of bytes.</param>
        /// <param name="offset">the offset to start from in the array of bytes.</param>
        /// <param name="length">the number of bytes to use, starting at the offset.</param>
        /// <returns>the same DataHasher object for chaining calls</returns>
        public IDataHasher AddData(byte[] data, int offset, int length)
        {
            if (_outputHash != null)
            {
                throw new HashingException("Output hash has already been calculated.");
            }

            if (data == null)
            {
                throw new HashingException("Invalid input data: null.");
            }

            _messageHasher.TransformBlock(data, offset, length, null, 0);
            return this;
        }

        /// <summary>
        ///     Adds data to the digest using the specified array of bytes, starting at an offset of 0.
        /// </summary>
        /// <param name="data">list of bytes</param>
        /// <returns>the same DataHasher object for chaining calls</returns>
        public IDataHasher AddData(byte[] data)
        {
            if (data == null)
            {
                throw new HashingException("Invalid input data: null.");
            }

            return AddData(data, 0, data.Length);
        }

        /// <summary>
        ///     Adds data to the digest using the specified input stream of bytes, starting at an offset of 0.
        /// </summary>
        /// <param name="inStream">input stream of bytes.</param>
        /// <returns>the same DataHasher object for chaining calls</returns>
        public IDataHasher AddData(Stream inStream)
        {
            return AddData(inStream, DefaultStreamBufferSize);
        }

        /// <summary>
        ///     Adds data to the digest using the specified input stream of bytes, starting at an offset of 0.
        /// </summary>
        /// <param name="inStream">input stream of bytes.</param>
        /// <param name="bufferSize">maximum allowed buffer size for reading data</param>
        /// <returns>the same DataHasher object for chaining calls</returns>
        public IDataHasher AddData(Stream inStream, int bufferSize)
        {
            if (inStream == null)
            {
                throw new HashingException("Invalid input stream: null.");
            }

            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int bytesRead = inStream.Read(buffer, 0, bufferSize);

                if (bytesRead == 0)
                {
                    return this;
                }

                AddData(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        ///     Get the final hash value for the digest.
        ///     This will not reset hash calculation.
        /// </summary>
        /// <returns>calculated hash</returns>
        public DataHash GetHash()
        {
            if (_outputHash != null)
            {
                return _outputHash;
            }
            _messageHasher.TransformFinalBlock(new byte[] { }, 0, 0);
            byte[] hash = _messageHasher.Hash;
            _outputHash = new DataHash(_algorithm, hash);

            return _outputHash;
        }

        /// <summary>
        ///     Resets hash calculation.
        /// </summary>
        /// <returns>the same DataHasher object for chaining calls</returns>
        public IDataHasher Reset()
        {
            _outputHash = null;
            _messageHasher.Clear();
            _messageHasher = System.Security.Cryptography.HashAlgorithm.Create(_algorithm.Name);

            return this;
        }
    }
}