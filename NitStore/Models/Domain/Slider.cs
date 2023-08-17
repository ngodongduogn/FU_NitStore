using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitStore.Models.Domain
{
    [Table("Slider")]
    public partial class Slider
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Required]
        [Column(Order = 1)]
        public int CampaignId { get; set; }

        [Required]
        [Column(Order = 2)]
        public int Image { get; set; }

        [Required]
        [Column(Order = 3)]
        public bool Status { get; set; }

        //[Required]
        //public DateTime CreatedDate { get; set; }

        //[Required]
        //public DateTime UpdatedDate { get; set; }
    }
}
