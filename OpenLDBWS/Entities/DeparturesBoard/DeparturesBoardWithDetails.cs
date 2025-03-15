using System.Xml.Serialization;

namespace OpenLDBWS.Entities;

public class DeparturesBoardWithDetails : BaseDeparturesBoard
{

    [XmlArray("departures")]
    [XmlArrayItem("destination")]
    public List<DepartureWithDetails> Departures { get; set; } = [];

}