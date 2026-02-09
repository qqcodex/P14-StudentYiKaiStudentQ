using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    internal class OrderedFoodItem : FoodItem
    {
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }

        public OrderedFoodItem() : base() { } // default constructor
        public OrderedFoodItem(string iname, string id, double ip,int qo) : base(iname,id,ip) //parameterised constructor
        {
            QtyOrdered = qo;
            SubTotal = CalculateSubtotal();
        }

        public double CalculateSubtotal()
        {
            SubTotal = QtyOrdered * ItemPrice;
            return SubTotal;
        }
        public override string ToString()
        {
            return $"{ItemName} x{QtyOrdered} - ${SubTotal:F2}";
        }
    }
}
