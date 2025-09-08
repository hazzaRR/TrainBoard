using RPiRgbLEDMatrix;
using TrainBoard.Entities;

namespace TrainBoard.Services;

public interface IRgbMatrixService
{
    RGBLedMatrix Matrix { get; }
    RGBLedCanvas Canvas { get; }
    RGBLedFont Font { get; }
    int FontWidth { get; }
    int FontHeight { get; }
    bool IsInitialised { get; }
    bool IsInParingMode { get; set; }
    Color StdColour { get; set; }
    Color PlatformColour { get; set; }
    Color DestinationColour { get; set; }
    Color CallingPointsColour { get; set; }
    Color CurrentTimeColour { get; set; }
    Color DelayColour { get; set; }
    Color OnTimeColour { get; set; }
    bool ShowCustomDisplay { get; set; }
    Color[] MatrixPixels { get; set; }
    void SetUserOptions(RgbMatrixConfiguration config);
}

