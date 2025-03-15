using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class DeparturesBoard : BaseDeparturesBoard
{

    [XmlArray("departures")]
    [XmlArrayItem("destination")]
    public List<Departure> Departures { get; set; } = [];

}