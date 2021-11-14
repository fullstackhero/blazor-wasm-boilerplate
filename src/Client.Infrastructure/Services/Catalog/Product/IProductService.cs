using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog;
public interface IProductService : IApiService
{
    Task<PaginatedResult<ProductDto>> GetProductsAsync(ProductListFilter request);
    Task<IResult<string>> GetProductImageAsync(Guid id);
    Task<IResult<Guid>> CreateAsync(CreateProductRequest request);
    Task<IResult<Guid>> UpdateAsync(UpdateProductRequest request, Guid id);
    Task<IResult<Guid>> DeleteAsync(Guid id);

}