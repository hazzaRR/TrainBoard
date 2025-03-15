using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetNextDeparturesResponse", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/")]
public class GetNextDeparturesResponse
{
    [XmlElement(ElementName = "DeparturesBoard")]
    public required DeparturesBoard DeparturesBoard { get; set; }
}