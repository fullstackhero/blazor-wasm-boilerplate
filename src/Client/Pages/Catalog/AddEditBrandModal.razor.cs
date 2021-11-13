using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class AddEditBrandModal
{
    [Parameter] public UpdateBrandRequest UpdateBrandRequest { get; set; } = new();
    [Parameter] public bool IsCreate { get; set; }
    [Parameter] public Guid Id { get; set; }
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        IResult<Guid> response;
        if (IsCreate)
        {
            CreateBrandRequest CreateBrandRequest = new() { Name = UpdateBrandRequest.Name, Description = UpdateBrandRequest.Description };
            response = await _brandService.CreateAsync(CreateBrandRequest);
        }
        else
        {
            response = await _brandService.UpdateAsync(UpdateBrandRequest, Id);
        }
        if (response.Succeeded)
        {
            if (response.Messages.Any())
                _snackBar.Add(response.Messages[0], Severity.Success);
            else
                _snackBar.Add(_localizer["Success"], Severity.Success);
            MudDialog.Close();
        }
        else
        {
            if (response.Messages.Any()) foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            else if (!string.IsNullOrEmpty(response.Exception))
                _snackBar.Add(response.Exception, Severity.Error);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }
}