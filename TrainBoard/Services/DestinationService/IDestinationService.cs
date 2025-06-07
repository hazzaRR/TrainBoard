namespace TrainBoard.Services;

public interface IDestinationService
{
    int ScrollTextPos { get; set; }
    public int DestinationWidth { get; set; }
    public bool IsDestinationScrollable { get; set; }
}