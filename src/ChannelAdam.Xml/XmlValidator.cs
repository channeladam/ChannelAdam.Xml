//-----------------------------------------------------------------------
// <copyright file="XmlValidator.cs">
//     Copyright (c) 2016-2021 Adam Craven. All rights reserved.
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Validates XML against specified XML schemas.
    /// </summary>
    public class XmlValidator
    {
        #region Private Fields

        private readonly IList<string> validationErrors;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlValidator"/> class.
        /// </summary>
        public XmlValidator()
        {
            this.validationErrors = new List<string>();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>
        /// The validation errors.
        /// </value>
        public IEnumerable<string> ValidationErrors
        {
            get { return this.validationErrors; }
        }

        #endregion Public Properties

        #region ValidateXml Methods

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        public void ValidateXml(string xml)
        {
            this.ValidateXml(xml, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        public void ValidateXml(string xml, bool treatWarningsAsErrors)
        {
            if (!this.IsValidXml(xml, treatWarningsAsErrors))
            {
                this.ThrowXmlSchemaValidationException();
            }
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        public void ValidateXml(string xml, Stream xmlSchema)
        {
            this.ValidateXml(xml, xmlSchema, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings as treated as errors.</param>
        public void ValidateXml(string xml, Stream xmlSchema, bool treatWarningsAsErrors)
        {
            var xsd = XmlSchema.Read(xmlSchema, null);
            if (xsd is null)
            {
                throw new InvalidOperationException("xmlSchema could not be successfully parsed");
            }

            this.ValidateXml(xml, xsd, treatWarningsAsErrors);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        public void ValidateXml(string xml, Stream xmlSchema, ValidationEventHandler validationEventHandler)
        {
            this.ValidateXml(xml, xmlSchema, validationEventHandler, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        public void ValidateXml(string xml, Stream xmlSchema, ValidationEventHandler validationEventHandler, bool treatWarningsAsErrors)
        {
            var xsd = XmlSchema.Read(xmlSchema, validationEventHandler);
            if (xsd is null)
            {
                throw new InvalidOperationException("xmlSchema could not be successfully parsed");
            }

            this.ValidateXml(xml, xsd, treatWarningsAsErrors);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        public void ValidateXml(string xml, XmlSchema xmlSchema)
        {
            this.ValidateXml(xml, xmlSchema, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        public void ValidateXml(string xml, XmlSchema xmlSchema, bool treatWarningsAsErrors)
        {
            if (!this.IsValidXml(xml, xmlSchema, treatWarningsAsErrors))
            {
                this.ThrowXmlSchemaValidationException();
            }
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchemas">The XML schemas.</param>
        public void ValidateXml(string xml, XmlSchemaSet xmlSchemas)
        {
            this.ValidateXml(xml, xmlSchemas, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchemas">The XML schemas.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        public void ValidateXml(string xml, XmlSchemaSet xmlSchemas, bool treatWarningsAsErrors)
        {
            if (!this.IsValidXml(xml, xmlSchemas, treatWarningsAsErrors))
            {
                this.ThrowXmlSchemaValidationException();
            }
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="schemaUri">The schema URI.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Consistent with .NET library.")]
        public void ValidateXml(string xml, string targetNamespace, string schemaUri)
        {
            this.ValidateXml(xml, targetNamespace, schemaUri, true);
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="schemaUri">The schema URI.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Consistent with .NET library.")]
        public void ValidateXml(string xml, string targetNamespace, string schemaUri, bool treatWarningsAsErrors)
        {
            if (!this.IsValidXml(xml, targetNamespace, schemaUri, treatWarningsAsErrors))
            {
                this.ThrowXmlSchemaValidationException();
            }
        }

        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlReaderSettings">The XML reader settings.</param>
        public void ValidateXml(string xml, XmlReaderSettings xmlReaderSettings)
        {
            if (!this.IsValidXml(xml, xmlReaderSettings))
            {
                this.ThrowXmlSchemaValidationException();
            }
        }

        #endregion ValidateXml Methods

        #region IsValidXml Methods

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml)
        {
            return this.IsValidXml(xml, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, bool treatWarningsAsErrors)
        {
            var settings = CreateXmlReaderSettings(treatWarningsAsErrors);

            return this.IsValidXml(xml, settings);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, Stream xmlSchema)
        {
            return this.IsValidXml(xml, xmlSchema, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, Stream xmlSchema, bool treatWarningsAsErrors)
        {
            var xsd = XmlSchema.Read(xmlSchema, null);
            if (xsd is null)
            {
                throw new InvalidOperationException("xmlSchema could not be successfully parsed");
            }

            return this.IsValidXml(xml, xsd, treatWarningsAsErrors);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, Stream xmlSchema, ValidationEventHandler validationEventHandler)
        {
            return this.IsValidXml(xml, xmlSchema, validationEventHandler, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="validationEventHandler">The validation event handler.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, Stream xmlSchema, ValidationEventHandler validationEventHandler, bool treatWarningsAsErrors)
        {
            var xsd = XmlSchema.Read(xmlSchema, validationEventHandler);
            if (xsd is null)
            {
                throw new InvalidOperationException("xmlSchema could not be successfully parsed");
            }

            return this.IsValidXml(xml, xsd, treatWarningsAsErrors);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, XmlSchema xmlSchema)
        {
            return this.IsValidXml(xml, xmlSchema, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchema">The XML schema.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, XmlSchema xmlSchema, bool treatWarningsAsErrors)
        {
            var settings = CreateXmlReaderSettings(treatWarningsAsErrors);

            if (xmlSchema != null)
            {
                settings.Schemas.Add(xmlSchema);
            }

            return this.IsValidXml(xml, settings);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchemas">The XML schemas.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, XmlSchemaSet xmlSchemas)
        {
            return this.IsValidXml(xml, xmlSchemas, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlSchemas">The XML schemas.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        public bool IsValidXml(string xml, XmlSchemaSet xmlSchemas, bool treatWarningsAsErrors)
        {
            var settings = CreateXmlReaderSettings(treatWarningsAsErrors);

            if (xmlSchemas != null)
            {
                settings.Schemas.Add(xmlSchemas);
            }

            return this.IsValidXml(xml, settings);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="schemaUri">The schema URI.</param>
        /// <returns>True if the given XML is valid.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Consistent with .NET library.")]
        public bool IsValidXml(string xml, string targetNamespace, string schemaUri)
        {
            return this.IsValidXml(xml, targetNamespace, schemaUri, true);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <param name="schemaUri">The schema URI.</param>
        /// <param name="treatWarningsAsErrors">If set to <c>true</c> warnings are treated as errors.</param>
        /// <returns>True if the given XML is valid.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "Consistent with .NET library.")]
        public bool IsValidXml(string xml, string targetNamespace, string schemaUri, bool treatWarningsAsErrors)
        {
            var settings = CreateXmlReaderSettings(treatWarningsAsErrors);

            if (targetNamespace != null || schemaUri != null)
            {
                settings.Schemas.Add(targetNamespace ?? string.Empty, schemaUri ?? string.Empty);
            }

            return this.IsValidXml(xml, settings);
        }

        /// <summary>
        /// Determines whether the given XML is valid.
        /// </summary>
        /// <param name="xml">The XML to be validated.</param>
        /// <param name="xmlReaderSettings">The XML reader settings.</param>
        /// <returns>True if the given XML is valid.</returns>
        /// <exception cref="ArgumentNullException">If the xmlReaderSettings parameter is null.</exception>
        public bool IsValidXml(string xml, XmlReaderSettings xmlReaderSettings)
        {
            if (xmlReaderSettings == null)
            {
                throw new ArgumentNullException(nameof(xmlReaderSettings));
            }

            TextReader? textReader = null;

            this.validationErrors.Clear();

            try
            {
                textReader = new StringReader(xml);
                xmlReaderSettings.ValidationEventHandler += this.ValidationEventHandler;

                using XmlReader? xmlReader = XmlReader.Create(textReader, xmlReaderSettings);
                textReader = null;
                while (xmlReader.Read())
                {
                }
            }
            finally
            {
                textReader?.Dispose();

                xmlReaderSettings.ValidationEventHandler -= this.ValidationEventHandler;
            }

            return this.validationErrors.Count == 0;
        }

        #endregion IsValidXml Methods

        #region Private Methods

        private static XmlReaderSettings CreateXmlReaderSettings(bool treatWarningsAsErrors)
        {
            var settings = new XmlReaderSettings
            {
                ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes,
                ValidationType = ValidationType.Schema
            };

            if (treatWarningsAsErrors)
            {
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;     // An example warning is when the namespace does not match
            }

            return settings;
        }

        private void ThrowXmlSchemaValidationException()
        {
            throw new XmlSchemaValidationException(string.Join("." + Environment.NewLine, this.validationErrors));
        }

        private void ValidationEventHandler(object? sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    this.validationErrors.Add("XSD Validation Error: " + e.Message);
                    break;

                case XmlSeverityType.Warning:
                    this.validationErrors.Add("XSD Validation Warning: " + e.Message);
                    break;
            }
        }

        #endregion Private Methods
    }
}