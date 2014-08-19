// <copyright file="IAssetFileExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class IAssetFileExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;
        private IAsset outputAsset;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetAssetFileMetadataIfAssetFileIsNull()
        {
            try
            {
                IAssetFile nullAssetFile = null;

                var sasLocator = this.context.Locators.Where(l => l.Type == LocatorType.Sas).FirstOrDefault();

                nullAssetFile.GetMetadata(sasLocator);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetAssetFileMetadataIfLocatorIsNull()
        {
            try
            {
                this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

                var assetFile = this.asset.AssetFiles.First();

                assetFile.GetMetadata(null);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetAssetFileMetadataIfLocatorTypeIsNotSas()
        {
            try
            {
                this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

                var originLocator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

                var assetFile = this.asset.AssetFiles.First();

                assetFile.GetMetadata(originLocator);
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetAssetFileMetadataIfLocatorDoesNotBelongToParentAsset()
        {
            try
            {
                var asset2 = this.context.Assets.Create("empty", AssetCreationOptions.None);
                var sasLocator = this.context.Locators.Create(LocatorType.Sas, asset2, AccessPermissions.Read, TimeSpan.FromDays(1));

                this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);
                var assetFile = this.asset.AssetFiles.First();

                try
                {
                    assetFile.GetMetadata(sasLocator);
                }
                finally
                {
                    if (asset2 != null)
                    {
                        asset2.Delete();
                    }
                }
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
                throw exception.InnerException;
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldReturnNullGetAssetFileMetadataIfAssetDoesNotContainMetadataFile()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var assetFile = this.asset.AssetFiles.First();

            var sasLocator = this.context.Locators.Create(
                LocatorType.Sas,
                this.asset,
                AccessPermissions.Read,
                TimeSpan.FromDays(1));

            var assetFileMetadata = assetFile.GetMetadata(sasLocator);

            Assert.IsNull(assetFileMetadata);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldGetAssetFileMetadata()
        {
            var source = "smallwmv1.wmv";
            this.asset = this.context.Assets.CreateFromFile(source, AssetCreationOptions.None);

            var job = this.context.Jobs.CreateWithSingleTask(
                MediaProcessorNames.WindowsAzureMediaEncoder,
                MediaEncoderTaskPresetStrings.H264AdaptiveBitrateMP4Set720p,
                this.asset,
                "Output Asset Name",
                AssetCreationOptions.None);

            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).Wait();

            this.outputAsset = job.OutputMediaAssets[0];

            var assetFile = this.outputAsset
                .AssetFiles
                .ToList()
                .First(af => af.Name.Equals("smallwmv1_H264_3400kbps_AAC_und_ch2_96kbps.mp4", StringComparison.OrdinalIgnoreCase));

            var sasLocator = this.context.Locators.Create(
                LocatorType.Sas,
                this.outputAsset,
                AccessPermissions.Read,
                TimeSpan.FromDays(1));

            var assetFileMetadata = assetFile.GetMetadata(sasLocator);

            Assert.IsNotNull(assetFileMetadata);
            Assert.AreEqual(assetFile.Name, assetFileMetadata.Name);
            Assert.AreEqual(assetFile.ContentFileSize, assetFileMetadata.Size);
            Assert.AreEqual(TimeSpan.FromSeconds(5.119), assetFileMetadata.Duration);

            Assert.IsNotNull(assetFileMetadata.Sources);
            Assert.AreEqual(1, assetFileMetadata.Sources.Count());
            Assert.IsNotNull(assetFileMetadata.Sources.ElementAt(0));
            Assert.AreEqual(source, assetFileMetadata.Sources.ElementAt(0).Name);

            Assert.IsNotNull(assetFileMetadata.VideoTracks);
            Assert.AreEqual(1, assetFileMetadata.VideoTracks.Count());
            Assert.IsNotNull(assetFileMetadata.VideoTracks.ElementAt(0));
            Assert.AreEqual(0, assetFileMetadata.VideoTracks.ElementAt(0).Id);
            Assert.AreEqual("AVC1", assetFileMetadata.VideoTracks.ElementAt(0).FourCC);
            Assert.AreEqual(960, assetFileMetadata.VideoTracks.ElementAt(0).Width);
            Assert.AreEqual(720, assetFileMetadata.VideoTracks.ElementAt(0).Height);
            Assert.AreEqual(4, assetFileMetadata.VideoTracks.ElementAt(0).DisplayAspectRatioNumerator);
            Assert.AreEqual(3, assetFileMetadata.VideoTracks.ElementAt(0).DisplayAspectRatioDenominator);
            ////Assert.AreEqual(29.974, assetFileMetadata.VideoTracks.ElementAt(0).Framerate);
            Assert.AreEqual(29.97, assetFileMetadata.VideoTracks.ElementAt(0).TargetFramerate);
            ////Assert.AreEqual(3804, assetFileMetadata.VideoTracks.ElementAt(0).Bitrate);
            Assert.AreEqual(3400, assetFileMetadata.VideoTracks.ElementAt(0).TargetBitrate);

            Assert.IsNotNull(assetFileMetadata.AudioTracks);
            Assert.AreEqual(1, assetFileMetadata.AudioTracks.Count());
            Assert.IsNotNull(assetFileMetadata.AudioTracks.ElementAt(0));
            Assert.IsNull(assetFileMetadata.AudioTracks.ElementAt(0).EncoderVersion);
            Assert.AreEqual(0, assetFileMetadata.AudioTracks.ElementAt(0).Id);
            Assert.AreEqual("AacLc", assetFileMetadata.AudioTracks.ElementAt(0).Codec);
            Assert.AreEqual(2, assetFileMetadata.AudioTracks.ElementAt(0).Channels);
            Assert.AreEqual(44100, assetFileMetadata.AudioTracks.ElementAt(0).SamplingRate);
            Assert.AreEqual(93, assetFileMetadata.AudioTracks.ElementAt(0).Bitrate);
            Assert.AreEqual(16, assetFileMetadata.AudioTracks.ElementAt(0).BitsPerSample);
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
            IAssetFile nullAssetFile = null;

            var sasLocator = this.context.Locators.Where(l => l.Type == LocatorType.Sas).FirstOrDefault();

            nullAssetFile.GetSasUri(sasLocator);
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

            var originLocator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var assetFile = this.asset.AssetFiles.First();

            assetFile.GetSasUri(originLocator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldThrowGetSasUriWithSpecificLocatorIfLocatorDoesNotBelongToParentAsset()
        {
            var asset2 = this.context.Assets.Create("empty", AssetCreationOptions.None);
            var sasLocator = this.context.Locators.Create(LocatorType.Sas, asset2, AccessPermissions.Read, TimeSpan.FromDays(1));

            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);
            var assetFile = this.asset.AssetFiles.First();

            try
            {
                assetFile.GetSasUri(sasLocator);
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
        public void ShouldThrowGetMediaContextIfAssetFileIsNull()
        {
            IAssetFile nullAssetFile = null;

            nullAssetFile.GetMediaContext();
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetMediaContext()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var assetFile = this.asset.AssetFiles.First();

            var mediaContext = assetFile.GetMediaContext();

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

            if (this.outputAsset != null)
            {
                this.outputAsset.Delete();
            }
        }
    }
}
