namespace TrainBoard.Entities;

public class RgbMatrixConfiguration
{
    public int NumRows {get; set;} = 1;
    public string Crs {get; set;} = "COL";
    public string FilterCrs {get; set;} = "";
    public int TimeOffset {get; set;} = 0;
    public int? TimeWindow {get; set;} = 120;
}