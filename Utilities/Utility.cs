using MD.PersianDateTime.Core;
using RadicalTherapy.Data.Repository;
using SobhanJuice.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SobhanJuice.Utilities
{
    public class Utility
    {
        public static int CalculateDelivey(int? distance,bool fromContinueShopping)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                var maxS = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxServiceArea").First().Value);
                var maxD = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxDelivery").First().Value);
                if (distance> maxS)
                {
                    return maxD;
                }
              return  CalculateDelivey(distance);
            }
        }
        public static int CalculateDelivey(int? distance)
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                var minD =Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MinDelivery").First().Value);
                var maxD =Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxDelivery").First().Value);
                var minS =Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MinServiceArea").First().Value);
                var maxS =Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "MaxServiceArea").First().Value);
                var distanceRange=maxS - minS; 
                var priceRange=maxD - minD;
                if (distance<=minS)
                {
                    return minD;
                }
                if (distance > maxS)
                {
                    return 0;
                }
                int Distance = distance ?? default(int);
                var x = Distance - minS;
                decimal y = (decimal)x / (decimal)distanceRange;
                decimal z = y * (decimal)priceRange;
                decimal r = z + minD;
                int result =(int)r;
                return (int)Math.Round(result / 1000d, 0) * 1000;

            }
           
        }
        public string DateBuilder(PersianDateTime datetime)
        {
            string year = datetime.Year.ToString();
            string mounth = lessThanTen(datetime.Month);
            string day = lessThanTen(datetime.Day);

            return year + "/" + mounth + "/" + day;
        }
        public string TimeBuilder(PersianDateTime datetime)
        {
            string hour = lessThanTen(datetime.Hour);
            string minute = lessThanTen(datetime.Minute);
            return minute + " : " + hour;
        }
        private string lessThanTen(int val)
        {
            if (val < 10)
            {
                return "0" + val.ToString();
            }
            else
            {
                return val.ToString();
            }
        }
        public static List<OrderModel> GetOrders(List<Order> orderlist)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
                List<OrderModel> Orders = new List<OrderModel>();
                foreach (var item in orderlist)
                {
                    User user = db.UserRepository.GetByID(item.UserId);
                    item.User = user;
                    Address address=new Address();
                    if (item.AddressId == null)
                    {
                        address.Text = "تحویل درب مغازه";
                    }
                    else
                    {
                        address = db.AddressRepository.GetByID(item.AddressId);
                    }
                    List<OrderDetail> orderDetail = db.OrderDetailRepository.Get(c => c.OrderId == item.ID).ToList();
                    foreach (var detail in orderDetail)
                    {
                        detail.Product = db.ProductRepository.GetByID(detail.ProductId);
                    }
                    Utilities.Utility ut = new Utility();
                    var dt = DateConvertor.ToShamsi(item.DateTime);
                    string time = ut.TimeBuilder(dt);
                    string date = ut.DateBuilder(dt);
                    Orders.Add(new OrderModel { Address = address, Order = item, Details = orderDetail, date = date, time = time });

                }
                return Orders;
            }
           
            
        }
        public static void UpdateUser(int Id)
        {
            using (UnitOfWork db = new UnitOfWork())
            {
               var user= db.UserRepository.GetByID(Id);
                user.CheckStatus = true;
                db.UserRepository.Update(user);
                db.Save();
            }
        }
        public static int CheckOpen()
        {
            using (UnitOfWork db=new UnitOfWork())
            {
                if (db.KeyValueRepository.Get(c=>c.Key== "TemporayClose").First().Value=="1")
                {
                    return -1;
                }
                var now = DateTime.Now;
                int closehour = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "CloseTimeH").First().Value);
                int closeMinute = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "CloseTimeM").First().Value);
                int openHour = 0;
                int openMinute = 0;
                if ((int)now.DayOfWeek==5)
                {
                     openHour = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayH").First().Value);
                     openMinute = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeFridayM").First().Value);
                }
                else
                {
                     openHour = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeH").First().Value);
                     openMinute = Convert.ToInt32(db.KeyValueRepository.Get(c => c.Key == "OpenTimeM").First().Value);
                }
                var datetimeOpen = new DateTime(now.Year, now.Month, now.Day, openHour, openMinute, 0);
                var datetimeClose = new DateTime(now.Year, now.Month, now.Day, closehour, closeMinute, 0);
                return (now > datetimeClose && now < datetimeOpen) ? 0 : 1;

            }
        }
       
    }
}
