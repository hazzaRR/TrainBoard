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

    [XmlElement("isCancelled")]
    public bool? IsCancelled { get; set; }
    
    [XmlElement("length", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public int Length { get; set; }

    [XmlElement("detachFront")]
    public bool? DetachFront { get; set;}

    [XmlElement("formation", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public Formation? Formation { get; set; }

    [XmlElement("adhocAlerts")]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("cancelReason")]
    public string? CancelReason { get; set; }
    [XmlElement("delayReason")]
    public string? DelayReason { get; set; }

    [XmlElement("affectedByDiversion")]
    public string? AffectedByDiversion { get; set; }

    [XmlElement("rerouteDelay")]
    public int? RerouteDelay { get; set; }

    [XmlElement("uncertainty")]
    public Uncertainty? Uncertainty { get; set; }


}