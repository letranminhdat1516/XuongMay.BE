using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class OrderTask : BaseEntity
    {
        public int Quantity { get; set; } = 0;

        public string TaskNote { get; set; } = string.Empty;

        [DefaultValue("Processing")]
        public string Status { get; set; } = "Processing";

        public int CompleteQuantity { get; set; } = 0;

        public required string OrderId { get; set; }

        [ForeignKey("OrderId")]
        [JsonIgnore]
        public virtual Orders? Orders { get; set; }

        public required string ConveyorId { get; set; }

        [ForeignKey("ConveyorId")]
        [JsonIgnore]
        public virtual Conveyor? Conveyor { get; set; }
    }
}
