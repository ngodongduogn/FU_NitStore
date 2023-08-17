using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitStore.Models.Domain
{
    [Table("Campaign")]
    public partial class Campaign
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
        public bool Status { get; set; }

    }
}
