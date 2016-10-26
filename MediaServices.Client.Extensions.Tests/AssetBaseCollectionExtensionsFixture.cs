// <copyright file="AssetBaseCollectionExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    [TestClass]
    public class AssetBaseCollectionExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;
        private CloudBlobContainer container;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfSelectionStrategyIsNullDuringAssetCreateAsync()
        {
            IAccountSelectionStrategy selectionStrategy = null;
            CancellationToken token;

            try
            {
                Task<IAsset> assetTask = this.context.Assets.CreateAsync(Guid.NewGuid().ToString(), selectionStrategy, AssetCreationOptions.None, token);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual(ane.ParamName, "strategy");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfSelectionStrategyIsNullDuringAssetCreate()
        {
            IAccountSelectionStrategy selectionStrategy = null;

            try
            {
                this.asset = this.context.Assets.Create(Guid.NewGuid().ToString(), selectionStrategy, AssetCreationOptions.None);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual(ane.ParamName, "strategy");
                throw;
            }
        }

        [TestMethod]
        public void ShouldCreateAssetWithRandomAccountSelectionStrategy()
        {
            IAccountSelectionStrategy selectionStrategy = RandomAccountSelectionStrategy.FromAccounts(this.context);

            this.asset = this.context.Assets.Create(Guid.NewGuid().ToString(), selectionStrategy, AssetCreationOptions.None);
        }

        [TestMethod]
        public void ShouldRedistributeCreationOfAssetBetweenAllStorageAccounts()
        {
            // Defining list of accounts to select from.
            string[] storageAccountNames = new[] { "account1", "account2", "account3" };

            IAccountSelectionStrategy selectionStrategy = new RandomAccountSelectionStrategy(storageAccountNames);

            var selectedStorageAccounts = new Dictionary<string, int>();
            for (int i = 0; i < 50; i++)
            {
                var selectedStorageAccount = selectionStrategy.SelectAccountForAsset();
                if (!selectedStorageAccounts.ContainsKey(selectedStorageAccount))
                {
                    selectedStorageAccounts.Add(selectedStorageAccount, 0);
                }
                else
                {
                    selectedStorageAccounts[selectedStorageAccount] += 1;
                }

                Thread.Sleep(100);
            }

            // Check if all storage accounts participated in redistribution.
            Assert.AreEqual(storageAccountNames.Length, selectedStorageAccounts.Keys.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowCreatingStrategyIfStorageAccountsArrayIsNull()
        {
            // Defining list of accounts to select from.
            string[] nullStorageAccountNames = null;

            IAccountSelectionStrategy strategy = new RandomAccountSelectionStrategy(nullStorageAccountNames);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCreatingStrategyIfStorageAccountsArrayIsEmpty()
        {
            // Defining list of accounts to select from.
            string[] nullStorageAccountNames = new string[0];

            IAccountSelectionStrategy strategy = new RandomAccountSelectionStrategy(nullStorageAccountNames);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromFileIfAssetCollectionIsNull()
        {
            AssetBaseCollection nullAssets = null;

            try
            {
                nullAssets.CreateFromFileAsync(string.Empty, AssetCreationOptions.None, CancellationToken.None).Wait();
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfSelectionStrategyIsNullDuringCreateFromFileAsync()
        {
            IAccountSelectionStrategy selectionStrategy = null;
            CancellationToken token;

            try
            {
                Task<IAsset> assetTask = this.context.Assets.CreateFromFileAsync(Guid.NewGuid().ToString(), selectionStrategy, AssetCreationOptions.None, null, token);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual(ane.ParamName, "strategy");
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldCreateAssetFromFile()
        {
            var fileName = "smallwmv1.wmv";
            this.asset = this.context.Assets.CreateFromFile(fileName, null, AssetCreationOptions.None);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(fileName, this.asset.Name);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(1, assetFiles.Count());
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(0).Name);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldCreateAssetFromFileWithRandomAccountSelectionStrategy()
        {
            RandomAccountSelectionStrategy strategy = RandomAccountSelectionStrategy.FromAccounts(this.context);

            var fileName = "smallwmv1.wmv";
            this.asset = this.context.Assets.CreateFromFile(fileName, strategy, AssetCreationOptions.None, null);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(fileName, this.asset.Name);
            IList<string> storageAccountNames = strategy.GetStorageAccounts();
            CollectionAssert.Contains((ICollection)storageAccountNames, this.asset.StorageAccountName);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(1, assetFiles.Count());
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(0).Name);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldCreateAssetFromFileWithUploadProgressChangedCallback()
        {
            var uploadResults = new ConcurrentDictionary<string, UploadProgressChangedEventArgs>();
            Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback =
                (af, e) =>
                {
                    IAssetFile assetFile = af;
                    UploadProgressChangedEventArgs eventArgs = e;

                    Assert.IsNotNull(assetFile);
                    Assert.IsNotNull(eventArgs);

                    uploadResults.AddOrUpdate(assetFile.Name, eventArgs, (k, e2) => eventArgs);
                };

            var fileName = "smallwmv1.wmv";
            this.asset = this.context.Assets.CreateFromFile(fileName, AssetCreationOptions.None, uploadProgressChangedCallback);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(fileName, this.asset.Name);

            Assert.AreEqual(1, uploadResults.Count);

            AssertUploadedFile(".\\", fileName, uploadResults[fileName]);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(1, assetFiles.Count());
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(0).Name);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromFolderIfAssetCollectionIsNull()
        {
            AssetBaseCollection nullAssets = null;

            try
            {
                Task<IAsset> assetTask = nullAssets.CreateFromFolderAsync(string.Empty, null, AssetCreationOptions.None, CancellationToken.None);

                this.asset = assetTask.Result;
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowIfSelectionStrategyIsNullDuringCreateFromFolderAsync()
        {
            IAccountSelectionStrategy selectionStrategy = null;
            CancellationToken token;

            try
            {
                Task<IAsset> assetTask = this.context.Assets.CreateFromFolderAsync(string.Empty, selectionStrategy, AssetCreationOptions.None, null, token);
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual(ane.ParamName, "strategy");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromFolderIfFolderDoesNotContainAnyFiles()
        {
            var emptyFolderName = "EmptyMediaFolder";
            if (Directory.Exists(emptyFolderName))
            {
                Directory.Delete(emptyFolderName, true);
            }

            Directory.CreateDirectory(emptyFolderName);

            try
            {
                Task<IAsset> assetTask = this.context.Assets.CreateFromFolderAsync(emptyFolderName, AssetCreationOptions.None, CancellationToken.None);

                this.asset = assetTask.Result;
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(FileNotFoundException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldCreateAssetFromBlob()
        {
            var fileName = "smallwmv1.wmv";
            var containerName = "createassetfromblobtest-" + Guid.NewGuid();
            var blobClient = TestHelper.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(containerName);

            var sourceBlob = CreateBlobFromFile(this.container, fileName);
            this.asset = this.context.Assets.CreateFromBlob(sourceBlob, blobClient.Credentials, AssetCreationOptions.None);

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(fileName, this.asset.Name);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(1, assetFiles.Count());
            Assert.AreEqual(fileName, assetFiles.ElementAt(0).Name);
            Assert.IsTrue(assetFiles.ElementAt(0).IsPrimary);
            Assert.AreEqual(sourceBlob.Properties.Length, assetFiles.ElementAt(0).ContentFileSize);
            Assert.AreEqual(sourceBlob.Properties.ContentType, assetFiles.ElementAt(0).MimeType);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromBlobIfAssetCollectionIsNull()
        {
            var fileName = "smallwmv1.wmv";
            var containerName = "createassetfromblobtest-" + Guid.NewGuid();
            var blobClient = TestHelper.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(containerName);
            var sourceBlob = CreateBlobFromFile(this.container, fileName);
            AssetBaseCollection nullAssets = null;

            try
            {
                this.asset = nullAssets.CreateFromBlob(sourceBlob, blobClient.Credentials, AssetCreationOptions.None);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromBlobIfSourceBlobIsNull()
        {
            var blobClient = TestHelper.CreateCloudBlobClient();
            CloudBlockBlob nullSourceBlob = null;

            try
            {
                this.asset = this.context.Assets.CreateFromBlob(nullSourceBlob, blobClient.Credentials, AssetCreationOptions.None);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromBlobIfStorageCredentialsIsNull()
        {
            var fileName = "smallwmv1.wmv";
            var containerName = "createassetfromblobtest-" + Guid.NewGuid();
            var blobClient = TestHelper.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(containerName);
            var sourceBlob = CreateBlobFromFile(this.container, fileName);
            StorageCredentials nullStorageCredentials = null;

            try
            {
                this.asset = this.context.Assets.CreateFromBlob(sourceBlob, nullStorageCredentials, AssetCreationOptions.None);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromBlobIfStorageCredentialsIsAnonymous()
        {
            var fileName = "smallwmv1.wmv";
            var containerName = "createassetfromblobtest-" + Guid.NewGuid();
            var blobClient = TestHelper.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(containerName);
            var sourceBlob = CreateBlobFromFile(this.container, fileName);
            var storageCredentials = new StorageCredentials();

            try
            {
                this.asset = this.context.Assets.CreateFromBlob(sourceBlob, storageCredentials, AssetCreationOptions.None);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAssetFromBlobIfStorageCredentialsIsSAS()
        {
            var fileName = "smallwmv1.wmv";
            var containerName = "createassetfromblobtest-" + Guid.NewGuid();
            var blobClient = TestHelper.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference(containerName);
            var sourceBlob = CreateBlobFromFile(this.container, fileName);
            var storageCredentials = new StorageCredentials("?se=2015-05-22T19%3A46%3A16Z&sr=c&si=efa38601-1f8e-4e3a-9a85-2485e2a4f374&sv=2012-02-12&sig=CwUScO98yTHKNRdzwJNRIB7BhRHc9fg4ng1Bb0KE0vo%3D");

            try
            {
                this.asset = this.context.Assets.CreateFromBlob(sourceBlob, storageCredentials, AssetCreationOptions.None);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                throw;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldCreateAssetFromFolder()
        {
            var folderName = "Media";
            this.asset = this.context.Assets.CreateFromFolder(folderName, null, AssetCreationOptions.None);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(folderName, this.asset.Name);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(3, assetFiles.Count());
            Assert.AreEqual("dummy.ism", assetFiles.ElementAt(0).Name);
            Assert.IsTrue(assetFiles.ElementAt(0).IsPrimary);
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(1).Name);
            Assert.IsFalse(assetFiles.ElementAt(1).IsPrimary);
            Assert.AreEqual("smallwmv2.wmv", assetFiles.ElementAt(2).Name);
            Assert.IsFalse(assetFiles.ElementAt(2).IsPrimary);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldCreateAssetFromFolderWithRandomAccountSelectionStrategy()
        {
            RandomAccountSelectionStrategy strategy = RandomAccountSelectionStrategy.FromAccounts(this.context);
            var folderName = "Media";
            this.asset = this.context.Assets.CreateFromFolder(folderName, strategy, AssetCreationOptions.None, null);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(folderName, this.asset.Name);
            IList<string> storageAccountNames = strategy.GetStorageAccounts();
            CollectionAssert.Contains((ICollection)storageAccountNames, this.asset.StorageAccountName);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(3, assetFiles.Count());
            Assert.AreEqual("dummy.ism", assetFiles.ElementAt(0).Name);
            Assert.IsTrue(assetFiles.ElementAt(0).IsPrimary);
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(1).Name);
            Assert.IsFalse(assetFiles.ElementAt(1).IsPrimary);
            Assert.AreEqual("smallwmv2.wmv", assetFiles.ElementAt(2).Name);
            Assert.IsFalse(assetFiles.ElementAt(2).IsPrimary);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        [DeploymentItem(@"Media\smallwmv2.wmv", "Media")]
        [DeploymentItem(@"Media\dummy.ism", "Media")]
        public void ShouldCreateAssetFromFolderWithUploadProgressChangedCallback()
        {
            var uploadResults = new ConcurrentDictionary<string, UploadProgressChangedEventArgs>();
            Action<IAssetFile, UploadProgressChangedEventArgs> uploadProgressChangedCallback =
                (af, e) =>
                {
                    IAssetFile assetFile = af;
                    UploadProgressChangedEventArgs eventArgs = e;

                    Assert.IsNotNull(assetFile);
                    Assert.IsNotNull(eventArgs);

                    uploadResults.AddOrUpdate(assetFile.Name, eventArgs, (k, e2) => eventArgs);
                };

            var folderName = "Media";
            this.asset = this.context.Assets.CreateFromFolder(folderName, AssetCreationOptions.None, uploadProgressChangedCallback);
            var assetId = this.asset.Id;

            Assert.IsNotNull(this.asset);
            Assert.AreEqual(folderName, this.asset.Name);

            Assert.AreEqual(3, uploadResults.Count);

            AssertUploadedFile(folderName, "smallwmv1.wmv", uploadResults["smallwmv1.wmv"]);
            AssertUploadedFile(folderName, "smallwmv2.wmv", uploadResults["smallwmv2.wmv"]);
            AssertUploadedFile(folderName, "dummy.ism", uploadResults["dummy.ism"]);

            var assetFiles = this.asset.AssetFiles.ToList().OrderBy(a => a.Name);

            Assert.AreEqual(3, assetFiles.Count());
            Assert.AreEqual("dummy.ism", assetFiles.ElementAt(0).Name);
            Assert.IsTrue(assetFiles.ElementAt(0).IsPrimary);
            Assert.AreEqual("smallwmv1.wmv", assetFiles.ElementAt(1).Name);
            Assert.IsFalse(assetFiles.ElementAt(1).IsPrimary);
            Assert.AreEqual("smallwmv2.wmv", assetFiles.ElementAt(2).Name);
            Assert.IsFalse(assetFiles.ElementAt(2).IsPrimary);

            this.context = TestHelper.CreateContext();
            Assert.AreEqual(0, this.context.Locators.Where(l => l.AssetId == assetId).Count());
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

            if (this.container != null)
            {
                this.container.Delete();
            }
        }

        private static void AssertUploadedFile(string originalFolderPath, string fileName, UploadProgressChangedEventArgs uploadProgressChangedEventArgs)
        {
            var expected = new FileInfo(Path.Combine(originalFolderPath, fileName));

            Assert.AreEqual(expected.Length, uploadProgressChangedEventArgs.BytesUploaded);
            Assert.AreEqual(expected.Length, uploadProgressChangedEventArgs.TotalBytes);
            Assert.AreEqual(100, uploadProgressChangedEventArgs.Progress);
        }

        private static CloudBlockBlob CreateBlobFromFile(CloudBlobContainer container, string fileName)
        {
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference(fileName);
            blob.UploadFromFile(fileName, FileMode.Open);

            return blob;
        }
    }
}
