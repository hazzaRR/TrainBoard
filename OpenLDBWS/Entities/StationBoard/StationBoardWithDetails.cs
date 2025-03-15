using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot(ElementName = "GetStationBoardResult")]
public class StationBoardWithDetails : BaseStationBoard
{
        [XmlArray(ElementName = "trainServices", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        [XmlArrayItem(ElementName = "service" , Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        public List<ServiceWithCallingPoints> TrainServices { get; set; } = [];

        [XmlArray(ElementName = "busServices", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        [XmlArrayItem(ElementName = "service", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        public List<ServiceWithCallingPoints> BusServices { get; set; } = [];

        [XmlArray(ElementName = "ferryServices", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        [XmlArrayItem(ElementName = "service", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
        public List<ServiceWithCallingPoints> FerryServices { get; set; } = [];
}