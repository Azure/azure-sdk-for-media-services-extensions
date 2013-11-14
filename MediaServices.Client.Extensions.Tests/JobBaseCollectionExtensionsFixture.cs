// <copyright file="JobBaseCollectionExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class JobBaseCollectionExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;
        private IAsset outputAsset;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowCreateWithSingleTaskIfJobCollectiontIsNull()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);
            JobBaseCollection nullJobs = null;

            nullJobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowCreateWithSingleTaskIfInputAssetIsNull()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            IAsset inputAsset = null;

            this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, inputAsset, outputAssetName, outputAssetOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowCreateWithSingleTaskIfStrategyIsNull()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            IAccountSelectionStrategy strategy = null;
            
            this.asset = this.context.Assets.CreateFromFile("smallwmv1.wmv", AssetCreationOptions.None);

            this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, strategy, outputAssetName, outputAssetOptions);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCreateWithSingleTaskIfMediaProcessorNameIsUnknown()
        {
            var mediaProcessorName = "Unknown Media Processor";
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.Create("empty", AssetCreationOptions.None);

            this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldCreateWithSingleTask()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.CreateFromFile("smallwmv1.wmv", AssetCreationOptions.None);

            var job = this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);

            Assert.IsNotNull(job);
            Assert.AreEqual(1, job.Tasks.Count);

            var task = job.Tasks[0];

            Assert.IsNotNull(task);
            Assert.AreEqual(taskConfiguration, task.Configuration);
            Assert.AreEqual(1, task.InputAssets.Count);
            Assert.AreSame(this.asset, task.InputAssets[0]);
            Assert.AreEqual(1, task.OutputAssets.Count);
            Assert.AreEqual(outputAssetName, task.OutputAssets[0].Name);
            Assert.AreEqual(outputAssetOptions, task.OutputAssets[0].Options);
            Assert.AreEqual(this.context.MediaProcessors.GetLatestMediaProcessorByName(mediaProcessorName).Id, task.MediaProcessorId);

            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).Wait();

            Assert.AreEqual(JobState.Finished, job.State);
            Assert.AreEqual(1, job.OutputMediaAssets.Count);

            this.outputAsset = job.OutputMediaAssets[0];

            Assert.IsNotNull(this.outputAsset);
            Assert.AreEqual(outputAssetName, this.outputAsset.Name);
            Assert.AreEqual(outputAssetOptions, this.outputAsset.Options);
        }

        [TestInitialize]
        public void Initialize()
        {
            this.context = TestHelper.CreateContext();
            this.asset = null;
            this.outputAsset = null;
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
