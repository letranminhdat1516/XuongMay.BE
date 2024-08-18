using System.ComponentModel;
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

        public string ConveyorName { get; set; } = string.Empty;

        [DefaultValue(false)]
        public bool IsWorking { get; set; } = false;

        public int MaxQuantity { get; set; }

        [DefaultValue("07:00:00")]
        public TimeOnly StartTime { get; set; }

        [DefaultValue("17:00:00")]
        public TimeOnly EndTime { get; set; }
    }
}
