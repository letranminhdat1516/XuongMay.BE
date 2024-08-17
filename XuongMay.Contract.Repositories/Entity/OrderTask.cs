using System.ComponentModel.DataAnnotations.Schema;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class OrderTask : BaseEntity
    {
        public string? OrderId { get; set; }

        public string? ConveyorId { get; set; }

        public int Quantity { get; set; } = 0;

        public string TaskNote { get; set; } = string.Empty;

        public string Status { get; set; } = "Processing";

        [ForeignKey("OrderId")]
        public virtual Orders? Orders { get; set; }

        [ForeignKey("ConveyorId")]
        public virtual Conveyor? Conveyor { get; set; }
    }
}
