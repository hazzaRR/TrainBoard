using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetStationBoardResult")]
public class StationBoard : BaseStationBoard
{
        [XmlArray(ElementName = "trainServices")]
        [XmlArrayItem(ElementName = "service")]
        public List<Service> TrainServices { get; set; } = [];

        [XmlArray(ElementName = "busServices")]
        [XmlArrayItem(ElementName = "service")]
        public List<Service> BusServices { get; set; } = [];

        [XmlArray(ElementName = "ferryServices")]
        [XmlArrayItem(ElementName = "service")]
        public List<Service> FerryServices { get; set; } = [];
}