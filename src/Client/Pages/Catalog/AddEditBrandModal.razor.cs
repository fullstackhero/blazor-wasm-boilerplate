using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public partial class AddEditBrandModal
{
    [Parameter]
    public UpdateBrandRequest UpdateBrandRequest { get; set; } = new();

    [Parameter]
    public bool IsCreate { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    public IBrandsClient _brandsClient { get; set; } = default!;

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        ResultOfGuid response;
        if (IsCreate)
        {
            CreateBrandRequest createBrandRequest = new() { Name = UpdateBrandRequest.Name, Description = UpdateBrandRequest.Description };
            response = await _brandsClient.CreateAsync(createBrandRequest);
        }
        else
        {
            response = await _brandsClient.UpdateAsync(Id, UpdateBrandRequest);
        }

        if (response.Succeeded)
        {
            if (response.Messages?.Count > 0)
                _snackBar.Add(response.Messages.First(), Severity.Success);
            else
                _snackBar.Add(_localizer["Success"], Severity.Success);
            MudDialog.Close();
        }
        else
        {
            if (response.Messages?.Count > 0)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }
}