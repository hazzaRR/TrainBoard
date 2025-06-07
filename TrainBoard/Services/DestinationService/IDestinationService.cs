namespace TrainBoard.Services;

public interface IDestinationService
{
    int ScrollTextPos { get; set; }
    public int DestinationWidthInPixels { get; set; }
    public bool IsDestinationScrollable { get; set; }
}