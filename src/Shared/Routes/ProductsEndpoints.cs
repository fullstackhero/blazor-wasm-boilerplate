namespace FSH.BlazorWebAssembly.Shared.Routes;
public static class ProductsEndpoints
{
    public static string GetCount = "api/v1/products/count";

    public static string GetProductImage(Guid productId)
    {
        return $"api/v1/products/image/{productId}";
    }

    public static string Search = "api/v1/products/search";
    public static string Save = "api/v1/products/";
    public static string Delete = "api/v1/products";
}