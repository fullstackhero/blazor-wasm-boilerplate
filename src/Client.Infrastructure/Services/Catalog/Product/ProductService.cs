using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Net.Http.Json;
using Routes = FSH.BlazorWebAssembly.Shared.Routes;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<Guid>> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ProductsEndpoints.Delete}/{id}");
            return await response.ToResult<Guid>();
        }

        public async Task<IResult<string>> GetProductImageAsync(Guid id)
        {
            var response = await _httpClient.GetAsync(Routes.ProductsEndpoints.GetProductImage(id));
            return await response.ToResult<string>();
        }

        public async Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductListFilter request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ProductsEndpoints.Search, request);
            return await response.ToPaginatedResult<ProductDto>();
        }

        public async Task<IResult<Guid>> CreateAsync(CreateProductRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ProductsEndpoints.Save, request);
            return await response.ToResult<Guid>();
        }
        public async Task<IResult<Guid>> UpdateAsync(UpdateProductRequest request, Guid id)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.ProductsEndpoints.Save + id, request);
            return await response.ToResult<Guid>();
        }
    }
}