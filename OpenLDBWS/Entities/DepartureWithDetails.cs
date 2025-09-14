using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class DepartureWithDetails
{
    [XmlAttribute("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("service", Namespace = LdbNamespaces.Lt8)]
    public List<ServiceWithCallingPoints> Service { get; set;} = [];
}