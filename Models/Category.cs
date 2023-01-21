using System.Collections.Generic;

namespace SobhanJuice.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public byte[] Picture { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
