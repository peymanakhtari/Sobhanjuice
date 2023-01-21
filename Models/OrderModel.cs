using System.Collections.Generic;

namespace SobhanJuice.Models
{
    public class OrderModel
    {

        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
        public Address Address { get; set; }
        public string  time { get; set; }
        public string  date { get; set; }
    }
}
