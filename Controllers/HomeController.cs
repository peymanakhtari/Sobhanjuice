using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using SobhanJuice.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SobhanJuice.Models.ViewModel;

namespace SobhanJuice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //  Email.Send("test", "test");
            // Utilities.Email.SendEmailAsync("peymanakhtari@outlook.com", "testsub", "message text");
            //   Utilities.Email.SendEmailAsync("peyman.firelance@gmail.com", "testsub", "message text");
            List<Product> ProductLst;
            List<string> Product_Runout_id = new List<string>();
            List<Category> categories = new List<Category>();
            using (UnitOfWork db = new UnitOfWork())
            {
                ProductLst = db.ProductRepository.Get().OrderBy(c => c.Sequence).ToList();
                foreach (var item in db.ProductRepository.Get(p => p.Exist == false))
                {
                    Product_Runout_id.Add(item.ID.ToString());
                }
                categories = db.CategoryRepository.Get().OrderBy(c => c.Code).ToList();
                var OpenH = db.KeyValueRepository.Get(c => c.Key == "OpenTimeH").First().Value;
                var openM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeM").First().Value);
                ViewBag.OpenTime = OpenH + ((openM < 10) ? "" : ":" + openM.ToString());

                var OpenFridayH = db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayH").First().Value;

                var openfridayM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayM").First().Value);

                ViewBag.OpenFriday = OpenFridayH + ((openfridayM < 10) ? "" : ":" + openfridayM.ToString());

                var CloseH = db.KeyValueRepository.Get(c => c.Key == "CloseTimeH").First().Value;

                var closeM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "CloseTimeM").First().Value);
                ViewBag.CloseTime = CloseH + ((closeM < 10) ? "" : ":" + closeM.ToString());

            }

            string[] BasketId;
            string[] BasketCount;
            if (Request.Cookies["basket-id"] != null)
            {
                BasketId = Request.Cookies["basket-id"].Split("-");
                BasketCount = Request.Cookies["basket-count"].Split("-");
                var listCount = BasketCount.ToList();
                var listId = BasketId.ToList();
                foreach (var item in Product_Runout_id)
                {
                    int indexOfListId = listId.IndexOf(item);
                    if (indexOfListId >= 0)
                    {
                        listId.RemoveAt(indexOfListId);
                        listCount.RemoveAt(indexOfListId);
                    }
                }
                BasketId = listId.ToArray();
                BasketCount = listCount.ToArray();
                var BasketId_string = string.Join("-", BasketId);
                var BasketCount_string = string.Join("-", BasketCount);
                if (listId.Count > 0)
                {
                    BasketCount = listCount.ToArray();
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Append("basket-id", BasketId_string, option);
                    Response.Cookies.Append("basket-count", BasketCount_string, option);
                }
                else
                {
                    Response.Cookies.Delete("basket-id");
                    Response.Cookies.Delete("basket-count");

                }
                ViewBag.BasketId_string = BasketId_string;
                ViewBag.BasketCount_string = BasketCount_string;
            }
            ViewBag.open = Utilities.Utility.CheckOpen();
            ViewBag.categories = categories;
            return View(ProductLst);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendConfirmationCode(string Mobile)
        {
            try
            {
                Random r = new Random();
                string rInt = r.Next(1000, 9999).ToString();
                HttpContext.Session.SetString("mobile", Mobile);
                HttpContext.Session.SetString("code", rInt);
                SendSMS.ConfirmationCode(Mobile, rInt);
                return Json("");
            }
            catch (System.Exception e)
            {
                Email.Send("SendConfirmationCode", e.ToString());
                return Json(e.ToString());
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginFromBasket(string Code)
        {

            try
            {
                string mobile = HttpContext.Session.GetString("mobile");
                if (Code == HttpContext.Session.GetString("code"))
                {

                    await LoginUserAsync(mobile);
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (System.Exception e)
            {
                // Email.Send("LoginFromBasket", e.ToString());
                return Json(e.ToString());
            }

        }
        [HttpPost]
        public IActionResult ResendConfirmationCode()
        {
            Random r = new Random();
            string rInt = r.Next(1000, 9999).ToString();
            HttpContext.Session.SetString("code", rInt);
            SendSMS.ConfirmationCode(HttpContext.Session.GetString("mobile"), rInt);
            return Json("");
        }
        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut    
            await HttpContext.SignOutAsync("UserAuth");
            //Redirect to home page    
            return RedirectToAction("Index");
        }
        public IActionResult GetCookieOnLoad()
        {
            string[] BasketId;
            string[] BasketCount;
            if (Request.Cookies["basket-id"] != null)
            {
                BasketId = Request.Cookies["basket-id"].Split("-");
                BasketCount = Request.Cookies["basket-count"].Split("-");
                return Json(new { BasketId, BasketCount });
            }
            else
            {
                return Json(null);
            }

        }
        [HttpPost]
        public IActionResult EditeShoppingBasket(int id, string edit)
        {
            string[] BasketId;
            string[] BasketCount;
            CookieOptions option = new CookieOptions();
            bool deleteCookie = false;
            if (Request.Cookies["basket-id"] == null)
            {
                if (edit == "add")
                {
                    option.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Append("basket-id", id.ToString(), option);
                    Response.Cookies.Append("basket-count", "1", option);
                }
                BasketId = new string[1] { id.ToString() };
                BasketCount = new string[1] { "1" };
                //return Json(new { BasketId, BasketCount });
                return Json(true);
            }
            else
            {
                BasketId = Request.Cookies["basket-id"].Split("-");
                BasketCount = Request.Cookies["basket-count"].Split("-");
                var listId = BasketId.ToList();
                var listCount = BasketCount.ToList();
                if (edit == "add")
                {
                    // if item already exist in basket just increase
                    if (BasketId.Any(c => c == id.ToString()))
                    {
                        for (int i = 0; i < BasketId.Length; i++)
                        {
                            if (BasketId[i] == id.ToString())
                            {
                                var count = Convert.ToInt32(BasketCount[i]);
                                count += 1;
                                BasketCount[i] = count.ToString();
                            }

                        }
                    }
                    // add item to basket
                    else
                    {
                        listId.Add(id.ToString());
                        BasketId = listId.ToArray();
                        listCount.Add("1");
                        BasketCount = listCount.ToArray();

                    }
                }
                else if (edit == "lowOff")
                {
                    if (listId.Count == 1)
                    {
                        deleteCookie = true;
                    }

                    for (int i = 0; i < BasketId.Length; i++)
                    {

                        if (BasketId[i] == id.ToString())
                        {
                            if (Convert.ToInt32(BasketCount[i]) > 1)
                            {
                                var count = Convert.ToInt32(BasketCount[i]);
                                count -= 1;
                                BasketCount[i] = count.ToString();
                            }
                            else
                            {
                                listId.RemoveAt(i);
                                listCount.RemoveAt(i);
                                BasketCount = listCount.ToArray();
                                BasketId = listId.ToArray();
                            }
                        }

                    }
                }

            }
            if (deleteCookie)
            {
                Response.Cookies.Delete("basket-id");
                Response.Cookies.Delete("basket-count");
            }
            else
            {
                option.Expires = DateTime.Now.AddYears(1);
                Response.Cookies.Append("basket-id", string.Join("-", BasketId), option);
                Response.Cookies.Append("basket-count", string.Join("-", BasketCount), option);
            }
            return Json(true);
        }
        [HttpPost]
        public IActionResult RemoveBasket()
        {
            Response.Cookies.Delete("basket-id");
            Response.Cookies.Delete("basket-count");
            return Json("");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult emailError(string subject, string body)
        {
            Email.Send(subject, body);
            return Json("");
        }
        public IActionResult testCoook()
        {
            Response.Cookies.Append("basket-id", "-");
            Response.Cookies.Append("basket-count", "-");
            return RedirectToAction("index");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Random r = new Random();
                string rInt = r.Next(1000, 9999).ToString();
                HttpContext.Session.SetString("mobile", model.number);
                HttpContext.Session.SetString("code", rInt);
                SendSMS.ConfirmationCode(model.number, rInt);
                return RedirectToAction("ConfirmationCode");
            }
            return View();
        }
        public IActionResult ConfirmationCode()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmationCodeAsync(string Code)
        {

            string mobile = HttpContext.Session.GetString("mobile");
            if (Code == HttpContext.Session.GetString("code"))
            {

                await LoginUserAsync(mobile);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.message = "کد وارد شده اشتباه است";
                return View();
            }
        }
        public async Task LoginUserAsync(string mobile)
        {
            var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, mobile ),
                    new Claim(ClaimTypes.Role,"User")
                };
            var identity = new ClaimsIdentity(claims, "UserAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("UserAuth", principal, new AuthenticationProperties()
            {
                IsPersistent = true
            });

            using (UnitOfWork db = new UnitOfWork())
            {
                if (!db.UserRepository.Get(c => c.Mobile == mobile).Any())
                {
                    db.UserRepository.Insert(new Models.User { Mobile = mobile });
                    db.Save();
                }
            }
        }
        public IActionResult ContactUs()
        {
            return View();
        }
    }
}
//ارسال پترن
//WebRequest request = HttpWebRequest.Create("http://ippanel.com:8080/?apikey=H7cP-G4k_2aI6OYAPRL1ZN-XER8Tl3Yciqz5kt3YlNs=&pid=spdxtia3y6&fnum=+983000505&tnum=09171898681&p1=name&p2=verification-code&v1=ippanel&v2=1596");
//WebResponse response = request.GetResponse();
