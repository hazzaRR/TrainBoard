namespace TrainBoard.Services;

public class CallingPointService : ICallingPointService
{
    public int ScrollTextPos {get; set;}
    public int PixelsDrawn {get; set;}
    public bool IsScrollComplete {get; set;} = false;
    public bool showDelayReason {get; set;} = false;
}