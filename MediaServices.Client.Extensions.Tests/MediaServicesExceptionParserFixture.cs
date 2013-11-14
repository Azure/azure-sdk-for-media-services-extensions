// <copyright file="MediaServicesExceptionParserFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;
    using System.Data.Services.Client;

    [TestClass]
    public class MediaServicesExceptionParserFixture
    {
        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException), AllowDerivedTypes = true)] // We really want this to be a DataServiceRequestException but there is an issue with multiple assemblies defining the type
        public void ShoudParseMediaServicesExceptionErrorMessage()
        {
            var context = TestHelper.CreateContext();
            var asset = context.Assets.Create("EmptyAsset", AssetCreationOptions.None);

            try
            {
                asset.Delete();
                asset.Delete();
            }
            catch (Exception exception)
            {
                var parsedException = MediaServicesExceptionParser.Parse(exception);

                Assert.IsNotNull(parsedException);
                Assert.AreEqual("Resource Asset not found", parsedException.Message);

                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ShoudParseMediaServicesExceptionErrorMessageFromAggregateException()
        {
            var context = TestHelper.CreateContext();
            var asset = context.Assets.Create("EmptyAsset", AssetCreationOptions.None);

            try
            {
                asset.DeleteAsync().Wait();
                asset.DeleteAsync().Wait();
            }
            catch (Exception exception)
            {
                var parsedException = MediaServicesExceptionParser.Parse(exception);

                Assert.IsNotNull(parsedException);
                Assert.AreEqual("Resource Asset not found", parsedException.Message);

                throw;
            }
        }

        [TestMethod]
        public void ShoudReturnOriginalExceptionIfCannotParseIt()
        {
            var exception = new Exception("exception message");
            var parsedException = MediaServicesExceptionParser.Parse(exception);

            Assert.AreSame(exception, parsedException);
        }

        [TestMethod]
        public void ShoudReturnNullIfOriginalExceptionIsNull()
        {
            var parsedException = MediaServicesExceptionParser.Parse(null);

            Assert.IsNull(parsedException);
        }

    }
}
