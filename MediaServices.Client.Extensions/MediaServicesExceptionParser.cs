// <copyright file="MediaServicesExceptionParser.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// Contains helper methods to parse Windows Azure Media Services error messages in XML format.
    /// </summary>
    public static class MediaServicesExceptionParser
    {
        /// <summary>
        /// Represents the XML namespace name of Windows Azure Media Services error messages.
        /// </summary>
        public const string DataServicesMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        private static readonly XName MessageXName = XName.Get("message", DataServicesMetadataNamespace);
        private static readonly XName CodeXName = XName.Get("code", DataServicesMetadataNamespace);

        /// <summary>
        /// Returns a new <see cref="System.Exception"/> instance with the XML error message content parsed.
        /// </summary>
        /// <param name="exception">The original exception with the XML error message.</param>
        /// <returns>A new <see cref="System.Exception"/> instance with the XML error message content parsed.</returns>
        public static Exception Parse(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            try
            {
                Exception baseException = exception.GetBaseException().GetBaseException();
                XDocument doc = XDocument.Parse(baseException.Message);

                return ParseExceptionErrorXElement(doc.Root) ?? exception;
            }
            catch
            {
                return exception;
            }
        }

        private static Exception ParseExceptionErrorXElement(XElement errorElement)
        {
            string errorCode = errorElement.GetElementValueOrDefault(CodeXName);
            string errorMessage = errorElement.GetElementValueOrDefault(MessageXName);

            Exception exception = null;
            if (!string.IsNullOrWhiteSpace(errorCode) && !string.IsNullOrWhiteSpace(errorMessage))
            {
                exception = new Exception(
                    string.Format(CultureInfo.InvariantCulture, "{0}: {1}", errorCode, errorMessage));
            }
            else if (!string.IsNullOrWhiteSpace(errorCode))
            {
                exception = new Exception(errorCode);
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                exception = new Exception(errorMessage);
            }

            return exception;
        }

        private static string GetElementValueOrDefault(this XElement element, XName name)
        {
            string value = null;
            XElement childElement = element.Element(name);
            if (childElement != null)
            {
                value = childElement.Value.ToString();
            }

            return value;
        }
    }
}