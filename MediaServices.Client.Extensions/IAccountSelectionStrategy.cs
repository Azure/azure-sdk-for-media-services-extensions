// <copyright file="IAccountSelectionStrategy.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    /// <summary>
    /// Defines account selection logic within asset creation scenarios.
    /// </summary>
    public interface IAccountSelectionStrategy
    {
        /// <summary>
        /// Selects a single storage account name from the internal list of storage accounts.
        /// </summary>
        /// <returns>The storage account name to use for creating a new asset.</returns>
        string SelectAccountForAsset();
    }
}