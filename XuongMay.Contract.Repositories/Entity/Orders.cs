using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Orders : BaseEntity
    {
        public string? UserInfoId { get; set; }
        public string? ProductId { get; set; }
        public string? OrdersCode { get; set; }
        public int Quantity { get; set; }
        public Decimal TotalPrice { get; set; }

        public virtual UserInfo? UserInfo { get; set; }
        public virtual Products? Products { get; set; }
        public virtual ICollection<ProductTask> ProductTasks { get; set; } = new List<ProductTask>();
    }
}
