using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Show { get; set; }
        public int Score { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public User user { get; set; }


    }
}
