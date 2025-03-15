using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class ServiceDiversionBetweenBetweenLocation
{
    [XmlElement("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("Value")]
    public string Value { get; set; } = "";

}