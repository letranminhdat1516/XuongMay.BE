namespace XuongMay.ModelViews.ProductTaskModelViews
{
    public class OrderTaskRequestModel
    {
        public string? OrderId { get; set; }
        public string? ConvenyorId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
        public string Status { get; private set; } = "Processing";
        public string? UpdateBy { get; private set; }
        public string? DeleteBy { get; private set; }
    }
}
