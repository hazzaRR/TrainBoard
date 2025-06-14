using System.Text.Json.Serialization;

namespace TrainBoardDashboard.Entities;

public class Station
{

    [JsonPropertyName("crs")]
    public required string Crs { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }

}