using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetNextDeparturesWithDetailsResponse", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/")]
public class GetNextDeparturesWithDetailsResponse
{
    [XmlElement(ElementName = "DeparturesBoard")]
    public required DeparturesBoardWithDetails DeparturesBoard { get; set; }
}