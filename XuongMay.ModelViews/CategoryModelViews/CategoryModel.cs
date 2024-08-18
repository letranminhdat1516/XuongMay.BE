using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.CategoryModelViews
{
    public class CategoryModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public float ProductPrice { get; set; }
        public string? CategoryId { get; set; }
    }
}
