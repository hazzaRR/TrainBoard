using System.Xml.Serialization;
using OpenLDBWS.Enums;

namespace OpenLDBWS.Entities;

public class ToiletAvailability
{

    [XmlElement("status")]
    public ToiletStatus Status { get; set; }


    [XmlElement("value")]
    public ToiletType Value { get; set; }
}