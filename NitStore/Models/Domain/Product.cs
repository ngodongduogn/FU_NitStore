using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace NitStore.Models.Domain
{
    [Table("Product")]
    public partial class Product
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        
    }
}
