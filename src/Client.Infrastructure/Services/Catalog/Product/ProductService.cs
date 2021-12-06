namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<Guid>> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{ProductsEndpoints.Delete}/{id}");
        return await response.ToResult<Guid>();
    }

    public async Task<IResult<string>> GetProductImageAsync(Guid id)
    {
        var response = await _httpClient.GetAsync(ProductsEndpoints.GetProductImage(id));
        return await response.ToResult<string>();
    }

    public async Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductListFilter request)
    {
        var response = await _httpClient.PostAsJsonAsync(ProductsEndpoints.Search, request);
        return await response.ToPaginatedResult<ProductDto>();
    }

    public async Task<IResult<Guid>> CreateAsync(CreateProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ProductsEndpoints.Save, request);
        return await response.ToResult<Guid>();
    }

    public async Task<IResult<Guid>> UpdateAsync(UpdateProductRequest request, Guid id)
    {
        var response = await _httpClient.PutAsJsonAsync(ProductsEndpoints.Save + id, request);
        return await response.ToResult<Guid>();
    }
}