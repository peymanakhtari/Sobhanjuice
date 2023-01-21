using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class Product_admin
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int FinalPrice { get; set; }
        public int Discount { get; set; }
        public bool Exist { get; set; }
        public string Picture { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }
}
