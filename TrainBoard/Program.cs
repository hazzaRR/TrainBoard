using RPiRgbLEDMatrix;

string std = "15:20";
string platformNumber = $"Platform {3}";
string etd = "On Time";
string destination = "London Liv St.";
string[] callingPoints = [];
string time = "15:12:20";

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

var running = true;
Console.CancelKeyPress += (_, e) =>
{
    running = false;
    e.Cancel = true;
};
while (running)
{
    int postFromEnd = 64 - platformNumber.Count;
    canvas.DrawText(font, 0, 5, new Color(255, 160, 0), text);
    canvas.DrawText(font, postFromEnd, 5, new Color(255, 160, 0), platformNumber);
    canvas.DrawText(font, 0, 11, new Color(255, 160, 0), destination);
    canvas.DrawText(font, 0, 16, new Color(255, 160, 0), callingPoints);
    canvas.DrawText(font, 27, 22, new Color(255, 160, 0), time);
    matrix.SwapOnVsync(canvas);
    Thread.Sleep(1000);
    // Thread.Yield();
}




return 0;
