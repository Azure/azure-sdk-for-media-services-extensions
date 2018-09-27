// <copyright file="TestHelper.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace MediaServices.Client.Extensions.Tests
{
    using System;
    using System.Configuration;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class TestHelper
    {
        public static string FragBlobAssetId
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaServiceFragBlobAssetId"];
            }
        }
        public static string ActiveDirectoryEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];
            }
        }
        public static string MediaServicesResource
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaServicesResource"];
            }
        }

        public static string MediaServicesSdkClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaServicesSdkClientId"];
            }
        }

        public static string MediaServicesSdkRedirectUri
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaServicesSdkRedirectUri"];
            }
        }

        public static string ClientIdForAdAuth
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientIdForAdAuth"];
            }
        }
        public static string ClientSecretForAdAuth
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientSecretForAdAuth"];
            }
        }

        public static string MediaServicesAccountCustomApiServerEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings["MediaServicesAccountCustomApiServerEndpoint"];
            }
        }

        private static AzureEnvironment GetSelfDefinedEnvironment()
        {
            return new AzureEnvironment(
                new Uri(ActiveDirectoryEndpoint),
                MediaServicesResource,
                MediaServicesSdkClientId,
                new Uri(MediaServicesSdkRedirectUri));
        }

        public static CloudMediaContext CreateContext()
        {
            var environment = GetSelfDefinedEnvironment();
            var clientId = ConfigurationManager.AppSettings["ClientIdForAdAuth"];
            var clientCertificateThumbprint = ConfigurationManager.AppSettings["ClientCertificateThumbprintForAdAuth"];
            var tokenCredentials = new AzureAdTokenCredentials(ConfigurationManager.AppSettings["UserTenant"], new AzureAdClientCertificate(clientId, clientCertificateThumbprint), environment);
            var tokenProvider = new AzureAdTokenProvider(tokenCredentials);

            return new CloudMediaContext(new Uri(ConfigurationManager.AppSettings["MediaServicesAccountCustomApiServerEndpoint"]), tokenProvider);
        }

        public static StorageCredentials CreateStorageCredentials()
        {
            return new StorageCredentials(
                ConfigurationManager.AppSettings["MediaServiceStorageAccountName"],
                ConfigurationManager.AppSettings["MediaServiceStorageAccountKey"]);
        }

        public static CloudBlobClient CreateCloudBlobClient()
        {
            var storageAccount = new CloudStorageAccount(CreateStorageCredentials(), true);

            return storageAccount.CreateCloudBlobClient();
        }
    }
}