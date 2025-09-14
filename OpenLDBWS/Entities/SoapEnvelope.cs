
using System.Xml;
using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "Envelope", Namespace = LdbNamespaces.Soap)]
public class SoapEnvelope
{
    [XmlElement(ElementName = "Body", Namespace = LdbNamespaces.Soap)]
    public required SoapBody Body { get; set; }
}


[XmlRoot(ElementName = "Body", Namespace = LdbNamespaces.Ldb)]
public class SoapBody
{
    [XmlAnyElement]
    public required XmlElement Content { get; set; }
}