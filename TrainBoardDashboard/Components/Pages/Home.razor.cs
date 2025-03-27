using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace TrainBoardDashboard;

public partial class Home
{
    

    private int NumRows {get; set;} = 1;
    private string Crs {get; set;} = "";
    private string FilterCrs {get; set;} = "";
    private int TimeOffset {get; set;} = 0;
    private int TimeWindow {get; set;} = 0;


    protected void UpdateMatrixConfig()
    {
        Console.WriteLine("This button was pressed");
    }

}