﻿using System;
using System.Collections.Generic;
using Guardtime.KSI.Parser;
using Guardtime.KSI.Utils;
using Guardtime.KSI.Exceptions;
using Guardtime.KSI.Signature;

namespace Guardtime.KSI.Publication
{
    /// <summary>
    /// Publication record TLV element
    /// </summary>
    public sealed class PublicationRecord : CompositeTag
    {
        public const uint TagTypeSignature = 0x803;
        public const uint TagTypePublication = 0x703;
        private const uint PublicationReferencesTagType = 0x9;
        private const uint PublicationRepositoryUriTagType = 0xa;

        private readonly PublicationData _publicationData;
        private readonly List<StringTag> _publicationReferences = new List<StringTag>();
        private readonly List<StringTag> _publicationRepositoryUri = new List<StringTag>();

        /// <summary>
        /// Get publication data
        /// </summary>
        public PublicationData PublicationData
        {
            get { return _publicationData; }
        }

        /// <summary>
        /// Get publication references
        /// </summary>
        public List<StringTag> PublicationReferences
        {
            get { return _publicationReferences; }
        }

        /// <summary>
        /// Get publication repository uri
        /// </summary>
        public List<StringTag> PubRepUri
        {
            get { return _publicationRepositoryUri; }
        }

        /// <summary>
        /// Get publication time
        /// </summary>
        public DateTime PublicationTime
        {
            get
            {
                return Util.ConvertUnixTimeToDateTime(PublicationData.PublicationTime.Value);
            } 
        }

        /// <summary>
        /// Create new publication record TLV element from TLV element
        /// </summary>
        /// <param name="tagList">TLV tag list</param>
        public PublicationRecord(TlvTag tag) : base(tag)
        {
            if (Type != TagTypeSignature && Type != TagTypePublication)
            {
                throw new InvalidTlvStructureException("Invalid publication record type: " + Type);
            }

            int publicationDataCount = 0;

            for (int i = 0; i < Count; i++)
            {
                StringTag listTag;

                switch (this[i].Type)
                {
                    case PublicationData.TagType:
                        _publicationData = new PublicationData(this[i]);
                        this[i] = _publicationData;
                        publicationDataCount++;
                        break;
                    case PublicationReferencesTagType:
                        listTag = new StringTag(this[i]);
                        _publicationReferences.Add(listTag);
                        this[i] = listTag;
                        break;
                    case PublicationRepositoryUriTagType:
                        listTag = new StringTag(this[i]);
                        _publicationRepositoryUri.Add(listTag);
                        this[i] = listTag;
                        break;
                    default:
                        VerifyCriticalTag(this[i]);
                        break;
                }
            }

            if (publicationDataCount != 1)
            {
                throw new InvalidTlvStructureException("Only one publication data is allowed in publication record");
            }
        }

    }
}