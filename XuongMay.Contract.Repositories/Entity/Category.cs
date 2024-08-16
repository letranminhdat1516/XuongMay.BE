using System.ComponentModel.DataAnnotations.Schema;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    [Table("Category")]
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDescription { get; set; } = string.Empty;

        // Navigation property
        public  ICollection<Products> Products { get; set; }
    }
}
