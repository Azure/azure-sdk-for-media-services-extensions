// <copyright file="MediaProcessorBaseCollectionExtensionsFixture.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.MediaServices.Client;

    [TestClass]
    public class MediaProcessorBaseCollectionExtensionsFixture
    {
        private CloudMediaContext context;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowWhenGetLatestMediaProcessorByNameIfMediaProcessorCollectionIsNull()
        {
            MediaProcessorBaseCollection nullMediaProcessorCollection = null;

            nullMediaProcessorCollection.GetLatestMediaProcessorByName(MediaProcessorNames.WindowsAzureMediaEncoder);
        }

        [TestMethod]
        public void ShouldGetLatestMediaProcessorByNameReturnNullIfMediaProcessorNameIsNotValid()
        {
            var mediaProcessorName = "Invalid Media Processor Name";
            var mediaProcessor = this.context.MediaProcessors.GetLatestMediaProcessorByName(mediaProcessorName);

            Assert.IsNull(mediaProcessor);
        }

        [TestMethod]
        public void ShouldGetLatestMediaProcessorByName()
        {
            var mediaProcessor = this.context.MediaProcessors.GetLatestMediaProcessorByName(MediaProcessorNames.WindowsAzureMediaEncoder);

            Assert.IsNotNull(mediaProcessor);

            var expectedMediaProcessor = this.context.MediaProcessors
                .Where(mp => mp.Name == MediaProcessorNames.WindowsAzureMediaEncoder)
                .ToList()
                .Select(mp => new { mp.Id, mp.Name, Version = new Version(mp.Version) })
                .OrderBy(mp => mp.Version)
                .Last();

            Assert.AreEqual(expectedMediaProcessor.Id, mediaProcessor.Id);
            Assert.AreEqual(expectedMediaProcessor.Name, mediaProcessor.Name);
            Assert.AreEqual(expectedMediaProcessor.Version, new Version(mediaProcessor.Version));
        }

        [TestInitialize]
        public void Initialize()
        {
            this.context = TestHelper.CreateContext();
        }
    }
}
