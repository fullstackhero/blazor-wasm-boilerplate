using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Roles;

public partial class RoleModal
{
    [Parameter]
    public RoleRequest RoleModel { get; set; } = new();

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        var response = await RolesClient.RegisterRoleAsync(RoleModel);
        if (response.Succeeded)
        {
            if (response.Messages?.Count > 0)
            {
                _snackBar.Add(response.Messages.First(), Severity.Success);
            }

            MudDialog.Close();
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}