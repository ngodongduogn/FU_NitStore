using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NitStore.Data;
using NitStore.Models.Domain;

namespace NitStore.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly NitDbContext dbContext;

        public CategoriesController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            return View(await dbContext.categories.ToListAsync());
        }

        public async Task<IActionResult> ViewAllCategory()
        {
            return View("ViewAllCategory",await dbContext.categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || dbContext.categories == null)
            {
                return NotFound();
            }

            var category = await dbContext.categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                bool result = await AddCategory(category);
                if(result == true)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }

        public async Task<bool> AddCategory(Category category)
        {
            if (category == null)
            {
                return false;
            } else if (category.Name == null || category.Description == null)
            {
                return false;
            } else if (category.Name.Trim() == "" || category.Description.Trim() == "") 
            {
                return false;
            } else
            {
                dbContext.Add(category);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || dbContext.categories == null)
            {
                return NotFound();
            }

            var category = await dbContext.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Category result = await EditCategory(category);
                if(result != null)
                {
                    return RedirectToAction(nameof(Index));
                } else
                {
                    return NotFound();
                }
                
            }
            return View(category);
        }

        public async Task<Category> EditCategory(Category category)
        {
            try
            {
                dbContext.Update(category);
                await dbContext.SaveChangesAsync();
                return category;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || dbContext.categories == null)
            {
                return NotFound();
            }

            var category = await dbContext.categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (dbContext.categories == null)
            {
                return Problem("Entity set 'NitdbContext.categories'  is null.");
            }
            var category = await dbContext.categories.FindAsync(id);
            if (category != null)
            {
                Product p = dbContext.products.Where(p => p.Category == id).FirstOrDefault();
                if (p != null)
                {
                    TempData["shortMessage"] = "Cannot Delete Category because still in use";
                    return RedirectToAction("Index");
                }
                dbContext.categories.Remove(category);
            }
            TempData["shortMessage"] = "Delete Category Success";
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return dbContext.categories.Any(e => e.Id == id);
        }
    }
}
