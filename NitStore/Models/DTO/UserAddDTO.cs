using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;


namespace NitStore.Models.DTO
{
    public class UserAddDTO
    {
        public string UserName { get; set; }

        public int Role { get; set; }

        //[Required]
        //public int roleId { get; set; }
        //public SelectList roleList { get; set; }

        [Required]
        public int Status { get; set; }

        public string Email { get; set; }
    }
}
