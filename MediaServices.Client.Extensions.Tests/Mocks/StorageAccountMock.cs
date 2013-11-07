// <copyright file="StorageAccountMock.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace MediaServices.Client.Extensions.Tests.Mocks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client;

    public class StorageAccountMock : IStorageAccount
    {
        public long? BytesUsed { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
    }
}
