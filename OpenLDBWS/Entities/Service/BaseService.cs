using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseService
{
    [XmlArray("origin", Namespace = LdbNamespaces.Lt5)]
    [XmlArrayItem("location", Namespace = LdbNamespaces.Lt4)]
    public List<ServiceLocation> Origin { get; set; } = [];

    [XmlArray("destination", Namespace = LdbNamespaces.Lt5)]
    [XmlArrayItem("location", Namespace = LdbNamespaces.Lt4)]
    public List<ServiceLocation> Destination { get; set; } = [];

    [XmlArray("currentOrigins", Namespace = LdbNamespaces.Lt5)]
    [XmlArrayItem("ServiceLocation")]
    public List<ServiceLocation>? CurrentOrigins { get; set; }

    [XmlArray("currentDestinations", Namespace = LdbNamespaces.Lt5)]
    [XmlArrayItem("ServiceLocation")]
    public List<ServiceLocation>? CurrentDestinations { get; set; }

    [XmlElement("sta", Namespace = LdbNamespaces.Lt4)]
    public string? Sta { get; set; }

    [XmlElement("eta", Namespace = LdbNamespaces.Lt4)]
    public string? Eta { get; set; }

    [XmlElement("std", Namespace = LdbNamespaces.Lt4)]
    public string? Std { get; set; }

    [XmlElement("etd", Namespace = LdbNamespaces.Lt4)]
    public string? Etd { get; set; }

    [XmlElement("platform", Namespace = LdbNamespaces.Lt4)]
    public string? Platform { get; set; }

    [XmlElement("operator", Namespace = LdbNamespaces.Lt4)]
    public string? Operator { get; set; }

    [XmlElement("operatorCode", Namespace = LdbNamespaces.Lt4)]
    public string? OperatorCode { get; set; }

    [XmlElement("isCircularRoute", Namespace = LdbNamespaces.Lt4)]
    public bool IsCircularRoute { get; set; }

    [XmlElement("isCancelled", Namespace = LdbNamespaces.Lt4)]
    public bool IsCancelled { get; set; }

    [XmlElement("filterLocationCancelled", Namespace = LdbNamespaces.Lt4)]
    public bool FilterLocationCancelled { get; set; }

    [XmlElement("serviceType", Namespace = LdbNamespaces.Lt4)]
    public string ServiceType { get; set; } = "";

    [XmlElement("length", Namespace = LdbNamespaces.Lt4)]
    public int Length { get; set; } = 0;

    [XmlElement("detachFront")]
    public bool? DetachFront { get; set; }

    [XmlElement("isReverseFormation")]
    public bool? IsReverseFormation { get; set; }

    [XmlElement("cancelReason", Namespace = LdbNamespaces.Lt4)]
    public string? CancelReason { get; set; }

    [XmlElement("delayReason", Namespace = LdbNamespaces.Lt4)]
    public string? DelayReason { get; set; }

    [XmlElement("serviceID", Namespace = LdbNamespaces.Lt4)]
    public string ServiceID { get; set; } = "";

    [XmlArray("adhocAlerts", Namespace = LdbNamespaces.Lt8)]
    [XmlArrayItem("alert")]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("uncertainty")]
    public Uncertainty? Uncertainty { get; set; }

}