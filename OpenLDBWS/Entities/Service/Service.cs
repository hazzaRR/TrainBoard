using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class Service : BaseService
{

    [XmlElement("rsid")]
    public string? Rsid { get; set; }

    [XmlElement("formation", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public required Formation Formation {get; set;}

    [XmlElement("futureCancellation")]
    public bool FutureCancellation { get; set; }

    [XmlElement("futureDelay")]
    public bool FutureDelay { get; set; }

    [XmlElement("diversion")]
    public ServiceDiversion? Diversion { get; set; }

}
