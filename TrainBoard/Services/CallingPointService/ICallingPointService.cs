namespace TrainBoard.Services;

public interface ICallingPointService
{
    int ScrollTextPos { get; set; }
    int PixelsDrawn { get; set; }
    bool IsScrollComplete { get; set; }
    public bool showDelayReason {get; set;}
}