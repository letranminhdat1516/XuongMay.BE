using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class ProductTask : BaseEntity
    {
        public string? OrderId { get; set; }
        public string? ConvenyorId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
        public string Status { get; set; } = "Processing";
        public virtual Orders? Orders { get; set; }
        public virtual Conveyor? Conveyor { get; set; }
    }
}
