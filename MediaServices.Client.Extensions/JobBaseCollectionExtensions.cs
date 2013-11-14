// <copyright file="JobBaseCollectionExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="JobBaseCollection"/> class.
    /// </summary>
    public static class JobBaseCollectionExtensions
    {
        /// <summary>
        /// Returns a <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.
        /// </summary>
        /// <param name="jobs">The <see cref="JobBaseCollection"/> instance.</param>
        /// <param name="mediaProcessorName">The name of the media processor.</param>
        /// <param name="taskConfiguration">The task configuration.</param>
        /// <param name="inputAsset">The input <see cref="IAsset"/> instance.</param>
        /// <param name="outputAssetName">The name of the output asset.</param>
        /// <param name="outputAssetStorageAccountName">The name of the Storage Account where to store the output asset.</param>
        /// <param name="outputAssetOptions">The <see cref="AssetCreationOptions"/> of the output asset.</param>
        /// <returns>A <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.</returns>
        public static IJob CreateWithSingleTask(this JobBaseCollection jobs, string mediaProcessorName, string taskConfiguration, IAsset inputAsset, string outputAssetName, string outputAssetStorageAccountName, AssetCreationOptions outputAssetOptions)
        {
            if (jobs == null)
            {
                throw new ArgumentNullException("jobs", "The jobs collection cannot be null.");
            }

            if (inputAsset == null)
            {
                throw new ArgumentNullException("inputAsset", "The input asset cannot be null.");
            }

            MediaContextBase context = jobs.MediaContext;

            IMediaProcessor processor = context.MediaProcessors.GetLatestMediaProcessorByName(mediaProcessorName);
            if (processor == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Unknown media processor: '{0}'", mediaProcessorName), "mediaProcessorName");
            }

            IJob job = jobs.Create(string.Format(CultureInfo.InvariantCulture, "Job for {0}", inputAsset.Name));

            ITask task = job.Tasks.AddNew(
                string.Format(CultureInfo.InvariantCulture, "Task for {0}", inputAsset.Name),
                processor,
                taskConfiguration,
                TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(inputAsset);

            if (string.IsNullOrWhiteSpace(outputAssetStorageAccountName))
            {
                outputAssetStorageAccountName = context.DefaultStorageAccount.Name;
            }

            task.OutputAssets.AddNew(outputAssetName, outputAssetStorageAccountName, outputAssetOptions);

            return job;
        }

        /// <summary>
        /// Returns a <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.
        /// </summary>
        /// <param name="jobs">The <see cref="JobBaseCollection"/> instance.</param>
        /// <param name="mediaProcessorName">The name of the media processor.</param>
        /// <param name="taskConfiguration">The task configuration.</param>
        /// <param name="inputAsset">The input <see cref="IAsset"/> instance.</param>
        /// <param name="outputAssetName">The name of the output asset.</param>
        /// <param name="outputAssetOptions">The <see cref="AssetCreationOptions"/> of the output asset.</param>
        /// <returns>A <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.</returns>
        public static IJob CreateWithSingleTask(this JobBaseCollection jobs, string mediaProcessorName, string taskConfiguration, IAsset inputAsset, string outputAssetName, AssetCreationOptions outputAssetOptions)
        {
            return jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, inputAsset, outputAssetName, null, outputAssetOptions);
        }

        /// <summary>
        /// Returns a <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.
        /// </summary>
        /// <param name="jobs">The <see cref="JobBaseCollection"/> instance.</param>
        /// <param name="mediaProcessorName">The name of the media processor.</param>
        /// <param name="taskConfiguration">The task configuration.</param>
        /// <param name="inputAsset">The input <see cref="IAsset"/> instance.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> instance used to pick the output asset storage account.</param>
        /// <param name="outputAssetName">The name of the output asset.</param>
        /// <param name="outputAssetOptions">The <see cref="AssetCreationOptions"/> of the output asset.</param>
        /// <returns>A <see cref="IJob"/> instance with a single <see cref="ITask"/> ready to be submitted.</returns>
        public static IJob CreateWithSingleTask(this JobBaseCollection jobs, string mediaProcessorName, string taskConfiguration, IAsset inputAsset, IAccountSelectionStrategy strategy, string outputAssetName, AssetCreationOptions outputAssetOptions)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string outputAssetStorageAccount = strategy.SelectAccountForAsset();

            return jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, inputAsset, outputAssetName, outputAssetStorageAccount, outputAssetOptions);
        }
    }
}
