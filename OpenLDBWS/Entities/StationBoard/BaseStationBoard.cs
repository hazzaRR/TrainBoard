using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseStationBoard
{
        [XmlElement(ElementName = "generatedAt", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public string GeneratedAt { get; set; } = "";

        [XmlElement(ElementName = "locationName", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public string LocationName { get; set; } = "";

        [XmlElement(ElementName = "crs", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public string Crs { get; set; } = "";

        [XmlArray(ElementName = "nrccMessages", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        [XmlArrayItem(ElementName = "message", Namespace = "http://thalesgroup.com/RTTI/2012-01-13/ldb/types")]
        public List<string> NrccMessages { get; set; } = [];

        [XmlElement(ElementName = "filterLocationName", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public string FilterLocationName { get; set; } = "";

        [XmlElement(ElementName = "filtercrs", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public string FilterCrs { get; set; } = "";

        [XmlElement(ElementName = "platformAvailable", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
        public bool PlatformAvailable { get; set; } = false;
}