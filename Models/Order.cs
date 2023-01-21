using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int Price { get; set; }
        public int DeliveryPrice { get; set; }
        public int TotalPrice { get; set; }
        public int Discount { get; set; }
        public int BoxPrice { get; set; }
        public int? AddressId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public int Duration { get; set; }
        public bool PickUp { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }

    }
}
