using RPiRgbLEDMatrix;

namespace TrainBoard.Services;

public interface IRgbMatrixService
{
    RGBLedMatrix Matrix {get;}
    RGBLedCanvas Canvas {get;}
    RGBLedFont Font {get;}
    bool IsInitialised {get;}
}

