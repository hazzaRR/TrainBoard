namespace TrainBoard.Services;

public class DestinationService : IDestinationService
{
    public int ScrollTextPos { get; set; } = 0;
    public int DestinationWidth { get; set; }
    public bool IsDestinationScrollable { get; set; } = false;
}