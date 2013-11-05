// <copyright file="ILocatorExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Globalization;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="ILocator"/> interface.
    /// </summary>
    public static class ILocatorExtensions
    {
        /// <summary>
        /// Represents the manifest file extension.
        /// </summary>
        public const string ManifestFileExtension = ".ism";

        /// <summary>
        /// Represents the URL dynamic packaging parameter for HLS.
        /// </summary>
        public const string HlsStreamingParameter = "(format=m3u8-aapl)";

        /// <summary>
        /// Represents the URL dynamic packaging parameter for MPEG-DASH.
        /// </summary>
        public const string MpegDashStreamingParameter = "(format=mpd-time-csf)";

        internal const string BaseStreamingUrlTemplate = "{0}/{1}/manifest{2}";

        /// <summary>
        /// Returns the Smooth Streaming URL of the <paramref name="originLocator"/>; otherwise, null.
        /// </summary>
        /// <param name="originLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the Smooth Streaming URL of the <paramref name="originLocator"/>; otherwise, null.</returns>
        public static Uri GetSmoothStreamingUri(this ILocator originLocator)
        {
            return originLocator.GetStreamingUri(string.Empty);
        }

        /// <summary>
        /// Returns the HLS URL of the <paramref name="originLocator"/>; otherwise, null.
        /// </summary>
        /// <param name="originLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the HLS URL of the <paramref name="originLocator"/>; otherwise, null.</returns>
        public static Uri GetHlsUri(this ILocator originLocator)
        {
            return originLocator.GetStreamingUri(HlsStreamingParameter);
        }

        /// <summary>
        /// Returns the MPEG-DASH URL of the <paramref name="originLocator"/>; otherwise, null.
        /// </summary>
        /// <param name="originLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the MPEG-DASH URL of the <paramref name="originLocator"/>; otherwise, null.</returns>
        public static Uri GetMpegDashUri(this ILocator originLocator)
        {
            return originLocator.GetStreamingUri(MpegDashStreamingParameter);
        }

        private static Uri GetStreamingUri(this ILocator originLocator, string streamingParameter)
        {
            if (originLocator == null)
            {
                throw new ArgumentNullException("locator", "The locator cannot be null.");
            }

            if (originLocator.Type != LocatorType.OnDemandOrigin)
            {
                throw new ArgumentException("The locator type must be on-demand origin.", "originLocator");
            }

            Uri smoothStreamingUri = null;
            IAsset asset = originLocator.Asset;
            IAssetFile manifestAssetFile = asset.GetManifestAssetFile();
            if (manifestAssetFile != null)
            {
                smoothStreamingUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        BaseStreamingUrlTemplate,
                        originLocator.Path.TrimEnd('/'),
                        manifestAssetFile.Name,
                        streamingParameter),
                    UriKind.Absolute);
            }

            return smoothStreamingUri;
        }
    }
}
