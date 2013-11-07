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
        private static IAccountSelectionStrategy CurrentSelectionStrategy = new RandomAccountSelectionStrategy();

        public static IAsset AddNew(this OutputAssetCollection collection, string assetName, string[] storageAccountNames, AssetCreationOptions options)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");            
            }

            if (storageAccountNames == null)
            {
                throw new ArgumentNullException("storageAccountNames");
            }

            IAccountSelectionStrategy strategy = collection.GetAccountSelectionStrategy();

            string storageAccount = strategy.SelectAccountForAssets(storageAccountNames);

            return collection.AddNew(assetName, storageAccount, options);
        }

        /// <summary>
        /// Gets the current IAccountSelectionStrategy.
        /// </summary>
        /// <param name="collection">The output asset collection.</param>
        /// <returns></returns>
        public static IAccountSelectionStrategy GetAccountSelectionStrategy(this OutputAssetCollection collection)
        {
            return CurrentSelectionStrategy;
        }

        /// <summary>
        /// Sets the Account Selection Strategy used to select storage accounts when using the CreateAsync overloads that take a list of storage account names.
        /// </summary>
        /// <param name="collection">The output asset collection.</param>
        /// <param name="accountSelectionStrategy">The IAccountSelectionStrategy to set.</param>
        public static void SetAccountSelectionStrategy(this OutputAssetCollection collection, IAccountSelectionStrategy accountSelectionStrategy)
        {
            if (accountSelectionStrategy == null)
            {
                throw new ArgumentNullException("accountSelectionStrategy");
            }

            CurrentSelectionStrategy = accountSelectionStrategy;
        }
    }
}
