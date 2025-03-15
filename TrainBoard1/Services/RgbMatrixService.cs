using RPiRgbLEDMatrix;

namespace TrainBoard.Services;

public class RgbMatrixService : IRgbMatrixService
{
    public RGBLedMatrix Matrix {get; private set;}
    public RGBLedCanvas Canvas {get; private set;}
    public RGBLedFont Font {get; private set;}
    public int FontWidth { get; private set;}
    public int FontHeight { get; private set;}
    public bool IsInitialised {get; private set;} = false; 


    public RgbMatrixService() 
    {

        RGBLedMatrixOptions options = new RGBLedMatrixOptions
        {
            Rows = 32,
            Cols = 64,
            Brightness = 50,
            HardwareMapping = "adafruit-hat",
            GpioSlowdown = 5,
            ShowRefreshRate = true
        };

        Matrix = new RGBLedMatrix(options);
        Canvas = Matrix.CreateOffscreenCanvas();
        Font = new RGBLedFont("../fonts/5x7.bdf");
        FontWidth = 5;
        FontHeight = 7;
        IsInitialised = true;

    }


}