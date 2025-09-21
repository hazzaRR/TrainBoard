using RPiRgbLEDMatrix;
using System;
using System.Device.Gpio;
using TrainBoard.Entities;
using TrainBoard.Utilities;

namespace TrainBoard.Services;

public class RgbMatrixService : IRgbMatrixService
{
    public RGBLedMatrix Matrix { get; private set; }
    public RGBLedCanvas Canvas { get; private set; }
    public RGBLedFont Font { get; private set; }
    public int FontWidth { get; private set; }
    public int FontHeight { get; private set; }
    public bool IsInitialised { get; private set; } = false;
    public bool IsInPairingMode { get; set; } = false;
    public bool IsApiKeyInvalid { get; set; } = false;
    public Color StdColour { get; set; } = new Color(255, 160, 0);
    public Color PlatformColour { get; set; } = new Color(255, 160, 0);
    public Color DestinationColour { get; set; } = new Color(255, 160, 0);
    public Color CallingPointsColour { get; set; } = new Color(255, 160, 0);
    public Color CurrentTimeColour { get; set; } = new Color(255, 160, 0);
    public Color DelayColour { get; set; } = new Color(255, 15, 0);
    public Color OnTimeColour { get; set; } = new Color(0, 255, 0);
    public bool ShowCustomDisplay { get; set; } = false;
    public Color[] MatrixPixels { get; set; } = new Color[32 * 64];

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

    public void SetUserOptions(RgbMatrixConfiguration config)
    {
        StdColour = ColourConverter.IntToRgb(config.StdColour);
        PlatformColour = ColourConverter.IntToRgb(config.PlatformColour);
        DestinationColour = ColourConverter.IntToRgb(config.DestinationColour);
        CallingPointsColour = ColourConverter.IntToRgb(config.CallingPointsColour);
        CurrentTimeColour = ColourConverter.IntToRgb(config.CurrentTimeColour);
        DelayColour = ColourConverter.IntToRgb(config.DelayColour);
        OnTimeColour = ColourConverter.IntToRgb(config.OnTimeColour);
        ShowCustomDisplay = config.ShowCustomDisplay;
        MatrixPixels = Flattern2dColourMatrix(config.MatrixPixels);
    }
    private Color[] Flattern2dColourMatrix(int[][]? colourMatrix)
    {
        int rows = 32;
        int cols = 64;

        if (colourMatrix == null || colourMatrix.Length == 0)
        {
            colourMatrix = new int[rows][];
        }

        Color[] colourArray = new Color[rows * cols];

        int pixel = 0;

        for (int row = 0; row < rows; row++)
        {
            if (colourMatrix[row] == null || colourMatrix[row].Length != cols)
            {
                colourMatrix[row] = new int[cols];
            }
            for (int col = 0; col < cols; col++)
            {
                colourArray[pixel] = ColourConverter.IntToRgb(colourMatrix[row][col]);
                pixel++;
            }
        }

        return colourArray;
    }


}