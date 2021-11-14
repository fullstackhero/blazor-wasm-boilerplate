using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog;
public interface IBrandService : IApiService
{
    Task<PaginatedResult<BrandDto>> SearchBrandAsync(BrandListFilter request);
    Task<IResult<Guid>> CreateAsync(CreateBrandRequest request);
    Task<IResult<Guid>> UpdateAsync(UpdateBrandRequest request, Guid id);
    Task<IResult<Guid>> DeleteAsync(Guid id);
    Task<IResult<string>> ExportToExcelAsync(string searchString = "");
}