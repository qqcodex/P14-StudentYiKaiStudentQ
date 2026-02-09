//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

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

        public OrderedFoodItem() : base() { }

        // Changed parameter name from 'id' to 'idesc' to match FoodItem constructor
        public OrderedFoodItem(string iname, string idesc, double ip, int qo) : base(iname, idesc, ip)
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
