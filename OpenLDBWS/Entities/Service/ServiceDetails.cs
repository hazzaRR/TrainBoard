using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class ServiceDetails
{

    [XmlElement("generatedAt")]
    public string GeneratedAt { get; set;} = "";

    [XmlElement("rsid")]
    public string Rsid { get; set; } = "";

    [XmlElement("serviceType")]
    public string ServiceType { get; set; } = "";

    [XmlElement("locationName")]
    public string LocationName { get; set; } = "";

    [XmlElement("crs")]
    public string Crs { get; set; } = "";

    [XmlElement("operator")]
    public string? Operator { get; set; }

    [XmlElement("operatorCode")]
    public string? OperatorCode { get; set; }

    [XmlElement("isCancelled")]
    public bool IsCancelled { get; set; }

    [XmlElement("cancelReason")]
    public string? CancelReason { get; set; }

    [XmlElement("delayReason")]
    public string? DelayReason { get; set; }

    [XmlElement("detachFront")]
    public bool DetachFront { get; set; }

    [XmlElement("diversionReason")]
    public string? DiversionReason { get; set; }

    [XmlElement("divertedVia")]
    public string? DivertedVia { get; set; }

    [XmlElement("overdueMessage")]
    public string? OverdueMessage { get; set; }

    [XmlElement("length")]
    public int? Length { get; set; }

    [XmlElement("isReverseFormation")]
    public bool IsReverseFormation { get; set; }

    [XmlElement("platform")]
    public string? Platform { get; set; }

    [XmlElement("sta")]
    public string? Sta { get; set; }

    [XmlElement("eta")]
    public string? Eta { get; set; }

    [XmlElement("ata")]
    public string? Ata { get; set; }

    [XmlElement("std")]
    public string? Std { get; set; }

    [XmlElement("etd")]
    public string? Etd { get; set; }

    [XmlElement("atd")]
    public string? Atd { get; set; }

    [XmlElement("serviceID")]
    public string ServiceID { get; set; } = "";

    [XmlArray("adhocAlerts")]
    [XmlArrayItem("alert")]
    public List<string> AdhocAlerts { get; set; } = [];

    [XmlElement("subsequentCallingPoints")]
    public SubsequentCallingPoints? SubsequentCallingPoints {get; set;}

    [XmlElement("previousCallingPoints")]
    public PreviousCallingPoints? PreviousCallingPoints {get; set;}

    [XmlElement("formation")]
    public required Formation Formation {get; set;}

}