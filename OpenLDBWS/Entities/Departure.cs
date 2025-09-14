using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class Departure
{
    [XmlAttribute("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("service", Namespace = LdbNamespaces.Lt8)]
    public List<Service> Service { get; set;} = [];
}