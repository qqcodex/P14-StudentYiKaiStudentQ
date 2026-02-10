//==========================================================
// Student Number: S10272698F
// Student Name : Tan Yi Kai
// Partner Number : S10272951
// Partner Name : Dg Muhammad Aqil Bin Md Alias
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    internal class Customer
    {
        public string emailAddress { get; set; }
        public string customerName { get; set; }

        public List<Order> orderList = new List<Order>();

        public Customer() { }

        public Customer(string emailAddress, string customerName)
        {
            this.emailAddress = emailAddress;
            this.customerName = customerName;
        }

        public void AddOrder(Order order)
        {
            orderList.Add(order);
        }

        public void DisplayAllOrders()
        {
            foreach (Order item in orderList)
            {
                Console.WriteLine(item);
            }
        }

        public bool RemoveOrder(Order order)
        {
            return orderList.Remove(order);
        }

        public override string ToString()
        {
            return $"CustomerName: {customerName}  EmailAddress: {emailAddress}";
        }

    }
}
