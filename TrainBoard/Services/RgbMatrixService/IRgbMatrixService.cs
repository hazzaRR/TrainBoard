using RPiRgbLEDMatrix;

namespace TrainBoard.Services;

public interface IRgbMatrixService
{
    RGBLedMatrix Matrix {get;}
    RGBLedCanvas Canvas {get;}
    RGBLedFont Font {get;}
    int FontWidth {get;}
    int FontHeight {get;}
    bool IsInitialised {get;}
    Color StdColour {get; set;} 
    Color PlatformColour {get; set;} 
    Color DestinationColour {get; set;} 
    Color CallingPointsColour {get; set;} 
    Color CurrentTimeColour {get; set;} 
    Color DelayColour {get; set;}
    Color OnTimeColour {get; set;}
}

