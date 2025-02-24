using Iot.Device.Graphics;
using Iot.Device.LEDMatrix;

PinMapping mapping = PinMapping.MatrixBonnetMapping32;

RGBLedMatrix matrix = new RGBLedMatrix(mapping, 64, 32);
matrix.StartRendering();

// matrix.FillRectangle(0, 0, 10, 10, 255, 0, 0);


BdfFont font = BdfFont.Load(@"fonts/4x6.bdf");

matrix.DrawText(0,1,"Hello World", font, 255, 255, 255, 0, 0, 0);
matrix.DrawText(0,10,"London Liv Street", font, 255, 255, 255, 0, 0, 0);


while(true)
{
    Thread.Sleep(1000);

}
