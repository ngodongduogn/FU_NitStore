using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Models.Domain;
using NitStore.Models.DTO;
using PagedList;

namespace NitStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly NitDbContext dbContext;
        public AdminController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IActionResult> HomeAdmin()
        {
            return View();
        }
        

    }
}
