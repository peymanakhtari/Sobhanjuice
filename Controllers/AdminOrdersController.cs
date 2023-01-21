using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SobhanJuice.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "AdminAuth")]
    public class AdminOrdersController : Controller
    {


        public IActionResult GetOrders()
        {
            try
            {
                using (UnitOfWork db = new UnitOfWork())
                {
                    var orders = db.OrderRepository.Get(c => c.Status == 1 || c.Status == 2 || c.Status == -1).ToList();
                    var ordersList = Utilities.Utility.GetOrders(orders);
                    var json = JsonConvert.SerializeObject(ordersList, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    return Json(json);
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public IActionResult confirmOrder(int time, int OrderId, int userId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var order = db.OrderRepository.GetByID(OrderId);
                order.Duration = time;
                order.Status = 2;
                db.OrderRepository.Update(order);
                db.Save();
                Utilities.Utility.UpdateUser(userId);
                return RedirectToAction("Index", "Admin");
            }
        }
        public IActionResult EditOrder(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<Product> products = db.ProductRepository.Get().ToList();
                Order order = db.OrderRepository.GetByID(id);
                List<OrderDetail> orderDetail = db.OrderDetailRepository.Get(c => c.OrderId == id).ToList();
                List<OrderDetailViewModel> orderProducts = new List<OrderDetailViewModel>();
                foreach (var item in orderDetail)
                {
                    var product = db.ProductRepository.GetByID(item.ProductId);
                    orderProducts.Add(new OrderDetailViewModel { Product = product, Count = item.Count });
                }
                ViewBag.products = products;
                ViewBag.orderProducts = orderProducts;
                ViewBag.orderId = order.ID;
                ViewBag.userId = order.UserId;
                return View();

            }
        }
        [HttpPost]
        public IActionResult EditOrder(int OrderId, int userId)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var products = db.ProductRepository.Get().ToList();
                var editedOrderDetail = new List<OrderDetailViewModel>();
                foreach (var item in products)
                {
                    var val = Request.Form[item.ID.ToString()];
                    if (val != "0")
                    {
                        editedOrderDetail.Add(new OrderDetailViewModel { Product = db.ProductRepository.GetByID(item.ID), Count = Convert.ToInt32(val) });
                    }
                }
                Order Order = db.OrderRepository.GetByID(OrderId);
                //edite order detail in database
                List<OrderDetail> orderDetails = db.OrderDetailRepository.Get(c => c.OrderId == OrderId).ToList();
                foreach (var item in orderDetails)
                {
                    db.OrderDetailRepository.Delete(item.ID);
                }
                db.Save();
                foreach (var item in editedOrderDetail)
                {
                    db.OrderDetailRepository.Insert(new OrderDetail { OrderId = OrderId, ProductId = item.Product.ID, Count = item.Count });
                }
                db.Save();

                int price = 0;
                int discount = 0;
                int Box = 0;
                int BoxPriceConsider = 0;
                int FinalBoxPrice = 0;
                foreach (var item in editedOrderDetail)
                {
                    price += item.Product.FinalPrice * item.Count;
                    Box += item.Product.Box * item.Count;
                    if (item.Product.ConsiderBoxPrice)
                    {
                        BoxPriceConsider += item.Product.Box * item.Count;
                    }
                    discount += (item.Product.Price - item.Product.FinalPrice) * item.Count;
                }
                if (Order.PickUp)
                {
                    FinalBoxPrice = BoxPriceConsider;
                }
                else
                {
                    FinalBoxPrice = Box;
                }
                int oldPrice = Order.TotalPrice;
                int newPrice = price + Order.DeliveryPrice + FinalBoxPrice;
                User user = db.UserRepository.GetByID(userId);
                user.Wallet = user.Wallet + oldPrice - newPrice;

                if (user.Wallet < 0)
                {
                    Order.Status = -1;
                }
                else
                {
                    if (Order.Status==-1)
                    {
                        Order.Status = 1;
                    }
                }
                db.UserRepository.Update(user);
                Order.Price = price;
                Order.TotalPrice = newPrice;
                Order.BoxPrice = FinalBoxPrice;
                Order.Discount = discount;
                db.OrderRepository.Update(Order);
                db.Save();
                Utilities.Utility.UpdateUser(userId);

            }

            return RedirectToAction("index", "admin");
        }
        public IActionResult Orders(DateTime from, DateTime until)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<Order> Orders = db.OrderRepository.Get(c => c.DateTime >= from && c.DateTime <= until).OrderByDescending(c=>c.DateTime).ToList();
                var orders = Utilities.Utility.GetOrders(Orders);
                return View(orders);
            }

        }
        public IActionResult DismissOrder(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var order = db.OrderRepository.GetByID(id);
                order.Status = 5;
                db.OrderRepository.Update(order);
                Utilities.Utility.UpdateUser(order.UserId);
                db.Save();
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult PrintOrder(int id)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
               var or= db.OrderRepository.GetByID(id);
                var order = new List<Order>();
                order.Add(or);
                var orderInfo = Utilities.Utility.GetOrders(order);
                return View(orderInfo.First());
            }
        }
        [HttpPost]
        public IActionResult CloseOrder()
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                var orders = db.OrderRepository.Get(c => c.Status == 2);
                bool reload = false;
                foreach (var item in orders)
                {
                    if (item.DateTime.AddMinutes(item.Duration).AddMinutes(15)<DateTime.Now)
                    {
                        item.Status = 5;
                        reload = true;
                        db.OrderRepository.Update(item);
                    }
                }
                db.Save();
                return Json(reload);
            }
          
        }
        public IActionResult CheckNewOrder()
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                if (db.OrderRepository.Get(c=>c.Status==1).Count()>0)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }
    }
}
