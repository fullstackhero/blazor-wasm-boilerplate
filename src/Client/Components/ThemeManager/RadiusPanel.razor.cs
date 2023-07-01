using FL_CRMS_ERP_WASM.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;

namespace FL_CRMS_ERP_WASM.Client.Components.ThemeManager;

public partial class RadiusPanel
{
    [Parameter]
    public double Radius { get; set; }

    [Parameter]
    public double MaxValue { get; set; } = 30;

    [Parameter]
    public EventCallback<double> OnSliderChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is not ClientPreference themePreference) themePreference = new ClientPreference();
        Radius = themePreference.BorderRadius;
    }

    private async Task ChangedSelection(ChangeEventArgs args)
    {
        Radius = int.Parse(args?.Value?.ToString() ?? "0");
        await OnSliderChanged.InvokeAsync(Radius);
    }
}