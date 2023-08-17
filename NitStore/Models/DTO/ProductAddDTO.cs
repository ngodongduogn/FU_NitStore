using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace NitStore.Models.DTO
{
    public class ProductAddDTO
    {
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
        public int CategoryId { get; set; }

        public SelectList CategoryList { get; set; }

        [Required]
        public decimal Price { get; set; }

        public List<IFormFile> Imagess { get; set; }
    }
}
