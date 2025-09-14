using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "location", Namespace = LdbNamespaces.Lt4)]
public class ServiceLocation 
{

    [XmlElement(ElementName = "locationName", Namespace = LdbNamespaces.Lt4)]
    public string LocationName {get; set;} = "";

    [XmlElement(ElementName = "crs", Namespace = LdbNamespaces.Lt4)]
    public string Crs {get; set;} = "";

    [XmlElement(ElementName = "via")]
    public string? Via {get; set;}

    [XmlElement(ElementName = "futureChangeTo")]
    public string? FutureChangeTo {get; set;}

    [XmlElement(ElementName = "assocIsCancelled")]
    public bool AssocIsCancelled {get; set;}

}
