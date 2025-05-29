namespace TrainBoardDashboard.Entities;

public class RgbMatrixConfiguration
{
    public int NumRows {get; set;} = 1;
    public string Crs {get; set;} = "COL";
    public string FilterCrs {get; set;} = "";
    public string FilterType  {get; set;} = "to";
    public int TimeOffset { get; set; } = 0;
    public int TimeWindow {get; set;} = 120;
}