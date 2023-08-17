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
using NitStore.Models.DTO;

namespace NitStore.Controllers
{
    public class SlidersController : Controller
    {
        private readonly NitDbContext dbContext;

        public SlidersController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: Sliders
        public async Task<IActionResult> Index()
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            var sliders = await dbContext.slider.Where(s => s.Status == true).ToListAsync();
            List<SliderShowDTO> lsResult = new List<SliderShowDTO>();
            foreach (var slider in sliders)
            {
                Image img = dbContext.images.Where(i => i.Id == slider.Image).FirstOrDefault();
                Campaign cmp = dbContext.campaigns.Where(c => c.Id == slider.CampaignId && c.Status == true).FirstOrDefault();
                SliderShowDTO temp = new SliderShowDTO
                {
                    CampaignId = cmp.Id,
                    CampaignName = cmp.Name,
                    SliderId = slider.Id,
                    ImageData = img.ImageData,
                    Status = slider.Status
                };
                lsResult.Add(temp);
            }
            return View(lsResult);
        }

        // GET: Sliders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || dbContext.slider == null)
            {
                return NotFound();
            }

            var slider = await dbContext.slider
                .FirstOrDefaultAsync(m => m.Id == id);
            var img = dbContext.images.Where(i => i.Id == slider.Image).FirstOrDefault();
            var cmp = dbContext.campaigns.Where(c => c.Id == slider.CampaignId).FirstOrDefault();
            SliderShowDTO result = new SliderShowDTO
            {
                CampaignId = cmp.Id,
                CampaignName = cmp.Name,
                SliderId =  slider.Id,
                ImageData = img.ImageData,
                Status = slider.Status
            };
            if (slider == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Sliders/Create
        public IActionResult Create()
        {
            var lsCampaign = dbContext.campaigns.Where(c => c.Status == true).ToList();
            List<Campaign> lsResult = new List<Campaign>();
            foreach(var item in lsCampaign)
            {
                lsResult.Add(item);
                var slider = dbContext.slider.Where(i => i.CampaignId == item.Id).FirstOrDefault();
                if (slider != null)
                {
                    lsResult.Remove(item);
                }
            }
            ViewBag.ListCampaigns = lsResult;
            return View();
        }

        // POST: Sliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<IActionResult> Create(SliderAddDTO slider)
        {
            if (ModelState.IsValid)
            {
                if(slider.CampaignId == 0 ||slider.Image == null || slider.Image.Count > 1 || slider.Image.Count == 0)
                {
                    TempData["shortMessage"] = "Add Slider Fail";
                } else
                {
                    var campaign = dbContext.campaigns.Where(c => c.Id == slider.CampaignId).FirstOrDefault();
                    foreach(var item in slider.Image)
                    {
                        Image img = new Image();
                        byte[] imgData = ConvertToBytes(item);
                        img.ImageData = imgData;

                        img.Description = "Slider_" + campaign.Name;
                        dbContext.images.Add(img);
                        await dbContext.SaveChangesAsync();

                        Slider newSlider = new Slider
                        {
                            CampaignId = slider.CampaignId,
                            Image = img.Id,
                            Status = true
                        };
                        dbContext.slider.Add(newSlider);
                        TempData["shortMessage"] = "Add Slider Success";
                        await dbContext.SaveChangesAsync();
                    }

                    
                }
                
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        private byte[] ConvertToBytes(IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // GET: Sliders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var slider = await dbContext.slider
                .FirstOrDefaultAsync(m => m.Id == id);

            if (slider == null)
            {
                return NotFound();
            }  else
            {
                var img = dbContext.images.Where(i => i.Id == slider.Image).FirstOrDefault();
                dbContext.slider.Remove(slider);
                await dbContext.SaveChangesAsync();
                dbContext.images.Remove(img);
                await dbContext.SaveChangesAsync();
                TempData["shortMessage"] = "Delete Slider Success";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SliderExists(int id)
        {
            return dbContext.slider.Any(e => e.Id == id);
        }
    }
}
