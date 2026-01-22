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
        public OrderedFoodItem() : base() { } // defualt constructor
        public OrderedFoodItem(string iname, string id, double ip, string c,int qo,double st) : base(iname,id,ip,c) //parameterised constructor
        {
            QtyOrdered = qo;
            SubTotal = st;
        }

        public double CalculateSubtotal()
        {
            SubTotal = QtyOrdered * ItemPrice;
            return SubTotal;
        }

    }
}
