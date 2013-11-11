// <copyright file="MediaContextBaseMock.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

    public class MediaContextBaseMock : MediaContextBase
    {
        private StorageAccountBaseCollection _storageAccounts;

        public MediaContextBaseMock(StorageAccountBaseCollection storageAccounts)
        {
            _storageAccounts = storageAccounts;
            if (_storageAccounts != null)
            {
                _storageAccounts.MediaContext = this;
            }

            this.MediaServicesClassFactory = new AzureMediaServicesClassFactory();
        }

        public override AccessPolicyBaseCollection AccessPolicies { get { throw new NotImplementedException(); } }
        public override AssetBaseCollection Assets { get { throw new NotImplementedException(); } }
        public override ContentKeyBaseCollection ContentKeys { get { throw new NotImplementedException(); } }
        public override IStorageAccount DefaultStorageAccount { get { throw new NotImplementedException(); } }
        public override AssetFileBaseCollection Files { get { throw new NotImplementedException(); } }
        public override IngestManifestAssetCollection IngestManifestAssets { get { throw new NotImplementedException(); } }
        public override IngestManifestFileCollection IngestManifestFiles { get { throw new NotImplementedException(); } }
        public override IngestManifestCollection IngestManifests { get { throw new NotImplementedException(); } }
        public override JobBaseCollection Jobs { get { throw new NotImplementedException(); } }
        public override JobTemplateBaseCollection JobTemplates { get { throw new NotImplementedException(); } }
        public override LocatorBaseCollection Locators { get { throw new NotImplementedException(); } }
        public override MediaProcessorBaseCollection MediaProcessors { get { throw new NotImplementedException(); } }
        public override NotificationEndPointCollection NotificationEndPoints { get { throw new NotImplementedException(); } }
        public override StorageAccountBaseCollection StorageAccounts { get { return _storageAccounts; } }

    }
}
