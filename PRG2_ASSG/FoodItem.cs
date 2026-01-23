using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    class FoodItem
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }

        public FoodItem() { } // default constructor 

        public FoodItem(string iname, string id, double ip) //parameterised constructor
        {
            ItemName = iname;
            ItemDesc = id;
            ItemPrice = ip;
        }

        public override string ToString()
        {
            return $"{ItemName}: {ItemDesc} - {ItemPrice}";
        }

    }
}
