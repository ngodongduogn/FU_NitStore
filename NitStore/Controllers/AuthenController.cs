using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Models.Domain;
using NitStore.Models.DTO;

namespace NitStore.Controllers
{
    public class AuthenController : Controller
    {
        private readonly NitDbContext dbContext;
        public AuthenController(NitDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User loginUser)
        {
            if (loginUser != null)
            {
                string username = loginUser.UserName;
                string password = EncryptPassword(loginUser.Password);
                var user = await dbContext.users.Where(u => u.UserName.Equals(username) && u.Password.Equals(password)).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (user.Status == 0)
                    {

                    }
                    else 
                    {
                        string action = "";
                        string method = "";
                        switch(user.Role)
                        {
                            case 1:
                                method = "ViewAllUser";
                                action = "Users";
                                break;
                            case 2:
                                method = "Index";
                                action = "Categories";
                                break;
                            case 3:
                                method = "Index";
                                action = "Campaigns";
                                break;
                            case 4:
                                method = "HomeSale";
                                action = "Home";
                                break;
                            case 5:
                                method = "ListProduct";
                                action = "Products";
                                var userDetail = dbContext.userDetail.Where(ud => ud.Id == user.Id).FirstOrDefault();
                                if(userDetail != null)
                                {
                                    HttpContext.Session.SetString("Username", userDetail.Name);
                                }

                                break;

                        }
                        HttpContext.Session.SetString("UserId", user.Id + "");
                        HttpContext.Session.SetInt32("LoginRole", user.Role);
                        HttpContext.Session.SetInt32("isLogin", 1);
                        return RedirectToAction(method, action, new { area = "" });
                    }

                }else
                {
                    ViewBag.ErrorMessage = "Username or password invalid!";
                }
            }
            else
            {
                return View();
            }
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDTO user)
        {
            if(ModelState.IsValid)
            {
                if (user != null)
                {
                    if (user.Password.Equals(user.RePassword))
                    {
                        User newUser = new User
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Password = EncryptPassword(user.Password),
                            Role = 5,
                            Status = 1
                        };
                        try
                        {
                            dbContext.users.Add(newUser);
                            await dbContext.SaveChangesAsync();
                            UserDetail userDetail = new UserDetail
                            {
                                Id = newUser.Id,
                                Name = user.Name,
                                Address = user.Address,
                                PhoneNumber = user.PhoneNumber,
                                Gender = user.Gender
                            };
                            dbContext.userDetail.Add(userDetail);
                            await dbContext.SaveChangesAsync();
                            return RedirectToAction("Login");
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                            return View();
                        }
                    }
                    else
                    {
                        return View(user);
                    }
                }
                else
                {
                    return View();
                }
            } else
            {
                return View(user);
            }
            
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
    }
}
