//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

using PRG2_ASSG;
using System.Numerics;
List<Restaurant> restaurantlist = new List<Restaurant>();
Dictionary<string, Restaurant> RestaurantMap = new Dictionary<string, Restaurant>();

List<Customer> customerlist = new List<Customer>();

Dictionary<int, Restaurant> orderRestaurantMap = new Dictionary<int, Restaurant>();

Stack<Order> refundStack = new Stack<Order>();


int foodItemCount = 0;
int orderCount = 0;

// -----------------------------INTRO-----------------------------
InitialiseRestaurant();
InitialiseFoodItem();
InitialiseCustomer();
InitialiseOrders();

Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
Console.WriteLine($"{restaurantlist.Count} restaurants loaded!");
Console.WriteLine($"{foodItemCount} food items loaded!");
Console.WriteLine($"{customerlist.Count} customers loaded!");
Console.WriteLine($"{orderCount} orders loaded!\n");

// -----------------------------MAIN MENU-----------------------------

bool exit = false;

while (!exit)
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    try
    {
        int choice = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine();

        if (choice == 1)
        {
            DisplayRestaurantMenuItem();
        }
        else if (choice == 2)
        {
            DisplayAllOrders();
        }
        else if (choice == 3)
        {
            CreateNewOrder();
        }
        else if (choice == 4)
        {
            ProcessOrder();
        }
        else if (choice == 5)
        {
            ModifyOrder();
        }
        else if (choice == 6)
        {
            DeleteOrder();
        }
        else if (choice == 0)
        {
            Console.WriteLine("Bye!");
            exit = true;
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter 0–6."); //input validation
        }
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid input. Please enter a number.");
    }
    finally
    {
        Console.WriteLine();
    }
}

// -----------------------------METHODS-----------------------------
//qn1 - Q
// initialise Restaurants
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


        restaurantlist.Add(r); //add restaurant object to list of restaurants
        RestaurantMap.Add(rid, r); //add restaurant object to map with restaurantId as key
    }
}
// initialise FoodItems
void InitialiseFoodItem()
{
    var lines = File.ReadAllLines("fooditems.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        string[] details = lines[i].Split(',');

        string rid = details[0];
        string iname = details[1];
        string idesc = details[2];
        double iprice = Convert.ToDouble(details[3]);

        string[] ingredients = idesc.Split(';');
        for (int j = 0; j < ingredients.Length; j++)
            ingredients[j] = ingredients[j].Trim();

        FoodItem fi = new FoodItem(iname, idesc, iprice);
        foodItemCount++; 

        Restaurant r = RestaurantMap[rid]; //link fooditem to restaurant
        r.menuList[0].AddFoodItem(fi);
    }
}

//qn2 - Yi Kai
// initialise Customers
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
// initialise Orders
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

        string combinedDeliv = delivDate + " " + delivTime;
        DateTime delivDateTime = DateTime.ParseExact(combinedDeliv, "d/M/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture);
        string delivAddr = data[5];
        
        string createdDateTimestr = data[6];
        DateTime createdDateTime = DateTime.ParseExact(createdDateTimestr, "d/M/yyyy H:mm", System.Globalization.CultureInfo.InvariantCulture); //convert from "M/d/yyyy HH:mm" to datetime data type

        double orderTotal = Convert.ToDouble(data[7]);
        string orderStatus = data[8];
        string items = data[9];
        string paymentMethod = data[10];

        Order o = new Order(orderId, createdDateTime, orderTotal, orderStatus, delivDateTime, delivAddr, paymentMethod); // No bool orderPaid object 
        orderCount++;

        foreach (Customer customer in customerlist)
        {
            if (custEmail == customer.emailAddress)
            {
                customer.orderList.Add(o);
                break;
            }
        }
        Restaurant r = RestaurantMap[restId];
        r.orderQueue.Enqueue(o);

        orderRestaurantMap[orderId] = r;
    }
}

// -----------------------------FEATURES-----------------------------
//Q3) List all restaurants and menu items - Yi Kai
void DisplayRestaurantMenuItem()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant restaurant in restaurantlist)
    {
        Console.WriteLine($"Restaurant: {restaurant.restaurantName} ({restaurant.restaurantId})");
        foreach (Menu menu in restaurant.menuList)
        {
            menu.DisplayFoodItems();
        }
    }
}
//Q4) List all orders with basic information - Q
void DisplayAllOrders()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-12}{1,-12}{2,-16}{3,-22}{4,-9}{5,-9}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-12}{1,-12}{2,-16}{3,-22}{4,-9}{5,-9}", "--------", "----------", "-------------", "------------------", "------", "---------");

    foreach (Customer c in customerlist)
    {
        foreach (Order o in c.orderList)
        {
            Restaurant r = orderRestaurantMap[o.OrderId];

            Console.WriteLine("{0,-10}{1,-15}{2,-18}{3,-22}{4,-10}{5,-12}",
                o.OrderId,
                c.customerName,
                r.restaurantName,
                o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
                $"${o.OrderTotal:F2}",
                o.OrderStatus
            );
        }
    }
}

//Q5) Create a new order - Yi Kai
void CreateNewOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    Console.WriteLine("Enter Customer Email: ");
    string custEmail = Console.ReadLine();
    Console.WriteLine("Enter Restaurent ID: ");
    string restaurantId = Console.ReadLine();
    Console.WriteLine("Enter Delivery Date (dd/mm/yyyy): ");
    string delivDate = Console.ReadLine();
    Console.WriteLine("Enter Delivery Time (hh:mm): ");
    string delivTime = Console.ReadLine();
    DateTime delivDateTime = Convert.ToDateTime(delivDate + " " + delivTime);
    Console.WriteLine("Enter Delivery Address: ");
    string delivAddr = Console.ReadLine();

    foreach (Restaurant r in restaurantlist)
    {
        if (restaurantId == r.restaurantId)
        {
            foreach (Menu m in r.menuList)
            {
                Console.WriteLine();
            }
        }
    }

}
//Q6) Process an order - Q
void ProcessOrder()
{
    Console.Write("Process Order\n");
    Console.WriteLine("=============");
    Console.WriteLine("Enter Restaurant ID: ");
    string rId = Console.ReadLine();

    foreach (var r in restaurantlist)
    {
        if (r.restaurantId != rId)
        {
            continue;
        }
        else if (r.restaurantId == rId)
        {
            while (r.orderQueue.Count > 0)
            {

                Order current = r.orderQueue.Peek();
                current.DisplayOrderedFoodItems();
                Console.WriteLine($"Delivery date/time: {current.DeliveryDateTime}");
                Console.WriteLine($"Total Amount: ${current.OrderTotal:F2}");
                Console.WriteLine($"Order Status: {current.OrderStatus}\n");


                Console.WriteLine("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string option = Console.ReadLine();
                try
                {
                    if (option == "C")
                    {
                        if (current.OrderStatus == "Pending")
                        {
                            current.OrderStatus = "Preparing";
                            r.orderQueue.Dequeue();      // remove from queue after action
                            r.orderQueue.Enqueue(current); // put back so it can be delivered later
                            Console.WriteLine("Order confirmed. Status updated to Preparing.");
                        }
                        else
                        {
                            Console.WriteLine("You can only confirm a Pending order.");
                        }
                    }
                    else if (option == "R")
                    {
                        if (current.OrderStatus == "Pending")
                        {
                            current.OrderStatus = "Rejected";
                            refundStack.Push(current);
                            r.orderQueue.Dequeue(); // remove from queue
                            Console.WriteLine("Order rejected. Added to refund stack.");
                        }
                        else
                        {
                            Console.WriteLine("You can only reject a Pending order.");
                        }
                    }
                    else if (option == "S")
                    {
                        // move front order to back without changing status
                        r.orderQueue.Dequeue();
                        r.orderQueue.Enqueue(current);
                        Console.WriteLine("Order skipped.");
                    }
                    else if (option == "D")
                    {
                        if (current.OrderStatus == "Preparing")
                        {
                            current.OrderStatus = "Delivered";
                            r.orderQueue.Dequeue(); // remove from queue
                            Console.WriteLine("Order delivered.");
                        }
                        else
                        {
                            Console.WriteLine("You can only deliver an order that is Preparing.");
                        }
                    }
                    else if (option == "Q")
                    {
                       
                    }
                    else
                    {
                        Console.WriteLine("Invalid option.");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("An error occurred while processing the order.");
                }

            }
        }
    }
}
//Q7) Modify an existing order - Yi Kai
void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();
}
    //8) Delete an existing order - Q
void DeleteOrder()
{
    Console.Write("Delete Order\n");
    Console.WriteLine("============");
    Console.WriteLine("Enter Customer Email: ");
    string custEmail = Console.ReadLine();
    Console.WriteLine("Pending orders: ");


    Console.WriteLine("Enter Order ID: ");
   int orderID = Convert.ToInt32(Console.ReadLine());
}

