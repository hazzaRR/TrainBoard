using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseDeparturesBoard
{
    [XmlElement("generatedAt", Namespace = LdbNamespaces.Lt4)]
    public string GeneratedAt { get; set; } = "";

    [XmlElement("locationName", Namespace = LdbNamespaces.Lt4)]
    public string LocationName { get; set; } = "";

    [XmlElement("crs", Namespace = LdbNamespaces.Lt4)]
    public string Crs { get; set; } = "";

    [XmlElement("filterLocationName", Namespace = LdbNamespaces.Lt4)]
    public string FilterLocationName { get; set; } = "";

    [XmlElement("filtercrs", Namespace = LdbNamespaces.Lt4)]
    public string FilterCrs { get; set; } = "";

    [XmlElement("filterType")]
    public string FilterType { get; set; } = "";

    [XmlArray("nrccMessages", Namespace = LdbNamespaces.Lt4)]
    [XmlArrayItem("message", Namespace = LdbNamespaces.Lt)]
    public List<string> NrccMessages { get; set; } = [];

    [XmlElement("platformAvailable", Namespace = LdbNamespaces.Lt4)]
    public bool? PlatformAvailable { get; set; }

    [XmlElement("areServicesAvailable")]
    public bool? AreServicesAvailable { get; set; }
}