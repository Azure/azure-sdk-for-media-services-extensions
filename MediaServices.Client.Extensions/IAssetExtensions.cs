// <copyright file="IAssetExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client.Metadata;
    using Microsoft.WindowsAzure.MediaServices.Client.TransientFaultHandling;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="IAsset"/> interface.
    /// </summary>
    public static class IAssetExtensions
    {
        /// <summary>
        /// File name suffix with extension which represents metadata about output of encoder.
        /// </summary>
        public const string MetadataFileSuffix = "_manifest.xml";

        /// <summary>
        /// File name suffix with extension which represents metadata about input of encoder.
        /// </summary>
        public const string InputMetadataFileSuffix = "_metadata.xml";

        private const int MaxNumberOfConcurrentCopyFromBlobOperations = 750;

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task"/> instance to generate <see cref="IAssetFile"/> for the <paramref name="asset"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance where to generate its <see cref="IAssetFile"/>.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance to generate <see cref="IAssetFile"/>.</returns>
        public static async Task GenerateFromStorageAsync(this IAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            MediaContextBase context = asset.GetMediaContext();

            Uri uriCreateFileInfos = new Uri(
                string.Format(CultureInfo.InvariantCulture, "/CreateFileInfos?assetid='{0}'", asset.Id),
                UriKind.Relative);

            await context
                .MediaServicesClassFactory
                .CreateDataServiceContext()
                .ExecuteAsync(uriCreateFileInfos, null, "GET")
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Generates <see cref="IAssetFile"/> for the <paramref name="asset"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance where to generate its <see cref="IAssetFile"/>.</param>
        public static void GenerateFromStorage(this IAsset asset)
        {
            using (Task task = asset.GenerateFromStorageAsync())
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;&gt;"/> instance to retrieve the <paramref name="asset"/> metadata.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to get the metadata.</param>
        /// <param name="sasLocator">The <see cref="ILocator"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;&gt;"/> instance to retrieve the <paramref name="asset"/> metadata.</returns>
        public static async Task<IEnumerable<AssetFileMetadata>> GetMetadataAsync(this IAsset asset, ILocator sasLocator, CancellationToken cancellationToken)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            if (sasLocator == null)
            {
                throw new ArgumentNullException("sasLocator", "The SAS locator cannot be null.");
            }

            if (sasLocator.Type != LocatorType.Sas)
            {
                throw new ArgumentException("The locator type must be SAS.", "sasLocator");
            }

            if (asset.Id != sasLocator.AssetId)
            {
                throw new ArgumentException("sasLocator", "The SAS locator does not belong to the asset.");
            }

            IEnumerable<AssetFileMetadata> assetMetadata = null;

            IAssetFile metadataAssetFile = asset
                .AssetFiles
                .ToArray()
                .FirstOrDefault(af => af.Name.EndsWith(MetadataFileSuffix, StringComparison.OrdinalIgnoreCase));

            if (metadataAssetFile != null)
            {
                MediaContextBase context = asset.GetMediaContext();

                Uri assetFileMetadataUri = metadataAssetFile.GetSasUri(sasLocator);

                assetMetadata = await AssetMetadataParser.ParseAssetFileMetadataAsync(
                        assetFileMetadataUri,
                        context.MediaServicesClassFactory.GetBlobStorageClientRetryPolicy().AsAzureStorageClientRetryPolicy(),
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            return assetMetadata;
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;&gt;"/> instance to retrieve the <paramref name="asset"/> metadata.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to get the metadata.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;&gt;"/> instance to retrieve the <paramref name="asset"/> metadata.</returns>
        public static async Task<IEnumerable<AssetFileMetadata>> GetMetadataAsync(this IAsset asset, CancellationToken cancellationToken)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            MediaContextBase context = asset.GetMediaContext();

            ILocator sasLocator = await context.Locators.CreateAsync(LocatorType.Sas, asset, AccessPermissions.Read, AssetBaseCollectionExtensions.DefaultAccessPolicyDuration).ConfigureAwait(false);

            IEnumerable<AssetFileMetadata> assetMetadata = await asset.GetMetadataAsync(sasLocator, cancellationToken).ConfigureAwait(false);

            await sasLocator.DeleteAsync().ConfigureAwait(false);

            return assetMetadata;
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;"/> enumeration with the <paramref name="asset"/> metadata.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to get the metadata.</param>
        /// <param name="sasLocator">The <see cref="ILocator"/> instance.</param>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;"/> enumeration with the <paramref name="asset"/> metadata.</returns>
        public static IEnumerable<AssetFileMetadata> GetMetadata(this IAsset asset, ILocator sasLocator)
        {
            using (Task<IEnumerable<AssetFileMetadata>> task = asset.GetMetadataAsync(sasLocator, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;"/> enumeration with the <paramref name="asset"/> metadata.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to get the metadata.</param>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerable&lt;AssetFileMetadata&gt;"/> enumeration with the <paramref name="asset"/> metadata.</returns>
        public static IEnumerable<AssetFileMetadata> GetMetadata(this IAsset asset)
        {
            using (Task<IEnumerable<AssetFileMetadata>> task = asset.GetMetadataAsync(CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task"/> instance to download all the asset files in the <paramref name="asset"/> to the <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to download the asset files.</param>
        /// <param name="folderPath">The path to the folder where to download the asset files in the <paramref name="asset"/>.</param>
        /// <param name="downloadProgressChangedCallback">A callback to report download progress for each asset file in the <paramref name="asset"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance to download all the asset files in the <paramref name="asset"/>.</returns>
        public static async Task DownloadToFolderAsync(this IAsset asset, string folderPath, Action<IAssetFile, DownloadProgressChangedEventArgs> downloadProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The folder '{0}' does not exist.", folderPath),
                    "folderPath");
            }

            MediaContextBase context = asset.GetMediaContext();

            ILocator sasLocator = await context.Locators.CreateAsync(LocatorType.Sas, asset, AccessPermissions.Read, AssetBaseCollectionExtensions.DefaultAccessPolicyDuration).ConfigureAwait(false);

            EventHandler<DownloadProgressChangedEventArgs> downloadProgressChangedHandler =
                (s, e) =>
                {
                    IAssetFile assetFile = (IAssetFile)s;
                    DownloadProgressChangedEventArgs eventArgs = e;

                    if (downloadProgressChangedCallback != null)
                    {
                        downloadProgressChangedCallback(assetFile, eventArgs);
                    }
                };

            List<Task> downloadTasks = new List<Task>();
            List<IAssetFile> assetFiles = asset.AssetFiles.ToList();
            foreach (IAssetFile assetFile in assetFiles)
            {
                string localDownloadPath = Path.Combine(folderPath, assetFile.Name);
                BlobTransferClient blobTransferClient = new BlobTransferClient
                {
                    NumberOfConcurrentTransfers = context.NumberOfConcurrentTransfers,
                    ParallelTransferThreadCount = context.ParallelTransferThreadCount
                };

                assetFile.DownloadProgressChanged += downloadProgressChangedHandler;

                downloadTasks.Add(
                    assetFile.DownloadAsync(Path.GetFullPath(localDownloadPath), blobTransferClient, sasLocator, cancellationToken));
            }

            await Task.Factory.ContinueWhenAll(downloadTasks.ToArray(), t => t, TaskContinuationOptions.ExecuteSynchronously).ConfigureAwait(false);

            await sasLocator.DeleteAsync().ConfigureAwait(false);

            assetFiles.ForEach(af => af.DownloadProgressChanged -= downloadProgressChangedHandler);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task"/> instance to download all the asset files in the <paramref name="asset"/> to the <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to download the asset files.</param>
        /// <param name="folderPath">The path to the folder where to download the asset files in the <paramref name="asset"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance to download all the asset files in the <paramref name="asset"/>.</returns>
        public static Task DownloadToFolderAsync(this IAsset asset, string folderPath, CancellationToken cancellationToken)
        {
            return asset.DownloadToFolderAsync(folderPath, null, cancellationToken);
        }

        /// <summary>
        /// Downloads all the asset files in the <paramref name="asset"/> to the <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to download the asset files.</param>
        /// <param name="folderPath">The path to the folder where to download the asset files in the <paramref name="asset"/>.</param>
        /// <param name="downloadProgressChangedCallback">A callback to report download progress for each asset file in the <paramref name="asset"/>.</param>
        public static void DownloadToFolder(this IAsset asset, string folderPath, Action<IAssetFile, DownloadProgressChangedEventArgs> downloadProgressChangedCallback)
        {
            using (Task task = asset.DownloadToFolderAsync(folderPath, downloadProgressChangedCallback, CancellationToken.None))
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Downloads all the asset files in the <paramref name="asset"/> to the <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance from where to download the asset files.</param>
        /// <param name="folderPath">The path to the folder where to download the asset files in the <paramref name="asset"/>.</param>
        public static void DownloadToFolder(this IAsset asset, string folderPath)
        {
            asset.DownloadToFolder(folderPath, null);
        }

        /// <summary>
        /// Returns the <see cref="IAssetFile"/> instance that represents the manifest file of the asset; otherwise, null.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>A <see cref="IAssetFile"/> instance that represents the manifest file of the asset; otherwise, null.</returns>
        public static IAssetFile GetManifestAssetFile(this IAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            return asset
                .AssetFiles
                .ToList()
                .Where(af => af.Name.EndsWith(ILocatorExtensions.ManifestFileExtension, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the Smooth Streaming URL of the <paramref name="asset"/> using the on-demand origin locator with the longest expiration time; otherwise, null.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the Smooth Streaming URL of the <paramref name="asset"/>; otherwise, null.</returns>
        public static Uri GetSmoothStreamingUri(this IAsset asset)
        {
            return asset.GetStreamingUri(string.Empty);
        }

        /// <summary>
        /// Returns the HLS version 4 URL of the <paramref name="asset"/> using the on-demand origin locator with the longest expiration time; otherwise, null.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the HLS version 4 URL of the <paramref name="asset"/>; otherwise, null.</returns>
        public static Uri GetHlsUri(this IAsset asset)
        {
            return asset.GetStreamingUri(ILocatorExtensions.HlsStreamingParameter);
        }

        /// <summary>
        /// Returns the HLS version 3 URL of the <paramref name="asset"/> using the on-demand origin locator with the longest expiration time; otherwise, null.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the HLS version 3 URL of the <paramref name="asset"/>; otherwise, null.</returns>
        public static Uri GetHlsv3Uri(this IAsset asset)
        {
            return asset.GetStreamingUri(ILocatorExtensions.Hlsv3StreamingParameter);
        }

        /// <summary>
        /// Returns the MPEG-DASH URL of the <paramref name="asset"/> using the on-demand origin locator with the longest expiration time; otherwise, null.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>A <see cref="System.Uri"/> representing the MPEG-DASH URL of the <paramref name="asset"/>; otherwise, null.</returns>
        public static Uri GetMpegDashUri(this IAsset asset)
        {
            return asset.GetStreamingUri(ILocatorExtensions.MpegDashStreamingParameter);
        }

        /// <summary>
        /// Copies the files in the <paramref name="sourceAsset"/> into into the <paramref name="destinationAsset"/> instance.
        /// </summary>
        /// <param name="sourceAsset">The <see cref="IAsset"/> instance that contains the asset files to copy.</param>
        /// <param name="destinationAsset">The <see cref="IAsset"/> instance that receives asset files.</param>
        /// <param name="destinationStorageCredentials">The <see cref="Microsoft.WindowsAzure.Storage.Auth.StorageCredentials"/> instance for the <paramref name="destinationAsset"/> Storage Account.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance to copy the files in the <paramref name="sourceAsset"/> into into the <paramref name="destinationAsset"/> instance.</returns>
        public static async Task CopyAsync(this IAsset sourceAsset, IAsset destinationAsset, StorageCredentials destinationStorageCredentials, CancellationToken cancellationToken)
        {
            if (sourceAsset == null)
            {
                throw new ArgumentNullException("sourceAsset", "The source asset cannot be null.");
            }

            if (destinationAsset == null)
            {
                throw new ArgumentNullException("destinationAsset", "The destination asset cannot be null.");
            }

            if (destinationStorageCredentials == null)
            {
                throw new ArgumentNullException("destinationStorageCredentials", "The destination storage credentials cannot be null.");
            }

            if (destinationStorageCredentials.IsAnonymous || destinationStorageCredentials.IsSAS)
            {
                throw new ArgumentException("The destination storage credentials must contain the account key credentials.", "destinationStorageCredentials");
            }

            if (!string.IsNullOrWhiteSpace(destinationStorageCredentials.AccountName) && !destinationStorageCredentials.AccountName.Equals(destinationAsset.StorageAccountName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The destination storage credentials does not belong to the destination asset storage account.", "destinationStorageCredentials");
            }

            MediaContextBase sourceContext = sourceAsset.GetMediaContext();
            ILocator sourceLocator = null;

            try
            {
                sourceLocator = await sourceContext.Locators.CreateAsync(LocatorType.Sas, sourceAsset, AccessPermissions.Read | AccessPermissions.List, AssetBaseCollectionExtensions.DefaultAccessPolicyDuration).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                IRetryPolicy retryPolicy = sourceContext.MediaServicesClassFactory.GetBlobStorageClientRetryPolicy().AsAzureStorageClientRetryPolicy();
                BlobRequestOptions options = new BlobRequestOptions { RetryPolicy = retryPolicy };
                CloudBlobContainer sourceContainer = new CloudBlobContainer(sourceAsset.Uri, new StorageCredentials(sourceLocator.ContentAccessComponent));
                CloudBlobContainer destinationContainer = new CloudBlobContainer(destinationAsset.Uri, destinationStorageCredentials);

                await CopyBlobsAsync(sourceContainer, destinationContainer, options, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                await CopyAssetFilesAsync(sourceAsset, destinationAsset, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (sourceLocator != null)
                {
                    sourceLocator.Delete();
                }
            }
        }

        /// <summary>
        /// Copies the files in the <paramref name="sourceAsset"/> into into the <paramref name="destinationAsset"/> instance.
        /// </summary>
        /// <param name="sourceAsset">The <see cref="IAsset"/> instance that contains the asset files to copy.</param>
        /// <param name="destinationAsset">The <see cref="IAsset"/> instance that receives asset files.</param>
        /// <param name="destinationStorageCredentials">The <see cref="Microsoft.WindowsAzure.Storage.Auth.StorageCredentials"/> instance for the <paramref name="destinationAsset"/> Storage Account.</param>
        public static void Copy(this IAsset sourceAsset, IAsset destinationAsset, StorageCredentials destinationStorageCredentials)
        {
            using (Task task = sourceAsset.CopyAsync(destinationAsset, destinationStorageCredentials, CancellationToken.None))
            {
                task.Wait();
            }
        }

        /// <summary>
        /// Returns the parent <see cref="MediaContextBase"/> instance.
        /// </summary>
        /// <param name="asset">The <see cref="IAsset"/> instance.</param>
        /// <returns>The parent <see cref="MediaContextBase"/> instance.</returns>
        public static MediaContextBase GetMediaContext(this IAsset asset)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            IMediaContextContainer mediaContextContainer = asset as IMediaContextContainer;
            MediaContextBase context = null;

            if (mediaContextContainer != null)
            {
                context = mediaContextContainer.GetMediaContext();
            }

            return context;
        }

        internal static async Task<IAssetFile> CreateAssetFileFromLocalFileAsync(this IAsset asset, string filePath, ILocator sasLocator, EventHandler<UploadProgressChangedEventArgs> uploadProgressChangedEventArgs, CancellationToken cancellationToken)
        {
            string assetFileName = Path.GetFileName(filePath);
            IAssetFile assetFile = await asset.AssetFiles.CreateAsync(assetFileName, cancellationToken).ConfigureAwait(false);
            MediaContextBase context = asset.GetMediaContext();

            assetFile.UploadProgressChanged += uploadProgressChangedEventArgs;

            BlobTransferClient blobTransferClient = new BlobTransferClient
            {
                NumberOfConcurrentTransfers = context.NumberOfConcurrentTransfers,
                ParallelTransferThreadCount = context.ParallelTransferThreadCount
            };

            await assetFile.UploadAsync(filePath, blobTransferClient, sasLocator, cancellationToken).ConfigureAwait(false);

            assetFile.UploadProgressChanged -= uploadProgressChangedEventArgs;

            if (assetFileName.EndsWith(ILocatorExtensions.ManifestFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                assetFile.IsPrimary = true;
                await assetFile.UpdateAsync().ConfigureAwait(false);
            }

            return assetFile;
        }

        private static Uri GetStreamingUri(this IAsset asset, string streamingParameter)
        {
            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            Uri smoothStreamingUri = null;
            IAssetFile manifestAssetFile = asset.GetManifestAssetFile();
            if (manifestAssetFile != null)
            {
                ILocator originLocator = asset
                    .Locators
                    .ToList()
                    .Where(l => l.Type == LocatorType.OnDemandOrigin)
                    .OrderBy(l => l.ExpirationDateTime)
                    .FirstOrDefault();
                if (originLocator != null)
                {
                    smoothStreamingUri = new Uri(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ILocatorExtensions.BaseStreamingUrlTemplate,
                            originLocator.Path.TrimEnd('/'),
                            manifestAssetFile.Name,
                            streamingParameter),
                        UriKind.Absolute);
                }
            }

            return smoothStreamingUri;
        }

        private static async Task CopyBlobsAsync(CloudBlobContainer sourceContainer, CloudBlobContainer destinationContainer, BlobRequestOptions options, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;

            do
            {
                BlobResultSegment resultSegment = await sourceContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, MaxNumberOfConcurrentCopyFromBlobOperations, continuationToken, options, null, cancellationToken).ConfigureAwait(false);

                IEnumerable<Task> copyTasks = resultSegment
                    .Results
                    .Cast<CloudBlockBlob>()
                    .Select(
                        sourceBlob =>
                        {
                            CloudBlockBlob destinationBlob = destinationContainer.GetBlockBlobReference(sourceBlob.Name);

                            return CopyBlobAsync(destinationBlob, sourceBlob, options, cancellationToken);
                        });

                await Task.WhenAll(copyTasks).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        private static async Task CopyBlobAsync(CloudBlockBlob destinationBlob, CloudBlockBlob sourceBlob, BlobRequestOptions options, CancellationToken cancellationToken)
        {
            await destinationBlob.StartCopyFromBlobAsync(sourceBlob, null, null, options, null, cancellationToken).ConfigureAwait(false);

            CopyState copyState = destinationBlob.CopyState;
            while (copyState == null || copyState.Status == CopyStatus.Pending)
            {
                await destinationBlob.FetchAttributesAsync(null, options, null, cancellationToken).ConfigureAwait(false);

                copyState = destinationBlob.CopyState;
                if (copyState != null && copyState.Status != CopyStatus.Pending && copyState.Status != CopyStatus.Success)
                {
                    throw new StorageException(copyState.StatusDescription);
                }
            }
        }

        private static Task CopyAssetFilesAsync(IAsset sourceAsset, IAsset destinationAsset, CancellationToken cancellationToken)
        {
            IAssetFile[] sourceAssetFiles = sourceAsset.AssetFiles.ToArray();
            IDictionary<string, IAssetFile> destinationAssetFiles = destinationAsset.AssetFiles.ToArray().ToDictionary(af => af.Name);
            IList<Task> copyTasks = new List<Task>();

            foreach (var sourceAssetFile in sourceAssetFiles)
            {
                if (!destinationAssetFiles.ContainsKey(sourceAssetFile.Name))
                {
                    copyTasks.Add(destinationAsset.AssetFiles.CreateCopyAsync(sourceAssetFile, cancellationToken));
                }
            }

            return Task.WhenAll(copyTasks);
        }

        private static async Task<IAssetFile> CreateCopyAsync(this AssetFileBaseCollection assetFiles, IAssetFile assetFile, CancellationToken cancellationToken)
        {
            IAssetFile assetFileCopy = await assetFiles.CreateAsync(assetFile.Name, cancellationToken).ConfigureAwait(false);

            assetFileCopy.IsPrimary = assetFile.IsPrimary;
            assetFileCopy.ContentFileSize = assetFile.ContentFileSize;
            assetFileCopy.MimeType = assetFile.MimeType;

            await assetFileCopy.UpdateAsync().ConfigureAwait(false);

            return assetFileCopy;
        }
    }
}
