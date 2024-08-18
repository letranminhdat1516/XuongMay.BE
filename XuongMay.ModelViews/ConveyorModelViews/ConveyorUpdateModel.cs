using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.ConveyorModelViews
{
    public class ConveyorUpdateModel
    {
        public void SetWorkingStatus(bool isWoking)
        {
            IsWorking = isWoking;
        }

        public required string ConveyorId { get; set; }
        public int ConveyorNumber { get; set; }
        public string ConveyorCode { get; set; } = string.Empty;
        public string ConveyorName { get; set; } = string.Empty;
        public int MaxQuantity { get; set; }

        [JsonIgnore]
        public bool IsWorking { get; private set; } = false;

        [JsonIgnore]
        public required string UpdateBy { get; set; }
    }
}
