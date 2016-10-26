// <copyright file="AssetFileMetadata.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a media file produced by the encoding <see cref="IJob"/>.
    /// </summary>
    public class AssetFileMetadata
    {
        /// <summary>
        /// Gets the name of the media file.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the size of the media file in bytes.
        /// </summary>
        public long Size { get; internal set; }

        /// <summary>
        /// Gets the play back duration of the media file.
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Gets a collection of source media files that was processed in order to produce this output media file.
        /// </summary>
        public IEnumerable<Source> Sources { get; internal set; }

        /// <summary>
        /// Gets a collection of audio tracks contained in the media file.
        /// </summary>
        public IEnumerable<AudioTrack> AudioTracks { get; internal set; }

        /// <summary>
        /// Gets a collection of video tracks contained in the media file.
        /// </summary>
        public IEnumerable<VideoTrack> VideoTracks { get; internal set; }

        internal static AssetFileMetadata Load(XElement assetFileElement)
        {
            AssetFileMetadata assetFileMetadata = new AssetFileMetadata();

            assetFileMetadata.Name = assetFileElement.GetAttributeOrDefault(AssetMetadataParser.NameAttributeName);
            assetFileMetadata.Size = assetFileElement.GetAttributeAsLongOrDefault(AssetMetadataParser.SizeAttributeName);
            assetFileMetadata.Duration = assetFileElement.GetAttributeAsTimeSpanOrDefault(AssetMetadataParser.DurationAttributeName);

            assetFileMetadata.Sources = ParseSources(assetFileElement.Element(AssetMetadataParser.SourcesElementName));
            assetFileMetadata.VideoTracks = ParseVideoTracks(assetFileElement.Element(AssetMetadataParser.VideoTracksElementName));
            assetFileMetadata.AudioTracks = ParseAudioTracks(assetFileElement.Element(AssetMetadataParser.AudioTracksElementName));

            return assetFileMetadata;
        }

        private static IEnumerable<Source> ParseSources(XElement sourcesElement)
        {
            IEnumerable<Source> sources = null;

            if (sourcesElement != null)
            {
                sources = sourcesElement
                    .Elements(AssetMetadataParser.SourceElementName)
                    .Select(s => Source.Load(s))
                    .ToArray();
            }

            return sources;
        }

        private static IEnumerable<VideoTrack> ParseVideoTracks(XElement videoTracksElement)
        {
            IEnumerable<VideoTrack> videoTracks = null;

            if (videoTracksElement != null)
            {
                videoTracks = videoTracksElement
                    .Elements(AssetMetadataParser.VideoTrackElementName)
                    .Select(v => VideoTrack.Load(v))
                    .ToArray();
            }

            return videoTracks;
        }

        private static IEnumerable<AudioTrack> ParseAudioTracks(XElement audioTracksElement)
        {
            IEnumerable<AudioTrack> audioTracks = null;

            if (audioTracksElement != null)
            {
                audioTracks = audioTracksElement
                    .Elements(AssetMetadataParser.AudioTrackElementName)
                    .Select(a => AudioTrack.Load(a))
                    .ToArray();
            }

            return audioTracks;
        }
    }
}
