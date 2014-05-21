// <copyright file="IAssetFileExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client.Metadata;
    using Microsoft.WindowsAzure.MediaServices.Client.TransientFaultHandling;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="IAssetFiles"/> interface.
    /// </summary>
    public static class IAssetFileExtensions
    {
        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;AssetFileMetadata&gt;"/> instance to retreive the <paramref name="assetFile"/> metadata.
        /// </summary>
        /// <param name="assetFile">The <see cref="IAssetFile"/> instance from where to get the metadata.</param>
        /// <param name="sasLocator">The <see cref="ILocator"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;AssetFileMetadata&gt;"/> instance to retreive the <paramref name="assetFile"/> metadata.</returns>
        public static async Task<AssetFileMetadata> GetMetadataAsync(this IAssetFile assetFile, ILocator sasLocator, CancellationToken cancellationToken)
        {
            if (assetFile == null)
            {
                throw new ArgumentNullException("assetFile", "The asset file cannot be null.");
            }

            if (sasLocator == null)
            {
                throw new ArgumentNullException("sasLocator", "The SAS locator cannot be null.");
            }

            if (sasLocator.Type != LocatorType.Sas)
            {
                throw new ArgumentException("The locator type must be SAS.", "sasLocator");
            }

            if (assetFile.ParentAssetId != sasLocator.AssetId)
            {
                throw new ArgumentException("sasLocator", "The SAS locator does not belong to the asset.");
            }

            AssetFileMetadata assetFileMetadata = null;

            if (!assetFile.Name.EndsWith(IAssetExtensions.MetadataFileSuffix, StringComparison.OrdinalIgnoreCase))
            {
                IAsset asset = assetFile.Asset;

                IEnumerable<AssetFileMetadata> assetMetadata = await asset.GetMetadataAsync(sasLocator, cancellationToken).ConfigureAwait(false);

                if (assetMetadata != null)
                {
                    assetFileMetadata = assetMetadata.FirstOrDefault(am => assetFile.Name.Equals(am.Name, StringComparison.OrdinalIgnoreCase));
                }
            }

            return assetFileMetadata;
        }

        /// <summary>
        /// Returns a <see cref="AssetFileMetadata"/> instance with the <paramref name="assetFile"/> metadata.
        /// </summary>
        /// <param name="assetFile">The <see cref="IAssetFile"/> instance from where to get the metadata.</param>
        /// <param name="sasLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="AssetFileMetadata"/> instance with the <paramref name="assetFile"/> metadata.</returns>
        public static AssetFileMetadata GetMetadata(this IAssetFile assetFile, ILocator sasLocator)
        {
            using (Task<AssetFileMetadata> task = assetFile.GetMetadataAsync(sasLocator, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns the SAS URL of the <paramref name="assetFile"/> for progressive download using the SAS locator with the longest expiration time; otherwise, null.
        /// </summary>
        /// <param name="assetFile">The <see cref="IAssetFile"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the SAS URL of the <paramref name="assetFile"/> for progressive download; otherwise, null.</returns>
        public static Uri GetSasUri(this IAssetFile assetFile)
        {
            if (assetFile == null)
            {
                throw new ArgumentNullException("assetFile", "The asset file cannot be null.");
            }

            Uri sasUri = null;
            IAsset asset = assetFile.Asset;
            if (asset != null)
            {
                ILocator sasLocator = asset
                    .Locators
                    .ToList()
                    .Where(l => l.Type == LocatorType.Sas)
                    .OrderBy(l => l.ExpirationDateTime)
                    .LastOrDefault();
                if (sasLocator != null)
                {
                    sasUri = BuildSasUri(assetFile, sasLocator);
                }
            }

            return sasUri;
        }

        /// <summary>
        /// Returns the SAS URL of the <paramref name="assetFile"/> for progressive download using the <paramref name="sasLocator"/>.
        /// </summary>
        /// <param name="assetFile">The <see cref="IAssetFile"/> instance.</param>
        /// <param name="sasLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the SAS URL of the <paramref name="assetFile"/> for progressive download; otherwise, null.</returns>
        public static Uri GetSasUri(this IAssetFile assetFile, ILocator sasLocator)
        {
            if (assetFile == null)
            {
                throw new ArgumentNullException("assetFile", "The asset file cannot be null.");
            }

            if (sasLocator == null)
            {
                throw new ArgumentNullException("sasLocator", "The SAS locator cannot be null.");
            }

            if (sasLocator.Type != LocatorType.Sas)
            {
                throw new ArgumentException("The locator type must be SAS.", "sasLocator");
            }

            if (assetFile.ParentAssetId != sasLocator.AssetId)
            {
                throw new ArgumentException("sasLocator", "The SAS locator does not belong to the parent asset.");
            }

            Uri sasUri = BuildSasUri(assetFile, sasLocator);

            return sasUri;
        }

        /// <summary>
        /// Returns the parent <see cref="MediaContextBase"/> instance.
        /// </summary>
        /// <param name="assetFile">The <see cref="IAssetFile"/> instance.</param>
        /// <returns>The parent <see cref="MediaContextBase"/> instance.</returns>
        public static MediaContextBase GetMediaContext(this IAssetFile assetFile)
        {
            if (assetFile == null)
            {
                throw new ArgumentNullException("assetFile", "The asset file cannot be null.");
            }

            IMediaContextContainer mediaContextContainer = assetFile as IMediaContextContainer;
            MediaContextBase context = null;

            if (mediaContextContainer != null)
            {
                context = mediaContextContainer.GetMediaContext();
            }

            return context;
        }

        private static Uri BuildSasUri(IAssetFile assetFile, ILocator sasLocator)
        {
            UriBuilder builder = new UriBuilder(new Uri(sasLocator.Path, UriKind.Absolute));
            builder.Path = Path.Combine(builder.Path, assetFile.Name);

            return builder.Uri;
        }
    }
}
