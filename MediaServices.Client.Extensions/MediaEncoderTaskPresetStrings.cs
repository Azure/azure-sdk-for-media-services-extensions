// <copyright file="MediaEncoderTaskPresetStrings.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    /// Contains string constants with the available Task Preset Strings for Windows Azure Media Encoder.
    /// For more information, please visit <a href="http://msdn.microsoft.com/library/windowsazure/jj129582.aspx">http://msdn.microsoft.com/library/windowsazure/jj129582.aspx</a>.
    /// </summary>
    public static class MediaEncoderTaskPresetStrings
    {
        #region Audio Coding Standard

        /// <summary>
        /// Produces a Windows Media file 44.1 kHz 16 bits/sample stereo audio encoded using WMA.
        /// <para>Use this preset name to produce an audio-only file for music services. The output file extension is *.WMA.</para>
        /// </summary>
        public const string WMAHighQualityAudio = "WMA High Quality Audio";

        /// <summary>
        /// Produces an MP4 file containing 44.1 kHz 16 bits/sample stereo audio CBR encoded at 192 kbps using AAC.
        /// <para>Use this preset name to produce an audio-only file for music services. The output file extension is *.MP4.</para>
        /// </summary>
        public const string AACGoodQualityAudio = "AAC Good Quality Audio";

        #endregion

        #region VC-1 Coding Standard

        /// <summary>
        /// Produces a single Windows Media file with:
        ///    <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///    <para> - 1080p video VBR encoded at 6750 kbps using VC-1 Advanced Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for 1080p (16:9 aspect ratio) content for delivery 
        /// over broadband connections. The output file extension is *.WMV. If the source frame size is not 1920x1080, 
        /// the video will be scaled horizontally to the width of the profile target (e.g. 1920, 1280, 852 or 640 pixels), 
        /// and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string VC1Broadband1080p = "VC1 Broadband 1080p";

        /// <summary>
        /// Produces a single Windows Media file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - 720p video VBR encoded at 4500 kbps using VC-1 Advanced Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for 720p (16:9 aspect ratio) content for delivery over 
        /// broadband connections. The output file extension is *.WMV. If the source frame size is not 1280x720, the video 
        /// will be scaled horizontally to the width of the profile target of 1280 pixels, and its height will be scaled 
        /// to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string VC1Broadband720p = "VC1 Broadband 720p";

        /// <summary>
        /// Produces a single Windows Media file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - SD video VBR encoded at 2200 kbps using VC-1 Advanced Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for SD (16:9 aspect ratio) content for delivery over broadband 
        /// connections. The output file extension is *.WMV. If the source frame size is not 852x480, the video will be scaled 
        /// horizontally to the width of the profile target of 852 pixels, and its height will be scaled to match the aspect 
        /// ratio of the source.
        /// </para>
        /// </summary>
        public const string VC1BroadbandSD16x9 = "VC1 Broadband SD 16x9";

        /// <summary>
        /// Produces a single Windows Media file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - SD video VBR encoded at 1800 kbps using VC-1 Advanced Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for SD (4:3 aspect ratio) content for delivery over broadband connections. 
        /// The output file extension is *.WMV. If the source frame size is not 640x480, the video will be scaled horizontally to 
        /// the width of the profile target of 640 pixels, and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string VC1BroadbandSD4x3 = "VC1 Broadband SD 4x3";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - 1080p video VBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the 
        /// source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height 
        /// will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreaming1080p = "VC1 Smooth Streaming 1080p";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - 720p video VBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the 
        /// source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height 
        /// will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreaming720p = "VC1 Smooth Streaming 720p";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using WMA Pro</para>
        ///     <para> - SD video VBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source 
        /// frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will 
        /// increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreamingSD16x9 = "VC1 Smooth Streaming SD 16x9";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using WMA Pro</para>
        ///     <para> - SD video VBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via IIS Smooth Streaming. If the source 
        /// frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will 
        /// increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreamingSD4x3 = "VC1 Smooth Streaming SD 4x3";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - 1080p video VBR encoded at 10 bitrates ranging from 9000 kbps to 350 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live Applications.
        /// If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will
        /// increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreaming1080pXboxLiveADK = "VC1 Smooth Streaming 1080p Xbox Live ADK";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using WMA Pro</para>
        ///     <para> - 720p video VBR encoded at 8 bitrates ranging from 4500 kbps to 350 kbps using VC-1 Advanced Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live 
        /// Applications. If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, 
        /// and the height will increase/decrease correspondingly. Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string VC1SmoothStreaming720pXboxLiveADK = "VC1 Smooth Streaming 720p Xbox Live ADK";

        #endregion

        #region H.264 Coding Standard

        /// <summary>
        /// Produces a single MP4 file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - 1080p video CBR encoded at 6750 kbps using H.264 High Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for 1080p (16:9 aspect ratio) content for delivery over broadband connections. 
        /// The output file extension is *.MP4. If the source frame size is not 1920x1080, the video will be scaled horizontally to the width 
        /// of the profile target of 1920 pixels, and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// <para>
        /// Note: This encoding is set to H.264 High Profile. Some devices with displays that do not support 1080p will not be able to decode H.264 High Profile content.
        /// </para>
        /// </summary>
        public const string H264Broadband1080p = "H264 Broadband 1080p";

        /// <summary>
        /// Produces a single MP4 file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 4500 kbps using H.264 Main Profile</para>
        /// <para>    
        /// Use this preset name to produce a downloadable file for 720p (16:9 aspect ratio) content for delivery over broadband connections. The output file 
        /// extension is *.MP4. If the source frame size is not 1280x720, the video will be scaled horizontally to the width of the profile target of 1280 pixels, 
        /// and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string H264Broadband720p = "H264 Broadband 720p";

        /// <summary>
        /// Produces a single MP4 file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - SD video VBR encoded at 2200 kbps using H.264 Main Profile</para>
        /// <para>
        /// Use this preset name to produce a downloadable file for SD (16:9 aspect ratio) content for delivery over broadband connections. The output file 
        /// extension is *.MP4. If the source frame size is not 852x480, the video will be scaled horizontally to the width of the profile target of 852 pixels, 
        /// and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string H264BroadbandSD16x9 = "H264 Broadband SD 16x9";

        /// <summary>
        /// Produces a single MP4 file with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - SD video VBR encoded at 1800 kbps using H.264 Main Profile</para>
        /// <para>    
        /// Use this preset name to produce a downloadable file for SD (4:3 aspect ratio) content for delivery over broadband connections. The output file 
        /// extension is *.MP4. If the source frame size is not 640x480, the video will be scaled horizontally to the width of the profile target of 640 pixels, 
        /// and its height will be scaled to match the aspect ratio of the source.
        /// </para>
        /// </summary>
        public const string H264BroadbandSD4x3 = "H264 Broadband SD 4x3";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - 1080p video CBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using H.264 High Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 
        /// 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. Videos at 
        /// lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Note: This encoding is set to H.264 High Profile. Some devices with displays that do not support 1080p will not be able to decode H.264 High Profile content.
        /// </para>
        /// </summary>
        public const string H264SmoothStreaming1080p = "H264 Smooth Streaming 1080p";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 1280x720, 
        /// will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates 
        /// will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264SmoothStreaming720p = "H264 Smooth Streaming 720p";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 8 bitrates ranging from 3400 kbps to 150 kbps using H.264 Main Profile, and two second GOPs.</para>
        /// <para>
        /// Same as H264 Smooth Streaming 720p, with audio lowered to 56 kbps, and two additional lower bitrate video layers added at 250 kbps and 150 kbps. These 
        /// lowest bitrate encodes should help when streaming over 3G or 4G connections to mobile devices
        /// </para>
        /// </summary>
        public const string H264SmoothStreaming720pFor3Gor4G = "H264 Smooth Streaming 720p for 3G or 4G";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 852x480, 
        /// will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates 
        /// will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264SmoothStreamingSD16x9 = "H264 Smooth Streaming SD 16x9";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via IIS Smooth Streaming. If the source frame size is not 640x480, 
        /// will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. Videos at lower bitrates 
        /// will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264SmoothStreamingSD4x3 = "H264 Smooth Streaming SD 4x3";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 128 kbps using AAC</para>
        ///     <para> - 1080p video CBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using H.264 High Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset name to produce an asset from 1080p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If the source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Note: This encoding is set to H.264 High Profile. Some devices with displays that do not support 1080p will not be able to decode H.264 High Profile content.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4Set1080p = "H264 Adaptive Bitrate MP4 Set 1080p";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4Set720p = "H264 Adaptive Bitrate MP4 Set 720p";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If the source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4SetSD16x9 = "H264 Adaptive Bitrate MP4 Set SD 16x9";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset name to produce an asset from SD (4:3 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If the source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4SetSD4x3 = "H264 Adaptive Bitrate MP4 Set SD 4x3";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC</para>
        ///     <para> - 1080p video CBR encoded at 8 bitrates ranging from 6000 kbps to 400 kbps using H.264 High Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset to produce an asset from 1080p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If source frame size is not 1920x1080, will stretch the video at the highest bitrate horizontally to 1920 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see <a href="https://developer.apple.com/library/ios/#qa/qa1767/_index.html">Resolving App Store Approval Issues for HTTP Live Streaming</a>.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4Set1080pForiOSCellularOnly = "H264 Adaptive Bitrate MP4 Set 1080p for iOS Cellular Only";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 6 bitrates ranging from 3400 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset to produce an asset from 720p (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see <a href="https://developer.apple.com/library/ios/#qa/qa1767/_index.html">Resolving App Store Approval Issues for HTTP Live Streaming</a>.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4Set720pForiOSCellularOnly = "H264 Adaptive Bitrate MP4 Set 720p for iOS Cellular Only";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1900 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset to produce an asset from SD (16:9 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If source frame size is not 852x480, will stretch the video at the highest bitrate horizontally to 852 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see <a href="https://developer.apple.com/library/ios/#qa/qa1767/_index.html">Resolving App Store Approval Issues for HTTP Live Streaming</a>.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4SetSD16x9ForiOSCellularOnly = "H264 Adaptive Bitrate MP4 Set SD 16x9 for iOS Cellular Only";

        /// <summary>
        /// Produces an asset with multiple GOP-aligned MP4 files:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 56 kbps using AAC</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1600 kbps to 400 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset to produce an asset from SD (4:3 aspect ratio) content for delivery via one of many adaptive streaming technologies after suitable packaging. 
        /// If source frame size is not 640x480, will stretch the video at the highest bitrate horizontally to 640 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled to one of 75%, 50% or 25% of the highest bitrate video.
        /// </para>
        /// <para>
        /// Audio is encoded at a low bitrate of 56 kbps, in order to satisfy App Store requirements for HLS. For more information, see <a href="https://developer.apple.com/library/ios/#qa/qa1767/_index.html">Resolving App Store Approval Issues for HTTP Live Streaming</a>.
        /// </para>
        /// </summary>
        public const string H264AdaptiveBitrateMP4SetSD4x3ForiOSCellularOnly = "H264 Adaptive Bitrate MP4 Set SD 4x3 for iOS Cellular Only";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 96 kbps using AAC</para>
        ///     <para> - 720p video CBR encoded at 8 bitrates ranging from 4500 kbps to 350 kbps using H.264 High Profile, and two second GOPs</para>
        /// <para>    
        /// Use this preset name to produce an asset from 720p (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Xbox Live Applications. 
        /// If the source frame size is not 1280x720, will stretch the video at the highest bitrate horizontally to 1280 pixels, and the height will increase/decrease correspondingly. 
        /// Videos at lower bitrates will be down-scaled respectively.
        /// </para>
        /// </summary>
        public const string H264SmoothStreaming720pXboxLiveADK = "H264 Smooth Streaming 720p Xbox Live ADK";

        /// <summary>
        /// Produces a Smooth Streaming asset with:
        ///     <para> - 44.1 kHz 16 bits/sample stereo audio CBR encoded at 64 kbps using HE-AAC Level 1</para>
        ///     <para> - SD video CBR encoded at 5 bitrates ranging from 1000 kbps to 200 kbps using H.264 Main Profile, and two second GOPs</para>
        /// <para>
        /// Use this preset name to produce an asset from SD (16:9 aspect ratio) content for delivery via IIS Smooth Streaming to Windows Phone 7 
        /// Series devices. If the source frame size is not 640x360, will stretch the video at all bitrates horizontally to 640 pixels, and the height 
        /// will increase/decrease correspondingly.
        /// </para>
        /// <para>
        /// Note: Windows Phone 7 doesn't support frame rates greater than 30fps. Also the Windows Azure Media Services encoder doesn’t do frame rate conversion. 
        /// So if the source content has a frame rate faster than 30fps, then the job output asset would as well. So it wouldn’t be supported on 
        /// Windows Phone 7 devices.
        /// </para>
        /// </summary>
        public const string H264SmoothStreamingWindowsPhone7Series = "H264 Smooth Streaming Windows Phone 7 Series";

        #endregion

        #region Thumbnail Presets
        /// <summary>
        /// Produces a series of JPEG thumbnails 5 seconds apart, 300 pixels wide. The height is determined by the source frame size.
        /// Use this preset name to generate a series of thumbnails for use in Xbox Live Applications. For information about providing 
        /// a custom configuration file, see <a href="http://msdn.microsoft.com/en-us/library/windowsazure/hh973624.aspx" >Task Preset for Thumbnail Generation.</a
        /// </summary>
        public const string Thumbnails = "Thumbnails";
        #endregion
    }
}
