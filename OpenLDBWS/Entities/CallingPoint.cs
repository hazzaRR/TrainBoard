using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot("callingPoint", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
public class CallingPoint
{

    [XmlElement("locationName", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string LocationName { get; set; } = "";

    [XmlElement("crs", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string Crs { get; set; } = "";

    [XmlElement("st", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string St { get; set; } = "";

    [XmlElement("et", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string Et { get; set; } = "";

    [XmlElement("at", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string At { get; set; } = "";

    [XmlElement("isCancelled", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public bool? IsCancelled { get; set; }
    
    [XmlElement("length", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public int Length { get; set; }

    [XmlElement("detachFront", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public bool? DetachFront { get; set;}

    [XmlElement("formation", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public Formation? Formation { get; set; }

    [XmlElement("adhocAlerts", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("cancelReason", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string? CancelReason { get; set; }
    [XmlElement("delayReason", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string? DelayReason { get; set; }

    [XmlElement("affectedByDiversion", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public string? AffectedByDiversion { get; set; }

    [XmlElement("rerouteDelay", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public int? RerouteDelay { get; set; }

    [XmlElement("uncertainty", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public Uncertainty? Uncertainty { get; set; }


}