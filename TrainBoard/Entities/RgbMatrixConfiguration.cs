using RPiRgbLEDMatrix;

namespace TrainBoard.Entities;

public class RgbMatrixConfiguration
{
    public int NumRows { get; set; } = 1;
    public string Crs { get; set; } = "COL";
    public string FilterCrs { get; set; } = "";
    public string FilterType { get; set; } = "to";
    public int TimeOffset { get; set; } = 2;
    public int TimeWindow { get; set; } = 120;
    public int StdColour { get; set; } = 16758784;
    public int PlatformColour { get; set; } = 16758784;
    public int DestinationColour { get; set; } = 16758784;
    public int CallingPointsColour { get; set; } = 16758784;
    public int CurrentTimeColour { get; set; } = 16758784;
    public int DelayColour { get; set; } = 16711680;
    public int OnTimeColour { get; set; } = 65280;
    public bool ShowCustomDisplay { get; set; } = false;
    public List<EncodedFrame> MatrixFrames { get; set; } = [];
    public int Brightness { get; set; } = 50;
}