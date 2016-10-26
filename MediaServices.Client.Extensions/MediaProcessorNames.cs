// <copyright file="MediaProcessorNames.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System;

    /// <summary>
    /// Contains string constants with the available media processors' names.
    /// </summary>
    public static class MediaProcessorNames
    {
        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Face Detector'.
        /// </summary>
        public const string AzureMediaFaceDetector = "Azure Media Face Detector";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Hyperlapse'.
        /// </summary>
        public const string AzureMediaHyperlapse = "Azure Media Hyperlapse";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Indexer'.
        /// </summary>
        public const string AzureMediaIndexer = "Azure Media Indexer";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Indexer 2 Preview'.
        /// </summary>
        public const string AzureMediaIndexer2Preview = "Azure Media Indexer 2 Preview";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Motion Detector'.
        /// </summary>
        public const string AzureMediaMotionDetector = "Azure Media Motion Detector";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media OCR'.
        /// </summary>
        public const string AzureMediaOCR = "Azure Media OCR";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Stabilizer'.
        /// </summary>
        public const string AzureMediaStabilizer = "Azure Media Stabilizer";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Video Thumbnails'.
        /// </summary>
        public const string AzureMediaVideoThumbnails = "Azure Media Video Thumbnails";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Media Encoder Premium Workflow'.
        /// </summary>
        public const string MediaEncoderPremiumWorkflow = "Media Encoder Premium Workflow";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Media Encoder Standard'.
        /// </summary>
        public const string MediaEncoderStandard = "Media Encoder Standard";

        /// <summary>
        /// Lets you decrypt media assets that were encrypted using storage encryption.
        /// </summary>
        public const string StorageDecryption = "Storage Decryption";

        #region Deprecated Processors

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Windows Azure Media Encoder'.
        /// </summary>
        [Obsolete]
        public const string WindowsAzureMediaEncoder = "Windows Azure Media Encoder";

        /// <summary>
        /// Lets you run encoding tasks using the processor 'Azure Media Encoder'.
        /// </summary>
        [Obsolete]
        public const string AzureMediaEncoder = "Azure Media Encoder";

        /// <summary>
        /// Lets you convert media assets from MP4 to Smooth Streaming format. Also, lets you convert media assets 
        /// from Smooth Streaming to the Apple HTTP Live Streaming (HLS) format.
        /// </summary>
        [Obsolete]
        public const string WindowsAzureMediaPackager = "Windows Azure Media Packager";

        /// <summary>
        /// Lets you encrypt media assets using PlayReady Protection.
        /// </summary>
        [Obsolete]
        public const string WindowsAzureMediaEncryptor = "Windows Azure Media Encryptor";

        #endregion
    }
}
