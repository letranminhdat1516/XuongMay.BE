using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class ProductTask : BaseEntity
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; } = 0;
        public string TaskNote { get; set; } = string.Empty;
    }
}
