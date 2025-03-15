using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseDeparturesBoard
{
    [XmlElement("generatedAt", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string GeneratedAt { get; set; } = "";

    [XmlElement("locationName", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string LocationName { get; set; } = "";

    [XmlElement("crs", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string Crs { get; set; } = "";

    [XmlElement("filterLocationName", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string FilterLocationName { get; set; } = "";

    [XmlElement("filtercrs", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string FilterCrs { get; set; } = "";

    [XmlElement("filterType")]
    public string FilterType { get; set; } = "";

    [XmlArray("nrccMessages", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    [XmlArrayItem("message")]
    public List<string> NrccMessages { get; set; } = [];

    [XmlElement("platformAvailable", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public bool? PlatformAvailable { get; set; }

    [XmlElement("areServicesAvailable")]
    public bool? AreServicesAvailable { get; set; }
}