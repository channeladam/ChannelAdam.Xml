using System;
using BehaviourSpecs.TestDoubles;
using TechTalk.SpecFlow;
using ChannelAdam.TestFramework.MSTest.Abstractions;
using ChannelAdam.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace BehaviourSpecs
{
    [Binding]
    [Scope(Feature = "XML Serialisation")]
    public class XmlSerialisationUnitTestSteps : MoqTestFixture
    {
        private TestObjectForXmlStuff testXmlObject;
        private string actualXml;
        private string expectedXml;

        #region Setup / Teardown

        [BeforeScenario]
        public void Setup()
        {
        }

        [AfterScenario]
        public void CleanUp()
        {
            Logger.Log("About to verify mock objects");
            base.MyMockRepository.Verify();
        }

        #endregion

        #region Given

        [Given(@"a test object")]
        [Given(@"an object with a default XML root attribute")]
        public void GivenAnObjectWithADefaultXMLRootAttribute()
        {
            CreateTestXmlObject();
        }

        [Given(@"a class with a default XML root attribute")]
        public void GivenAClassWithADefaultXMLRootAttribute()
        {
            // Nothing to do...  class TestObjectForXmlStuff exists
        }

        [Given(@"an XML string with the default XML root attribute")]
        public void GivenAnXMLStringWithTheDefaultXMLRootAttribute()
        {
            this.expectedXml = GetAsString(Assembly.GetExecutingAssembly(), "BehaviourSpecs.TestData.ExpectedXmlWithDefaultRoot.xml");
            Logger.Log($"XML string to deserialise: {Environment.NewLine}" + this.expectedXml);
        }

        [Given(@"an XML string with a root attribute that is different from the default XML root attribute")]
        public void GivenAnXMLStringWithARootAttributeThatIsDifferentFromTheDefaultXMLRootAttribute()
        {
            this.expectedXml = GetAsString(Assembly.GetExecutingAssembly(), "BehaviourSpecs.TestData.ExpectedXmlWithNewRoot.xml");
            Logger.Log($"XML string to deserialise: {Environment.NewLine}" + this.expectedXml);
        }

        #endregion

        #region When

        [When(@"the object with a default XML root attribute is serialised with no serialisation overrides")]
        public void WhenTheObjectWithADefaultXMLRootAttributeIsSerialisedWithNoSerialisationOverrides()
        {
            this.expectedXml = GetAsString(Assembly.GetExecutingAssembly(), "BehaviourSpecs.TestData.ExpectedXmlWithDefaultRoot.xml");
            Logger.Log($"Expected XML: {Environment.NewLine}" + this.expectedXml);

            this.actualXml = this.testXmlObject.SerialiseToXml();
            Logger.Log($"Actual XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the object with a default XML root attribute is serialised with an override of the XML root attribute")]
        public void WhenTheObjectWithADefaultXMLRootAttributeIsSerialisedWithAnOverrideOfTheXMLRootAttribute()
        {
            this.expectedXml = GetAsString(Assembly.GetExecutingAssembly(), "BehaviourSpecs.TestData.ExpectedXmlWithNewRoot.xml");
            Logger.Log($"Expected XML: {Environment.NewLine}" + this.expectedXml);

            var newXmlRoot = new XmlRootAttribute("NewRoot")
            {
                Namespace = "uri:new:namespace"
            };
            this.actualXml = this.testXmlObject.SerialiseToXml(newXmlRoot);
            Logger.Log($"Actual XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the object with a default XML root attribute is serialised with an override of the XML attributes")]
        public void WhenTheObjectWithADefaultXMLRootAttributeIsSerialisedWithAnOverrideOfTheXMLAttributes()
        {
            this.expectedXml = GetAsString(Assembly.GetExecutingAssembly(), "BehaviourSpecs.TestData.ExpectedXmlWithNewRoot.xml");
            Logger.Log($"Expected XML: {Environment.NewLine}" + this.expectedXml);

            XmlRootAttribute newXmlRoot = CreateNewXmlRootAttribute();
            var xmlAttributes = new XmlAttributes
            {
                XmlRoot = newXmlRoot
            };
            var attributeOverrides = new XmlAttributeOverrides();
            attributeOverrides.Add(typeof(TestObjectForXmlStuff), xmlAttributes);

            this.actualXml = this.testXmlObject.SerialiseToXml(attributeOverrides);
            Logger.Log($"Actual XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the XML string with the default XML root attribute is deserialised with no serialisation overrides")]
        public void WhenTheXMLStringWithTheDefaultXMLRootAttributeIsDeserialisedWithNoSerialisationOverrides()
        {
            this.testXmlObject = this.expectedXml.DeserialiseFromXml<TestObjectForXmlStuff>();
            this.actualXml = this.testXmlObject.SerialiseToXml();
            Logger.Log($"Actual re-serialised XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the XML string with the different XML root attribute is deserialised with an override of the XML root attribute")]
        public void WhenTheXMLStringWithTheDifferentXMLRootAttributeIsDeserialisedWithAnOverrideOfTheXMLRootAttribute()
        {
            var newXmlRoot = CreateNewXmlRootAttribute();

            this.testXmlObject = this.expectedXml.DeserialiseFromXml<TestObjectForXmlStuff>(newXmlRoot);
            this.actualXml = this.testXmlObject.SerialiseToXml(newXmlRoot);
            Logger.Log($"Actual re-serialised XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the XML string with the different XML root attribute is deserialised with an override of the XML attributes")]
        public void WhenTheXMLStringWithTheDifferentXMLRootAttributeIsDeserialisedWithAnOverrideOfTheXMLAttributes()
        {
            var newXmlRoot = CreateNewXmlRootAttribute();
            var xmlAttributes = new XmlAttributes
            {
                XmlRoot = newXmlRoot
            };
            var attributeOverrides = new XmlAttributeOverrides();
            attributeOverrides.Add(typeof(TestObjectForXmlStuff), xmlAttributes);

            this.testXmlObject = this.expectedXml.DeserialiseFromXml<TestObjectForXmlStuff>(attributeOverrides);
            this.actualXml = this.testXmlObject.SerialiseToXml(attributeOverrides);
            Logger.Log($"Actual re-serialised XML: {Environment.NewLine}" + this.actualXml);
        }

        [When(@"the instance cast as an object is serialised")]
        public void WhenTheInstanceCastAsAnObjectIsSerialised()
        {
            this.expectedXml = this.testXmlObject.SerialiseToXml();
            Logger.Log($"Expected XML: {Environment.NewLine}" + this.expectedXml);

            object obj = this.testXmlObject;
            this.actualXml = obj.SerialiseToXml();
            Logger.Log($"Actual XML: {Environment.NewLine}" + this.actualXml);
        }

        #endregion

        #region Then

        [Then(@"the object is serialised correctly")]
        [Then(@"the XML string is deserialised successfully")]
        public void ThenTheObjectIsSerialisedCorrectly()
        {
            LogAssert.AreEqual("XML", this.expectedXml, this.actualXml);
        }

        #endregion

        #region Private Methods

        private void CreateTestXmlObject()
        {
            this.testXmlObject = new TestObjectForXmlStuff
            {
                MyIntProperty = 12,
                MyStringProperty = "Dummy data",
                MyStringPropertyWithDifferentNamespace = "Dummy data 2"
            };
        }

        private static XmlRootAttribute CreateNewXmlRootAttribute()
        {
            var newXmlRoot = new XmlRootAttribute("NewRoot")
            {
                Namespace = "uri:new:namespace"
            };
            return newXmlRoot;
        }

        private static Stream GetAsStream(Assembly assembly, string resourceName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new System.IO.FileNotFoundException($"Cannot find the embedded resource '{resourceName}' in assembly '{assembly.FullName}'.");
            }

            return stream;
        }

        private static string GetAsString(Assembly assembly, string resourceName)
        {
            Stream stream = null;
            try
            {
                stream = GetAsStream(assembly, resourceName);
                using (var reader = new StreamReader(stream))
                {
                    stream = null;
                    return reader.ReadToEnd();
                }
            }
            finally
            {
                stream?.Dispose();
            }
        }

        #endregion
    }
}
