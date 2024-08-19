using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.ProductTaskModelViews
{
    public class OrderTaskRequestModel
    {
        public required string OrderId { get; set; }
        public required string ConveyorId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
        public string Status { get; private set; } = "Processing";

        [JsonIgnore]
        public string? CreateBy { get; set; }
    }
}
