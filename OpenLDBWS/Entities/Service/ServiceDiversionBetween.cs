using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


public class ServiceDiversionBetween
{
    [XmlElement("start")]
    public ServiceDiversionBetweenBetweenLocation Start { get; set; }

    [XmlElement("end")]

    public ServiceDiversionBetweenBetweenLocation End { get; set; }
}