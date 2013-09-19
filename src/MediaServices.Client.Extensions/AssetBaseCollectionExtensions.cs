//-----------------------------------------------------------------------
// <copyright file="AssetBaseCollectionExtensions.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains extension methods  to the <see cref="AssetBaseCollection"/>.
    /// </summary>
    public static class AssetBaseCollectionExtensions
    {

        private static readonly Lazy<IAccountSelectionStrategy> DefaultAccountSelectionStrategy = new Lazy<IAccountSelectionStrategy>(() => new RandomAccountSelectionStrategy());


        /// <summary>
        /// Creates asset within one selected storage account based on default  <see cref="IAccountSelectionStrategy"/>
        /// </summary>
        /// <param name="collection">The asset collection.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="storageAccountNames">The storage account names to select from.</param>
        /// <param name="options">The AssetCreationOptions.</param>
        /// <returns></returns>
        public static IAsset Create(this AssetBaseCollection collection,String assetName, string[] storageAccountNames, AssetCreationOptions options)
        {
            var strategy = collection.GetAccountSelectionStrategy();
            return collection.Create(assetName, strategy.SelectAccountForInputAssets(storageAccountNames), options);
        }

        /// <summary>
        /// Creates asset asyncronously within one selected storage account based on default  <see cref="IAccountSelectionStrategy"/>
        /// </summary>
        /// <param name="collection">The asset collection.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="storageAccountNames">The storage account names to select from.</param>
        /// <param name="options">The AssetCreationOptions.</param>
        /// <param name="token">CancellationToken</param>
        /// <returns></returns>
        public static Task<IAsset> CreateAsync(this AssetBaseCollection collection, String assetName, string[] storageAccountNames, AssetCreationOptions options,CancellationToken token)
        {
            var strategy = collection.GetAccountSelectionStrategy();
            return collection.CreateAsync(assetName, strategy.SelectAccountForInputAssets(storageAccountNames), options,token);
        }

        /// <summary>
        /// Gets the default IAccountSelectionStrategy.
        /// </summary>
        /// <param name="collection">The asset collection.</param>
        /// <returns></returns>
        public static IAccountSelectionStrategy GetAccountSelectionStrategy(this AssetBaseCollection collection)
        {
            return DefaultAccountSelectionStrategy.Value;
        }
       
    }
}
