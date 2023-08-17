using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Models.Domain;
using NitStore.Models.DTO;

namespace NitStore.Controllers
{
    public class CampaignsController : Controller
    {
        private readonly NitDbContext _context;

        public CampaignsController(NitDbContext context)
        {
            _context = context;
        }

        // GET: Campaigns
        public async Task<IActionResult> Index()
        {
            return View(await _context.campaigns.ToListAsync());
        }

        // GET: Campaigns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.campaigns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }

        // GET: Campaigns/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create(Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                _context.Add(campaign);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Edit(int id, Campaign campaign)
        {
            if (id != campaign.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(campaign);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignExists(campaign.Id))
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
            return View(campaign);
        }

        // GET: Campaigns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.campaigns == null)
            {
                return NotFound();
            }

            var campaign = await _context.campaigns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.campaigns == null)
            {
                return Problem("Entity set 'NitDbContext.campaigns'  is null.");
            }
            var campaign = await _context.campaigns.FindAsync(id);
            if (campaign != null)
            {
                var slider = _context.slider.Where(s => s.CampaignId == campaign.Id).FirstOrDefault();
                if (slider != null)
                {
                    var img = _context.images.Where(i => i.Id == slider.Image).FirstOrDefault();
                    _context.slider.Remove(slider);
                    await _context.SaveChangesAsync();
                    _context.images.Remove(img);
                    await _context.SaveChangesAsync();
                }
                var campaignItems = _context.campaignItems.Where(c => c.CampaignId == campaign.Id).ToList();
                if(campaignItems != null)
                {
                    foreach(var item in campaignItems)
                    {
                        _context.campaignItems.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }
                _context.campaigns.Remove(campaign);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CampaignExists(int id)
        {
            return _context.campaigns.Any(e => e.Id == id);
        }


        public async Task<IActionResult> CampaignItem(int id)
        {
            var campaign = _context.campaigns.FirstOrDefault(e => e.Id == id);
            ViewBag.Campaign = campaign;
            var lsCampaignItem = await _context.campaignItems.Where(e => e.CampaignId == id).ToListAsync();
            List<CampaignItemShowDTO> lsResult = new List<CampaignItemShowDTO>();
            foreach (var item in lsCampaignItem)
            {
                var tempProduct = _context.products.FirstOrDefault(p => p.Id == item.ProductId);
                ProductImage img = _context.productsImage.FirstOrDefault(i => i.ProductId == tempProduct.Id);
                Image image = _context.images.FirstOrDefault(p => p.Id == img.ImageId);
                CampaignItemShowDTO temp = new CampaignItemShowDTO
                {
                    CampaignItemID = item.Id,
                    CampaignID = item.CampaignId,
                    ProductId = item.ProductId,
                    Name = tempProduct.Name,
                    CategoryId = tempProduct.Category,
                    Price = tempProduct.Price,
                    Discount = item.Discount,
                    imageBit = image.ImageData
                };
                lsResult.Add(temp);
            }
            return View(lsResult);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewCampaignItem(int id)
        {
            ViewBag.CampaignId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCampaignItem(CampaignItem campaignItem)
        {

            if (ModelState.IsValid)
            {
                campaignItem.Id = 0;
                _context.Add(campaignItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("CampaignItem", new { id = campaignItem.CampaignId });
            }
            return View(campaignItem);
        }

        public async Task<IActionResult> DeleteCampaignItem(int id)
        {
            var campaignItem = _context.campaignItems.FirstOrDefault(c => c.Id == id);
            int campaignId = campaignItem.CampaignId;
            if(campaignItem != null)
            {
                _context.Remove(campaignItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("CampaignItem", new {id = campaignId });
        }
    }
}
