namespace XuongMay.ModelViews.ProductTaskModelViews
{
    public class TaskRequestModel
    {
        public string? OrderId { get; set; }
        public string? ConvenyorId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
        public string Status { get; set; } = "Processing";
    }
}
