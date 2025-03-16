using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetServiceDetailsResponse", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/")]
public class GetServiceDetailsResponse
{
    [XmlElement(ElementName = "GetServiceDetailsResult")]
    public required ServiceDetails ServiceDetails { get; set; }
}