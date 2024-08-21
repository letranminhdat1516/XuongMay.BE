namespace XuongMay.ModelViews.ProductModelViews
{
    public class ProductModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? CategoryId { get; set; }

    }
}
