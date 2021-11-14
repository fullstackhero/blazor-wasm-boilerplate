using FSH.BlazorWebAssembly.Client.Infrastructure.Preference;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager
{
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
            if (await _clientPreferenceManager.GetPreference() is not ClientPreference _themePreference) _themePreference = new ClientPreference();
            Radius = _themePreference.BorderRadius;
        }
        private async Task ChangedSelection(ChangeEventArgs args)
        {
            Radius = int.Parse(args?.Value?.ToString() ?? "0");
            await OnSliderChanged.InvokeAsync(Radius);
        }
    }
}