using RPiRgbLEDMatrix;
using System;
using System.Device.Gpio;

namespace TrainBoard.Services;

public class RgbMatrixService : IRgbMatrixService
{
    public RGBLedMatrix Matrix {get; private set;}
    public RGBLedCanvas Canvas {get; private set;}
    public RGBLedFont Font {get; private set;}
    public int FontWidth { get; private set;}
    public int FontHeight { get; private set;}
    public bool IsInitialised {get; private set;} = false; 
    public Color StdColour {get; set;} = new Color(255, 160, 0);
    public Color PlatformColour {get; set;} = new Color(255, 160, 0);
    public Color DestinationColour {get; set;} = new Color(255, 160, 0);
    public Color CallingPointsColour {get; set;} = new Color(255, 160, 0);
    public Color CurrentTimeColour {get; set;} = new Color(255, 160, 0);
    public Color DelayColour {get; set;} = new Color(255, 15, 0);
    public Color OnTimeColour {get; set;} = new Color(0, 255, 0);

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


        string fontPath = "./fonts/4x6.bdf";

        Canvas = Matrix.CreateOffscreenCanvas();
        // Font = new RGBLedFont("../fonts/5x7.bdf");
        // FontWidth = 5;
        // FontHeight = 7;

        Font = new RGBLedFont(fontPath);
        FontWidth = 4;
        FontHeight = 6;
        // Font = new RGBLedFont("../fonts/04B_03__6pt.bdf");
        // FontWidth = 4;
        // FontHeight = 6;
        IsInitialised = true;

    }


}