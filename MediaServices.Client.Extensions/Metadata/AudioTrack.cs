// <copyright file="AudioTrack.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Xml.Linq;

    /// <summary>
    /// Represents an audio track contained in the <see cref="IAssetFile"/>.
    /// </summary>
    public class AudioTrack
    {
        /// <summary>
        /// Get the zero-based index of the audio track.
        /// <remarks>
        /// This is not necessarily the TrackID as used in an MP4 file.
        /// </remarks>
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Gets the average video bit rate in bits per second, as calculated from the media file. Counts only the elementary stream payload, and does not include the packaging overhead.
        /// </summary>
        public int Bitrate { get; internal set; }

        /// <summary>
        /// Get the bits per sample for the audio track.
        /// </summary>
        public int BitsPerSample { get; internal set; }

        /// <summary>
        /// Get the number of audio channels in the audio track.
        /// </summary>
        public int Channels { get; internal set; }

        /// <summary>
        /// Get the audio codec used for encoding the audio track.
        /// </summary>
        public string Codec { get; internal set; }

        /// <summary>
        /// Get the language.
        /// </summary>
        public string Language { get; internal set; }

        /// <summary>
        /// Get the optional encoder version string, required for EAC3.
        /// </summary>
        public string EncoderVersion { get; internal set; }

        /// <summary>
        /// Get the audio sampling rate in samples/sec or Hz.
        /// </summary>
        public int SamplingRate { get; internal set; }

        internal static AudioTrack Load(XElement audioTrackElement)
        {
            AudioTrack audioTrack = new AudioTrack();

            audioTrack.Id = audioTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.IdAttributeName);
            audioTrack.Bitrate = audioTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.BitrateAttributeName);
            audioTrack.Codec = audioTrackElement.GetAttributeOrDefault(AssetMetadataParser.CodecAttributeName);
            audioTrack.Channels = audioTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.ChannelsAttributeName);
            audioTrack.SamplingRate = audioTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.SamplingRateAttributeName);
            audioTrack.BitsPerSample = audioTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.BitsPerSampleAttributeName);
            audioTrack.EncoderVersion = audioTrackElement.GetAttributeOrDefault(AssetMetadataParser.EncoderVersionAttributeName);
            audioTrack.Language = audioTrackElement.GetAttributeOrDefault(AssetMetadataParser.LanguageAttributeName);

            return audioTrack;
        }
    }
}
