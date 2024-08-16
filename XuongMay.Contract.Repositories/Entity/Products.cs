using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Products : BaseEntity
    {
        public String ProductName { get; set; } = string.Empty;
        public String ProductDescription { get; set; } = string.Empty;
        public Decimal ProductPrice { get; set; }
        public String? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
    }
}
