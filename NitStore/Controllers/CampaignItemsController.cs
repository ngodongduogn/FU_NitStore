using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Models.Domain;

namespace NitStore.Controllers
{
    public class CampaignItemsController : Controller
    {
        private readonly NitDbContext dbContext;

        public CampaignItemsController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: CampaignItems
        public async Task<IActionResult> Index()
        {
              return View(await dbContext.campaignItems.ToListAsync());
        }

        // GET: CampaignItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || dbContext.campaignItems == null)
            {
                return NotFound();
            }

            var campaignItem = await dbContext.campaignItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaignItem == null)
            {
                return NotFound();
            }

            return View(campaignItem);
        }

        // GET: CampaignItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CampaignItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(CampaignItem campaignItem)
        {
            if (ModelState.IsValid)
            {
                AddCampaignItem(campaignItem);
                return RedirectToAction(nameof(Index));
            }
            return View(campaignItem);
        }

        public async Task<bool> AddCampaignItem(CampaignItem campaignItem)
        {
            if (campaignItem.Discount < 0)
            {
                return false;
            }
            else
            {
                dbContext.Add(campaignItem);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }

        // GET: CampaignItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || dbContext.campaignItems == null)
            {
                return NotFound();
            }

            var campaignItem = await dbContext.campaignItems.FindAsync(id);
            if (campaignItem == null)
            {
                return NotFound();
            }
            return View(campaignItem);
        }

        // POST: CampaignItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, CampaignItem campaignItem)
        {
            if (id != campaignItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(campaignItem);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignItemExists(campaignItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(campaignItem);
        }

        // GET: CampaignItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || dbContext.campaignItems == null)
            {
                return NotFound();
            }

            var campaignItem = await dbContext.campaignItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaignItem == null)
            {
                return NotFound();
            }

            return View(campaignItem);
        }

        // POST: CampaignItems/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (dbContext.campaignItems == null)
            {
                return Problem("Entity set 'NitDbContext.campaignItems'  is null.");
            }
            var campaignItem = await dbContext.campaignItems.FindAsync(id);
            if (campaignItem != null)
            {
                dbContext.campaignItems.Remove(campaignItem);
            }
            
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CampaignItemExists(int id)
        {
          return dbContext.campaignItems.Any(e => e.Id == id);
        }
    }
}
