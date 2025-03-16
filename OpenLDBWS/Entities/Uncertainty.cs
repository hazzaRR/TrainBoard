using System.Xml.Serialization;
using OpenLDBWS.Enums;

namespace OpenLDBWS.Entities;

public class Uncertainty
{
    [XmlElement("status")]
    public UncertaintyStatus Status { get; set;}

    [XmlElement("reason")]
    public string Reason { get; set;} = "";
}