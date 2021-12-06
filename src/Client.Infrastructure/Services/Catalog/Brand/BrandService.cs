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
            ? BrandsEndpoints.Export
            : BrandsEndpoints.ExportFiltered(searchString));
        return await response.ToResultAsync<string>();
    }

    public async Task<IResult<Guid>> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BrandsEndpoints.Delete}/{id}");
        return await response.ToResultAsync<Guid>();
    }

    public async Task<PaginatedResult<BrandDto>> SearchBrandAsync(BrandListFilter request)
    {
        var response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Search, request);
        return await response.ToPaginatedResultAsync<BrandDto>();
    }

    public async Task<IResult<Guid>> CreateAsync(CreateBrandRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Save, request);
        return await response.ToResultAsync<Guid>();
    }

    public async Task<IResult<Guid>> UpdateAsync(UpdateBrandRequest request, Guid id)
    {
        var response = await _httpClient.PutAsJsonAsync(BrandsEndpoints.Save + id, request);
        return await response.ToResultAsync<Guid>();
    }

}