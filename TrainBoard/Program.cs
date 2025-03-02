using RPiRgbLEDMatrix;

string text = "Hello World!";


RGBLedMatrixOptions options = new RGBLedMatrixOptions
{
    Rows = 32,
    Cols = 64,
    Brightness = 50,
    HardwareMapping = "adafruit-hat",
    GpioSlowdown = 3,
    ShowRefreshRate = true
};

using var matrix = new RGBLedMatrix(options);
var canvas = matrix.CreateOffscreenCanvas();
using var font = new RGBLedFont("../fonts/4x6.bdf");

canvas.DrawText(font, 1, 6, new Color(0, 255, 0), text);
matrix.SwapOnVsync(canvas);

// run until user presses Ctrl+C
var running = true;
Console.CancelKeyPress += (_, e) =>
{
    running = false;
    e.Cancel = true; // do not terminate program with Ctrl+C, we need to dispose
};
while (running) Thread.Yield();

return 0;
