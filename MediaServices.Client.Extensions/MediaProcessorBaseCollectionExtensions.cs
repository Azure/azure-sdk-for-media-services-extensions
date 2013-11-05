// <copyright file="MediaProcessorBaseCollectionExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="MediaProcessorBaseCollection"/> class.
    /// </summary>
    public static class MediaProcessorBaseCollectionExtensions
    {
        /// <summary>
        /// Returns the latest version of the <see cref="IMediaProcessor"/> by its <paramref name="mediaProcessorName"/>. 
        /// </summary>
        /// <param name="mediaProcessorCollection">The <see cref="MediaProcessorBaseCollection"/> instance.</param>
        /// <param name="mediaProcessorName">The name of the media processor.</param>
        /// <returns>The latest version of the <see cref="IMediaProcessor"/> by its <paramref name="mediaProcessorName"/>.</returns>
        public static IMediaProcessor GetLatestMediaProcessorByName(this MediaProcessorBaseCollection mediaProcessorCollection, string mediaProcessorName)
        {
            if (mediaProcessorCollection == null)
            {
                throw new ArgumentNullException("mediaProcessorCollection", "The media processor collection cannot be null.");
            }

            return mediaProcessorCollection
                .Where(mp => mp.Name == mediaProcessorName)
                .ToList()
                .OrderBy(mp => new Version(mp.Version))
                .LastOrDefault();
        }
    }
}
