using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
namespace TrainBoardDashboard;

public partial class MatrixDisplay: IAsyncDisposable
{

    [Inject]
    private ILogger<Home> Logger { get; set; }
    
    [Parameter]
    public string[,] MatrixPixels { get; set; }
    
    [Parameter]
    public EventCallback<string[,]> MatrixPixelsChanged { get; set; }
    public string SelectedColor { get; set; } = "#ff0000ff";

    protected override async Task OnInitializedAsync()
    {

        // MatrixPixels = new string[32, 64];
        // ClearMatrix();
        await base.OnInitializedAsync();
    }

    protected void SetPixelColor(int row, int col)
    {
        MatrixPixels[row, col] = SelectedColor;
        StateHasChanged();
        MatrixPixelsChanged.InvokeAsync(MatrixPixels);

    }

    protected void ResetPixelColor(int row, int col)
    {
        MatrixPixels[row, col] = "#000000";
        StateHasChanged();
        MatrixPixelsChanged.InvokeAsync(MatrixPixels);
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