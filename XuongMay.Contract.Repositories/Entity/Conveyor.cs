using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Conveyor : BaseEntity
    {
        public Conveyor()
        {
            StartTime = new TimeOnly(07,00,00);
            EndTime = new TimeOnly(17, 00, 00);
        }
        public int ConveyorNumber { get; set; }
        public string ConveyorCode { get; set; } = string.Empty;
        public string ConveyorName { get; set; } = string.Empty;
        public bool IsWorking { get; set; } = false;
        public int MaxQuantity { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
