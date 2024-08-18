using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class Orders : BaseEntity
    {
        public string? UserInfoId { get; set; }

        public string? ProductId { get; set; }

        public string? OrdersCode { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("UserInfoId")]
        [JsonIgnore]
        public virtual UserInfo? UserInfo { get; set; }

        [ForeignKey("ProductId")]
        [JsonIgnore]
        public virtual Products? Product { get; set; }
    }
}
