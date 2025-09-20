using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class Service : BaseService
{

    [XmlElement("rsid", Namespace = LdbNamespaces.Lt5)]
    public string? Rsid { get; set; }

    [XmlElement("formation", Namespace = LdbNamespaces.Lt8)]
    public required Formation Formation {get; set;}

    [XmlElement("futureCancellation")]
    public bool FutureCancellation { get; set; }

    [XmlElement("futureDelay")]
    public bool FutureDelay { get; set; }

    [XmlElement("diversion")]
    public ServiceDiversion? Diversion { get; set; }

}
