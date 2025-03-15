using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

[XmlRoot(ElementName = "GetArrDepBoardWithDetailsResponse", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/")]
public class GetArrDepBoardWithDetailsResponse
{
    [XmlElement(ElementName = "GetStationBoardResult")]
    public required StationBoardWithDetails StationBoardWithDetails { get; set; }
}