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
    public class OrdersController : Controller
    {
        private readonly NitDbContext dbContext;

        public OrdersController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            List<OrderHistory> itemInside = new List<OrderHistory>();
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = await dbContext.orders.Where(x => x.Status != 0).ToListAsync();
            foreach(Order item in orders)
            {
                orderDetails = dbContext.ordersDetail.Where(x => x.OrderId == item.Id).ToList();
                decimal totalMoney = 0;
                foreach (OrderDetail itemDetails in orderDetails)
                {
                    Product p = dbContext.products.Where(x => x.Id == itemDetails.ProductId).FirstOrDefault();
                    totalMoney = totalMoney + p.Price;
                }
                string orderStatus = "";
                if (item.Status == 1)
                {
                    orderStatus = "Order Confirm";
                }
                else if (item.Status == 2)
                {
                    orderStatus = "Order Shipping";
                }
                else
                {
                    orderStatus = "Order Received";
                }

                OrderHistory dto = new OrderHistory()
                {
                    Id = item.Id,
                    Quantity = orderDetails.Count,
                    OrderDate = item.UpdatedDate.ToString("dd/MM/yyyy"),
                    Status = orderStatus,
                    TotalMoney = totalMoney,
                    FeedbackAble = false
                };

                itemInside.Add(dto);
            }

            ViewBag.ListOrder = itemInside;
              return View();
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || dbContext.orders == null)
            {
                return NotFound();
            }

            var order = await dbContext.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(order);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || dbContext.orders == null)
            {
                return NotFound();
            }

            var order = await dbContext.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(order);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || dbContext.orders == null)
            {
                return NotFound();
            }

            var order = await dbContext.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (dbContext.orders == null)
            {
                return Problem("Entity set 'NitDbContext.orders'  is null.");
            }
            var order = await dbContext.orders.FindAsync(id);
            if (order != null)
            {
                dbContext.orders.Remove(order);
            }
            
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return dbContext.orders.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(PaymentMethodDTO dto)
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
            //get customer order
            Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
            List<OrderDetail> detail = new List<OrderDetail>();
            if (order != null)
            {
                detail = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
            }
            if(detail.Count > 0)
            {
                // process cart to order
                if (dto.PayNow == true)
                {
                    return RedirectToAction("Checkout", "VNPay", new { orderId = order.Id });
                }
                else
                {
                    // put out quantity of a product
                    decimal totalPrice = 0; 
                    foreach(OrderDetail item in detail)
                    {
                        Product product = dbContext.products.Where(x => x.Id== item.ProductId).First();
                        product.Quantity = product.Quantity - item.Quantity;
                        totalPrice = totalPrice + (product.Price * item.Quantity);
                        dbContext.SaveChanges();
                    }
                    // put cart into order
                    order.Status = 1;
                    order.UpdatedDate= DateTime.Now;
                    order.Total = totalPrice;
                    dbContext.SaveChanges();
                    return RedirectToAction("ListProduct","Products", new { area = "" });
                }
            }
            
            return View();
        }

        public async Task<IActionResult> OrderHistory()
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            int userId = -1;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            else
            {
                return RedirectToAction("Login", "Authen");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<Order> orders = dbContext.orders.ToList();
            //recived
            orders = orders.Where(x => x.Status != 0).ToList();
            List<OrderHistory> itemInside = new List<OrderHistory>();
            foreach (Order item in orders)
            {
                
                orderDetails = dbContext.ordersDetail.Where(x => x.OrderId == item.Id).ToList();
                decimal totalMoney = 0;
                foreach(OrderDetail itemDetails in orderDetails)
                {
                    Product p = dbContext.products.Where(x => x.Id == itemDetails.ProductId).FirstOrDefault();
                    totalMoney = totalMoney + (p.Price * itemDetails.Quantity);
                }
                string orderStatus = "";
                if(item.Status == 1)
                {
                    orderStatus = "Order Confirm";
                }else if (item.Status == 2)
                {
                    orderStatus = "Order Shipping";
                }
                else
                {
                    orderStatus = "Order Received";
                }
                List<Feedback> feedback = dbContext.feedbacks.Where(x => x.OrderId == item.Id).ToList();
                bool isFeedback = false;
                if(feedback.Count <= 0 && orderStatus == "Order Received")
                {
                    isFeedback = true;
                }
                OrderHistory dto = new OrderHistory()
                {
                    Id = item.Id,
                    Quantity = orderDetails.Count,
                    OrderDate = item.UpdatedDate.ToString("dd/MM/yyyy"),
                    Status = orderStatus,
                    TotalMoney= totalMoney,
                    FeedbackAble = isFeedback
                };
                itemInside.Add(dto);
            }
            ViewBag.OrderHistory = itemInside;
            return View(itemInside);
        }

        public async Task<IActionResult> ChangeOrderStatus(int orderId, int status)
        {
            if(orderId == null)
            {
                return NotFound();
            }
            Order order = dbContext.orders.Where(x => x.Id== orderId).FirstOrDefault();
            if(order != null)
            {
                order.Status = status;
                dbContext.SaveChanges();
            }
            TempData["shortMessage"] = "Change Order Status Success";
            return RedirectToAction("Index");
        }
    }
}
