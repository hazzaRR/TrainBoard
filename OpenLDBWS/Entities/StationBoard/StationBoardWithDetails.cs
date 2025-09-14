using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetStationBoardResult", Namespace = LdbNamespaces.Ldb)]
public class StationBoardWithDetails : BaseStationBoard
{
        [XmlArray(ElementName = "trainServices", Namespace = LdbNamespaces.Lt8)]
        [XmlArrayItem(ElementName = "service" , Namespace = LdbNamespaces.Lt8)]
        public List<ServiceWithCallingPoints> TrainServices { get; set; } = [];

        [XmlArray(ElementName = "busServices", Namespace = LdbNamespaces.Lt8)]
        [XmlArrayItem(ElementName = "service", Namespace = LdbNamespaces.Lt8)]
        public List<ServiceWithCallingPoints> BusServices { get; set; } = [];

        [XmlArray(ElementName = "ferryServices", Namespace = LdbNamespaces.Lt8)]
        [XmlArrayItem(ElementName = "service", Namespace = LdbNamespaces.Lt8)]
        public List<ServiceWithCallingPoints> FerryServices { get; set; } = [];
}