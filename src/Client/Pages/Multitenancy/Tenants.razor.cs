using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Multitenancy;

public partial class Tenants
{
    public bool Dense = false;
    public bool Hover = true;
    public bool Ronly = false;
    public bool CanCancelEdit = false;
    public TenantDto SelectedItem1 = default!;
    public TenantDto SelectedItem2 = default!;
    public TenantDto? ElementBeforeEdit;
    public string _searchString = string.Empty;

    public MudDatePicker _picker = default!;
    public DateTime? Date = DateTime.Today;
    public bool AutoClose;
    public bool ReadOnly;
    public bool _loading = true;

    public List<TenantDto> Elements = new List<TenantDto>();

    [Inject]
    private ITenantsClient TenantsClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => TenantsClient.GetAllAsync(),
                Snackbar)
            is ICollection<TenantDto> tenants)
        {
            Elements = tenants.ToList();
        }

        _loading = false;
    }

    public void BackupItem(object element)
    {
        ElementBeforeEdit = new()
        {
            Name = ((TenantDto)element).Name,
            Key = ((TenantDto)element).Key,
            AdminEmail = ((TenantDto)element).AdminEmail,
            ConnectionString = ((TenantDto)element).ConnectionString,
            IsActive = ((TenantDto)element).IsActive,
            ValidUpto = ((TenantDto)element).ValidUpto,
        };
    }

    public void ResetItemToOriginalValues(object element)
    {
        if (ElementBeforeEdit is not null)
        {
            ((TenantDto)element).Name = ElementBeforeEdit.Name;
            ((TenantDto)element).Key = ElementBeforeEdit.Key;
            ((TenantDto)element).AdminEmail = ElementBeforeEdit.AdminEmail;
            ((TenantDto)element).ConnectionString = ElementBeforeEdit.ConnectionString;
            ((TenantDto)element).IsActive = ElementBeforeEdit.IsActive;
            ((TenantDto)element).ValidUpto = ElementBeforeEdit.ValidUpto;
        }
    }

    public bool FilterFunc(TenantDto element)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        return false;
    }

    public void ItemHasBeenCommitted(object element)
    {
        Console.WriteLine("tst");
    }

    public void OnSearch(string text)
    {
        _searchString = text;
    }

    public void InvokeModal(Guid id = new())
    {
        // var parameters = new DialogParameters
        //    {
        //        { nameof(AddEditBrandModal.IsCreate), id == new Guid() },
        //        { nameof(AddEditBrandModal.Id), id }
        //    };
        // if (id != new Guid())
        // {
        //    var brand = _pagedData?.FirstOrDefault(c => c.Id == id);
        //    if (brand != null)
        //    {
        //        parameters.Add(nameof(AddEditBrandModal.UpdateBrandRequest), new UpdateBrandRequest
        //        {
        //            Name = brand.Name,
        //            Description = brand.Description,
        //        });
        //    }
        // }

        // var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        // var dialog = _dialogService.Show<AddEditBrandModal>(id == new Guid() ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        // var result = await dialog.Result;
        // if (!result.Cancelled)
        // {
        //    await Reset();
        // }
    }
}