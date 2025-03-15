using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "location", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
public class ServiceLocation 
{

    [XmlElement(ElementName = "locationName", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string LocationName {get; set;} = "";

    [XmlElement(ElementName = "crs", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string Crs {get; set;} = "";

    [XmlElement(ElementName = "via")]
    public string? Via {get; set;}

    [XmlElement(ElementName = "futureChangeTo")]
    public string? FutureChangeTo {get; set;}

    [XmlElement(ElementName = "assocIsCancelled")]
    public bool AssocIsCancelled {get; set;}

}
