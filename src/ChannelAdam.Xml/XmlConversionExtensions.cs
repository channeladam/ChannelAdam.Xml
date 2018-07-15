//-----------------------------------------------------------------------
// <copyright file="XmlConversionExtensions.cs">
//     Copyright (c) 2016-2018 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.Xml
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static class XmlConversionExtensions
    {
        public static XElement ToXElement(this string xml)
        {
            return XElement.Parse(xml);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XElement ToXElement(this XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return XElement.Parse(node.OuterXml);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlNode ToXmlNode(this XNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(node.ToString());
            return xmlDoc.DocumentElement;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlNode ToXmlNode(this string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc.DocumentElement;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlDocument ToXmlDocument<T>(this T valueToSerialise)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(valueToSerialise.SerialiseToXml());
            return xmlDoc;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlDocument ToXmlDocument<T>(this T valueToSerialise, XmlRootAttribute xmlRootAttribute)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(valueToSerialise.SerialiseToXml(xmlRootAttribute));
            return xmlDoc;
        }

        /// <summary>
        /// OBSOLETE. CAUTION - this is subject to XmlSerializer memory leaks as described in "Dynamically Generated Assemblies" in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSerialise"></param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        [Obsolete("This is subject to XmlSerializer memory leaks as described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks. Use ToXmlDocument<T>(this T valueToSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides) instead.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlDocument ToXmlDocument<T>(this T valueToSerialise, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(valueToSerialise.SerialiseToXml(xmlAttributeOverrides));
            return xmlDoc;
        }

        /// <summary>
        /// ToXmlDocument with XmlAttributeOverrides - and avoid the XmlSerializer memory leak described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToSerialise"></param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "As designed")]
        public static XmlDocument ToXmlDocument<T>(this T valueToSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(valueToSerialise.SerialiseToXml(equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides));
            return xmlDoc;
        }
    }
}
