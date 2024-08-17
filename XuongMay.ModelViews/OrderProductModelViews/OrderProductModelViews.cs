using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.OrderProductModelViews
{
    internal class OrderProductModelViews
    {
        public string? UserInfoId { get; set; }
        public string? ProductId {  get; set; }
        public string? OrdersCode {  get; set; }
        public int Quantity {  get; set; }
        public float TotalPrice {  get; set; }
        public string? ProductsId { get; set; }
        public string? CreatedBy {  get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
    }
}
