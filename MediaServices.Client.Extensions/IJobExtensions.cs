// <copyright file="IJobExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="IJob"/> interface.
    /// </summary>
    public static class IJobExtensions
    {
        private const int DefaultJobRefreshIntervalInMilliseconds = 2500;

        /// <summary>
        /// Returns the overall progress of the <paramref name="job"/> by aggregating the progress of all its tasks.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> instance.</param>
        /// <returns>The overall progress of the <paramref name="job"/> by aggregating the progress of all its tasks.</returns>
        public static double GetOverallProgress(this IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job", "The job cannot be null.");
            }

            return job.Tasks.Sum(t => t.Progress) / job.Tasks.Count;
        }

        /// <summary>
        /// Returns a started <see cref="System.Threading.Tasks.Task"/> to monitor the <paramref name="job"/> progress by invoking the <paramref name="executionProgressChangedCallback"/> when its state or overall progress change.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> instance.</param>
        /// <param name="jobRefreshIntervalInMilliseconds">The time interval in milliseconds to refresh the <paramref name="job"/>.</param>
        /// <param name="executionProgressChangedCallback">A callback that is invoked when the <paramref name="job"/> state or overall progress change.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A started <see cref="System.Threading.Tasks.Task&lt;IJob&gt;"/> instance to monitor the <paramref name="job"/> progress by invoking the <paramref name="executionProgressChangedCallback"/> when its state or overall progress change.</returns>
        public static Task<IJob> StartExecutionProgressTask(this IJob job, int jobRefreshIntervalInMilliseconds, Action<IJob> executionProgressChangedCallback, CancellationToken cancellationToken)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job", "The job cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(job.Id))
            {
                throw new ArgumentException("The job does not have a valid Id. Please, make sure to submit it first.", "job");
            }

            return Task.Factory.StartNew(
                    originalJob =>
                    {
                        IJob refreshedJob = (IJob)originalJob;
                        while ((refreshedJob.State != JobState.Canceled) && (refreshedJob.State != JobState.Error) && (refreshedJob.State != JobState.Finished))
                        {
                            Thread.Sleep(jobRefreshIntervalInMilliseconds);

                            cancellationToken.ThrowIfCancellationRequested();

                            JobState previousState = refreshedJob.State;
                            double previousOverallProgress = refreshedJob.GetOverallProgress();

                            IMediaDataServiceContext dataContext = refreshedJob.GetMediaContext().MediaServicesClassFactory.CreateDataServiceContext();
                            refreshedJob.Refresh();

                            if ((executionProgressChangedCallback != null) && ((refreshedJob.State != previousState) || (refreshedJob.GetOverallProgress() != previousOverallProgress)))
                            {
                                executionProgressChangedCallback(refreshedJob);
                            }
                        }

                        return refreshedJob;
                    },
                    job,
                    cancellationToken);
        }

        /// <summary>
        /// Returns a started <see cref="System.Threading.Tasks.Task"/> to monitor the <paramref name="job"/> progress by invoking the <paramref name="executionProgressChangedCallback"/> when its state or overall progress change.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> instance.</param>
        /// <param name="executionProgressChangedCallback">A callback that is invoked when the <paramref name="job"/> state or overall progress change.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A started <see cref="System.Threading.Tasks.Task&lt;IJob&gt;"/> instance to monitor the <paramref name="job"/> progress by invoking the <paramref name="executionProgressChangedCallback"/> when its state or overall progress change.</returns>
        public static Task<IJob> StartExecutionProgressTask(this IJob job, Action<IJob> executionProgressChangedCallback, CancellationToken cancellationToken)
        {
            return job.StartExecutionProgressTask(DefaultJobRefreshIntervalInMilliseconds, executionProgressChangedCallback, cancellationToken);
        }

        /// <summary>
        /// Returns the parent <see cref="MediaContextBase"/> instance.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> instance.</param>
        /// <returns>The parent <see cref="MediaContextBase"/> instance.</returns>
        public static MediaContextBase GetMediaContext(this IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job", "The job cannot be null.");
            }

            IMediaContextContainer mediaContextContainer = job as IMediaContextContainer;
            MediaContextBase context = null;

            if (mediaContextContainer != null)
            {
                context = mediaContextContainer.GetMediaContext();
            }

            return context;
        }

    }
}
