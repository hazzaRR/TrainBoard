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

        using (var gpioController = new GpioController())
        {
            gpioController.OpenPin(26, PinMode.Output);
            gpioController.OpenPin(27, PinMode.Output);
            gpioController.Write(26, PinValue.High);
            gpioController.Write(27, PinValue.Low);

        }

        Canvas = Matrix.CreateOffscreenCanvas();
        // Font = new RGBLedFont("../fonts/5x7.bdf");
        // FontWidth = 5;
        // FontHeight = 7;
        Font = new RGBLedFont("../fonts/4x6.bdf");
        FontWidth = 4;
        FontHeight = 6;
        // Font = new RGBLedFont("../fonts/04B_03__6pt.bdf");
        // FontWidth = 4;
        // FontHeight = 6;
        IsInitialised = true;

    }


}