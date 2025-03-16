using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class ServiceDiversion
{
    [XmlElement("reason")]

    public string Reason { get; set;} = "";

    [XmlElement("divertedVia")]

    public string DivertedVia { get; set; } = "";

    [XmlElement("between")]

    public ServiceDiversionBetween? Between { get; set;}

    [XmlElement("reRouteDelay")]
    public int ReRouteDelay { get; set;}

}