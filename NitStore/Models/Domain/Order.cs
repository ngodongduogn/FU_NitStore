using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace NitStore.Models.Domain
{
    [Table("Order")]
    public partial class Order
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column(Order = 1)]
        public int CustomerId { get; set; }

        [Required]
        public int Status { get; set; }
       // 0 cart || 1 order confirm || 2 shipping || 3 recived
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}
