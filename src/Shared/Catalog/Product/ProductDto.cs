namespace FSH.BlazorWebAssembly.Shared.Catalog;
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Rate { get; set; }
    public string ImagePath { get; set; }
    public Guid BrandId { get; set; }
    public string BrandIdString { get => BrandId.ToString();}
}