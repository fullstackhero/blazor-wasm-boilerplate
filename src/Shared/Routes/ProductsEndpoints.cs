namespace FSH.BlazorWebAssembly.Shared.Routes;

public static class ProductsEndpoints
{
    public const string GetCount = "api/v1/products/count";

    public static string GetProductImage(Guid productId)
    {
        return $"api/v1/products/image/{productId}";
    }

    public const string Search = "api/v1/products/search";
    public const string Save = "api/v1/products/";
    public const string Delete = "api/v1/products";
}