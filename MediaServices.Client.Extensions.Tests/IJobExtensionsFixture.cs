// <copyright file="IJobExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using MediaServices.Client.Extensions.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class IJobExtensionsFixture
    {
        private CloudMediaContext context;
        private IAsset asset;
        private IAsset outputAsset;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetOverallProgressIfJobIsNull()
        {
            IJob nullJob = null;

            nullJob.GetOverallProgress();
        }

        [TestMethod]
        public void ShouldGetOverallProgressWhenJobContainsSingleTask()
        {
            var task = new TaskMock();
            var taskCollection = new TaskCollectionMock();

            taskCollection.Add(task);

            var job = new JobMock();
            job.Tasks = taskCollection;

            task.Progress = 0;
            Assert.AreEqual(0, job.GetOverallProgress());

            task.Progress = 25;
            Assert.AreEqual(25, job.GetOverallProgress());

            task.Progress = 75;
            Assert.AreEqual(75, job.GetOverallProgress());

            task.Progress = 100;
            Assert.AreEqual(100, job.GetOverallProgress());
        }

        [TestMethod]
        public void ShouldGetOverallProgressWhenJobContainsMultipleTask()
        {
            var task1 = new TaskMock();
            var task2 = new TaskMock();
            var taskCollection = new TaskCollectionMock();

            taskCollection.Add(task1);
            taskCollection.Add(task2);

            var job = new JobMock();
            job.Tasks = taskCollection;

            task1.Progress = 0;
            task2.Progress = 0;
            Assert.AreEqual(0, job.GetOverallProgress());

            task1.Progress = 25;
            task2.Progress = 0;
            Assert.AreEqual(12.5, job.GetOverallProgress());

            task1.Progress = 25;
            task2.Progress = 50;
            Assert.AreEqual(37.5, job.GetOverallProgress());

            task1.Progress = 50;
            task2.Progress = 50;
            Assert.AreEqual(50, job.GetOverallProgress());

            task1.Progress = 75;
            task2.Progress = 25;
            Assert.AreEqual(50, job.GetOverallProgress());

            task1.Progress = 75;
            task2.Progress = 50;
            Assert.AreEqual(62.5, job.GetOverallProgress());

            task1.Progress = 50;
            task2.Progress = 100;
            Assert.AreEqual(75, job.GetOverallProgress());

            task1.Progress = 100;
            task2.Progress = 75;
            Assert.AreEqual(87.5, job.GetOverallProgress());

            task1.Progress = 100;
            task2.Progress = 100;
            Assert.AreEqual(100, job.GetOverallProgress());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowStartExecutionProgressTaskIfJobIsNull()
        {
            IJob nullJob = null;

            nullJob.StartExecutionProgressTask(j => { }, CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowStartExecutionProgressTaskIfJobDoesNotHaveValidId()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.Create("TestAsset", AssetCreationOptions.None);

            var job = this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);

            job.StartExecutionProgressTask(j => { }, CancellationToken.None);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldStartExecutionProgressTaskAndInvokeCallbackWhenStateOrOverallProgressChange()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.CreateFromFile("smallwmv1.wmv", AssetCreationOptions.None);

            var job = this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);
            job.Submit();

            var previousState = job.State;
            var previousOverallProgress = job.GetOverallProgress();
            var callbackInvocations = 0;

            var executionProgressTask = job.StartExecutionProgressTask(
                j =>
                {
                    callbackInvocations++;

                    Assert.IsTrue((j.State != previousState) || (j.GetOverallProgress() != previousOverallProgress));

                    previousState = j.State;
                    previousOverallProgress = j.GetOverallProgress();
                },
                CancellationToken.None);
            job = executionProgressTask.Result;

            Assert.IsTrue(callbackInvocations > 0);
            Assert.AreEqual(JobState.Finished, previousState);
            Assert.AreEqual(100, previousOverallProgress);

            Assert.AreEqual(JobState.Finished, job.State);
            Assert.AreEqual(100, job.GetOverallProgress());
            Assert.AreEqual(1, job.OutputMediaAssets.Count);

            this.outputAsset = job.OutputMediaAssets[0];

            Assert.IsNotNull(this.outputAsset);
            Assert.AreEqual(outputAssetName, this.outputAsset.Name);
            Assert.AreEqual(outputAssetOptions, this.outputAsset.Options);
        }

        [TestMethod]
        [DeploymentItem(@"Media\smallwmv1.wmv")]
        public void ShouldStartExecutionProgressTaskWhenExecutionProgressChangedCallbackIsNull()
        {
            var mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;
            var taskConfiguration = MediaEncoderTaskPresetStrings.H264SmoothStreaming720p;
            var outputAssetName = "Output Asset Name";
            var outputAssetOptions = AssetCreationOptions.None;
            this.asset = this.context.Assets.CreateFromFile("smallwmv1.wmv", AssetCreationOptions.None);

            var job = this.context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, this.asset, outputAssetName, outputAssetOptions);
            job.Submit();

            var executionProgressTask = job.StartExecutionProgressTask(null, CancellationToken.None);
            job = executionProgressTask.Result;

            Assert.AreEqual(JobState.Finished, job.State);
            Assert.AreEqual(100, job.GetOverallProgress());
            Assert.AreEqual(1, job.OutputMediaAssets.Count);

            this.outputAsset = job.OutputMediaAssets[0];

            Assert.IsNotNull(this.outputAsset);
            Assert.AreEqual(outputAssetName, this.outputAsset.Name);
            Assert.AreEqual(outputAssetOptions, this.outputAsset.Options);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowGetMediaContextIfJobIsNull()
        {
            IJob nullJob = null;

            nullJob.GetMediaContext();
        }

        [TestMethod]
        public void ShouldGetMediaContext()
        {
            var job = this.context.Jobs.Create("test");

            var mediaContext = job.GetMediaContext();

            Assert.IsNotNull(mediaContext);
            Assert.AreSame(this.context, mediaContext);
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
