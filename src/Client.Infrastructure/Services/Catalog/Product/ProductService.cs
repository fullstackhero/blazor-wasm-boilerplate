//using FSH.BlazorWebAssembly.Shared.Wrapper;
//using System.Net.Http.Json;
//using Routes = FSH.BlazorWebAssembly.Shared.Routes;

//namespace FSH.BlazorWebAssembly.Client.Infrastructure.Catalog
//{
//    public class ProductService : IProductService
//    {
//        private readonly HttpClient _httpClient;

//        public ProductService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<IResult<int>> DeleteAsync(int id)
//        {
//            var response = await _httpClient.DeleteAsync($"{Routes.ProductsEndpoints.Delete}/{id}");
//            return await response.ToResult<int>();
//        }

//        public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
//        {
//            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
//                ? Routes.ProductsEndpoints.Export
//                : Routes.ProductsEndpoints.ExportFiltered(searchString));
//            return await response.ToResult<string>();
//        }

//        public async Task<IResult<string>> GetProductImageAsync(int id)
//        {
//            var response = await _httpClient.GetAsync(Routes.ProductsEndpoints.GetProductImage(id));
//            return await response.ToResult<string>();
//        }

//        public async Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request)
//        {
//            var response = await _httpClient.GetAsync(Routes.ProductsEndpoints.GetAllPaged(request.PageNumber, request.PageSize, request.SearchString, request.Orderby));
//            return await response.ToPaginatedResult<GetAllPagedProductsResponse>();
//        }

//        public async Task<IResult<int>> SaveAsync(AddEditProductCommand request)
//        {
//            var response = await _httpClient.PostAsJsonAsync(Routes.ProductsEndpoints.Save, request);
//            return await response.ToResult<int>();
//        }
//    }
//}