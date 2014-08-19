// <copyright file="AssetMetadataParser.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
// <license>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </license>

namespace Microsoft.WindowsAzure.MediaServices.Client.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    internal static class AssetMetadataParser
    {
        internal const string AssetFilesMetadataNamespace = "http://schemas.microsoft.com/windowsazure/mediaservices/2013/05/mediaencoder/metadata";

        internal static readonly XName NameAttributeName = XName.Get("Name");

        internal static readonly XName SizeAttributeName = XName.Get("Size");

        internal static readonly XName DurationAttributeName = XName.Get("Duration");

        internal static readonly XName SourcesElementName = XName.Get("Sources", AssetFilesMetadataNamespace);

        internal static readonly XName SourceElementName = XName.Get("Source", AssetFilesMetadataNamespace);

        internal static readonly XName VideoTracksElementName = XName.Get("VideoTracks", AssetFilesMetadataNamespace);

        internal static readonly XName VideoTrackElementName = XName.Get("VideoTrack", AssetFilesMetadataNamespace);

        internal static readonly XName AudioTracksElementName = XName.Get("AudioTracks", AssetFilesMetadataNamespace);

        internal static readonly XName AudioTrackElementName = XName.Get("AudioTrack", AssetFilesMetadataNamespace);

        internal static readonly XName IdAttributeName = XName.Get("Id");

        internal static readonly XName BitrateAttributeName = XName.Get("Bitrate");

        internal static readonly XName TargetBitrateAttributeName = XName.Get("TargetBitrate");

        internal static readonly XName FourCCAttributeName = XName.Get("FourCC");

        internal static readonly XName WidthAttributeName = XName.Get("Width");

        internal static readonly XName HeightAttributeName = XName.Get("Height");

        internal static readonly XName DisplayAspectRatioNumeratorAttributeName = XName.Get("DisplayAspectRatioNumerator");

        internal static readonly XName DisplayAspectRatioDenominatorAttributeName = XName.Get("DisplayAspectRatioDenominator");

        internal static readonly XName FramerateAttributeName = XName.Get("Framerate");

        internal static readonly XName TargetFramerateAttributeName = XName.Get("TargetFramerate");

        internal static readonly XName CodecAttributeName = XName.Get("Codec");

        internal static readonly XName ChannelsAttributeName = XName.Get("Channels");

        internal static readonly XName SamplingRateAttributeName = XName.Get("SamplingRate");

        internal static readonly XName BitsPerSampleAttributeName = XName.Get("BitsPerSample");

        internal static readonly XName EncoderVersionAttributeName = XName.Get("EncoderVersion");

        internal static async Task<IEnumerable<AssetFileMetadata>> ParseAssetFileMetadataAsync(Uri assetFileMetadataUri, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            IList<AssetFileMetadata> assetFileMetadataList = new List<AssetFileMetadata>();
            try
            {
                using (var assetFileMetadataStream = new MemoryStream())
                {
                    CloudBlockBlob blob = new CloudBlockBlob(assetFileMetadataUri);
                    await blob.DownloadToStreamAsync(assetFileMetadataStream, null, new BlobRequestOptions { RetryPolicy = retryPolicy }, null, cancellationToken).ConfigureAwait(false);

                    assetFileMetadataStream.Seek(0, SeekOrigin.Begin);

                    XElement root = XElement.Load(assetFileMetadataStream);
                    foreach (XElement assetFileElement in root.Elements())
                    {
                        assetFileMetadataList.Add(AssetFileMetadata.Load(assetFileElement));
                    }
                }
            }
            catch
            {
            }

            return assetFileMetadataList;
        }
    }
}
