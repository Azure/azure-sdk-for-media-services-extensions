//-----------------------------------------------------------------------
// <copyright file="OutputAssetExtensionsFixture.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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
using System.IO;

namespace MediaServices.Client.Extensions.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using System.Configuration;
    using System.Linq;

    [TestClass]
    public class OutputAssetExtensionsFixture
    {
        public readonly string Encoder = "Windows Azure Media Encoder";
        public readonly string Preset = "H264 Broadband SD 4x3";
        private readonly string smallWmv = @"Media\smallwmv1.wmv";
        private CloudMediaContext context;
        private IAsset asset;

        public TestContext TestContext { get; set; }

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

        private IMediaProcessor GetMediaProcessor(string mpName)
        {
            IMediaProcessor mp = context.MediaProcessors.Where(c => c.Name == mpName).ToList().OrderByDescending(c => new Version(c.Version)).FirstOrDefault();

            if (mp == null)
            {
                throw new ArgumentException(string.Format("Media Processor {0} is not found", mpName), "mpName");
            }

            return mp;
        }


        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        public void ShouldCreateOutputAssetWithAccountSelectionStrategy()
        {
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, true);

            string inputAssetFilePath = Path.Combine(TestContext.TestDeploymentDir, smallWmv);
            string inputAssetFileName = Path.GetFileName(inputAssetFilePath);

            IAsset inputAsset = context.Assets.Create("", strategy, AssetCreationOptions.StorageEncrypted);
            IAssetFile file = inputAsset.AssetFiles.Create(inputAssetFileName);
            file.Upload(inputAssetFilePath);

            IJob job = context.Jobs.Create("Job to test using an account selection strategy for an output asset");
            ITask task = job.Tasks.AddNew("Task to test using an account selection strategy for an output asset", GetMediaProcessor(Encoder), Preset, TaskOptions.None);
            task.InputAssets.Add(inputAsset);
            task.OutputAssets.AddNew("OutputAsset", strategy, AssetCreationOptions.None);

            job.Submit();       

            // Note that we don't want for the job to finish.  We just need the submit to succeed.
            IJob refreshedJob = context.Jobs.Where(c => c.Id == job.Id).FirstOrDefault();
            Assert.IsNotNull(refreshedJob);
            Assert.AreEqual(1, refreshedJob.Tasks.Count, "Unexpected number of tasks in job");
            Assert.AreEqual(1, refreshedJob.Tasks[0].OutputAssets.Count, "Unexpected number of output assets in the job");
            Assert.IsNotNull(refreshedJob.Tasks[0].OutputAssets[0].StorageAccountName, "Storage account name in output assset is null");
        }
    }
}
