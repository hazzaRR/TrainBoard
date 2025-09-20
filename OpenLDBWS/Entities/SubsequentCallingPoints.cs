using System.Xml.Serialization;
using OpenLDBWS.Entities;

namespace OpenLDBWS.Entities;

public class SubsequentCallingPoints
{
    [XmlArray("callingPointList", Namespace = LdbNamespaces.Lt8)]
    [XmlArrayItem(ElementName = "callingPoint", Namespace = LdbNamespaces.Lt8)]
    public List<CallingPoint> CallingPoints {get; set;} = [];
}