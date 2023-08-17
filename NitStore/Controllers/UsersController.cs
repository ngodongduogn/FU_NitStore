using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Models.Domain;
using NitStore.Models.DTO;
using PagedList;

namespace NitStore.Controllers
{
    public class UsersController : Controller
    {
        private readonly NitDbContext dbContext;

        public UsersController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> ViewAllUser()
        {
            var userList = await dbContext.users.ToListAsync();
            return View(userList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddDTO dto)
        {
            if(string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.UserName))
            {
                ViewBag.Error = "You must fill all field!";
                return View();
            }
            List<Role> roleList = new List<Role>();
            //roleList = = await dbContext.
            var User = new User()
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = 1,
                Status = dto.Status,
                Password = "123456"
            };

            dbContext.users.Add(User);
            dbContext.SaveChanges();
            return RedirectToAction("ViewAllUser", "Users", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if(user != null)
            {
                if(user.Role >= 1 && user.Role <= 5)
                {
                    if(user.Status == 0 || user.Status == 1)
                    {
                        user.Password = EncryptPassword(user.Password);
                        dbContext.users.Add(user);
                        await dbContext.SaveChangesAsync();
                    } else
                    {
                        
                    }
                   
                } else
                {

                }
            }
            return RedirectToAction("ViewAllUser", "Users", new { area = "" });
        }

        private string EncryptPassword(string password)
        {
            string result = "";
            try
            {
                byte[] enCryptByte = new byte[password.Length];
                enCryptByte = System.Text.Encoding.UTF8.GetBytes(password);
                result = Convert.ToBase64String(enCryptByte);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = dbContext.users.Where(x => x.Id == id).First();
            if (user != null)
            {
                dbContext.users.Remove(user);
                dbContext.SaveChangesAsync();
            }
            return RedirectToAction("ViewAllUser", "Users", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = dbContext.users.Where(x => x.Id == id).First();

            if (user != null)
            {
                return View(user);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(User dto)
        {
            User currentUser = dbContext.users.Where(x => x.Id == dto.Id).First();
            currentUser.UserName = dto.UserName;
            currentUser.Status = dto.Status;
            //currentUser.NeedToChange = true;
            //currentUser.Email = dto.UserName;
            //currentUser.Password = "123456";
            if(currentUser.Status == 0 || currentUser.Status == 1)
            {
                dbContext.users.Update(currentUser);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("ViewAllUser", "Users", new { area = "" });
        }

        // GET: UserDetails/Edit/5
        public async Task<IActionResult> EditUserDetail(int? id)
        {
            if (id == null || dbContext.userDetail == null)
            {
                return NotFound();
            }

            var userDetail = await dbContext.userDetail.FindAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }
            return View(userDetail);
        }

        // POST: UserDetails/Edit/5
        [HttpPost]
        public async Task<IActionResult> EditUserDetail(int id, UserDetail userDetail)
        {
            if (id != userDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Update(userDetail);
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDetailExists(userDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Cart", "Home", new { area = "" });
            }
            string shortMessage = "Update user detail success!";
            TempData["shortMessage"] = shortMessage;
            //return RedirectToAction("Cart", "Home");
            return RedirectToAction("Cart", "Home", new { area = "", arg = shortMessage });

        }
        private bool UserDetailExists(int id)
        {
            return (dbContext.userDetail?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
