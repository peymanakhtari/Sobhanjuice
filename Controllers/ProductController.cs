using Microsoft.AspNetCore.Mvc;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SobhanJuice.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index(int id)
        {
            var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db=new UnitOfWork())
            {
                bool writeComment = false;
                if (mobile!=null)
                {
                    User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                    var orders = db.OrderRepository.Get(c => (c.UserId == user.ID) && (c.Status == 5 || c.Status == 2)).ToList();
                    var orderDetail = Utilities.Utility.GetOrders(orders);
                    foreach (var item in orderDetail)
                    {
                        if (item.Details.Any(c => c.ProductId == id))
                        {
                            writeComment = true;
                        }
                    }
                }
                var comments = db.CommentRepository.Get(c => c.ProductId == id&&c.Text!=null&&c.Show).ToList();
                ViewBag.comments = comments;
                var Comments = db.CommentRepository.Get(c => c.ProductId == id && c.Show);
              
                if (Comments.Count()!=0)
                {
                    double scoreSum = 0;
                    foreach (var item in Comments)
                    {
                        scoreSum += item.Score;
                    }
                    double average = scoreSum / Comments.Count();
                    average = Math.Round(average, 1);
                    ViewBag.average = average;  
                }
               
                ViewBag.writeComment = writeComment;
                var product = db.ProductRepository.GetByID(id);
                return View(product);
            }
        }
        [HttpPost]
        public IActionResult SubmitComment(int rate,string text,int productId)
        {
            var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db=new UnitOfWork())
            {
                if (mobile!=null)
                {
                    var user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                    db.CommentRepository.Insert(new Comment { ProductId = productId, Score = rate, Show = false, Text = text, UserId = user.ID,Name=user.Name });
                    db.Save();
                }
            }
            return Json("");
        }
    }
}
