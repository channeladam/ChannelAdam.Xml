//-----------------------------------------------------------------------
// <copyright file="XmlSerialisationExtensions.cs">
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
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlSerialisationExtensions
    {
        #region Public Methods

        public static string SerialiseToXml<T>(this T toSerialise)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType());
            return SerialiseToXml(xmlSerialiser, toSerialise);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlWriterSettings settings)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType());
            return SerialiseToXml(xmlSerialiser, settings, toSerialise);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlRootAttribute xmlRootAttributeOverride)
        {
            var xmlAttributeOverrides = CreateXmlAttributeOverrides(toSerialise.GetType(), xmlRootAttributeOverride);
            return SerialiseToXml(toSerialise, xmlAttributeOverrides);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlRootAttribute xmlRootAttributeOverride, XmlWriterSettings settings)
        {
            var xmlAttributeOverrides = CreateXmlAttributeOverrides(toSerialise.GetType(), xmlRootAttributeOverride);
            return SerialiseToXml(toSerialise, xmlAttributeOverrides, settings);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType(), xmlAttributeOverrides);
            return SerialiseToXml(xmlSerialiser, toSerialise);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlAttributeOverrides xmlAttributeOverrides, XmlWriterSettings settings)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType(), xmlAttributeOverrides);
            return SerialiseToXml(xmlSerialiser, settings, toSerialise);
        }

        public static T DeserialiseFromXml<T>(this string xml)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T));
            return DeserialiseFromXml<T>(xmlSerialiser, xml);
        }

        public static T DeserialiseFromXml<T>(this string xml, XmlRootAttribute xmlRootAttributeOverride)
        {
            var xmlAttributeOverrides = CreateXmlAttributeOverrides(typeof(T), xmlRootAttributeOverride);
            return DeserialiseFromXml<T>(xml, xmlAttributeOverrides);
        }

        public static T DeserialiseFromXml<T>(this string xml, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T), xmlAttributeOverrides);
            return DeserialiseFromXml<T>(xmlSerialiser, xml);
        }

        public static T DeserialiseFromXml<T>(this Stream xmlStream)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T));
            return DeserialiseFromXml<T>(xmlSerialiser, xmlStream);
        }

        public static T DeserialiseFromXml<T>(this Stream xmlStream, XmlRootAttribute xmlRootAttributeOverride)
        {
            var xmlAttributeOverrides = CreateXmlAttributeOverrides(typeof(T), xmlRootAttributeOverride);
            return DeserialiseFromXml<T>(xmlStream, xmlAttributeOverrides);
        }

        public static T DeserialiseFromXml<T>(this Stream xmlStream, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T), xmlAttributeOverrides);
            return DeserialiseFromXml<T>(xmlSerialiser, xmlStream);
        }

        #endregion

        #region Private Methods

        private static XmlAttributeOverrides CreateXmlAttributeOverrides(Type objectType, XmlRootAttribute newRootAttribute)
        {
            var xmlAttributeOverrides = new XmlAttributeOverrides();
            var xmlAttributes = new XmlAttributes
            {
                XmlRoot = newRootAttribute
            };
            xmlAttributeOverrides.Add(objectType, xmlAttributes);
            return xmlAttributeOverrides;
        }

        private static string SerialiseToXml(XmlSerializer xmlSerialiser, object toSerialise)
        {
            using (var writer = new StringWriter())
            {
                xmlSerialiser.Serialize(writer, toSerialise);
                return writer.ToString();
            }
        }

        private static string SerialiseToXml(XmlSerializer xmlSerialiser, XmlWriterSettings xmlWriterSettings, object toSerialise)
        {
            var sb = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(sb, xmlWriterSettings))
            {
                xmlSerialiser.Serialize(xmlWriter, toSerialise);
                return sb.ToString();
            }
        }

        private static T DeserialiseFromXml<T>(XmlSerializer xmlSerialiser, string xml)
        {
            xmlSerialiser.UnknownAttribute += XmlSerialiser_UnknownAttribute;
            xmlSerialiser.UnknownElement += XmlSerialiser_UnknownElement;
            xmlSerialiser.UnknownNode += XmlSerialiser_UnknownNode;
            xmlSerialiser.UnreferencedObject += XmlSerialiser_UnreferencedObject;

            try
            {
                using (var reader = new StringReader(xml))
                {
                    return (T)xmlSerialiser.Deserialize(reader);
                }
            }
            finally
            {
                xmlSerialiser.UnknownAttribute -= XmlSerialiser_UnknownAttribute;
                xmlSerialiser.UnknownElement -= XmlSerialiser_UnknownElement;
                xmlSerialiser.UnknownNode -= XmlSerialiser_UnknownNode;
                xmlSerialiser.UnreferencedObject -= XmlSerialiser_UnreferencedObject;
            }
        }

        private static T DeserialiseFromXml<T>(XmlSerializer xmlSerialiser, Stream toDeserialise)
        {
            xmlSerialiser.UnknownAttribute += XmlSerialiser_UnknownAttribute;
            xmlSerialiser.UnknownElement += XmlSerialiser_UnknownElement;
            xmlSerialiser.UnknownNode += XmlSerialiser_UnknownNode;
            xmlSerialiser.UnreferencedObject += XmlSerialiser_UnreferencedObject;

            try
            {
                return (T)xmlSerialiser.Deserialize(toDeserialise);
            }
            finally
            {
                xmlSerialiser.UnknownAttribute -= XmlSerialiser_UnknownAttribute;
                xmlSerialiser.UnknownElement -= XmlSerialiser_UnknownElement;
                xmlSerialiser.UnknownNode -= XmlSerialiser_UnknownNode;
                xmlSerialiser.UnreferencedObject -= XmlSerialiser_UnreferencedObject;
            }
        }

        private static void XmlSerialiser_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            Trace.WriteLine($"XmlSerialiser Error - Unreferenced Object - Id:{e.UnreferencedId}");
        }

        private static void XmlSerialiser_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Trace.WriteLine($"XmlSerialiser Error - Unknown Node - LineNumber:{e.LineNumber}, LinePosition:{e.LinePosition}, Namespace:'{e.NamespaceURI}', Name:'{e.Name}', Text:'{e.Text}'");
        }

        private static void XmlSerialiser_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Trace.WriteLine($"XmlSerialiser Error - Unknown Element - LineNumber:{e.LineNumber}, LinePosition:{e.LinePosition}, Namespace:'{e.Element.NamespaceURI}', Name:'{e.Element.Name}', Expected:'{e.ExpectedElements}'");
        }

        private static void XmlSerialiser_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Trace.WriteLine($"XmlSerialiser Error - Unknown Attribute - LineNumber:{e.LineNumber}, LinePosition:{e.LinePosition}, Namespace:'{e.Attr.NamespaceURI}', Name:'{e.Attr.Name}', Expected:'{e.ExpectedAttributes}'");
        }

        #endregion
    }
}
