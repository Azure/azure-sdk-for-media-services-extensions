//-----------------------------------------------------------------------
// <copyright file="RandomAccountSelectionStrategy.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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
    /// <summary>
    /// Defines simple pseudo random account selection based on Random class implementation
    /// </summary>
    public class RandomAccountSelectionStrategy : IAccountSelectionStrategy
    {
        private readonly Random _inputRnd = new Random();

        public string SelectAccountForInputAssets(string[] accountNames)
        {
            return accountNames[_inputRnd.Next(0, accountNames.Length - 1)];
        }
    }
}