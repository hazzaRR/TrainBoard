using Tmds.DBus.Protocol;

namespace TrainBoard.Entities;

public class AvailableNetworkDto
{
    public string Ssid { get; set; } = string.Empty;
    public string Bars { get; set; } = string.Empty;
    public bool IsSaved { get; set; }
    public bool IsActive { get; set; }

}