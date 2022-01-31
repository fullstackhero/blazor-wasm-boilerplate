using AKSoftware.Blazor.Utilities;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager;

public partial class TableCustomizationPanel
{
    [Parameter]
    public bool IsDense { get; set; }
    [Parameter]
    public bool IsStriped { get; set; }
    [Parameter]
    public bool HasBorder { get; set; }
    [Parameter]
    public bool IsHoverable { get; set; }
    private EntityTablePreference _entityTablePreference = new();
    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is not ClientPreference themePreference) themePreference = new ClientPreference();
        _entityTablePreference = themePreference.EntityTablePreference;
        IsDense = _entityTablePreference.IsDense;
        IsStriped = _entityTablePreference.IsStriped;
        HasBorder = _entityTablePreference.HasBorder;
        IsHoverable = _entityTablePreference.IsHoverable;
    }

    [Parameter]
    public EventCallback<bool> OnDenseSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnStripedSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnBorderdedSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnHoverableSwitchToggled { get; set; }

    private async Task ToggleDenseSwitch()
    {
        _entityTablePreference.IsDense = !_entityTablePreference.IsDense;
        await OnDenseSwitchToggled.InvokeAsync(_entityTablePreference.IsDense);
        MessagingCenter.Send(this, nameof(EntityTablePreference), _entityTablePreference);
    }

    private async Task ToggleStripedSwitch()
    {
        _entityTablePreference.IsStriped = !_entityTablePreference.IsStriped;
        await OnStripedSwitchToggled.InvokeAsync(_entityTablePreference.IsStriped);
        MessagingCenter.Send(this, nameof(EntityTablePreference), _entityTablePreference);
    }

    private async Task ToggleBorderedSwitch()
    {
        _entityTablePreference.HasBorder = !_entityTablePreference.HasBorder;
        await OnBorderdedSwitchToggled.InvokeAsync(_entityTablePreference.HasBorder);
        MessagingCenter.Send(this, nameof(EntityTablePreference), _entityTablePreference);
    }

    private async Task ToggleHoverableSwitch()
    {
        _entityTablePreference.IsHoverable = !_entityTablePreference.IsHoverable;
        await OnHoverableSwitchToggled.InvokeAsync(_entityTablePreference.IsHoverable);
        MessagingCenter.Send(this, nameof(EntityTablePreference), _entityTablePreference);
    }
}