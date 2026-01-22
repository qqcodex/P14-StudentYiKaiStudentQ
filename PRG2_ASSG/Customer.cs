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

        public Customer(string emailAddress, string customerName, List<Order> orderList)
        {
            this.emailAddress = emailAddress;
            this.customerName = customerName;
            this.orderList = orderList;
        }

        public void AddOrder(Order order) 
        { 
            orderList.Add(order); 
        } 
        public void DisplayAllOrders(Order order)
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
