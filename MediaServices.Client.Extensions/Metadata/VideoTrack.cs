// <copyright file="VideoTrack.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    /// Represents a video track contained within the <see cref="IAssetFile"/>.
    /// </summary>
    public class VideoTrack
    {
        /// <summary>
        /// Get the zero-based index of this video track.
        /// <remarks>
        /// This is not necessarily the TrackID as used in an MP4 file.
        /// </remarks>
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Gets the average audio bitrate in bits per second, as calculated from the media file. Takes into consideration only the elementary stream payload and does not include the packaging overhead.
        /// </summary>
        public int Bitrate { get; internal set; }

        /// <summary>
        /// Gets the target average bitrate for this video track, as requested in the encoding preset, in bits per second.
        /// </summary>
        public int TargetBitrate { get; internal set; }

        /// <summary>
        /// Get the demonitnator of the video display aspect ratio.
        /// </summary>
        public double DisplayAspectRatioDenominator { get; internal set; }

        /// <summary>
        /// Get the numerator of the video display aspect ratio.
        /// </summary>
        public double DisplayAspectRatioNumerator { get; internal set; }

        /// <summary>
        /// Get the measured video frame rate in frames per second (Hsz) in .3f format.
        /// </summary>
        public double Framerate { get; internal set; }

        /// <summary>
        /// Gets the preset target video frame rate in .3f format.
        /// </summary>
        public double TargetFramerate { get; internal set; }

        /// <summary>
        /// Get the video codec FourCC code.
        /// </summary>
        public string FourCC { get; internal set; }

        /// <summary>
        /// Get the encoded video height in pixels.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// Get the encoded video width in pixels.
        /// </summary>
        public int Width { get; internal set; }

        internal static VideoTrack Load(XElement videoTrackElement)
        {
            VideoTrack videoTrack = new VideoTrack();

            videoTrack.Id = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.IdAttributeName);
            videoTrack.Bitrate = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.BitrateAttributeName);
            videoTrack.TargetBitrate = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.TargetBitrateAttributeName);
            videoTrack.FourCC = videoTrackElement.GetAttributeOrDefault(AssetMetadataParser.FourCCAttributeName);
            videoTrack.Width = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.WidthAttributeName);
            videoTrack.Height = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.HeightAttributeName);
            videoTrack.DisplayAspectRatioNumerator = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.DisplayAspectRatioNumeratorAttributeName);
            videoTrack.DisplayAspectRatioDenominator = videoTrackElement.GetAttributeAsIntOrDefault(AssetMetadataParser.DisplayAspectRatioDenominatorAttributeName);
            videoTrack.Framerate = videoTrackElement.GetAttributeAsDoubleOrDefault(AssetMetadataParser.FramerateAttributeName);
            videoTrack.TargetFramerate = videoTrackElement.GetAttributeAsDoubleOrDefault(AssetMetadataParser.TargetFramerateAttributeName);

            return videoTrack;
        }
    }
}
