using System.Xml.Serialization;
using OpenLDBWS.Entities;

namespace OpenLDBWS.Entities;

public class PreviousCallingPoints
{
    [XmlArray("callingPointList")]
    [XmlArrayItem(ElementName = "callingPoint")]
    public List<CallingPoint> CallingPoints {get; set;} = [];
}