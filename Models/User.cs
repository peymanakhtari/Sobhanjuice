using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobhanJuice.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Wallet { get; set; }
        public bool CheckStatus { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Comment> Comments { get; set; }

    }
}
