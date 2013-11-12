// <copyright file="IAssetExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace MediaServices.Client.Extensions.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.Storage.Blob;

    [TestClass]
    public class IAssetExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowGenerateAssetFilesFromStorageIfAssetIsNull()
        {
            IAsset nullAsset = null;

            try
            {
                nullAsset.GenerateFromStorage();
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldGenerateAssetFilesFromBlobStorage()
        {
            var fileName = "smallwmv1.wmv";

            // Create empty asset.
            this.asset = this.context.Assets.Create(Path.GetFileNameWithoutExtension(fileName), AssetCreationOptions.None);

            // Create a SAS locator for the empty asset with write access.
            var sasLocator = this.context.Locators.Create(LocatorType.Sas, this.asset, AccessPermissions.Write, TimeSpan.FromDays(1));

            // Get a refence to the asset container in Blob storage.
            var locatorUri = new Uri(sasLocator.Path, UriKind.Absolute);
            var assetContainer = new CloudBlobContainer(locatorUri);

            // Upload a blob directly to the asset container.
            var blob = assetContainer.GetBlockBlobReference(fileName);
            blob.UploadFromStream(File.OpenRead(fileName));

            // Refresh the asset reference.
            this.asset = this.context.Assets.Where(a => a.Id == this.asset.Id).First();

            Assert.AreEqual(0, this.asset.AssetFiles.Count());

            // Create the AssetFiles from Blob storage.
            this.asset.GenerateFromStorage();

            // Refresh the asset reference.
            this.asset = this.context.Assets.Where(a => a.Id == this.asset.Id).First();

            Assert.AreEqual(1, this.asset.AssetFiles.Count());
            Assert.AreEqual(fileName, this.asset.AssetFiles.ToArray()[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowDownloadAssetFilesToFolderIfAssetIsNull()
        {
            IAsset nullAsset = null;

            string downloadFolderPath = CreateEmptyDirectory();

            try
            {
                Task downloadTask = nullAsset.DownloadToFolderAsync(downloadFolderPath, CancellationToken.None);

                downloadTask.Wait();
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
            finally
            {
                DeleteDirectoryIfExists(downloadFolderPath);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowDownloadAssetFilesToFolderIfFolderPathDoesNotExist()
        {
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            string downloadFolderPath = CreateEmptyDirectory();

            try
            {
                this.asset.DownloadToFolder(downloadFolderPath);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                throw;
            }
            finally
            {
                DeleteDirectoryIfExists(downloadFolderPath);
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldDownloadAssetFilesToFolder()
        {
            var originalFolderPath = "Media";
            this.asset = this.context.Assets.CreateFromFolder(originalFolderPath, AssetCreationOptions.None);
            var assetId = this.asset.Id;

            string downloadFolderPath = CreateEmptyDirectory();

            try
            {
                this.asset.DownloadToFolder(downloadFolderPath);

                Assert.AreEqual(3, Directory.GetFiles(downloadFolderPath).Length);

                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "smallwmv1.wmv");
                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "smallwmv2.wmv");
                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "dummy.ism");

                this.context = TestHelper.CreateContext();
                Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
            }
            finally
            {
                DeleteDirectoryIfExists(downloadFolderPath);
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldDownloadAssetFilesToFolderWithDownloadProgressChangedCallback()
        {
            var originalFolderPath = "Media";
            this.asset = this.context.Assets.CreateFromFolder(originalFolderPath, AssetCreationOptions.None);
            var assetId = this.asset.Id;

            string downloadFolderPath = CreateEmptyDirectory();

            try
            {
                var downloadResults = new ConcurrentDictionary<string, DownloadProgressChangedEventArgs>();
                Action<IAssetFile, DownloadProgressChangedEventArgs> downloadProgressChangedCallback =
                    (af, e) =>
                    {
                        IAssetFile assetFile = af;
                        DownloadProgressChangedEventArgs eventArgs = e;

                        Assert.IsNotNull(assetFile);
                        Assert.IsNotNull(eventArgs);

                        downloadResults.AddOrUpdate(assetFile.Name, eventArgs, (k, e2) => eventArgs);
                    };

                this.asset.DownloadToFolder(downloadFolderPath, downloadProgressChangedCallback);

                Assert.AreEqual(3, downloadResults.Count);
                Assert.AreEqual(3, Directory.GetFiles(downloadFolderPath).Length);

                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "smallwmv1.wmv", downloadResults["smallwmv1.wmv"]);
                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "smallwmv2.wmv", downloadResults["smallwmv2.wmv"]);
                AssertDownloadedFile(originalFolderPath, downloadFolderPath, "dummy.ism", downloadResults["dummy.ism"]);

                this.context = TestHelper.CreateContext();
                Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
            }
            finally
            {
                DeleteDirectoryIfExists(downloadFolderPath);
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldGetManifestAssetFile()
        {
            var folderName = "Media";
            this.asset = this.context.Assets.CreateFromFolder(folderName, AssetCreationOptions.None);

            var manifestAssetFile = this.asset.GetManifestAssetFile();

            Assert.IsNotNull(manifestAssetFile);
            Assert.AreEqual("dummy.ism", manifestAssetFile.Name);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldGetManifestAssetFileReturnNullIfThereIsNoManifestFile()
        {
            this.asset = this.context.Assets.CreateFromFile("smallwmv1.wmv", AssetCreationOptions.None);

            var manifestAssetFile = this.asset.GetManifestAssetFile();

            Assert.IsNull(manifestAssetFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetManifestAssetFileIfAssetIsNull()
        {
            IAsset nullAsset = null;

            nullAsset.GetManifestAssetFile();
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetSmoothStreamingUri()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var smoothStreamingUrl = this.asset.GetSmoothStreamingUri();

            Assert.IsNotNull(smoothStreamingUrl);
            Assert.IsTrue(
                smoothStreamingUrl
                    .AbsoluteUri
                    .EndsWith(locator.ContentAccessComponent + "/dummy.ism/manifest", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetHlsUri()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var hlsUri = this.asset.GetHlsUri();

            Assert.IsNotNull(hlsUri);
            Assert.IsTrue(
                hlsUri
                    .AbsoluteUri
                    .EndsWith(locator.ContentAccessComponent + "/dummy.ism/manifest(format=m3u8-aapl)", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetMpegDashUri()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var mpegDashUri = this.asset.GetMpegDashUri();

            Assert.IsNotNull(mpegDashUri);
            Assert.IsTrue(
                mpegDashUri
                    .AbsoluteUri
                    .EndsWith(locator.ContentAccessComponent + "/dummy.ism/manifest(format=mpd-time-csf)", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetStreamingUriIfAssetIsNull()
        {
            IAsset nullAsset = null;

            nullAsset.GetSmoothStreamingUri();
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetSasUri()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.Sas, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var assetFile = this.asset.AssetFiles.First();

            var sasUri = assetFile.GetSasUri();

            Assert.IsNotNull(sasUri);

            var client = new HttpClient();
            var response = client.GetAsync(sasUri, HttpCompletionOption.ResponseHeadersRead).Result;

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetSasUriIfAssetFileIsNull()
        {
            IAssetFile nullAssetFile = null;

            nullAssetFile.GetSasUri();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetSasUriWithSpecificLocatorIfAssetFileIsNull()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.Sas, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            IAssetFile nullAssetFile = null;

            nullAssetFile.GetSasUri(locator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetSasUriWithSpecificLocatorIfLocatorIsNull()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var assetFile = this.asset.AssetFiles.First();

            assetFile.GetSasUri(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetSasUriWithSpecificLocatorIfLocatorTypeIsNotSas()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var assetFile = this.asset.AssetFiles.First();

            assetFile.GetSasUri(locator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetSasUriWithSpecificLocatorIfLocatorDoesNotBelongToParentAsset()
        {
            var asset2 = this.context.Assets.Create("empty", AssetCreationOptions.None);
            var locator = this.context.Locators.Create(LocatorType.Sas, asset2, AccessPermissions.Read, TimeSpan.FromDays(1));

            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);
            var assetFile = this.asset.AssetFiles.First();

            try
            {
                assetFile.GetSasUri(locator);
            }
            finally
            {
                if (asset2 != null)
                {
                    asset2.Delete();
                }
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetSasUriWithSpecificLocator()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.Sas, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var assetFile = this.asset.AssetFiles.First();

            var sasUri = assetFile.GetSasUri(locator);

            Assert.IsNotNull(sasUri);

            var client = new HttpClient();
            var response = client.GetAsync(sasUri, HttpCompletionOption.ResponseHeadersRead).Result;

            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetMediaContextIfAssetIsNull()
        {
            IAsset nullAsset = null;

            nullAsset.GetMediaContext();
        }

        [TestMethod]
        public void ShouldGetMediaContext()
        {
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            var mediaContext = this.asset.GetMediaContext();

            Assert.IsNotNull(mediaContext);
            Assert.AreSame(this.context, mediaContext);
        }

        [TestInitialize]
        public void Initialize()
        {
            this.context = TestHelper.CreateContext();
            this.asset = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (this.asset != null)
            {
                this.asset.Delete();
            }
        }

        private static void AssertDownloadedFile(string originalFolderPath, string downloadFolderPath, string fileName, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs = null)
        {
            var expected = new FileInfo(Path.Combine(originalFolderPath, fileName));
            var result = new FileInfo(Path.Combine(downloadFolderPath, fileName));

            Assert.AreEqual(expected.Length, result.Length);

            if (downloadProgressChangedEventArgs != null)
            {
                Assert.AreEqual(expected.Length, downloadProgressChangedEventArgs.BytesDownloaded);
                Assert.AreEqual(expected.Length, downloadProgressChangedEventArgs.TotalBytes);
                Assert.AreEqual(100, downloadProgressChangedEventArgs.Progress);
            }
        }
        
        private static string CreateEmptyDirectory()
        {
            var downloadFolderPath = Path.GetRandomFileName();

            DeleteDirectoryIfExists(downloadFolderPath);

            Directory.CreateDirectory(downloadFolderPath);

            return downloadFolderPath;
        }

        private static void DeleteDirectoryIfExists(string downloadFolderPath)
        {
            if (downloadFolderPath != null)
            {
                if (Directory.Exists(downloadFolderPath))
                {
                    Directory.Delete(downloadFolderPath, true);
                }
            }
        }
    }
}
