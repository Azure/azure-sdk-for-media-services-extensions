//-----------------------------------------------------------------------
// <copyright file="OutputAssetCollectionExtensions.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    public static class OutputAssetCollectionExtensions
    {
        /// <summary>
        /// Returns a new empty <see cref="IAsset"/> within one selected storage account from <paramref name="storageAccountNames"/> based on the default <see cref="IAccountSelectionStrategy"/>.
        /// </summary>
        /// <param name="assets">The <see cref="AssetBaseCollection"/> instance.</param>
        /// <param name="assetName">The asset name.</param>
        /// <param name="strategy">The <see cref="IAccountSelectionStrategy"/> used to select a storage account for the new output asset.</param>
        /// <param name="options">The <see cref="AssetCreationOptions"/>.</param>
        /// <returns>A new empty <see cref="IAsset"/> within one selected storage account from the provided <see cref="IAccountSelectionStrategy"/>.</returns>
        public static IAsset AddNew(this OutputAssetCollection collection, string assetName, IAccountSelectionStrategy strategy, AssetCreationOptions options)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");            
            }

            if (strategy == null)
            {
                throw new ArgumentNullException("strategy");
            }

            string storageAccount = strategy.SelectAccountForAsset();

            return collection.AddNew(assetName, storageAccount, options);
        }
    }
}
