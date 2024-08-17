﻿using System.ComponentModel.DataAnnotations.Schema;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    [Table("Products")]
    public class Products : BaseEntity
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
        public string? CategoryId { get; set; }

        // Navigation property
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

    }
}




