// <copyright file="ILocatorExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class ILocatorExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        public void ShouldGetSmoothStreamingUri()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var smoothStreamingUrl = locator.GetSmoothStreamingUri();

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

            var hlsUri = locator.GetHlsUri();

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

            var mpegDashUri = locator.GetMpegDashUri();

            Assert.IsNotNull(mpegDashUri);
            Assert.IsTrue(
                mpegDashUri
                    .AbsoluteUri
                    .EndsWith(locator.ContentAccessComponent + "/dummy.ism/manifest(format=mpd-time-csf)", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetStreamingUriIfLocatorIsNull()
        {
            ILocator nullLocator = null;

            nullLocator.GetSmoothStreamingUri();
        }

        [TestMethod]
        [DeploymentItem(@"Media\dummy.ism")]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowGetStreamingUriIfLocatorTypeIsNotOrigin()
        {
            this.asset = this.context.Assets.CreateFromFile("dummy.ism", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(LocatorType.Sas, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));

            var smoothStreamingUrl = locator.GetSmoothStreamingUri();
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
    }
}
