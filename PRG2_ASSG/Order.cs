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
    class Order
    {
        // Only 8 attributes as per class diagram
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }

        public List<OrderedFoodItem> itemList { get; set; } = new List<OrderedFoodItem>();

        public Order() //default constructor 
        {
        }

        public Order(int oi, DateTime odt, double ot, string os, DateTime ddt, string da, string opm, bool paid) //parameterised constructor
        {
            OrderId = oi;
            OrderDateTime = odt;
            OrderTotal = ot;
            OrderStatus = os;
            DeliveryDateTime = ddt;
            DeliveryAddress = da;
            OrderPaymentMethod = opm;
            OrderPaid = paid;
        }

        public double CalculateOrderTotal()
        {
            double totalNoFee = 0;
            double deliveryFee = 5;
            foreach (var item in itemList)
            {
                totalNoFee += item.CalculateSubtotal();
            }
            OrderTotal = totalNoFee + deliveryFee;
            return OrderTotal;
        }
        
        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            itemList.Add(item);
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            return itemList.Remove(item);
        }

        public void DisplayOrderedFoodItems()
        {
            Console.WriteLine("Ordered Items:");
            for (int i = 0; i < itemList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {itemList[i].ItemName} - {itemList[i].QtyOrdered}");
            }
        }

        public override string ToString()
        {
            return $"Order {OrderId}: Status={OrderStatus}, Total=${OrderTotal:F2}, Delivery={DeliveryDateTime:dd/MM/yyyy HH:mm}";
        }
    }
}