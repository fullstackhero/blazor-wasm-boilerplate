using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Net.Http.Json;
using Routes = FSH.BlazorWebAssembly.Shared.Routes;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog;
public class BrandService : IBrandService
{
    private readonly HttpClient _httpClient;

    public BrandService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
    {
        var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? Routes.BrandsEndpoints.Export
            : Routes.BrandsEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<IResult<Guid>> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{Routes.BrandsEndpoints.Delete}/{id}");
        return await response.ToResult<Guid>();
    }

    public async Task<PaginatedResult<BrandDto>> SearchBrandAsync(BrandListFilter request)
    {
        var response = await _httpClient.PostAsJsonAsync(Routes.BrandsEndpoints.Search, request);
        return await response.ToPaginatedResult<BrandDto>();
    }
    public async Task<IResult<Guid>> CreateAsync(CreateBrandRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(Routes.BrandsEndpoints.Save, request);
        return await response.ToResult<Guid>();
    }
    public async Task<IResult<Guid>> UpdateAsync(UpdateBrandRequest request, Guid id)
    {
        var response = await _httpClient.PutAsJsonAsync(Routes.BrandsEndpoints.Save + id, request);
        return await response.ToResult<Guid>();
    }

}