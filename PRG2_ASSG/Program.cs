//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

using PRG2_ASSG;
using System.Numerics;
void Main()
{
    InitialiseRestaurant();
    InitialiseFoodItem();
    InitialiseCustomer();
    InitialiseOrders();

    Console.Write("Welcome to the Gruberoo Food Delivery System");
    Console.WriteLine($"{restaurants.Count} restaurants loaded!");
    Console.WriteLine($"{.Count} food items loaded!");
    Console.WriteLine($"{restaurants.Count} customers loaded!");
    Console.WriteLine($"{.Count} orders loaded!\n");
    


}
//qn1 - Q

List<Restaurant> restaurants = new List<Restaurant>();
Dictionary<string, Restaurant> restaurantMap = new Dictionary<string, Restaurant>();
void InitialiseRestaurant()
{
    var lines = File.ReadAllLines("restaurants.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] details = lines[i].Split(',');
        string rid = details[0];
        string rn = details[1];
        string re = details[2];

        Restaurant r = new Restaurant(rid, rn, re);
        r.AddMenu(new Menu("M001", "Main Menu"));
        restaurants.Add(r);
        restaurantMap.Add(rid, r);
    }
}
InitialiseRestaurant();

void InitialiseFoodItem()
{
    var lines = File.ReadAllLines("fooditems.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] details = lines[i].Split(',');
        string rid = details[0];
        string iname = details[1];
        string idesc = string.Join(",", details.Skip(2).Take(details.Length - 3));
        double iprice = Convert.ToDouble(details[details.Length - 1]);

        FoodItem fi = new FoodItem(iname, idesc, iprice);
        Restaurant r = restaurantMap[rid]; //link fooditem to restaurant

        r.menuList[0].AddFoodItem(fi);
    }
}
 
InitialiseFoodItem();

//qn4 - Q 
void DisplayALLOrders()
{
    Console.Write("All Orders\n");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-12}{1,-12}{2,-16}{3,-22}{4,-9}{5,-9}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-12}{1,-12}{2,-16}{3,-22}{4,-9}{5,-9}", "--------", "----------", "-------------", "------------------", "------", "---------");

    foreach (Customer c in customers)
    {
        foreach (Order o in c.orderList)
        {
            Restaurant r =
        }

    }
}

//qn6 - Q

//qn8 - Q 
void DeleteOrder()
{
    Console.Write("Delete Order\n");
    Console.WriteLine("============");
    Console.WriteLine("Enter Customer Email: ");
    string email = Console.ReadLine();
    Console.WriteLine("Pending orders: ");

    Console.WriteLine("Enter Order ID: ");
    int orderID = Convert.ToInt32(Console.ReadLine());

}

//qn2 - Yi Kai

//qn3 - Yi Kai

//qn5 - Yi Kai

//qn7 - Yi Kai