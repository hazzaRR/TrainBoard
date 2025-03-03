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

int scrollTextPos = canvas.Width;

void ScrollText(RGBLedCanvas canvas, RGBLedMatrix matrix)
{
    canvas.SetPixels(0, 6, canvas.Width, 6, new Color(0,0,0));

    int pixelsDrawn = canvas.DrawText(font, scrollTextPos, 16, new Color(255, 160, 0), callingPoints);

    scrollTextPos -= 1;

    if (scrollTextPos + pixelsDrawn < 0)
    {
        scrollTextPos = canvas.Width;
    }

    Thread.Sleep(1000);
}


while (running)
{
    int postFromEnd = canvas.Width - platformNumber.Count;
    int timeStartingPos = (canvas.Width - time.Count) / 2;

    canvas.DrawText(font, 0, 5, new Color(255, 160, 0), text);
    canvas.DrawText(font, postFromEnd, 5, new Color(255, 160, 0), platformNumber);
    canvas.DrawText(font, 0, 11, new Color(255, 160, 0), destination);
    ScrollText(canvas, matrix);
    canvas.DrawText(font, timeStartingPos, 22, new Color(255, 160, 0), time);
    matrix.SwapOnVsync(canvas);
    Thread.Sleep(1000);
    // Thread.Yield();
}




return 0;
