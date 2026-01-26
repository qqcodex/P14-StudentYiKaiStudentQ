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
        string idesc = details[2];
        double iprice = Convert.ToDouble(details[details.Length - 1]);

        FoodItem fi = new FoodItem(iname, idesc, iprice);
        Restaurant r = restaurantMap[rid]; //link fooditem to restaurant

        r.menuList[0].AddFoodItem(fi);
    }
}
 
InitialiseFoodItem();

//qn4 - Q 
void DisplayRestaurantOrders()
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
void ProcessOrder()
{
    Console.Write("Process Order\n");
    Console.WriteLine("=============");
    Console.WriteLine("Enter Restaurant ID: ");
    string rId = Console.ReadLine();

    while (orderQueue.Count > 0)
    {
        DisplayOrderedFoodItems();
        Console.WriteLine($"Delivery date/time: {.Order.DeliveryDateTime}");
        Console.WriteLine($"Total Amount: {.Order.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {.Order.OrderStatus}");
    }


    Console.WriteLine("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
    string option = Console.ReadLine();
    if (option == "C")
    {

    }

}
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
List<Customer> customerlist = new List<Customer>();
void InitialiseCustomer()
{
    var lines = File.ReadAllLines("customers.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] data = lines[i].Split(',');
        string email = data[0];
        string name = data[1];

        Customer c = new Customer(email, name);
        customerlist.Add(c);
    }
}

//int oi, DateTime odt,double ot,string os, DateTime ddt,string da, string opm

void InitialiseOrders()
{
    var lines = File.ReadAllLines("orders.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] data = lines[i].Split(',');

        int orderId = Convert.ToInt32(data[0]);
        string custEmail = data[1];
        string restId = data[2];
        string delivDate = data[3];
        string delivTime = data[4];
        DateTime delivDateTime = Convert.ToDateTime(delivDate + " " + delivTime);
        string delivAddr = data[5];
        DateTime createdDateTime = Convert.ToDateTime(data[6]);
        double orderTotal = Convert.ToDouble(data[7]);
        string orderStatus = data[8];
        string items = data[9];

        Order o = new Order(orderId, createdDateTime, orderTotal, orderStatus, delivDateTime, delivAddr); // No bool orderPaid object 
        foreach (Customer customer in customerlist)
        {
            if (custEmail == customer.emailAddress)
            {
                customer.orderList.Add(o);
            }
        }
        foreach (string id in restaurantMap.Keys)
        {
            if (restId == id)
            {
                restaurantMap[id].orderQueue.Enqueue(o);
            }
        }
    }
}
//qn3 - Yi Kai
void DisplayRestaurantMenuItem()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant restaurant in restaurants)
    {
        Console.WriteLine($"Restaurant: {restaurant.restaurantName} ({restaurant.restaurantId})");
        foreach (Menu menu in restaurant.menuList)
        {
            foreach (FoodItem foodItem in menu.foodItemList)
            {
                Console.WriteLine($"- {foodItem.ItemName}: {foodItem.ItemDesc} - ${foodItem.ItemPrice}");
            }
        }
    }
}
//qn5 - Yi Kai

//qn7 - Yi Kai