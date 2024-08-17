namespace XuongMay.ModelViews.ConveyorModelViews
{
    public class ConveyorRequestModel
    {
        public string? ConveyorId { get; private set; }
        public int ConveyorNumber { get; set; }
        public string ConveyorCode { get; set; } = string.Empty;
        public string ConveyorName { get; set; } = string.Empty;
        public bool IsWorking { get; private set; } = false;
        public string? UpdateBy { get; private set; }
        public string? DeleteBy { get; private set; }
    }
}
