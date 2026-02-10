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
    internal class Restaurant
    {
        public string restaurantId { get; set; }
        public string restaurantName { get; set; }
        public string restaurantEmail { get; set; }

        public List<Menu> menuList = new List<Menu>();
        public List<SpecialOffer> specialOfferList = new List<SpecialOffer>();
        public Queue<Order> orderQueue = new Queue<Order>();

        public Restaurant() { } //default constructor 

        public Restaurant(string rid, string rn, string re) // parameterized constructor
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
            foreach (var offer in specialOfferList)
            {
                Console.WriteLine(offer.ToString());
            }
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

        public void AddSpecialOffer(SpecialOffer offer)
        {
            specialOfferList.Add(offer);
        }

        public override string ToString()
        {
            return $"Restaurant: {restaurantName} ({restaurantId})";
        }
    }
}

