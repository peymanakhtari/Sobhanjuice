using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SobhanJuice.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class AdminController : Controller
    {

        private IWebHostEnvironment _hostingEnvironment;
        public AdminController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        public IActionResult Index()
        {

            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (db.KeyValueRepository.Get(c => c.Key == "adminUser" && c.Value == username).Any() && db.KeyValueRepository.Get(c => c.Key == "adminPass" && c.Value == password).Any())
                {
                    var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, username ),
                    new Claim(ClaimTypes.Role,"Admin")
                };
                    var identity = new ClaimsIdentity(claims, "AdminAuth");

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("AdminAuth", principal, new AuthenticationProperties()
                    {
                        IsPersistent = true
                    });
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Login");
        }
        public IActionResult Products(int CategoryId)
        {
            List<Category> categories = new List<Category>();
            List<Product> products = new List<Product>();
            using (UnitOfWork db = new UnitOfWork())
            {
                categories = db.CategoryRepository.Get().OrderBy(c => c.Code).ToList();
                if (CategoryId != 0)
                {
                    products = db.ProductRepository.Get(c => c.CategoryId == CategoryId).ToList();
                }
                products = db.ProductRepository.Get(c => c.CategoryId == CategoryId).OrderBy(c => c.Sequence).ToList();
            }
            ViewBag.CategoryId = CategoryId;
            ViewBag.categories = categories;
            return View(products);
        }
        public IActionResult AddOrEditProduct(int Id)
        {
            List<Category> categories = new List<Category>();
            Product product = new Product();

            using (UnitOfWork db = new UnitOfWork())
            {
                categories = db.CategoryRepository.Get().OrderBy(c => c.Code).ToList();
                if (Id != 0)
                {
                    product = db.ProductRepository.GetByID(Id);
                }
            }
            ViewBag.categories = categories;
            return View(product);

        }
        [HttpPost]
        public IActionResult AddOrEditProduct(int Id, string name, string description, int categoryId, int price, int discount,
            bool exist, string imgBase64, int Box, bool ConsiderBoxPrice, int Sequence)
        {
            int finalPrice = price - price * discount / 100;
            Product product = new Product()
            {
                Name = name,
                Description = description,
                CategoryId = categoryId,
                Price = price,
                Discount = discount,
                FinalPrice = finalPrice,
                Exist = exist,
                Box = Box,
                ConsiderBoxPrice = ConsiderBoxPrice,
                Sequence = Sequence
            };

            using (UnitOfWork db = new UnitOfWork())
            {
                if (Id == 0)
                {

                    product.Picture = Encoding.ASCII.GetBytes(imgBase64);
                    db.ProductRepository.Insert(product);
                    db.Save();
                    return RedirectToAction("Products");
                }
                else
                {
                    product.ID = Id;
                    if (imgBase64 != null)
                    {
                        product.Picture = Encoding.ASCII.GetBytes(imgBase64);
                        db.ProductRepository.Update(product);
                    }
                    else
                    {
                        using (UnitOfWork db1 = new UnitOfWork())
                        {
                            product.Picture = db1.ProductRepository.GetByID(Id).Picture;
                        }
                        db.ProductRepository.Update(product);
                    }
                    db.Save();
                    return RedirectToAction("Products", new { CategoryId = categoryId });
                }
            }

        }
        public  IActionResult DeleteProduct(int id)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                db.ProductRepository.Delete(id);
                db.Save();
                return RedirectToAction("Products");
            }
        }
        public IActionResult Categories()
        {
            List<Category> categories;
            using (UnitOfWork db = new UnitOfWork())
            {
                categories = db.CategoryRepository.Get().ToList();
            }
            var OrderedCategories = categories.OrderBy(c => c.Code).ToList();
            return View(OrderedCategories);
        }
        public IActionResult AddOrEditCategory(int Id)
        {
            Category category = new Category();
            category.ID = 0;
            using (UnitOfWork db = new UnitOfWork())
            {
                if (Id != 0)
                {
                    category = db.CategoryRepository.GetByID(Id);
                }
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult AddOrEditCategory(Category category, string imgBase64)
        {
            byte[] Image;
            if (imgBase64 != null)
            {
                Image = Encoding.ASCII.GetBytes(imgBase64);
                category.Picture = Image;
            }
            using (UnitOfWork db = new UnitOfWork())
            {
                if (category.ID == 0)
                {
                    db.CategoryRepository.Insert(category);
                }
                else
                {
                    if (imgBase64 == null)
                    {
                        using (UnitOfWork db1 = new UnitOfWork())
                        {
                            category.Picture = db1.CategoryRepository.GetByID(category.ID).Picture;
                        }
                    }
                    db.CategoryRepository.Update(category);
                }
                db.Save();
            }
            return RedirectToAction("Categories");
        }
        public IActionResult changeExistance(int id)
        {
            Product product;
            using (UnitOfWork db = new UnitOfWork())
            {
                product = db.ProductRepository.GetByID(id);
                bool exist = product.Exist ? false : true;
                product.Exist = exist;
                db.ProductRepository.Update(product);
                db.Save();
            }
            return RedirectToAction("Products", new { CategoryId = product.CategoryId });
        }
        public IActionResult Comments(int id, int show)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                ViewBag.productId = id;
                IEnumerable<Comment> comments;
                if (id != 0)
                {
                    comments = db.CommentRepository.Get(c => c.ProductId == id);
                }
                else
                {
                    comments = db.CommentRepository.Get();
                }
                foreach (var item in comments)
                {
                    item.Product = db.ProductRepository.GetByID(item.ProductId);
                }
                if(show==1)
                {
                    comments = comments.Where(c => c.Show).ToList();
                    return View(comments.ToList());
                }
                else if (show==-1)
                {
                    comments = comments.Where(c => !c.Show).ToList();
                    return View(comments.ToList());
                }
                else
                {
                    return View(comments.ToList());
                }
            }
        }
        [HttpPost]
        public IActionResult ChangeCommentShow(int id, bool show)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var comment = db.CommentRepository.GetByID(id);
                comment.Show = show;
                db.CommentRepository.Update(comment);
                db.Save();
            }
            return Json("");
        }
        public ActionResult ProductList(int id)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                List<Product> products = new List<Product>();
                if (id==0)
                {
                    products = db.ProductRepository.Get().ToList();
                }
                if (id == 1)
                {
                    products=db.ProductRepository.Get(c=>c.Exist).ToList();
                }
                if (id==-1)
                {
                    products = db.ProductRepository.Get(c => !c.Exist).ToList();
                }
                return View(products);
            }
        }
        [HttpPost]
        public IActionResult changeExistAjax(int id,bool exist)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                var product = db.ProductRepository.GetByID(id);
                product.Exist = exist;
                db.ProductRepository.Update(product);
                db.Save();
                return Json(!exist);
            }
        }
    }
}
