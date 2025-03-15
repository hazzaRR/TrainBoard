using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class Departure
{
    [XmlAttribute("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("service", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public List<Service> Service { get; set;} = [];
}