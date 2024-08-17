namespace XuongMay.ModelViews.ConveyorModelViews
{
    public class ConveyorCreateModel
    {
        public string? ConveyorId { get; set; }
        public int ConveyorNumber { get; set; }
        public string ConveyorCode { get; set; } = string.Empty;
        public string ConveyorName { get; set; } = string.Empty;
        public bool IsWorking { get; set; } = false;
    }
}
