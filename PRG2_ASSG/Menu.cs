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
    internal class Menu
    {
        public string menuId { get; set; }
        private string menuName { get; set; }

        public List<FoodItem> foodItemList = new List<FoodItem>();

        public Menu(string menuId, string menuName)
        {
            this.menuId = menuId;
            this.menuName = menuName;
        }

        public void AddFoodItem(FoodItem item)
        {
            foodItemList.Add(item);
        }

        public bool RemoveFoodItem(FoodItem item)
        {
            return foodItemList.Remove(item);
        }

        public void DisplayFoodItems()
        {
            foreach (var item in foodItemList)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public override string ToString()
        {
            return $"MenuID: {menuId}  MenuName: {menuName}";
        }


    }
}
