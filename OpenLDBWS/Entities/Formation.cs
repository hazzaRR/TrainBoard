using System.Xml.Serialization;
namespace OpenLDBWS.Entities;
public class Formation 
{


    [XmlElement("loadingCategory", Namespace = LdbNamespaces.Lt8)]
    public LoadingCategory? LoadingCategory {get; set; }
    [XmlArray("coaches", Namespace = LdbNamespaces.Lt8)]
    [XmlArrayItem("coach", Namespace = LdbNamespaces.Lt7)]
    public List<Coach> Coaches {get; set;} = [];

}
