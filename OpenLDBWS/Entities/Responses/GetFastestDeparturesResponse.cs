using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

[XmlRoot(ElementName = "GetFastestDeparturesResponse", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/")]
public class GetFastestDeparturesResponse
{
    [XmlElement(ElementName = "DeparturesBoard")]
    public required DeparturesBoard DeparturesBoard { get; set; }
}