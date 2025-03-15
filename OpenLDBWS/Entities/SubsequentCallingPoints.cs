using System.Xml.Serialization;
using OpenLDBWS.Entities;

namespace OpenLDBWS.Entities;

public class SubsequentCallingPoints
{
    [XmlArray("callingPointList", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    [XmlArrayItem(ElementName = "callingPoint", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public List<CallingPoint> CallingPoints {get; set;} = [];
}