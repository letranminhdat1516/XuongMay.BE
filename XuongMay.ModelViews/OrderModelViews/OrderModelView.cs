using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.OrderModelViews
{
    public class OrderModelView
    {        
        public string? ProductId {  get; set; }
        public string? OrdersCode {  get; set; }
        public int Quantity {  get; set; }
        public Decimal TotalPrice { get; set; }
    }
}
