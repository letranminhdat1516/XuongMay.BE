using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.OrderTaskModelViews
{
    public class OrderTaskUpdateModel
    {
        public required string OrderTaskId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
        public string Status { get; private set; } = "Processing";

        public void SetStatus(string status)
        {
            Status = status;
        }

        [JsonIgnore]
        public required string UpdateBy { get; set; }
    }
}
