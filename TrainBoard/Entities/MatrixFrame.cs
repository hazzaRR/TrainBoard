using RPiRgbLEDMatrix;

namespace TrainBoard.Entities;

public class MatrixFrame
{
    public Color[] Pixels { get; set; } = [];
    public int Delay { get; set; } = 1000;
}