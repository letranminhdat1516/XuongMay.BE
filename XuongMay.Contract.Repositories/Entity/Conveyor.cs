using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Conveyor : BaseEntity
    {
        public int ConveyorNumber { get; set; }
        public string ConveyorCode { get; set; } = string.Empty;
        public string ConveyorName { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; } = new TimeOnly(7, 0);
        public TimeOnly EndTime { get; set; } = new TimeOnly(17, 0);
    }
}
