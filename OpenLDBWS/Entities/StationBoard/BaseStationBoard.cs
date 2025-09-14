using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseStationBoard
{
        [XmlElement(ElementName = "generatedAt", Namespace = LdbNamespaces.Lt4)]
        public string GeneratedAt { get; set; } = "";

        [XmlElement(ElementName = "locationName", Namespace = LdbNamespaces.Lt4)]
        public string LocationName { get; set; } = "";

        [XmlElement(ElementName = "crs", Namespace = LdbNamespaces.Lt4)]
        public string Crs { get; set; } = "";

        [XmlArray(ElementName = "nrccMessages", Namespace = LdbNamespaces.Lt4)]
        [XmlArrayItem(ElementName = "message", Namespace = LdbNamespaces.Lt)]
        public List<string> NrccMessages { get; set; } = [];

        [XmlElement(ElementName = "filterLocationName", Namespace = LdbNamespaces.Lt4)]
        public string FilterLocationName { get; set; } = "";

        [XmlElement(ElementName = "filtercrs", Namespace = LdbNamespaces.Lt4)]
        public string FilterCrs { get; set; } = "";

        [XmlElement(ElementName = "platformAvailable", Namespace = LdbNamespaces.Lt4)]
        public bool PlatformAvailable { get; set; } = false;
}