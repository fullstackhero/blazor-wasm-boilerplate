using AKSoftware.Blazor.Utilities;
using FSH.BlazorWebAssembly.Client.Components.ThemeManager;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Common;

public class FshTable<T> : MudTable<T>
{
    [Inject]
    private IClientPreferenceManager ClientPreferences { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            SetTablePreference(clientPreference.EntityTablePreference);
        }

        MessagingCenter.Subscribe<TableCustomizationPanel, EntityTablePreference>(this, nameof(ClientPreference.EntityTablePreference), (_, value) =>
        {
            SetTablePreference(value);
            StateHasChanged();
        });

        await base.OnInitializedAsync();
    }

    private void SetTablePreference(EntityTablePreference tablePreference)
    {
        Dense = tablePreference.IsDense;
        Striped = tablePreference.IsStriped;
        Bordered = tablePreference.HasBorder;
        Hover = tablePreference.IsHoverable;
    }
}