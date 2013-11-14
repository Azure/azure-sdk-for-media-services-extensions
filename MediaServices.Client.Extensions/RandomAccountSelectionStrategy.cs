// <copyright file="RandomAccountSelectionStrategy.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Represents simple pseudo random account selection based on the <see cref="System.Random"/> class.
    /// </summary>
    public class RandomAccountSelectionStrategy : IAccountSelectionStrategy
    {
        private readonly Random random = new Random();
        private string[] _storageAccountNames;

        /// <summary>
        /// Creates a RandomAccountSelectionStrategy instance using all of storage account names found in the MediaContextBase.StorageAccounts collection.
        /// </summary>
        /// <param name="mediaContextBase">MediaContextBase to use to query the storage account names from.</param>
        /// <returns>A new RandomAccountSelectionStrategy instance.</returns>
        public static RandomAccountSelectionStrategy FromAccounts(MediaContextBase mediaContextBase)
        {
            string[] storageAccountNames = mediaContextBase.StorageAccounts.ToList().Select(c => c.Name).ToArray();

            return new RandomAccountSelectionStrategy(storageAccountNames);
        }

        /// <summary>
        /// Constructs a new RandomAccountSelectionStrategy that will use a pseudo random selection based on the <see cref="System.Random"/> class of the
        /// storage account names provided.
        /// </summary>
        /// <param name="storageAccountNames">Array of storage account names to choose from.</param>
        public RandomAccountSelectionStrategy(string[] storageAccountNames)
        {
            if (storageAccountNames == null)
            {
                throw new ArgumentNullException("storageAccountNames", "The storage account names array cannot be null."); 
            }

            if (storageAccountNames.Length == 0)
            {
                throw new ArgumentException("The storage account names array cannot be empty.", "storageAccountNames");
            }

            _storageAccountNames = storageAccountNames;
        }

        /// <summary>
        /// Gets the storage account names that the class selects from.
        /// </summary>
        /// <returns>Returns an <see cref="System.IList<T>"/> of storage account names that the class selects from. </returns>
        public IList<string> GetStorageAccounts()
        {
            return _storageAccountNames.ToList().AsReadOnly();
        }

        /// <summary>
        /// Selects a single storage account name from the storage account names provided when the class is constructed using a pseudo random selection based on the <see cref="System.Random"/> class.
        /// </summary>
        /// <returns>A single storage account name</returns>
        public string SelectAccountForAsset()
        {
            return _storageAccountNames[this.random.Next(0, _storageAccountNames.Length)];
        }
    }
}