// <copyright file="AssetBaseCollectionExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="AssetBaseCollection"/> class..
    /// </summary>
    public static class AssetBaseCollectionExtensions
    {
        internal static readonly TimeSpan DefaultAccessPolicyDuration = TimeSpan.FromDays(1);

        /// <summary>
        /// Returns a new empty <see cref="IAsset"/> within one selected storage account from <paramref name="storageAccountNames"/> based on the default <see cref="IAccountSelectionStrategy"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="assetName">The asset name.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new empty <see cref="IAsset"/> within one selected storage account from the provided <see cref="IAccountSelectionStrategy"/>.</returns>
        public static IAsset Create(this AssetBaseCollection assets, string assetName, IAccountSelectionStrategy strategy, AssetCreationOptions options)
        {
            if (assets == null)
            {
                throw new ArgumentNullException("assets");
            }

            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string storageAccountName = strategy.SelectAccountForAsset();

            return assets.Create(assetName, storageAccountName, options);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new empty <see cref="IAsset"/> asset within one selected storage account from <paramref name="storageAccountNames"/> based on the default <see cref="IAccountSelectionStrategy"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="assetName">The asset name.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="token">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new empty <see cref="IAsset"/> within one selected storage account from the given <see cref="IAccountSelectionStrategy"/>.</returns>
        public static Task<IAsset> CreateAsync(this AssetBaseCollection assets, string assetName, IAccountSelectionStrategy strategy, AssetCreationOptions options, CancellationToken token)
        {
            if (assets == null)
            {
                throw new ArgumentNullException("assets");
            }

            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string storageAccountName = strategy.SelectAccountForAsset();

            return assets.CreateAsync(assetName, storageAccountName, options, token);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static Task<IAsset> CreateFromFileAsync(this AssetBaseCollection assets, string filePath, IAccountSelectionStrategy strategy, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string storageAccountName = strategy.SelectAccountForAsset();

            return assets.CreateFromFileAsync(filePath, storageAccountName, options, uploadProgressChangedCallback, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static async Task<IAsset> CreateFromFileAsync(this AssetBaseCollection assets, string filePath, string storageAccountName, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (assets == null)
            {
                throw new ArgumentNullException("assets", "The assets collection cannot be null.");
            }

            MediaContextBase context = assets.MediaContext;

            string assetName = Path.GetFileName(filePath);

            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                storageAccountName = context.DefaultStorageAccount.Name;
            }

            IAsset asset = await assets.CreateAsync(assetName, storageAccountName, options, cancellationToken);

            ILocator sasLocator = await context.Locators.CreateAsync(LocatorType.Sas, asset, AccessPermissions.Write | AccessPermissions.List, DefaultAccessPolicyDuration);

            EventHandler<UploadProgressChangedEventArgs> uploadProgressChangedHandler =
                (s, e) =>
                {
                    IAssetFile assetFile = (IAssetFile)s;
                    UploadProgressChangedEventArgs eventArgs = e;

                    if (uploadProgressChangedCallback != null)
                    {
                        uploadProgressChangedCallback(assetFile, eventArgs);
                    }
                };

            await asset.CreateAssetFileFromLocalFileAsync(filePath, sasLocator, uploadProgressChangedHandler, cancellationToken);

            await sasLocator.DeleteAsync();

            return asset;
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static Task<IAsset> CreateFromFileAsync(this AssetBaseCollection assets, string filePath, string storageAccountName, AssetCreationOptions options, CancellationToken cancellationToken)
        {
            return assets.CreateFromFileAsync(filePath, storageAccountName, options, null, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static Task<IAsset> CreateFromFileAsync(this AssetBaseCollection assets, string filePath, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            return assets.CreateFromFileAsync(filePath, (string)null, options, uploadProgressChangedCallback, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static Task<IAsset> CreateFromFileAsync(this AssetBaseCollection assets, string filePath, AssetCreationOptions options, CancellationToken cancellationToken)
        {
            return assets.CreateFromFileAsync(filePath, options, null, cancellationToken);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <returns>A new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static IAsset CreateFromFile(this AssetBaseCollection assets, string filePath, IAccountSelectionStrategy strategy, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            using (Task<IAsset> task = assets.CreateFromFileAsync(filePath, strategy, options, uploadProgressChangedCallback, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <returns>A new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static IAsset CreateFromFile(this AssetBaseCollection assets, string filePath, string storageAccountName, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            using (Task<IAsset> task = assets.CreateFromFileAsync(filePath, storageAccountName, options, uploadProgressChangedCallback, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static IAsset CreateFromFile(this AssetBaseCollection assets, string filePath, string storageAccountName, AssetCreationOptions options)
        {
            return assets.CreateFromFile(filePath, storageAccountName, options, null);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report the upload progress of the file.</param>
        /// <returns>A new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static IAsset CreateFromFile(this AssetBaseCollection assets, string filePath, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            return assets.CreateFromFile(filePath, (string)null, options, uploadProgressChangedCallback);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="filePath">The path to the file to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new <see cref="IAsset"/> with the file in <paramref name="filePath"/>.</returns>
        public static IAsset CreateFromFile(this AssetBaseCollection assets, string filePath, AssetCreationOptions options)
        {
            return assets.CreateFromFile(filePath, options, null);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static Task<IAsset> CreateFromFolderAsync(this AssetBaseCollection assets, string folderPath, IAccountSelectionStrategy strategy, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (assets == null)
            {
                throw new ArgumentNullException("assets");
            }

            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string storageAccountName = strategy.SelectAccountForAsset();

            return assets.CreateFromFolderAsync(folderPath, storageAccountName, options, uploadProgressChangedCallback, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static async Task<IAsset> CreateFromFolderAsync(this AssetBaseCollection assets, string folderPath, string storageAccountName, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (assets == null)
            {
                throw new ArgumentNullException("assets", "The assets collection cannot be null.");
            }

            IEnumerable<string> filePaths = Directory.EnumerateFiles(folderPath);
            if (!filePaths.Any())
            {
                throw new FileNotFoundException(
                    string.Format(CultureInfo.InvariantCulture, "No files in directory, check the folder path: '{0}'", folderPath));
            }

            MediaContextBase context = assets.MediaContext;

            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                storageAccountName = context.DefaultStorageAccount.Name;
            }

            string assetName = Path.GetFileName(Path.GetFullPath(folderPath.TrimEnd('\\')));
            IAsset asset = await context.Assets.CreateAsync(assetName, storageAccountName, options, cancellationToken);

            ILocator sasLocator = await context.Locators.CreateAsync(LocatorType.Sas, asset, AccessPermissions.Write | AccessPermissions.List, DefaultAccessPolicyDuration);

            EventHandler<UploadProgressChangedEventArgs> uploadProgressChangedHandler =
                (s, e) =>
                {
                    IAssetFile assetFile = (IAssetFile)s;
                    UploadProgressChangedEventArgs eventArgs = e;

                    if (uploadProgressChangedCallback != null)
                    {
                        uploadProgressChangedCallback(assetFile, eventArgs);
                    }
                };

            IList<Task> uploadTasks = new List<Task>();
            foreach (string filePath in filePaths)
            {
                uploadTasks.Add(asset.CreateAssetFileFromLocalFileAsync(filePath, sasLocator, uploadProgressChangedHandler, cancellationToken));
            }

            await Task.Factory.ContinueWhenAll(uploadTasks.ToArray(), t => t, TaskContinuationOptions.ExecuteSynchronously);

            await sasLocator.DeleteAsync();

            return asset;
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static Task<IAsset> CreateFromFolderAsync(this AssetBaseCollection assets, string folderPath, string storageAccountName, AssetCreationOptions options, CancellationToken cancellationToken)
        {
            return assets.CreateFromFolderAsync(folderPath, storageAccountName, options, null, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static Task<IAsset> CreateFromFolderAsync(this AssetBaseCollection assets, string folderPath, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback, CancellationToken cancellationToken)
        {
            return assets.CreateFromFolderAsync(folderPath, (string)null, options, uploadProgressChangedCallback, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for the new <see cref="IAsset"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;IAsset&gt;"/> instance for new <see cref="IAsset"/>.</returns>
        public static Task<IAsset> CreateFromFolderAsync(this AssetBaseCollection assets, string folderPath, AssetCreationOptions options, CancellationToken cancellationToken)
        {
            return assets.CreateFromFolderAsync(folderPath, options, null, cancellationToken);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <returns>A new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static IAsset CreateFromFolder(this AssetBaseCollection assets, string folderPath, IAccountSelectionStrategy strategy, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            using (Task<IAsset> task = assets.CreateFromFolderAsync(folderPath, strategy, options, uploadProgressChangedCallback, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <returns>A new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static IAsset CreateFromFolder(this AssetBaseCollection assets, string folderPath, string storageAccountName, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            using (Task<IAsset> task = assets.CreateFromFolderAsync(folderPath, storageAccountName, options, uploadProgressChangedCallback, CancellationToken.None))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="storageAccountName">The name of the storage account where to store the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static IAsset CreateFromFolder(this AssetBaseCollection assets, string folderPath, string storageAccountName, AssetCreationOptions options)
        {
            return assets.CreateFromFolder(folderPath, storageAccountName, options, null);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <param name="uploadProgressChangedCallback">A callback to report upload progress of the files.</param>
        /// <returns>A new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static IAsset CreateFromFolder(this AssetBaseCollection assets, string folderPath, AssetCreationOptions options, Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback)
        {
            return assets.CreateFromFolder(folderPath, (string)null, options, uploadProgressChangedCallback);
        }

        /// <summary>
        /// Returns a new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="folderPath">The path to the folder with the files to upload to the new <see cref="IAsset"/>.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new <see cref="IAsset"/> with the files in <paramref name="folderPath"/>.</returns>
        public static IAsset CreateFromFolder(this AssetBaseCollection assets, string folderPath, AssetCreationOptions options)
        {
            return assets.CreateFromFolder(folderPath, options, null);
        }
    }
}
