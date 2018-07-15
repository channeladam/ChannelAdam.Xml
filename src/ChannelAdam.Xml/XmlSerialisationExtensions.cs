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
    using ChannelAdam.Xml.Internal;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlSerialisationExtensions
    {
        #region Private Fields

        private static readonly ConcurrentDictionary<Tuple<Type, string>, XmlSerializer> _serialiserCache = new ConcurrentDictionary<Tuple<Type, string>, XmlSerializer>();
        private static readonly NamedLocker _namedLocker = new NamedLocker();

        #endregion Private Fields

        #region Public Methods

        #region Serialise

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
            var (serialiser, equalityKey) = GetOrAddXmlSerialiserFromCache(toSerialise.GetType(), xmlRootAttributeOverride);
            return SerialiseToXml(serialiser, toSerialise);
        }

        public static string SerialiseToXml<T>(this T toSerialise, XmlRootAttribute xmlRootAttributeOverride, XmlWriterSettings settings)
        {
            var (serialiser, equalityKey) = GetOrAddXmlSerialiserFromCache(toSerialise.GetType(), xmlRootAttributeOverride);
            return SerialiseToXml(serialiser, settings, toSerialise);
        }

        /// <summary>
        /// OBSOLETE. CAUTION - this is subject to XmlSerializer memory leaks as described in "Dynamically Generated Assemblies" in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialise"></param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        [Obsolete("This is subject to XmlSerializer memory leaks as described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks. Use SerialiseToXml<T>(this T toSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides) instead.")]
        public static string SerialiseToXml<T>(this T toSerialise, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType(), xmlAttributeOverrides);
            return SerialiseToXml(xmlSerialiser, toSerialise);
        }

        /// <summary>
        /// SerialiseToXml with XmlAttributeOverrides - and avoid the XmlSerializer memory leak described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialise"></param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        public static string SerialiseToXml<T>(this T toSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            XmlSerializer serialiser = GetOrAddXmlSerialiserFromCache(toSerialise.GetType(), equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides);
            return SerialiseToXml(serialiser, toSerialise);
        }

        /// <summary>
        /// OBSOLETE. CAUTION - this is subject to XmlSerializer memory leaks as described in "Dynamically Generated Assemblies" in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialise"></param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [Obsolete("This is subject to XmlSerializer memory leaks as described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks. Use SerialiseToXml<T>(this T toSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides, XmlWriterSettings settings) instead.")]
        public static string SerialiseToXml<T>(this T toSerialise, XmlAttributeOverrides xmlAttributeOverrides, XmlWriterSettings settings)
        {
            var xmlSerialiser = new XmlSerializer(toSerialise.GetType(), xmlAttributeOverrides);
            return SerialiseToXml(xmlSerialiser, settings, toSerialise);
        }

        /// <summary>
        /// SerialiseToXml with XmlAttributeOverrides - and avoid the XmlSerializer memory leak described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialise"></param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string SerialiseToXml<T>(this T toSerialise, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides, XmlWriterSettings settings)
        {
            XmlSerializer serialiser = GetOrAddXmlSerialiserFromCache(toSerialise.GetType(), equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides);
            return SerialiseToXml(serialiser, settings, toSerialise);
        }

        #endregion Serialise

        #region Deserialise

        public static T DeserialiseFromXml<T>(this string xml)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T));
            return DeserialiseFromXml<T>(xmlSerialiser, xml);
        }

        public static T DeserialiseFromXml<T>(this string xml, XmlRootAttribute xmlRootAttributeOverride)
        {
            Type typeOfT = typeof(T);
            var (serialiser, equalityKey) = GetOrAddXmlSerialiserFromCache(typeOfT, xmlRootAttributeOverride);

            var lockName = typeOfT.FullName + equalityKey;
            return _namedLocker.RunWithLock(lockName, () => DeserialiseFromXml<T>(serialiser, xml));
        }

        /// <summary>
        /// OBSOLETE. CAUTION - this is subject to XmlSerializer memory leaks as described in "Dynamically Generated Assemblies" in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        [Obsolete("This is subject to XmlSerializer memory leaks as described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks. Use DeserialiseFromXml<T>(this string xml, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides) instead.")]
        public static T DeserialiseFromXml<T>(this string xml, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T), xmlAttributeOverrides);
            return DeserialiseFromXml<T>(xmlSerialiser, xml);
        }

        /// <summary>
        /// DeserialiseFromXml with XmlAttributeOverrides - and avoid the XmlSerializer memory leak described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        public static T DeserialiseFromXml<T>(this string xml, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            Type typeOfT = typeof(T);
            XmlSerializer serialiser = GetOrAddXmlSerialiserFromCache(typeOfT, equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides);

            var lockName = typeOfT.FullName + equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak;
            return _namedLocker.RunWithLock(lockName, () => DeserialiseFromXml<T>(serialiser, xml));
        }

        public static T DeserialiseFromXml<T>(this Stream xmlStream)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T));
            return DeserialiseFromXml<T>(xmlSerialiser, xmlStream);
        }

        public static T DeserialiseFromXml<T>(this Stream xmlStream, XmlRootAttribute xmlRootAttributeOverride)
        {
            Type typeOfT = typeof(T);
            var (serialiser, equalityKey) = GetOrAddXmlSerialiserFromCache(typeOfT, xmlRootAttributeOverride);

            var lockName = typeOfT.FullName + equalityKey;
            return _namedLocker.RunWithLock(lockName, () => DeserialiseFromXml<T>(serialiser, xmlStream));
        }

        /// <summary>
        /// OBSOLETE. CAUTION - this is subject to XmlSerializer memory leaks as described in "Dynamically Generated Assemblies" in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStream"></param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        [Obsolete("This is subject to XmlSerializer memory leaks as described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks. Use DeserialiseFromXml<T>(this Stream xmlStream, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides) instead.")]
        public static T DeserialiseFromXml<T>(this Stream xmlStream, XmlAttributeOverrides xmlAttributeOverrides)
        {
            var xmlSerialiser = new XmlSerializer(typeof(T), xmlAttributeOverrides);
            return DeserialiseFromXml<T>(xmlSerialiser, xmlStream);
        }

        /// <summary>
        /// DeserialiseFromXml with XmlAttributeOverrides - and avoid the XmlSerializer memory leak described in 'Dynamically Generated Assemblies' in https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx#Remarks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStream"></param>
        /// <param name="equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak">CAUTION: XmlAttributeOverrides.GetHashCode() returns a different value for each instance, even if each instance has the exact same objects - so consider making your own equality key based on what you added to the XmlAttributeOverrides.</param>
        /// <param name="xmlAttributeOverrides"></param>
        /// <returns></returns>
        public static T DeserialiseFromXml<T>(this Stream xmlStream, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            Type typeOfT = typeof(T);
            XmlSerializer serialiser = GetOrAddXmlSerialiserFromCache(typeOfT, equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, xmlAttributeOverrides);

            var lockName = typeOfT.FullName + equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak;
            return _namedLocker.RunWithLock(lockName, () => DeserialiseFromXml<T>(serialiser, xmlStream));
        }

        #endregion Deserialise

        #endregion

        #region Private Methods

        private static (XmlSerializer xmlSerializer, string equalityKey) GetOrAddXmlSerialiserFromCache(Type objectType, XmlRootAttribute xmlRootAttributeOverride)
        {
            /*
             * The XmlSerializer(Type, XmlAttributeOverrides) ctor leaks memory, so cache it per unique input values: 
             *      https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx
             *     "To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types. 
             *      The infrastructure finds and reuses those assemblies. This behavior occurs only when using the following constructors:
             *          XmlSerializer.XmlSerializer(Type)
             *          XmlSerializer.XmlSerializer(Type, String)
             *      If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, 
             *      which results in a memory leak and poor performance. The easiest solution is to use one of the previously mentioned two constructors. 
             *      Otherwise, you must cache the assemblies..."
            */

            string equalityKey = $"ROOT|{xmlRootAttributeOverride.DataType}|{xmlRootAttributeOverride.ElementName}|{xmlRootAttributeOverride.IsNullable}|{xmlRootAttributeOverride.Namespace}";

            return (_serialiserCache.GetOrAdd(
                        Tuple.Create(objectType, equalityKey),
                        _ =>
                        {
                            XmlAttributeOverrides xmlAttributeOverrides = CreateXmlAttributeOverrides(objectType, xmlRootAttributeOverride);
                            return new XmlSerializer(objectType, xmlAttributeOverrides);
                        }),
                equalityKey);
        }

        private static XmlSerializer GetOrAddXmlSerialiserFromCache(Type objectType, string equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak, XmlAttributeOverrides xmlAttributeOverrides)
        {
            /*
             * The XmlSerializer(Type, XmlAttributeOverrides) ctor leaks memory, so cache it per unique input values: 
             *      https://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx
             *     "To increase performance, the XML serialization infrastructure dynamically generates assemblies to serialize and deserialize specified types. 
             *      The infrastructure finds and reuses those assemblies. This behavior occurs only when using the following constructors:
             *          XmlSerializer.XmlSerializer(Type)
             *          XmlSerializer.XmlSerializer(Type, String)
             *      If you use any of the other constructors, multiple versions of the same assembly are generated and never unloaded, 
             *      which results in a memory leak and poor performance. The easiest solution is to use one of the previously mentioned two constructors. 
             *      Otherwise, you must cache the assemblies..."
            */

            return _serialiserCache.GetOrAdd(
                Tuple.Create(objectType, equalityKeyOfXmlAttributeOverridesToAvoidXmlSerializerMemoryLeak),
                _ => new XmlSerializer(objectType, xmlAttributeOverrides));
        }

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
