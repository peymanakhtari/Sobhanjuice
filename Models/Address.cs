using System.Collections;
using System.Collections.Generic;

namespace SobhanJuice.Models
{
    public class Address
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string Coordinate { get; set; }
        public int UserId { get; set; }
        public int? distance { get; set; }
        public User user { get; set; }

    }
}
