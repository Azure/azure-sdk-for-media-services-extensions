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

    /// <summary>
    /// Represents simple pseudo random account selection based on the <see cref="System.Random"/> class.
    /// </summary>
    public class RandomAccountSelectionStrategy : IAccountSelectionStrategy
    {
        private readonly Random random = new Random();

        /// <summary>
        /// Selects a single storage account name from the <paramref name="storageAccountNames"/> array using a pseudo random selection based on the <see cref="System.Random"/> class.
        /// </summary>
        /// <param name="storageAccountNames">The storage account names to select from.</param>
        /// <returns>A single storage account name from the <paramref name="storageAccountNames"/> array using a pseudo random selection based on the <see cref="System.Random"/> class.</returns>
        public string SelectAccountForInputAssets(string[] storageAccountNames)
        {
            if (storageAccountNames == null)
            {
                throw new ArgumentNullException("storageAccountNames", "The storage account names array cannot be null."); 
            }

            if (storageAccountNames.Length == 0)
            {
                throw new ArgumentException("The storage account names array cannot be empty.", "storageAccountNames");
            }

            return storageAccountNames[this.random.Next(0, storageAccountNames.Length)];
        }
    }
}