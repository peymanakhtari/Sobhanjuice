using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SobhanJuice.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class Admin2Controller : Controller
    {
        public IActionResult Setting()
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                ViewBag.MinServiceArea = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MinServiceArea").First().Value);
                ViewBag.MaxServiceArea = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxServiceArea").First().Value);
                ViewBag.MinDelivery = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MinDelivery").First().Value);
                ViewBag.MaxDelivery = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxDelivery").First().Value);
                ViewBag.OpenTimeH = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeH").First().Value);
                ViewBag.OpenTimeM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeM").First().Value);
                ViewBag.OpenTimeFridayH = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayH").First().Value);
                ViewBag.OpenTimeFridayM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayM").First().Value);
                ViewBag.CloseTimeH = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "CloseTimeH").First().Value);
                ViewBag.CloseTimeM = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "CloseTimeM").First().Value);
                ViewBag.TemporayClose = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "TemporayClose").First().Value);
                ViewBag.DeliveryService = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "DeliveryService").First().Value);
                ViewBag.minFaktorPrice = db.KeyValueRepository.Get(c => c.Key == "MinimonFaktorPrice").First().Value;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Delivery(int MinServiceArea, int MaxServiceArea, int MinDelivery, int MaxDelivery)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var _MinServiceArea = db.KeyValueRepository.Get(c => c.Key == "MinServiceArea").First();
                _MinServiceArea.Value = MinServiceArea.ToString();
                db.KeyValueRepository.Update(_MinServiceArea);

                var _MaxServiceArea = db.KeyValueRepository.Get(c => c.Key == "MaxServiceArea").First();
                _MaxServiceArea.Value = MaxServiceArea.ToString();
                db.KeyValueRepository.Update(_MaxServiceArea);

                var _MinDelivery = db.KeyValueRepository.Get(c => c.Key == "MinDelivery").First();
                _MinDelivery.Value = MinDelivery.ToString();
                db.KeyValueRepository.Update(_MinDelivery);

                var _MaxDelivery = db.KeyValueRepository.Get(c => c.Key == "MaxDelivery").First();
                _MaxDelivery.Value = MaxDelivery.ToString();
                db.KeyValueRepository.Update(_MaxDelivery);
                db.Save();
            }
            return RedirectToAction("Setting");
        }
        [HttpPost]
        public IActionResult OpenHour(int OpenTimeH, int OpenTimeM, int OpenTimeFridayH, int OpenTimeFridayM, int CloseTimeH, int CloseTimeM)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var _OpenTimeH = db.KeyValueRepository.Get(c => c.Key == "OpenTimeH").First();
                _OpenTimeH.Value = OpenTimeH.ToString();
                db.KeyValueRepository.Update(_OpenTimeH);

                var _OpenTimeM = db.KeyValueRepository.Get(c => c.Key == "OpenTimeM").First();
                _OpenTimeM.Value = OpenTimeM.ToString();
                db.KeyValueRepository.Update(_OpenTimeM);

                var _OpenTimeFridayH = db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayH").First();
                _OpenTimeFridayH.Value = OpenTimeFridayH.ToString();
                db.KeyValueRepository.Update(_OpenTimeFridayH);

                var _OpenTimeFridayM = db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayM").First();
                _OpenTimeFridayM.Value = OpenTimeFridayM.ToString();
                db.KeyValueRepository.Update(_OpenTimeFridayM);

                var _CloseTimeH = db.KeyValueRepository.Get(c => c.Key == "CloseTimeH").First();
                _CloseTimeH.Value = CloseTimeH.ToString();
                db.KeyValueRepository.Update(_CloseTimeH);

                var _CloseTimeM = db.KeyValueRepository.Get(c => c.Key == "CloseTimeM").First();
                _CloseTimeM.Value = CloseTimeM.ToString();
                db.KeyValueRepository.Update(_CloseTimeM);

                db.Save();
            }
            return RedirectToAction("Setting");
        }
        public IActionResult CloseOpenShop(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (id == 1)
                {
                    var val = db.KeyValueRepository.Get(c => c.Key == "TemporayClose").First();
                    val.Value = "0";
                    db.KeyValueRepository.Update(val);
                }
                else
                {
                    var val = db.KeyValueRepository.Get(c => c.Key == "TemporayClose").First();
                    val.Value = "1";
                    db.KeyValueRepository.Update(val);
                }
                db.Save();
            }
            return RedirectToAction("Setting");
        }
        public IActionResult DeliveryService(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                if (id == 1)
                {
                    var val = db.KeyValueRepository.Get(c => c.Key == "DeliveryService").First();
                    val.Value = "0";
                    db.KeyValueRepository.Update(val);
                }
                else
                {
                    var val = db.KeyValueRepository.Get(c => c.Key == "DeliveryService").First();
                    val.Value = "1";
                    db.KeyValueRepository.Update(val);
                }
                db.Save();
            }
            return RedirectToAction("Setting");
        }
        public IActionResult Users(string id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<User> users = new List<User>();
                if (id == "-1")
                {
                    users = db.UserRepository.Get().ToList();
                }
                else if (id == "0")
                {
                    users = null;
                }
                else
                {
                    users = db.UserRepository.Get(c => c.Mobile == id).ToList();
                }
                return View(users);
            }
        }
        [HttpPost]
        public IActionResult updatewallet(int userid,int wallet)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
               var user= db.UserRepository.GetByID(userid);
                user.Wallet = wallet;
                db.UserRepository.Update(user);
                db.Save();
            }
            return RedirectToAction("Users");  
        }
        [HttpPost]
        public IActionResult minFaktorPrice(string minFaktorPrice)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                var value = db.KeyValueRepository.Get(c => c.Key == "MinimonFaktorPrice").First();
                value.Value = minFaktorPrice;
                db.KeyValueRepository.Update(value);
                db.Save();
                return RedirectToAction("Setting");
            }
        }
    }
}
