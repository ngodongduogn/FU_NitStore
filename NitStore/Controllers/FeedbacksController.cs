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
    public class FeedbacksController : Controller
    {
        private readonly NitDbContext dbContext;

        public FeedbacksController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
              return View(await dbContext.feedbacks.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || dbContext.feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await dbContext.feedbacks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult FeedbackOrder(int orderId)
        {
            int userId = -1;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            else
            {
                return RedirectToAction("Login", "Authen");
            }
            Order order = dbContext.orders.Where(x => x.Id == orderId).FirstOrDefault();
            List<OrderDetail > orderDetail = new List<OrderDetail>();
            List<FeedbackOrderDTO> feedbacks = new List<FeedbackOrderDTO>();
            if (order != null)
            {
                orderDetail = dbContext.ordersDetail.Where(x => x.OrderId == orderId).ToList();

                foreach (OrderDetail item in orderDetail)
                {
                    ProductImage productImage = dbContext.productsImage.Where(x => x.ProductId == item.ProductId).First();
                    Product p = dbContext.products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                    Image image = dbContext.images.Where(x => x.Id == productImage.ImageId).First();
                    FeedbackOrderDTO dto = new FeedbackOrderDTO()
                    {
                        imageBit = image.ImageData,
                        ProductId = item.ProductId,
                        ProductName = p.Name,
                        CustomerId = userId,
                        feedback = "",
                        OrderID = order.Id
                    };
                    feedbacks.Add(dto);
                }
            }
            ViewBag.Feedbacks = feedbacks;
            return View(feedbacks);
        }
        [HttpPost]
        public IActionResult FeedbackOrder(List<FeedbackOrderDTO> feedbacks)
        {
            foreach(FeedbackOrderDTO dto in feedbacks)
            {
                Feedback f = new Feedback()
                {
                    ProductId = dto.ProductId,
                    CustomerId = dto.CustomerId,
                    Description = dto.feedback,
                    UpdatedDate = DateTime.Now,
                    OrderId = dto.OrderID,
                    Rate = dto.Rate
                };
                dbContext.feedbacks.Add(f);
                dbContext.SaveChanges();
            }
            TempData["shortMessage"] = "Feedback Success";
            return RedirectToAction("OrderHistory", "Orders");
        }
        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                AddFeedback(feedback);
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        public async Task<bool> AddFeedback(Feedback feedback)
        {
            dbContext.Add(feedback);
            await dbContext.SaveChangesAsync();
            return true;
        }


        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || dbContext.feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await dbContext.feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(feedback);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || dbContext.feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await dbContext.feedbacks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (dbContext.feedbacks == null)
            {
                return Problem("Entity set 'NitDbContext.feedbacks'  is null.");
            }
            var feedback = await dbContext.feedbacks.FindAsync(id);
            if (feedback != null)
            {
                dbContext.feedbacks.Remove(feedback);
            }
            
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
          return dbContext.feedbacks.Any(e => e.Id == id);
        }
    }
}
