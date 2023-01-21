using Microsoft.AspNetCore.Mvc;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System.Linq;
using System.Security.Claims;

namespace SobhanJuice.Controllers
{
    public class PeymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UpdateOrder(int id)
        {
            string mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db=new UnitOfWork())
            {
                User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                Order order=db.OrderRepository.GetByID(id);
                user.Wallet = 0;
                order.Status = 1;
                db.OrderRepository.Update(order);
                db.UserRepository.Update(user);
                db.Save();
            }
            return RedirectToAction("Index","Order");
        }
    }
}
