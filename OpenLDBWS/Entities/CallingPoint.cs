using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


[XmlRoot("callingPoint", Namespace = LdbNamespaces.Lt8)]
public class CallingPoint
{

    [XmlElement("locationName", Namespace = LdbNamespaces.Lt8)]
    public string LocationName { get; set; } = "";

    [XmlElement("crs", Namespace = LdbNamespaces.Lt8)]
    public string Crs { get; set; } = "";

    [XmlElement("st", Namespace = LdbNamespaces.Lt8)]
    public string St { get; set; } = "";

    [XmlElement("et", Namespace = LdbNamespaces.Lt8)]
    public string Et { get; set; } = "";

    [XmlElement("at", Namespace = LdbNamespaces.Lt8)]
    public string At { get; set; } = "";

    [XmlElement("isCancelled", Namespace = LdbNamespaces.Lt8)]
    public bool? IsCancelled { get; set; }
    
    [XmlElement("length", Namespace = LdbNamespaces.Lt8)]
    public int Length { get; set; }

    [XmlElement("detachFront", Namespace = LdbNamespaces.Lt8)]
    public bool? DetachFront { get; set;}

    [XmlElement("formation", Namespace = LdbNamespaces.Lt8)]
    public Formation? Formation { get; set; }

    [XmlElement("adhocAlerts", Namespace = LdbNamespaces.Lt8)]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("cancelReason", Namespace = LdbNamespaces.Lt8)]
    public string? CancelReason { get; set; }
    [XmlElement("delayReason", Namespace = LdbNamespaces.Lt8)]
    public string? DelayReason { get; set; }

    [XmlElement("affectedByDiversion", Namespace = LdbNamespaces.Lt8)]
    public string? AffectedByDiversion { get; set; }

    [XmlElement("rerouteDelay", Namespace = LdbNamespaces.Lt8)]
    public int? RerouteDelay { get; set; }

    [XmlElement("uncertainty", Namespace = LdbNamespaces.Lt8)]
    public Uncertainty? Uncertainty { get; set; }


}