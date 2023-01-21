using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using SobhanJuice.Utilities;
using Microsoft.AspNetCore.Http;

namespace SobhanJuice.Controllers
{
    [Authorize(Roles = "User", AuthenticationSchemes = "UserAuth")]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            string mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db = new UnitOfWork())
            {
                User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                var orderlist = db.OrderRepository.Get(c => c.UserId == user.ID && c.Status != 0).ToList();
                List<OrderModel> Orders = Utilities.Utility.GetOrders(orderlist).OrderBy(c => c.Order.Status).ThenByDescending(c => c.Order.DateTime).ToList();
                ViewBag.UserHasActiveOrder = orderlist.Any(c => c.Status == 1 || c.Status == -1 || c.Status == 2);
                return View(Orders);
            }

        }
        public IActionResult ContinueShopping()
        {
            string mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                string[] BasketId;
                string[] BasketCount;
                BasketId = Request.Cookies["basket-id"].Split("-");
                BasketCount = Request.Cookies["basket-count"].Split("-");

                var listCount = BasketCount.ToList();
                var listId = BasketId.ToList();

                User _user;
                Order _order;

                using (UnitOfWork db = new UnitOfWork())
                {
                    if(Utilities.Utility.CheckOpen()!= 1)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    _user = db.UserRepository.Get(c => c.Mobile == mobile).FirstOrDefault();
                    //check if user has submited name
                    if (_user.Name != null)
                    {
                        ViewBag.name = _user.Name;
                    }
                    //check if user has more than one order and delete order and order details
                    var orderlist = db.OrderRepository.Get(c => c.UserId == _user.ID && c.Status == 0);
                    if (orderlist.Count() > 1)
                    {
                        foreach (var item in orderlist)
                        {
                            var orderDetail = db.OrderDetailRepository.Get(c => c.OrderId == item.ID);
                            foreach (var detail in orderDetail)
                            {
                                db.OrderDetailRepository.Delete(detail.ID);
                            }
                            db.OrderRepository.Delete(item.ID);
                        }
                        db.Save();
                        return RedirectToAction("Index", "home");
                    }
                    _order = orderlist.FirstOrDefault();

                    // if user allready has order delete order detail
                    if (_order != null)
                    {
                        var orderDetails = db.OrderDetailRepository.Get(c => c.OrderId == _order.ID).ToList();
                        foreach (var item in orderDetails)
                        {
                            db.OrderDetailRepository.Delete(item.ID);
                            db.Save();
                        }
                    }

                    //if user doesnt have order then insert
                    else
                    {
                        db.OrderRepository.Insert(new Order { UserId = _user.ID, DeliveryPrice = 0, Price = 0, Status = 0, TotalPrice = 0 });
                        db.Save();
                        _order = db.OrderRepository.Get(c => c.UserId == _user.ID && c.Status == 0).FirstOrDefault();
                    }

                    //insert order details
                    List<OrderDetailViewModel> Products = new List<OrderDetailViewModel>();
                    foreach (var item in listId)
                    {
                        var product = db.ProductRepository.GetByID(Convert.ToInt32(item));
                        Products.Add(new OrderDetailViewModel
                        {
                            Product = product,
                            Count = Convert.ToInt32(listCount.ElementAt(listId.IndexOf(item)))
                        });
                    }
                    int _totalPrict = 0;
                    int BoxPrice = 0;
                    int BoxPriceConsider = 0;
                    int FinalBoxPrice = 0;
                    int discount = 0;
                    ViewBag.minPrice = db.KeyValueRepository.Get(c => c.Key == "MinimonFaktorPrice").First().Value;
                    foreach (var item in Products)
                    {
                        _totalPrict += item.Product.FinalPrice * item.Count;
                        BoxPrice += item.Product.Box * item.Count;
                        if (item.Product.ConsiderBoxPrice)
                        {
                            BoxPriceConsider += item.Product.Box * item.Count;
                        }
                        discount += (item.Product.Price - item.Product.FinalPrice) * item.Count;
                        db.OrderDetailRepository.Insert(new OrderDetail { ProductId = item.Product.ID, Count = item.Count, OrderId = _order.ID });
                    }


                    int deliveryPrice = 0;
                    int deliveryService =Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "DeliveryService").First().Value);
                    ViewBag.deliveryService = deliveryService;
                    // address information 
                    if (Request.Cookies["pickup"] == "True")
                    {
                        ViewBag.pickup = true;
                        _order.PickUp = true;
                        _order.AddressId = null;
                        FinalBoxPrice = BoxPriceConsider;
                        db.OrderRepository.Update(_order);
                        db.Save();
                    }
                    else
                    {
                        if (deliveryService==0)
                        {
                            PickUp(true);
                            return RedirectToAction("ContinueShopping");
                        }
                        ViewBag.pickup = false;
                        FinalBoxPrice = BoxPrice;
                        List<Address> addresses = db.AddressRepository.Get(c => c.UserId == _user.ID).ToList();
                        if (addresses.Count == 0)
                        {
                            ViewBag.addressStatus = "noAddressIsSubmited";
                        }
                        else
                        {
                            ViewBag.addressList = addresses;
                            if (_order.AddressId == null)
                            {
                                ViewBag.addressStatus = "SelectAddress";
                            }
                            else
                            {
                                Address address = db.AddressRepository.GetByID(_order.AddressId);
                                deliveryPrice = Utilities.Utility.CalculateDelivey(address.distance,true);
                                ViewBag.CurrentAddress = address.ID;
                                ViewBag.currentCoordinate = address.Coordinate;
                                ViewBag.AddressText = address.Text;
                                ViewBag.addressStatus = "AddressIsSelected";
                                ViewBag.deliveryPrice = deliveryPrice;
                            }
                        }
                    }
                    _order.BoxPrice = FinalBoxPrice;
                    _order.Price = _totalPrict;
                    _order.TotalPrice = _totalPrict + deliveryPrice + FinalBoxPrice;
                    _order.DeliveryPrice = deliveryPrice;
                    _order.Discount = discount;
                    db.OrderRepository.Update(_order);
                    db.Save();
                    ViewBag.mobile = _user.Mobile;
                    ViewBag.productPrice = _totalPrict;
                    var factorPrice = _totalPrict + deliveryPrice + FinalBoxPrice;
                    ViewBag.totalPrice = factorPrice;
                    ViewBag.discount = discount;
                    ViewBag.boxPrice = FinalBoxPrice;
                    if (_user.Wallet > 0)
                    {
                        ViewBag.wallet = _user.Wallet;
                    }
                    var readyToPay = factorPrice - _user.Wallet;
                    if (readyToPay > 0)
                    {
                        ViewBag.readyToPay = readyToPay;
                        ViewBag.NoPeyment = 0;
                    }
                    else
                    {
                        ViewBag.readyToPay = 0;
                        ViewBag.NoPeyment = 1;
                    }
                    ViewBag.userid = _user.ID;
                    ViewBag.orderid = _order.ID;
                    ViewBag.ActiveOrders = db.OrderRepository.Get(c => c.UserId == _user.ID && (c.Status == -1 || c.Status == 1)).Count();
                    return View(Products);
                }

            }
            catch (System.Exception e)
            {
                Email.Send(mobile + "ContinueShopping", e.ToString());
                Response.Cookies.Delete("basket-id");
                Response.Cookies.Delete("basket-count");
                return RedirectToAction("index", "home");
            }
        }
        public IActionResult submitOrder(bool id)
        {
            string mobile = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (UnitOfWork db = new UnitOfWork())
            {
                User user = db.UserRepository.Get(c => c.Mobile == mobile).First();
                Order order = db.OrderRepository.Get(c => c.UserId == user.ID && c.Status == 0).First();
                Address address;
                if (db.OrderRepository.Get(c => c.UserId == user.ID && (c.Status == 1 || c.Status == -1)).Count() != 0)
                {
                    return RedirectToAction("ContinueShopping");
                }
                if (id == false && order.AddressId == null)
                {
                    return RedirectToAction("ContinueShopping");
                }
                if (id == true)
                {
                    if (order.AddressId != null)
                    {
                        order.AddressId = null;
                    }
                }
                else
                {
                    address = db.AddressRepository.GetByID(order.AddressId);
                    int distance = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxServiceArea").First().Value);
                    if (distance < address.distance)
                    {
                        return RedirectToAction("ContinueShopping");
                    }
                }
                var wallet = user.Wallet;
                bool peyment;

                if (wallet > order.TotalPrice)
                {
                    user.Wallet = wallet - order.TotalPrice;
                    peyment = false;
                }
                else
                {
                    user.Wallet = 0;
                    peyment = true;
                }
                if (peyment)
                {
                    int UserShouldPey = wallet - order.TotalPrice;
                }
                else
                {

                }
                order.Status = 1;
                order.PickUp = id;
                order.DateTime = DateTime.Now;
                db.OrderRepository.Update(order);
                db.UserRepository.Update(user);

                Response.Cookies.Delete("basket-id");
                Response.Cookies.Delete("basket-count");
                db.Save();

            }
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult PickUp(bool pickup)
        {

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Append("pickup", pickup.ToString(), option);

            return Json("");
        }
        [HttpPost]
        public IActionResult calulateDistance(int distance)
        {
            var Price = Utilities.Utility.CalculateDelivey(distance);
            if (Price==0)
            {
                return Json("notInServiceArea");
            }

            return Json(new { price = Price.ToString(), distance = distance });
        }
        public IActionResult CheckWallet(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var order = db.OrderRepository.GetByID(id);
                var user = db.UserRepository.GetByID(order.UserId);
                if (user.Wallet > order.TotalPrice)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }

        }

        public IActionResult CheckActiveOrders(int id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                var orders = db.OrderRepository.Get(c => c.UserId == id);
                if (orders.Any(c => c.Status == -1 || c.Status == 1))
                {
                    return Json(true);
                }
            }
            return Json(false);
        }
    }
}
