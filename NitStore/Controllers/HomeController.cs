using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using NitStore.Models;
using NitStore.Data;
using NitStore.Models.Domain;
using System.Web;
using NitStore.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using NitStore.Service;

namespace NitStore.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly NitDbContext dbContext;
        

        public HomeController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //HttpContext.Session.SetString("UserId", "15");
            //return RedirectToAction("HomeAdmin", "Admin", new { area = "" });
            return RedirectToAction("ListProduct", "Products");
        }

        //[HttpGet]
        //public async Task<IActionResult> LandingPage()
        //{
        //    List<Category> categoryList = dbContext.categories.ToList();
        //    ViewBag.CategoryList = categoryList;
        //    //List<Category> categories = new List<Category>();
        //    //categories = dbContext.categories.ToList()/*.GetRange(0,5)*/;
        //    //ViewBag.Categories = categories;
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> HomeMaketer()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            //cart item 

            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty()){
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            List<CartShowDTO> returnList = new List<CartShowDTO>();
            int userId = -1;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            else
            {
                return RedirectToAction("Login", "Authen");
            }
            Order order = dbContext.orders.Where(x => x.Status == 0 && x.CustomerId == userId).FirstOrDefault();
            if (order != null)
            {
                List<OrderDetail> list = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                foreach (OrderDetail detail in list)
                {
                    Product product = dbContext.products.Where(x => x.Id == detail.ProductId).First();
                    if (product.Quantity == 0)
                    {
                        dbContext.ordersDetail.Remove(detail);
                        dbContext.SaveChanges();
                        list.Remove(detail);
                    }
                }

                foreach (OrderDetail item in list)
                {
                    Product product = dbContext.products.Where(x => x.Id == item.ProductId).First();
                    Category category = dbContext.categories.Where(x => x.Id == product.Category).First();
                    ProductImage productImage = dbContext.productsImage.Where(x => x.ProductId == item.ProductId).First();
                    Image image = dbContext.images.Where(x => x.Id == productImage.ImageId).First();
                    CartShowDTO dto = new CartShowDTO()
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        ProductCategory = category.Name,
                        ProductDescription = product.Description,
                        Quantity = item.Quantity,
                        ProductPrice = product.Price * item.Quantity,
                        imageBit = image.ImageData
                    };
                    returnList.Add(dto);
                }
            }
            ViewBag.ShoppingCart = returnList;

            //user detail
            UserDetail userDetail = dbContext.userDetail.Where(x => x.Id == userId).FirstOrDefault();
            if(userDetail != null)
            {
                ViewBag.UserDetail = userDetail;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddCartItem(int productId)
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
            Product product = dbContext.products.Where(x => x.Id == productId).First();

            //product con` hang`
            if (product.Quantity > 0)
            {
                Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
                if (order != null)
                {
                    //cart exist
                    order.UpdatedDate = DateTime.Now;
                    List<OrderDetail> itemList = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                    if (itemList.Count > 0)
                    {
                        //cart has that product item
                        OrderDetail currentItem = itemList.Where(x => x.ProductId == productId).FirstOrDefault();
                        if (currentItem != null)
                        {

                            if (currentItem.Quantity < product.Quantity)
                            {
                                currentItem.Quantity = currentItem.Quantity + 1;
                                order.Total = order.Total + product.Price;
                            }
                        }
                        else
                        {
                            //co gio hang nhung chua co item do trong gio hang
                            OrderDetail orderDetail = new OrderDetail()
                            {
                                OrderId = order.Id,
                                ProductId = product.Id,
                                Quantity = 1
                            };
                            order.Total+= product.Price;
                            dbContext.ordersDetail.Add(orderDetail);

                        }
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        // cart has no item
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            OrderId = order.Id,
                            ProductId = product.Id,
                            Quantity = 1
                        };
                        order.Total += product.Price;
                        dbContext.ordersDetail.Add(orderDetail);
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    //create 1st time
                    order = new Order()
                    {
                        CustomerId = userId,
                        Status = 0,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Total = product.Price
                    };
                    dbContext.orders.Add(order);
                    dbContext.SaveChanges();

                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        Quantity = 1
                    };

                    dbContext.ordersDetail.Add(orderDetail);
                    dbContext.SaveChanges();

                }
            }
            else
            {
                Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
                List<OrderDetail> itemList = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                foreach (OrderDetail item in itemList)
                {
                    if (item.ProductId == product.Id)
                    {
                        itemList.Remove(item);
                        dbContext.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
                if (itemList.Count <= 0)
                {
                    dbContext.orders.Remove(order);
                    dbContext.SaveChanges();
                }
                // xoa ra khoi gio hang
            }
            TempData["shortMessage"] = "Add Cart Item Success";
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveCartItem(int productId)
        {
            int userId = -1;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            Product product = dbContext.products.Where(x => x.Id == productId).First();
            if (product.Quantity > 0)
            {
                Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
                if (order != null)
                {
                    order.UpdatedDate = DateTime.Now;
                    OrderDetail orderDetail = dbContext.ordersDetail.Where(x => x.OrderId == order.Id && x.ProductId == productId).FirstOrDefault();
                    if(orderDetail != null)
                    {
                        if(orderDetail.Quantity > 1)
                        {
                            orderDetail.Quantity = orderDetail.Quantity - 1;
                            order.Total = order.Total - product.Price;
                        }
                        else
                        {
                            dbContext.ordersDetail.Remove(orderDetail);
                            order.Total = order.Total - product.Price;
                            dbContext.SaveChanges();
                            List<OrderDetail> otherOrder = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                            if(otherOrder.Count <= 0)
                            {
                                dbContext.orders.Remove(order);
                                dbContext.SaveChanges();
                            }
                        }
                        
                    }
                }
            }
            else
            {
                Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
                List<OrderDetail> itemList = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                foreach (OrderDetail item in itemList)
                {
                    if (item.ProductId == product.Id)
                    {
                        itemList.Remove(item);
                        dbContext.Remove(item);
                        dbContext.SaveChanges();
                    }
                }
                if (itemList.Count <= 0)
                {
                    dbContext.orders.Remove(order);
                    dbContext.SaveChanges();
                }
                // xoa ra khoi gio hang
            }
            dbContext.SaveChanges();
            TempData["shortMessage"] = "Remove Cart Item Success";
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCartItem(int productId)
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
            Product product = dbContext.products.Where(x => x.Id == productId).First();
            Order order = dbContext.orders.Where(x => x.CustomerId == userId && x.Status == 0).FirstOrDefault();
            OrderDetail orderDetail = dbContext.ordersDetail.Where(x => x.OrderId == order.Id && x.ProductId == productId).FirstOrDefault();
            dbContext.ordersDetail.Remove(orderDetail);
            dbContext.SaveChanges();
            List<OrderDetail> ordersDetail = dbContext.ordersDetail.Where(x => x.OrderId == orderDetail.OrderId).ToList();
            if(ordersDetail.Count <= 0)
            {
                dbContext.orders.Remove(order);
                dbContext.SaveChanges();
            }
            TempData["shortMessage"] = "Delete Cart Item Success";
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> HomeSale()
        {
            return View();
        }
    }
}