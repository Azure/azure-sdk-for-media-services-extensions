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

namespace MediaServices.Client.Extensions.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class OutputAssetExtensionsFixture
    {
        private readonly string smallWmv = @"Media\smallwmv1.wmv";
        private CloudMediaContext context;
        private IAsset inputAsset;
        private IAsset outputAsset;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            this.context = TestHelper.CreateContext();
            this.inputAsset = null;
            this.outputAsset = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (this.inputAsset != null)
            {
                this.inputAsset.Delete();
            }

            if (this.outputAsset != null)
            {
                this.outputAsset.Delete();
            }
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv", "Media")]
        public void ShouldCreateOutputAssetWithAccountSelectionStrategy()
        {
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, true);

            string inputAssetFilePath = Path.Combine(TestContext.TestDeploymentDir, smallWmv);
            string inputAssetFileName = Path.GetFileName(inputAssetFilePath);

            this.inputAsset = context.Assets.Create("InputAsset", strategy, AssetCreationOptions.StorageEncrypted);
            IAssetFile file = this.inputAsset.AssetFiles.Create(inputAssetFileName);
            file.Upload(inputAssetFilePath);

            IJob job = context.Jobs.Create("Job to test using an account selection strategy for an output asset");
            ITask task = job.Tasks.AddNew(
                "Task to test using an account selection strategy for an output asset",
                context.MediaProcessors.GetLatestMediaProcessorByName(MediaProcessorNames.MediaEncoderStandard),
                MediaEncoderStandardTaskPresetStrings.H264SingleBitrate4x3SD,
                TaskOptions.None);
            task.InputAssets.Add(this.inputAsset);
            task.OutputAssets.AddNew("OutputAsset", strategy, AssetCreationOptions.None);

            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).Wait();

            Assert.IsNotNull(job);
            Assert.AreEqual(1, job.Tasks.Count, "Unexpected number of tasks in job");
            Assert.AreEqual(1, job.OutputMediaAssets.Count, "Unexpected number of output assets in the job");

            this.outputAsset = job.OutputMediaAssets[0];

            Assert.IsNotNull(outputAsset.StorageAccountName, "Storage account name in output assset is null");
        }
    }
}
