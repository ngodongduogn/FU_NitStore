using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using NitStore.Data;
using NitStore.Models.Domain;
using NitStore.Models.DTO;

namespace NitStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NitDbContext dbContext;

        public ProductsController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> ListProduct(int? id)
        {
            //Slider
            var sliders = dbContext.slider.Where(s => s.Status == true).ToList();
            List<SliderShowDTO> lsSliderShow = new List<SliderShowDTO>();
            foreach(var slider in sliders)
            {
                var img = dbContext.images.Where(i => i.Id == slider.Image).FirstOrDefault();
                var cmp = dbContext.campaigns.Where(c => c.Id == slider.CampaignId).FirstOrDefault();
                SliderShowDTO result = new SliderShowDTO
                {
                    CampaignId = cmp.Id,
                    CampaignName = cmp.Name,
                    SliderId = slider.Id,
                    ImageData = img.ImageData,
                    Status = slider.Status
                };
                lsSliderShow.Add(result);
            }
            ViewBag.ListSliders = lsSliderShow;
            //End of Slider
            List<Product> productList = new List<Product>();
            if (id != null)
            {
                productList = dbContext.products.Where(x => x.Category == id).ToList();
            }
            else
            {
                productList = dbContext.products.ToList();
            }
            
            List<ProductShowDTO> productShowList = new List<ProductShowDTO>();

            //Category
            List<Category> categoryList = dbContext.categories.ToList(); 
            ViewBag.CategoryList = categoryList;

            foreach (Product item in productList)
            {
                ProductImage productImage = dbContext.productsImage.Where(x => x.ProductId == item.Id).First();
                Image image = dbContext.images.Where(x => x.Id == productImage.ImageId).First();
                ProductShowDTO productShowDTO = new ProductShowDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Status = item.Status,
                    Quantity = item.Quantity,
                    CategoryId = item.Category,
                    CategoryName = categoryList.Where(x => x.Id == item.Category).First().Name,
                    Price = item.Price,
                    imageBit = image.ImageData
                };
                productShowList.Add(productShowDTO);
            }
            ViewBag.ListProduct = productShowList;
            return View();
        }

        public async Task<IActionResult> ViewAllProduct()
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            List<Product> productList = dbContext.products.ToList();
            List<ProductShowDTO> productShowList = new List<ProductShowDTO>();
            List<Category> categoryList = dbContext.categories.ToList();
            ViewBag.CategoryList = categoryList;

            foreach (Product item in productList)
            {
                ProductImage productImage = dbContext.productsImage.Where(x => x.ProductId == item.Id).First();
                Image image = dbContext.images.Where(x => x.Id== productImage.ImageId).First();
                ProductShowDTO productShowDTO = new ProductShowDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Status = item.Status,
                    Quantity = item.Quantity,
                    CategoryId = item.Category,
                    CategoryName = categoryList.Where(x => x.Id == item.Category).First().Name,
                    Price = item.Price,
                    imageBit = image.ImageData
                };
                productShowList.Add(productShowDTO);
            }
            ViewBag.ListProduct = productShowList;
            return View();
        }

        public async Task<IActionResult> AddProduct()
        {
            List<Category> categoryList = dbContext.categories.ToList();
            ProductAddDTO dto = new ProductAddDTO();
            dto.CategoryList = new SelectList(categoryList, "Id", "Name");
            return View(dto);
        }

        [HttpPost]
        public ActionResult AddProduct(ProductAddDTO dto)
        {
            
            List<Category> categoryList = dbContext.categories.ToList();
            ProductAddDTO dtos = new ProductAddDTO();
            dtos.CategoryList = new SelectList(categoryList, "Id", "Name");
            dto.CategoryList = new SelectList(categoryList, "Id", "Name");
            //if (ModelState.IsValid)
            //{
                Product product = new Product()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Status = dto.Status,
                    Quantity = dto.Quantity,
                    Category = dto.CategoryId,
                    Price = dto.Price
                };
                dbContext.products.Add(product);
                dbContext.SaveChanges();
                if (dto.Imagess != null)
                {
                int index = 1;
                    foreach (IFormFile file in dto.Imagess)
                    {
                        
                        Image image = new Image();
                        byte[] bytes = ConvertToBytes(file);
                        image.ImageData = bytes;
                        image.Description = "Product_" + dto.Name + "_" + index;
                        index++;
                        dbContext.images.Add(image);
                        dbContext.SaveChanges();

                        ProductImage productImage = new ProductImage()
                        {
                            ProductId = product.Id,
                            ImageId = image.Id
                        };

                        dbContext.productsImage.Add(productImage);
                        dbContext.SaveChanges();
                    } 
                }
            //}
            TempData["shortMessage"] = "Add Product Success!";
            return RedirectToAction("ViewAllProduct");
            //return View(dtos);
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

        //private IFormFile ConvertToIFormFile(byte[] data)
        //{
        //    Stream stream = data.OpenReadStream();
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        stream.CopyTo(memoryStream);
        //        return memoryStream.ToArray();
        //    }
        //}

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return View(await dbContext.products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> ProductDetail(int? id)
        {
            List<Category> categoryList = dbContext.categories.ToList();
            ViewBag.CategoryList = categoryList;
            if (id == null || dbContext.products == null)
            {
                return NotFound();
            }

            var product = await dbContext.products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            List<ProductImage> productImage = dbContext.productsImage.Where(x => x.ProductId == product.Id).ToList();
            List<Image> imageList = new List<Image>();
            foreach(ProductImage item in productImage)
            {
                Image image = dbContext.images.Where(x => x.Id == item.ImageId).First();
                imageList.Add(image);
            }
            List<Feedback> feedbackList = dbContext.feedbacks.Where(x => x.ProductId == id).OrderByDescending(x => x.UpdatedDate).ToList();
            List<FeedbackOrderDTO> feedsOrderDTO = new List<FeedbackOrderDTO>();
            foreach(Feedback feedbackss in feedbackList)
            {
                UserDetail userDetail = dbContext.userDetail.Where(x => x.Id == feedbackss.CustomerId).FirstOrDefault();
                FeedbackOrderDTO dto = new FeedbackOrderDTO()
                {
                    CustomerName = userDetail.Name,
                    feedback = feedbackss.Description,
                    DateFeedback = feedbackss.UpdatedDate.ToString(),
                    Rate = feedbackss.Rate
                };
                feedsOrderDTO.Add(dto);
            }
            ViewBag.Product = product;
            ViewBag.Images = imageList;
            ViewBag.Feedbacks = feedsOrderDTO;
            return View();
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(product);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || dbContext.products == null)
            {
                return NotFound();
            }

            var product = await dbContext.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            if (!(TempData["shortMessage"] ?? "").ToString().IsNullOrEmpty())
            {
                ViewBag.Message = TempData["shortMessage"].ToString();
            }
            if (id == null || dbContext.products == null)
            {
                return NotFound();
            }

            var product = await dbContext.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            List<Category> categoryList = dbContext.categories.ToList();
            List<ProductImage> list = dbContext.productsImage.Where(x => x.ProductId == id).ToList();
            List<byte[]> forms = new List<byte[]>();
            List<int> imageIdList = new List<int>();
            foreach(ProductImage productImage in list)
            {
                Image image = dbContext.images.Where(x => x.Id == productImage.ImageId).FirstOrDefault();
                if (image != null)
                {
                    forms.Add(image.ImageData);
                    imageIdList.Add(image.Id);
                }
            }
            ProductEditDTO dto = new ProductEditDTO()
            {
                Id = product.Id,
                Name= product.Name,
                Description= product.Description,
                Status= product.Status,
                Quantity= product.Quantity,
                CategoryId  = product.Category,
                Price= product.Price,
                CategoryList = new SelectList(categoryList, "Id", "Name"),
                imageBit = forms,
                imageIds= imageIdList
            };
            ViewBag.Product = dto;
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductEditDTO dto)
        {
            Product p = dbContext.products.Where(x => x.Id == dto.Id).FirstOrDefault();
            if (p != null)
            {
                p.Price = dto.Price;
                p.Quantity = dto.Quantity;
                p.Status = dto.Status;
                p.Category = dto.CategoryId;
                p.Description = dto.Description;
                p.Name = dto.Name;
                dbContext.SaveChanges();
            }
            List<Category> categoryList = dbContext.categories.ToList();
            if (dto.Imagess != null)
            {
                List<ProductImage> imageList = dbContext.productsImage.Where(x => x.ProductId == dto.Id).ToList();
                
                int index = 1;
                if(imageList.Count > 0)                 
                {
                    index = imageList.Count + 1;
                }
                foreach (IFormFile file in dto.Imagess)
                {

                    Image image = new Image();
                    byte[] bytes = ConvertToBytes(file);
                    image.ImageData = bytes;
                    image.Description = "Product_" + dto.Name + "_" + index;
                    index++;
                    dbContext.images.Add(image);
                    dbContext.SaveChanges();

                    ProductImage productImage = new ProductImage()
                    {
                        ProductId = dto.Id,
                        ImageId = image.Id
                    };

                    dbContext.productsImage.Add(productImage);
                    dbContext.SaveChanges();
                }
            }
            TempData["shortMessage"] = "Update Product success!";
            return RedirectToAction("EditProduct", new { id = dto.Id });
        }
        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(product);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || dbContext.products == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await dbContext.products
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        // POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (dbContext.products == null)
            {
                return Problem("Entity set 'NitDbContext.products'  is null.");
            }
            var product = await dbContext.products.FindAsync(id);
            if (product != null)
            {
                List<CampaignItem> campaignItems = await dbContext.campaignItems.Where(x => x.ProductId == product.Id).ToListAsync();
                foreach(var campaignItem in campaignItems)
                {
                    dbContext.campaignItems.Remove(campaignItem);
                }
                List<Feedback> feedbacks = await dbContext.feedbacks.Where(x => x.ProductId == product.Id).ToListAsync();
                foreach(var feedback in feedbacks)
                {
                    dbContext.feedbacks.Remove(feedback);
                }
                List<ProductImage> listProdctImage = await dbContext.productsImage.Where(x => x.ProductId == product.Id).ToListAsync();
                foreach(var productImage in listProdctImage)
                {
                    Image img = await dbContext.images.Where(x => x.Id == productImage.ImageId).FirstAsync();
                    
                    dbContext.productsImage.Remove(productImage);
                    dbContext.SaveChanges();
                    dbContext.images.Remove(img);

                }
                List<OrderDetail> orderDetail = await dbContext.ordersDetail.Where(x => x.ProductId ==  product.Id).ToListAsync();
                foreach(var items in orderDetail)
                {
                    dbContext.ordersDetail.Remove(items);
                    dbContext.SaveChanges();
                    Order order = dbContext.orders.Where(x => x.Id == items.OrderId).FirstOrDefault();
                    List<OrderDetail> orderDetailsLeft = dbContext.ordersDetail.Where(x => x.OrderId == order.Id).ToList();
                    if(orderDetailsLeft.Count() <= 0) 
                    {
                        dbContext.orders.Remove(order);
                        dbContext.SaveChanges();
                    }
                }
                dbContext.SaveChanges();
                dbContext.products.Remove(product);
                dbContext.SaveChanges();
            }
            
            await dbContext.SaveChangesAsync();
            TempData["shortMessage"] = "Remove product successfull";
            return RedirectToAction("ViewAllProduct");
        }

        private bool ProductExists(int id)
        {
          return dbContext.products.Any(e => e.Id == id);
        }
    }
}
