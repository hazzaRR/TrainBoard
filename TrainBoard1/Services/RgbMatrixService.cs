using RPiRgbLEDMatrix;

namespace TrainBoard.Services;

public class RgbMatrixService : IRgbMatrixService
{
    public RGBLedMatrix Matrix {get; private set;}
    public RGBLedCanvas Canvas {get; private set;}
    public RGBLedFont Font {get; private set;}
    public bool IsInitialised {get; private set;} = false; 


    public RgbMatrixService() 
    {

        RGBLedMatrixOptions options = new RGBLedMatrixOptions
        {
            Rows = 32,
            Cols = 64,
            Brightness = 50,
            HardwareMapping = "adafruit-hat",
            GpioSlowdown = 3,
            ShowRefreshRate = true
        };

        Matrix = new RGBLedMatrix(options);
        Canvas = Matrix.CreateOffscreenCanvas();
        Font = new RGBLedFont("../fonts/4x6.bdf");
        IsInitialised = true;

    }


}