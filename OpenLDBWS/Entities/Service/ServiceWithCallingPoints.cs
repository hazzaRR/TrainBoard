using System.Xml.Serialization;

namespace OpenLDBWS.Entities;


public class ServiceWithCallingPoints : BaseService
{

    [XmlElement("subsequentCallingPoints", Namespace = LdbNamespaces.Lt8)]
    public SubsequentCallingPoints? SubsequentCallingPoints {get; set;}

    [XmlElement("previousCallingPoints", Namespace = LdbNamespaces.Lt8)]
    public PreviousCallingPoints? PreviousCallingPoints {get; set;}

}