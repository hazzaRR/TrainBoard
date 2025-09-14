using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot("coach", Namespace = LdbNamespaces.Lt8)]
public class Coach
{
    [XmlAttribute("number", Namespace = LdbNamespaces.Lt7)]
    public string? Number { get; set; }

    [XmlElement("coachClass", Namespace = LdbNamespaces.Lt7)]
    public string? CoachClass { get; set; }

    [XmlElement("loading", Namespace = LdbNamespaces.Lt7)]
    public int? Loading { get; set; }

    [XmlElement("loadingSpecified", Namespace = LdbNamespaces.Lt7)]
    public bool LoadingSpecified { get; set; }

    [XmlElement("toilet", Namespace = LdbNamespaces.Lt7)]
    public ToiletAvailability? Toilet { get; set; }
}