using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class LoadingCategory
{
    [XmlElement("code")]
    public string Code {get; set;} = "";

    [XmlElement("colour")]
    public string Colour {get; set;} = "";

    [XmlElement("image")]
    public string Image { get; set; } = "";
    

}