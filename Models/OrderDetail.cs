using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class OrderDetail
    {
        public int ID { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
