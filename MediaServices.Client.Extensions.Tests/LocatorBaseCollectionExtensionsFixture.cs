// <copyright file="LocatorBaseCollectionExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    public class LocatorBaseCollectionExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAccessPolicyAndLocatorIfLocatorCollectionIsNull()
        {
            LocatorBaseCollection nullLocators = null;
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            try
            {
                nullLocators.Create(LocatorType.OnDemandOrigin, this.asset, AccessPermissions.Read, TimeSpan.FromDays(1));
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShouldThrowCreateAccessPolicyAndLocatorIfAssetIsNull()
        {
            IAsset nullAsset = null;

            try
            {
                this.context.Locators.Create(LocatorType.OnDemandOrigin, nullAsset, AccessPermissions.Read, TimeSpan.FromDays(1));
            }
            catch (AggregateException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
                throw;
            }
        }

        [TestMethod]
        public void ShouldCreateAccessPolicyAndOriginLocator()
        {
            var locatorType = LocatorType.OnDemandOrigin;
            DateTime? locatorStartTime = null;
            var accessPolicyPermissions = AccessPermissions.Read;
            var accessPolicyDuration = TimeSpan.FromDays(1);
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(locatorType, this.asset, accessPolicyPermissions, accessPolicyDuration, locatorStartTime);

            Assert.IsNotNull(locator);
            Assert.AreEqual(locatorType, locator.Type);
            Assert.AreEqual(locatorStartTime, locator.StartTime);

            var accessPolicy = locator.AccessPolicy;

            Assert.IsNotNull(accessPolicy);
            Assert.AreEqual(accessPolicyPermissions, accessPolicy.Permissions);
            Assert.AreEqual(accessPolicyDuration, accessPolicy.Duration);
        }

        [TestMethod]
        public void ShouldCreateAccessPolicyAndSasLocator()
        {
            var locatorType = LocatorType.Sas;
            DateTime? locatorStartTime = DateTime.Today;
            var accessPolicyPermissions = AccessPermissions.Read;
            var accessPolicyDuration = TimeSpan.FromDays(1);
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            var locator = this.context.Locators.Create(locatorType, this.asset, accessPolicyPermissions, accessPolicyDuration, locatorStartTime);

            Assert.IsNotNull(locator);
            Assert.AreEqual(locatorType, locator.Type);
            Assert.AreEqual(locatorStartTime, locator.StartTime);

            var accessPolicy = locator.AccessPolicy;

            Assert.IsNotNull(accessPolicy);
            Assert.AreEqual(accessPolicyPermissions, accessPolicy.Permissions);
            Assert.AreEqual(accessPolicyDuration, accessPolicy.Duration);
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
