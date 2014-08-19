// <copyright file="Source.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace Microsoft.WindowsAzure.MediaServices.Client.Metadata
{
    using System.Xml.Linq;

    /// <summary>
    /// Represents a source media file which was encoded to produce the current output media file.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// Gets the source file name.
        /// </summary>
        public string Name { get; internal set; }

        internal static Source Load(XElement sourceElement)
        {
            Source source = new Source();

            source.Name = sourceElement.GetAttributeOrDefault(AssetMetadataParser.NameAttributeName);

            return source;
        }
    }
}
