using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


public class ServiceWithCallingPoints : BaseService
{

    [XmlElement("subsequentCallingPoints", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public SubsequentCallingPoints? SubsequentCallingPoints {get; set;}

    [XmlElement("previousCallingPoints", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public PreviousCallingPoints? PreviousCallingPoints {get; set;}

}