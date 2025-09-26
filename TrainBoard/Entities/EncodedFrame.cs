namespace TrainBoard.Entities;

public class EncodedFrame
{
    public int[] Pixels { get; set; } = [];
    public int Delay { get; set; } = 1000;
}