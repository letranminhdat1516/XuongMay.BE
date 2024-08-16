using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Category : BaseEntity
    {
        public String CategoryName { get; set; } = string.Empty;
        public String CategoryDescription { get; set; } = string.Empty;

        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
