using System.Xml.Serialization;
namespace OpenLDBWS.Entities;
public class Formation 
{


    [XmlElement("loadingCategory", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    public LoadingCategory? LoadingCategory {get; set; }
    [XmlArray("coaches", Namespace = "http://thalesgroup.com/RTTI/2021-11-01/ldb/types")]
    [XmlArrayItem("coach", Namespace = "http://thalesgroup.com/RTTI/2017-10-01/ldb/types")]
    public List<Coach> Coaches {get; set;} = [];

}
