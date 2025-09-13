using Tmds.DBus.Protocol;

namespace TrainBoard.Entities;

public class AvailableNetwork
{
    public string Ssid { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public int Signal { get; set; }
    public string Security { get; set; } = string.Empty;
    public bool IsSaved { get; set; }
    public bool IsActive { get; set; }
    public ObjectPath? ApPath { get; set; }
    public ObjectPath? ConnPath { get; set; }

}