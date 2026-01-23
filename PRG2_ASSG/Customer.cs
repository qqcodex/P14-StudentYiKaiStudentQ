using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    internal class Customer
    {
        public string emailAddress {  get; set; } 
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
            foreach (Order item in orderList)
            {
                if (order.Equals(item))
                {
                    orderList.Remove(item);
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"CustomerName: {customerName}  EmailAddress: {emailAddress}";
        }
        
    }
    
}
