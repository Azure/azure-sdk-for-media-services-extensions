//-----------------------------------------------------------------------
// <copyright file="AssetExtensionsFixture.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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

using System;

namespace MediaServices.Client.Extensions.Tests
{
    using System.Configuration;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class AssetExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;
    

        [TestInitialize]
        public void Initialize()
        {
            this.context = this.CreateContext();
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

        private CloudMediaContext CreateContext()
        {
            return new CloudMediaContext(
                ConfigurationManager.AppSettings["MediaServicesAccountName"],
                ConfigurationManager.AppSettings["MediaServicesAccountKey"]);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        public void ShouldCreateAssetWithDefaultAccountSelectionStrategy()
        {
            var folderName = "Media";
            //Defining list of accounts to select from
            string[] accounts = context.StorageAccounts.ToList().Select(c=>c.Name).ToArray();
            this.asset = this.context.Assets.Create(Guid.NewGuid().ToString(), accounts, AssetCreationOptions.None);
        }

    }
}
