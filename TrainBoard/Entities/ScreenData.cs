namespace TrainBoard.Entities;

public class ScreenData
{
    public string? Std { get; set;}
    public string? Platform { get; set;}
    public string? Etd { get; set;}
    public string? Destination { get; set;}
    public string? CallingPoints { get; set;}
    public bool? IsCancelled {get; set;} = false;
    public bool? IsBusReplacement {get; set;} = false;
    public string CancelReason { get; set; } = "";
    public string DelayReason { get; set; } = "";
    public bool NoServices {get; set;} = false;
}