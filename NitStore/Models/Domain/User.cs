using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitStore.Models.Domain
{
    [Table("User")]
    public partial class User
    {
        [Key]
        [Column (Order = 0)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        public int Role { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
