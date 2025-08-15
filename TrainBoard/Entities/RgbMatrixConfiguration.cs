namespace TrainBoard.Entities;

public class RgbMatrixConfiguration
{
    public int NumRows {get; set;} = 1;
    public string Crs {get; set;} = "COL";
    public string FilterCrs {get; set;} = "";
    public string FilterType  {get; set;} = "to";
    public int TimeOffset {get; set;} = 2;
    public int TimeWindow {get; set;} = 120;
    public string StdColour { get; set; } = "#ffa000";
    public string DestinationColour { get; set; } = "#ffa000";
    public string CallingPointsColour { get; set; } = "#ffa000";
    public string CurrentTimeColour { get; set; } = "#ffa000";
    public string DelayColour { get; set; } = "#ff0f00";
    public string OnTimeColour { get; set; } = "#00ff00";
}