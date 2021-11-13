namespace FSH.BlazorWebAssembly.Shared.Catalog
{
    public class ProductDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public string ImagePath { get; set; }
        public BrandDto Brand { get; set; }
    }
}