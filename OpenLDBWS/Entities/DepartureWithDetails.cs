using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class DepartureWithDetails
{
    [XmlAttribute("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("service", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public List<ServiceWithCallingPoints> Service { get; set;} = [];
}