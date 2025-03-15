
using System.Xml;
using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
public class SoapEnvelope
{
    [XmlElement(ElementName = "Body")]
    public required SoapBody Body { get; set; }
}


[XmlRoot(ElementName = "Body")]
public class SoapBody
{
    [XmlAnyElement]
    public required XmlElement Content { get; set; }
}