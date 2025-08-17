using Microsoft.AspNetCore.Components;
namespace TrainBoardDashboard;

public partial class MatrixDisplay: IAsyncDisposable
{

    [Inject]
    private ILogger<Home> Logger { get; set; }
    
    // [Parameter]
    public string[,] MatrixPixels { get; set; }
    public string SelectedColor { get; set; } = "#ff0000";

    protected override async Task OnInitializedAsync()
    {

        MatrixPixels = new string[32, 64];
        ClearMatrix();
        await base.OnInitializedAsync();
    }

    protected void SetPixelColor(int row, int col)
    {
        MatrixPixels[row, col] = SelectedColor;
        StateHasChanged();
    }

    protected void ClearMatrix()
    {
        for (int i = 0; i < MatrixPixels.GetLength(0); i++)
        {
            for (int j = 0; j < MatrixPixels.GetLength(1); j++)
            {
                MatrixPixels[i, j] = "#000000";
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {

        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }
}