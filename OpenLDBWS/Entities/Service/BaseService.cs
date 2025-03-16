using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class BaseService
{
    [XmlArray("origin", Namespace = "http://thalesgroup.com/RTTI/2016-02-16/ldb/types")]
    [XmlArrayItem("location", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public List<ServiceLocation> Origin { get; set; } = [];

    [XmlArray("destination", Namespace = "http://thalesgroup.com/RTTI/2016-02-16/ldb/types")]
    [XmlArrayItem("location", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public List<ServiceLocation> Destination { get; set; } = [];

    [XmlArray("currentOrigins", Namespace = "http://thalesgroup.com/RTTI/2016-02-16/ldb/types")]
    [XmlArrayItem("ServiceLocation")]
    public List<ServiceLocation>? CurrentOrigins { get; set; }

    [XmlArray("currentDestinations", Namespace = "http://thalesgroup.com/RTTI/2016-02-16/ldb/types")]
    [XmlArrayItem("ServiceLocation")]
    public List<ServiceLocation>? CurrentDestinations { get; set; }

    [XmlElement("sta", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Sta { get; set; }

    [XmlElement("eta", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Eta { get; set; }

    [XmlElement("std", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Std { get; set; }

    [XmlElement("etd", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Etd { get; set; }

    [XmlElement("platform", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Platform { get; set; }

    [XmlElement("operator", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? Operator { get; set; }

    [XmlElement("operatorCode", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? OperatorCode { get; set; }

    [XmlElement("isCircularRoute")]
    public bool IsCircularRoute { get; set; }

    [XmlElement("isCancelled")]
    public bool IsCancelled { get; set; }

    [XmlElement("filterLocationCancelled")]
    public bool FilterLocationCancelled { get; set; }

    [XmlElement("serviceType", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string ServiceType { get; set; } = "";

    [XmlElement("length", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public int Length { get; set; } = 0;

    [XmlElement("detachFront")]
    public bool? DetachFront { get; set; }

    [XmlElement("isReverseFormation")]
    public bool? IsReverseFormation { get; set; }

    [XmlElement("cancelReason", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? CancelReason { get; set; }

    [XmlElement("delayReason", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string? DelayReason { get; set; }

    [XmlElement("serviceID", Namespace = "http://thalesgroup.com/RTTI/2015-11-27/ldb/types")]
    public string ServiceID { get; set; } = "";

    [XmlArray("adhocAlerts", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    [XmlArrayItem("alert")]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("uncertainty")]
    public Uncertainty? Uncertainty { get; set; }

}