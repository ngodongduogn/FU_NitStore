using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitStore.Models.Domain
{
    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column(Order = 1)]
        public int OrderId { get; set; }

        [Required]
        [Column(Order = 2)]
        public int ProductId { get; set; }

        [Required]
        [Column(Order = 3)]
        public int Quantity { get; set; }
    }
}
