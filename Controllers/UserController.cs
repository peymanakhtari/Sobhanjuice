using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System.Linq;
using System.Security.Claims;

namespace SobhanJuice.Controllers
{
    [Authorize(Roles = "User", AuthenticationSchemes = "UserAuth")]
    public class UserController : Controller
    {

        public IActionResult AddOrEditAddress(int id)
        {
            if (id == 0)
            {
                return View(new Address() { ID = 0, Coordinate = "", distance = 0, Text = "" });
            }
            else
            {
                using (UnitOfWork db = new UnitOfWork())
                {
                    return View(db.AddressRepository.GetByID(id));
                }
            }
        }
        [HttpPost]
        public IActionResult AddOrEditAddress(Address address)
        {

            using (UnitOfWork db = new UnitOfWork())
            {
                var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
                User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                Order order = db.OrderRepository.Get(c => c.UserId == user.ID && c.Status == 0).FirstOrDefault();
                address.UserId = user.ID;
                if (address.ID == 0)
                {
                    db.AddressRepository.Insert(address);
                    db.Save();
                    if (order != null)
                    {
                        order.AddressId = db.AddressRepository.Get(c => c.Coordinate == address.Coordinate).First().ID;
                        db.OrderRepository.Update(order);
                        db.Save();
                    }
                }
                else
                {
                    db.AddressRepository.Update(address);
                    db.Save();
                }
            }
            return RedirectToAction("ContinueShopping", "Order");
        }
        public IActionResult setAddress(int Id)
        {
            var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db = new UnitOfWork())
            {
                User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                var order = db.OrderRepository.Get(c => c.UserId == user.ID && c.Status == 0).FirstOrDefault();
                if (order != null)
                {
                    if (Id != 0)
                    {
                        order.AddressId = Id;
                    }
                    else
                    {
                        order.AddressId = null;
                    }
                    db.OrderRepository.Update(order);
                    db.Save();
                }
            }
            return Json("");
        }
        [HttpPost]
        public IActionResult AddUserName(string username)
        {
            User user;
            var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db = new UnitOfWork())
            {
                user = db.UserRepository.Get(c => c.Mobile == mobile).First();
            }
            using (UnitOfWork db = new UnitOfWork())
            {
                user.Name = username;
                db.UserRepository.Update(user);
                db.Save();
            }
            return Json("");
        }
        public IActionResult RemoveUserName()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                user.Name = null;
                db.UserRepository.Update(user);
                db.Save();
            }
            return RedirectToAction("ContinueShopping", "Order");
        }
        [HttpPost]
        public IActionResult RefreshOrder()
        {
            var mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db= new UnitOfWork())
            {
               var user=db.UserRepository.Get(c=>c.Mobile == mobile).First();
                if (user.CheckStatus)
                {
                    user.CheckStatus = false;
                    db.UserRepository.Update(user);
                    db.Save();
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
        }
    }
}
