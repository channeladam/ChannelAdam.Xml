using System.Xml.Serialization;

namespace BehaviourSpecs.TestDoubles
{
    [XmlRoot(Namespace = "uri:normal:namespace")]
    public class TestObjectForXmlStuff
    {
        public string? MyStringProperty { get; set; }

        [XmlElement(Namespace = "uri:different:namespace")]
        public string? MyStringPropertyWithDifferentNamespace { get; set; }

        public int MyIntProperty { get; set; }
    }
}
