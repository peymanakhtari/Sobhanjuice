using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class Product
    {
        public  int ID{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int FinalPrice { get; set; }
        public int Discount { get; set; }
        public int Box { get; set; }
        public bool ConsiderBoxPrice { get; set; }
        public bool Exist { get; set; }
        public int Sequence { get; set; }
        public byte[] Picture { get; set; }
        public  int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }

}
