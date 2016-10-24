// <copyright file="MediaEncoderStandardTaskPresetStrings.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// Contains string constants with the available Task Preset Strings for Media Encoder Standard (MES).
    /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269960.aspx">https://msdn.microsoft.com/library/azure/mt269960.aspx</a>.
    /// </summary>
    public static class MediaEncoderStandardTaskPresetStrings
    {
        #region H264 Multiple Bitrate Presets

        /// <summary>
        /// Produces a set of 8 GOP-aligned MP4 files, ranging from 6000 kbps to 400 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269929.aspx">https://msdn.microsoft.com/library/azure/mt269929.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate1080pAudio51 = "H264 Multiple Bitrate 1080p Audio 5.1";

        /// <summary>
        /// Produces a set of 8 GOP-aligned MP4 files, ranging from 6000 kbps to 400 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269937.aspx">https://msdn.microsoft.com/library/azure/mt269937.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate1080p = "H264 Multiple Bitrate 1080p";

        /// <summary>
        /// Produces a set of 8 GOP-aligned MP4 files, ranging from 8500 kbps to 200 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269925.aspx">https://msdn.microsoft.com/library/azure/mt269925.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate16x9foriOS = "H264 Multiple Bitrate 16x9 for iOS";

        /// <summary>
        /// Produces a set of 5 GOP-aligned MP4 files, ranging from 1900 kbps to 400 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269939.aspx">https://msdn.microsoft.com/library/azure/mt269939.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate16x9SDAudio51 = "H264 Multiple Bitrate 16x9 SD Audio 5.1";

        /// <summary>
        /// Produces a set of 5 GOP-aligned MP4 files, ranging from 1900 kbps to 400 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269954.aspx">https://msdn.microsoft.com/library/azure/mt269954.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate16x9SD = "H264 Multiple Bitrate 16x9 SD";

        /// <summary>
        /// Produces a set of 12 GOP-aligned MP4 files, ranging from 20000 kbps to 1000 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269950.aspx">https://msdn.microsoft.com/library/azure/mt269950.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate4KAudio51 = "H264 Multiple Bitrate 4K Audio 5.1";

        /// <summary>
        /// Produces a set of 12 GOP-aligned MP4 files, ranging from 20000 kbps to 1000 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269933.aspx">https://msdn.microsoft.com/library/azure/mt269933.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate4K = "H264 Multiple Bitrate 4K";

        /// <summary>
        /// Produces a set of 8 GOP-aligned MP4 files, ranging from 8500 kbps to 200 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269947.aspx">https://msdn.microsoft.com/library/azure/mt269947.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate4x3foriOS = "H264 Multiple Bitrate 4x3 for iOS";

        /// <summary>
        /// Produces a set of 5 GOP-aligned MP4 files, ranging from 1600 kbps to 400 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269931.aspx">https://msdn.microsoft.com/library/azure/mt269931.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate4x3SDAudio51 = "H264 Multiple Bitrate 4x3 SD Audio 5.1";

        /// <summary>
        /// Produces a set of 5 GOP-aligned MP4 files, ranging from 1600 kbps to 400 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269963.aspx">https://msdn.microsoft.com/library/azure/mt269963.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate4x3SD = "H264 Multiple Bitrate 4x3 SD";

        /// <summary>
        /// Produces a set of 6 GOP-aligned MP4 files, ranging from 3400 kbps to 400 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269941.aspx">https://msdn.microsoft.com/library/azure/mt269941.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate720pAudio51 = "H264 Multiple Bitrate 720p Audio 5.1";

        /// <summary>
        /// Produces a set of 6 GOP-aligned MP4 files, ranging from 3400 kbps to 400 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269953.aspx">https://msdn.microsoft.com/library/azure/mt269953.aspx</a>.
        /// </summary>
        public const string H264MultipleBitrate720p = "H264 Multiple Bitrate 720p";

        #endregion

        #region H264 Single Bitrate Presets

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 6750 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269928.aspx">https://msdn.microsoft.com/library/azure/mt269928.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate1080pAudio51 = "H264 Single Bitrate 1080p Audio 5.1";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 6750 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269926.aspx">https://msdn.microsoft.com/library/azure/mt269926.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate1080p = "H264 Single Bitrate 1080p";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 18000 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269942.aspx">https://msdn.microsoft.com/library/azure/mt269942.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate4KAudio51 = "H264 Single Bitrate 4K Audio 5.1";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 18000 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269936.aspx">https://msdn.microsoft.com/library/azure/mt269936.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate4K = "H264 Single Bitrate 4K";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 18000 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269927.aspx">https://msdn.microsoft.com/library/azure/mt269927.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate4x3SDAudio51 = "H264 Single Bitrate 4x3 SD Audio 5.1";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 18000 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269956.aspx">https://msdn.microsoft.com/library/azure/mt269956.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate4x3SD = "H264 Single Bitrate 4x3 SD";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 2200 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269944.aspx">https://msdn.microsoft.com/library/azure/mt269944.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate16x9SDAudio51 = "H264 Single Bitrate 16x9 SD Audio 5.1";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 2200 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269934.aspx">https://msdn.microsoft.com/library/azure/mt269934.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate16x9SD = "H264 Single Bitrate 16x9 SD";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 4500 kbps, and AAC 5.1 audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269930.aspx">https://msdn.microsoft.com/library/azure/mt269930.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate720pAudio51 = "H264 Single Bitrate 720p Audio 5.1";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 2000 kbps, and stereo AAC.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269965.aspx">https://msdn.microsoft.com/library/azure/mt269965.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate720pforAndroid = "H264 Single Bitrate 720p for Android";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 4500 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269949.aspx">https://msdn.microsoft.com/library/azure/mt269949.aspx</a>.
        /// </summary>
        public const string H264SingleBitrate720p = "H264 Single Bitrate 720p";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 500 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269964.aspx">https://msdn.microsoft.com/library/azure/mt269964.aspx</a>.
        /// </summary>
        public const string H264SingleBitrateHighQualitySDforAndroid = "H264 Single Bitrate High Quality SD for Android";

        /// <summary>
        /// Produces a single MP4 file with a bitrate of 56 kbps, and stereo AAC audio.
        /// For more information, please visit <a href="https://msdn.microsoft.com/library/azure/mt269948.aspx">https://msdn.microsoft.com/library/azure/mt269948.aspx</a>.
        /// </summary>
        public const string H264SingleBitrateLowQualitySDforAndroid = "H264 Single Bitrate Low Quality SD for Android";

        #endregion
    }
}
