using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    class FoodItem
    {
        public string itemName { get; set; }
        public string itemDesc { get; set; }
        public double itemPrice { get; set; }
        public string Customise { get; set; }

        public FoodItem() { } // default constructor 

        public FoodItem(string iname, string id, double ip, string c)
        {
            itemName = iname;
            itemDesc = id;
            itemPrice = ip;
            Customise = c;
        }

        public override string ToString()
        {
            return $"{itemName}"    
        }

    }
}
