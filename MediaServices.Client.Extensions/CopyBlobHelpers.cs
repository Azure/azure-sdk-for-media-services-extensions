// <copyright file="CopyBlobHelpers.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Contains helper methods for copying blobs.
    /// </summary>
    public static class CopyBlobHelpers
    {
        /// <summary>
        /// Represents the maximum number of concurrent Copy Blob operations when copying content from a source container to a destination container.
        /// </summary>
        public const int MaxNumberOfConcurrentCopyFromBlobOperations = 750;

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task"/> instance for the copy blobs operation from <paramref name="sourceContainer"/> to <paramref name="destinationContainer"/>.
        /// </summary>
        /// <param name="sourceContainer">The <see cref="Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer"/> instance that contains the blobs to be copied into <paramref name="destinationContainer"/>.</param>
        /// <param name="destinationContainer">The <see cref="Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer"/> instance where the blobs from <paramref name="sourceContainer"/> will be copied.</param>
        /// <param name="options">The <see cref="Microsoft.WindowsAzure.Storage.Blob.BlobRequestOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance for the copy blobs operation from <paramref name="sourceContainer"/> to <paramref name="destinationContainer"/>.</returns>
        public static async Task CopyBlobsAsync(CloudBlobContainer sourceContainer, CloudBlobContainer destinationContainer, BlobRequestOptions options, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;

            do
            {
                BlobResultSegment resultSegment = await sourceContainer.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, MaxNumberOfConcurrentCopyFromBlobOperations, continuationToken, options, null, cancellationToken).ConfigureAwait(false);

                IEnumerable<Task> copyTasks = resultSegment
                    .Results
                    .Cast<CloudBlockBlob>()
                    .Select(
                        sourceBlob =>
                        {
                            CloudBlockBlob destinationBlob = destinationContainer.GetBlockBlobReference(sourceBlob.Name);

                            return CopyBlobAsync(sourceBlob, destinationBlob, options, cancellationToken);
                        });

                await Task.WhenAll(copyTasks).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task"/> instance for the copy blob operation from <paramref name="sourceBlob"/> to <paramref name="destinationBlob"/>.
        /// </summary>
        /// <param name="sourceBlob">The <see cref="Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob"/> instance to be copied to <paramref name="destinationBlob"/>.</param>
        /// <param name="destinationBlob">The <see cref="Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob"/> instance where <paramref name="sourceBlob"/> will be copied.</param>
        /// <param name="options">The <see cref="Microsoft.WindowsAzure.Storage.Blob.BlobRequestOptions"/>.</param>
        /// <param name="cancellationToken">The <see cref="System.Threading.CancellationToken"/> instance used for cancellation.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> instance for the copy blob operation from <paramref name="sourceBlob"/> to <paramref name="destinationBlob"/>.</returns>
        public static async Task CopyBlobAsync(CloudBlockBlob sourceBlob, CloudBlockBlob destinationBlob, BlobRequestOptions options, CancellationToken cancellationToken)
        {
            await destinationBlob.StartCopyFromBlobAsync(sourceBlob, null, null, options, null, cancellationToken).ConfigureAwait(false);

            CopyState copyState = destinationBlob.CopyState;
            while (copyState == null || copyState.Status == CopyStatus.Pending)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await destinationBlob.FetchAttributesAsync(null, options, null, cancellationToken).ConfigureAwait(false);

                copyState = destinationBlob.CopyState;
                if (copyState != null && copyState.Status != CopyStatus.Pending && copyState.Status != CopyStatus.Success)
                {
                    throw new StorageException(copyState.StatusDescription);
                }
            }
        }
    }
}
