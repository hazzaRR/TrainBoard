using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TrainBoardDashboard.Entities;
using TrainBoardDashboard.Utilities;
namespace TrainBoardDashboard;

public partial class MatrixDisplay: IAsyncDisposable
{

    [Inject]
    private ILogger<Home> Logger { get; set; }
    
    [Parameter]
    public int[][] MatrixPixels { get; set; }
    
    [Parameter]
    public EventCallback<int[][]> MatrixPixelsChanged { get; set; }
    public string SelectedColor { get; set; } = "#ff0000";

    protected override async Task OnInitializedAsync()
    {

        // MatrixPixels = new string[32, 64];
        // ClearMatrix();
        await base.OnInitializedAsync();
    }

    protected void SetPixelColor(int row, int col)
    {
        MatrixPixels[row][col] = ColourConverter.HexToInt(SelectedColor);
        StateHasChanged();
        MatrixPixelsChanged.InvokeAsync(MatrixPixels);

    }

    protected void ResetPixelColor(int row, int col)
    {
        MatrixPixels[row][col] = 0;
        StateHasChanged();
        MatrixPixelsChanged.InvokeAsync(MatrixPixels);
    }

    protected void ClearMatrix()
    {
        for (int i = 0; i < MatrixPixels.Length; i++)
        {
            for (int j = 0; j < MatrixPixels[i].Length; j++)
            {
                MatrixPixels[i][j] = 0;
            }
        }
        MatrixPixelsChanged.InvokeAsync(MatrixPixels);
    }
    protected void DragDraw(MouseEventArgs e, int row, int col)
    {
        if (e.ShiftKey)
        {
            SetPixelColor(row, col);
            MatrixPixelsChanged.InvokeAsync(MatrixPixels);
        }
    }
    public async ValueTask DisposeAsync()
    {

        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }
}