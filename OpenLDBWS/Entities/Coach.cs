using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot("coach", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
public class Coach
{
    [XmlAttribute("number", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public string? Number { get; set; }

    [XmlElement("coachClass", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public string? CoachClass { get; set; }

    [XmlElement("loading", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public int? Loading { get; set; }

    [XmlElement("loadingSpecified", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public bool LoadingSpecified { get; set; }

    [XmlElement("toilet", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public ToiletAvailability? Toilet { get; set; }
}