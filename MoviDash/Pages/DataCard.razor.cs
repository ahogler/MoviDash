using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MoviDash.Pages;

public partial class DataCard : MudComponentBase
{
    private string Classname =>
        new CssBuilder()
            .AddClass(Class)
            .Build();

    [EditorRequired] [Parameter] public string Title { get; set; } = string.Empty;
    [EditorRequired] [Parameter] public string ChartId { get; set; } = string.Empty;
    [Parameter] public string BarColor { get; set; } = "var(--mud-palette-primary)";
    [EditorRequired] [Parameter] public double IncreaseDecrease { get; set; }
    [EditorRequired] [Parameter] public int Total { get; set; }
    [EditorRequired] [Parameter] public List<int> LastTenDays { get; set; } = new();

    protected override void OnInitialized()
    {
    }
}