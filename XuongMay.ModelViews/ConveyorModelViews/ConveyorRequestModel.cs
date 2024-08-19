using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.ConveyorModelViews
{
    public class ConveyorRequestModel
    {
        public string ConveyorName { get; set; } = string.Empty;
        public int MaxQuantity { get; set; }
        
        [JsonIgnore]
        public bool IsWorking { get; set; } = false;

        [JsonIgnore]
        public string? CreateBy { get; set; }
    }
}
