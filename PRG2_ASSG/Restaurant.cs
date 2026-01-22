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
        
        public List<Menu> menus = new List<Menu>(); 
        
        public Queue<Order> orderQueue = new Queue<Order>(); 

        public void DisplayOrders(Order order)
        {
            foreach (var menu in orderQueue)
            {
                Console.WriteLine(menu.ToString);
            }
        }
        
        public void DisplaySpecialOffers()
        {

        }

        public void DisplayMenu() 
        {
            foreach (var menu in menus) menu.DisplayFoodItems();
        }

        public void AddMenu(Menu menu) 
        { 
            menus.Add(menu); 
        }

        public void RemoveMenu(Menu menu)
        {
            menus.Remove(menu);
        }

        public override string ToString()
        {
            return $"RestaurantName: {restaurantName}  RestaurantID: {restaurantId}  RestaurantEmail: {restaurantEmail}";
        }
    }
}
