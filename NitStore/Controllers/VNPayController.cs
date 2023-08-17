using Microsoft.AspNetCore.Mvc;
using NitStore.Data;
using NitStore.Models;
using NitStore.Models.Domain;
using NitStore.Service;

namespace NitStore.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly NitDbContext dbContext;
        public VNPayController(IVnPayService vnPayService, NitDbContext dbContext)
        {
            _vnPayService = vnPayService;
            this.dbContext = dbContext;
        }

        public IActionResult Checkout(int orderId)
        {

            //check out form
            List<Category> categoryList = dbContext.categories.ToList();
            ViewBag.CategoryListVNPAY = categoryList;
            string customerName = "";
            decimal totalMoney = 0;
            Order order = dbContext.orders.Where(x => x.Id == orderId).FirstOrDefault();
            if(order != null)
            {
                User user = dbContext.users.Where(x => x.Id == order.CustomerId).FirstOrDefault();
                if (user != null)
                {
                    customerName = user.UserName;
                }
                totalMoney = order.Total;
            }
            ViewBag.CustomerName = customerName;
            ViewBag.TotalMoney = totalMoney;
            ViewBag.OrderId = orderId;
            return View();
        }
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            User user = dbContext.users.Where(x => x.UserName == model.Name).FirstOrDefault();
            if(user != null)
            {
                int userId = user.Id;
                Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
                List<OrderDetail> detail = new List<OrderDetail>();
                if (order != null)
                {
                    detail = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                }
                decimal totalPrice = 0;
                foreach (OrderDetail item in detail)
                {
                    Product product = dbContext.products.Where(x => x.Id == item.ProductId).First();
                    product.Quantity = product.Quantity - item.Quantity;
                    totalPrice = totalPrice + (product.Price * item.Quantity);
                    dbContext.SaveChanges();
                }
                // put cart into order
                order.Status = 1;
                order.UpdatedDate = DateTime.Now;
                order.Total = totalPrice;
                dbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }
            
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if(response != null)
            {
                PaymentResponseModel dto = response;
                if(dto.Success == false)
                {
                    TempData["shortMessage"] = "Order successfull!!";
                    return RedirectToAction("OrderHistory", "Orders");
                }
                else
                {
                    TempData["shortMessage"] = "Order fail, you must pay when revice order!!";
                    return RedirectToAction("OrderHistory", "Orders");
                }
            }

            return Json(response);
        }
    }
}
