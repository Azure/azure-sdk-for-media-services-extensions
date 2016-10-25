// <copyright file="XmlElementExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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
    using System;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Linq;

    internal static class XmlElementExtensions
    {
        private const long DefaultLongAttributeValue = 0L;

        private const int DefaultIntAttributeValue = 0;

        private const double DefaultDoubleAttributeValue = 0;

        private static readonly TimeSpan DefaultTimeSpanAttributeValue = TimeSpan.Zero;

        internal static string GetAttributeOrDefault(this XElement element, XName name)
        {
            string attributeValue = null;

            XAttribute attribute = element.Attribute(name);
            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }

            return attributeValue;
        }

        internal static long GetAttributeAsLongOrDefault(this XElement element, XName name)
        {
            string attributeValueString = element.GetAttributeOrDefault(name);

            long attributeValue;
            if (!long.TryParse(attributeValueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out attributeValue))
            {
                attributeValue = DefaultLongAttributeValue;
            }

            return attributeValue;
        }

        internal static int GetAttributeAsIntOrDefault(this XElement element, XName name)
        {
            string attributeValueString = element.GetAttributeOrDefault(name);

            int attributeValue;
            if (!int.TryParse(attributeValueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out attributeValue))
            {
                attributeValue = DefaultIntAttributeValue;
            }

            return attributeValue;
        }

        internal static double GetAttributeAsDoubleOrDefault(this XElement element, XName name)
        {
            string attributeValueString = element.GetAttributeOrDefault(name);

            double attributeValue;
            if (!double.TryParse(attributeValueString, NumberStyles.Float, CultureInfo.InvariantCulture, out attributeValue))
            {
                attributeValue = DefaultDoubleAttributeValue;
            }

            return attributeValue;
        }

        internal static TimeSpan GetAttributeAsTimeSpanOrDefault(this XElement element, XName name)
        {
            string attributeValueString = element.GetAttributeOrDefault(name);

            TimeSpan attributeValue;
            try
            {
                attributeValue = XmlConvert.ToTimeSpan(attributeValueString);
            }
            catch (FormatException)
            {
                attributeValue = DefaultTimeSpanAttributeValue;
            }

            return attributeValue;
        }
    }
}
