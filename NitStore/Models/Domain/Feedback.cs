using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitStore.Models.Domain
{
    [Table("Feedback")]
    public partial class Feedback
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column(Order = 1)]
        public int CustomerId { get; set; }

        [Required]
        [Column(Order = 2)]
        public int ProductId { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        public int OrderId { get; set; }

        public int Rate { get; set; }
    }
}
