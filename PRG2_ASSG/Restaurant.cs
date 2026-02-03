using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASSG
{
    internal class Restaurant
    {
        public string restaurantId {  get; set; } 
        public string restaurantName {  get; set; }
        public string restaurantEmail {  get; set; }
        
        public List<Menu> menuList = new List<Menu>(); 
        
        public Queue<Order> orderQueue = new Queue<Order>(); 

        public Restaurant() { } //default constructor 

        public Restaurant(string rid,string rn,string re) // parameterized constructor
        {
            restaurantId = rid;
            restaurantName = rn;
            restaurantEmail = re;
        }
        public void DisplayOrders()
        {
            foreach (Order order in orderQueue)
            {
                Console.WriteLine(order.ToString());
            }
        }
        
        public void DisplaySpecialOffers()
        {

        }

        public void DisplayMenu() 
        {
            foreach (var menu in menuList)
            {
                menu.DisplayFoodItems();
            }
        }

        public void AddMenu(Menu menu) 
        { 
            menuList.Add(menu); 
        }

        public void RemoveMenu(Menu menu)
        {
            menuList.Remove(menu);
        }

        public override string ToString()
        {
            return $"RestaurantName: {restaurantName}  RestaurantID: {restaurantId}  RestaurantEmail: {restaurantEmail}";
        }
    }
}
