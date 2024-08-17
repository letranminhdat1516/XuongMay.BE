using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    [Table("Products")]
    public class Products : BaseEntity
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public float ProductPrice { get; set; }
        public string? CategoryId { get; set; }

        // Navigation property
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public virtual Category Category { get; set; }

    }
}




