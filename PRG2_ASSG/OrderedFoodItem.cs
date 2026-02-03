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
        public OrderedFoodItem() : base() { } // default constructor
        public OrderedFoodItem(string iname, string id, double ip,int qo) : base(iname,id,ip) //parameterised constructor
        {
            QtyOrdered = qo;
        }

        public double CalculateSubtotal()
        {
            double SubTotal = QtyOrdered * ItemPrice;
            return SubTotal;
        }

    }
}
