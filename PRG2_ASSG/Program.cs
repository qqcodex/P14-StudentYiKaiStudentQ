//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

using PRG2_ASSG;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


// Global lists and dictionaries
List<Restaurant> restaurantList = new List<Restaurant>();
Dictionary<string, Restaurant> restaurantMap = new Dictionary<string, Restaurant>();
List<Customer> customerList = new List<Customer>();
Dictionary<string, Customer> customerMap = new Dictionary<string, Customer>();
Stack<Order> refundStack = new Stack<Order>();

// Mapping dictionaries to link orders to customers and restaurants (since Order class doesn't store these)
Dictionary<Order, Customer> orderToCustomerMap = new Dictionary<Order, Customer>();
Dictionary<Order, Restaurant> orderToRestaurantMap = new Dictionary<Order, Restaurant>();

int nextOrderId = 1036; // Starting from next available ID

// Initialize data
InitialiseRestaurant();
InitialiseFoodItem();
InitialiseCustomer();
InitialiseOrders();


Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
Console.WriteLine($"{restaurantList.Count} restaurants loaded!");
// Count total food items using nested loops instead of LINQ
int totalFoodItems = 0;
for (int i = 0; i < restaurantList.Count; i++)
{
    for (int j = 0; j < restaurantList[i].menuList.Count; j++)
    {
        totalFoodItems += restaurantList[i].menuList[j].foodItemList.Count;
    }
}
Console.WriteLine($"{totalFoodItems} food items loaded!");
Console.WriteLine($"{customerList.Count} customers loaded!");
// Count total orders using loop instead of LINQ
int totalOrders = 0;
for (int i = 0; i < customerList.Count; i++)
{
    totalOrders += customerList[i].orderList.Count;
}
Console.WriteLine($"{totalOrders} orders loaded!\n");

// Main menu loop
bool exit = false;
while (!exit)
{
    DisplayMainMenu();
    string choice = Console.ReadLine();
    Console.WriteLine();


    if (choice == "1")
    {
        DisplayRestaurantMenuItem();
    }
    else if (choice == "2")
    {
        DisplayAllOrders();
    }
    else if (choice == "3")
    {
        CreateNewOrder();
    }
    else if (choice == "4")
    {
        ProcessOrder();
    }
    else if (choice == "5")
    {
        ModifyOrder();
    }
    else if (choice == "6")
    {
        DeleteOrder();
    }
    else if (choice == "7")
    {
        BulkProcessOrders();
    }
    else if (choice == "8")
    {
        DisplayTotalOrderAmount();
    }
    else if (choice == "9")
    {
        DisplayCustomerOrderHistory();

    }
    else if (choice == "10")
    {
        DisplayCustomerNotifications();
    }
    else if (choice == "0")
    {
        SaveQueueAndStack();
        Console.WriteLine("Thank you for using Gruberoo!");
        exit = true;
    }
    else
    {
        Console.WriteLine("Invalid option. Please try again.");
    }

    if (!exit)
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}


// Display main menu
void DisplayMainMenu()
{
    Console.WriteLine("\n==== Gruberoo Food Delivery System ====");
    Console.WriteLine("BASIC FEATURES:");
    Console.WriteLine("[1] List all restaurants and food items");
    Console.WriteLine("[2] List all orders");
    Console.WriteLine("[3] Create a new order");
    Console.WriteLine("[4] Process an order");
    Console.WriteLine("[5] Modify an existing order");
    Console.WriteLine("[6] Delete an existing order");
    Console.WriteLine("\nADVANCED FEATURES:");
    Console.WriteLine("[7] Bulk process unprocessed orders");
    Console.WriteLine("[8] Display total order amount");
    Console.WriteLine("[9] Customer order history & statistics");
    Console.WriteLine("[10] Customer Notifications & ETA");
    Console.WriteLine("[0] Exit");
    Console.Write("Enter your option: ");
}


// FEATURE 1 - Load restaurants and food items (Q)
void InitialiseRestaurant()
{
    var lines = File.ReadAllLines("restaurants.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(lines[i])) continue;

        string[] details = lines[i].Split(',');

        if (details.Length < 3)
        {
            Console.WriteLine($"Warning: Skipping malformed line {i} in restaurants.csv");
            continue;
        }

        string rid = details[0].Trim();
        string rn = details[1].Trim();
        string re = details[2].Trim();

        Restaurant r = new Restaurant(rid, rn, re);
        r.AddMenu(new Menu("M001", "Main Menu"));
        restaurantList.Add(r);
        restaurantMap.Add(rid, r);
    }
}

void InitialiseFoodItem()
{
    var lines = File.ReadAllLines("fooditems.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(lines[i])) continue;

        string[] details = lines[i].Split(',');

        if (details.Length < 4)
        {
            Console.WriteLine($"Warning: Skipping malformed line {i} in fooditems.csv");
            continue;
        }

        string rid = details[0].Trim();
        string iname = details[1].Trim();
        string idesc = details[2].Trim();
        double iprice;

        if (!double.TryParse(details[3].Trim(), out iprice))
        {
            Console.WriteLine($"Warning: Invalid price on line {i} in fooditems.csv");
            continue;
        }

        FoodItem fi = new FoodItem(iname, idesc, iprice);
        if (restaurantMap.ContainsKey(rid))
        {
            Restaurant r = restaurantMap[rid];
            r.menuList[0].AddFoodItem(fi);
        }
    }
}


// FEATURE 2 - Load customers and orders (Yi Kai)
void InitialiseCustomer()
{
    var lines = File.ReadAllLines("customers.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(lines[i])) continue;

        string[] data = lines[i].Split(',');

        if (data.Length < 2)
        {
            Console.WriteLine($"Warning: Skipping malformed line {i} in customers.csv");
            continue;
        }

        string name = data[0].Trim();
        string email = data[1].Trim();

        Customer c = new Customer(email, name);
        customerList.Add(c);
        customerMap.Add(email, c);
    }
}
void InitialiseOrders()
{
    var lines = File.ReadAllLines("orders.csv");

    for (int i = 1; i < lines.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(lines[i])) continue;

        // Manual CSV parsing to handle quoted fields
        List<string> data = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int j = 0; j < lines[i].Length; j++)
        {
            char c = lines[i][j];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                data.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        data.Add(currentField); // Add last field

        if (data.Count < 11)
        {
            Console.WriteLine($"Warning: Skipping malformed line {i} in orders.csv");
            continue;
        }

        try
        {
            int orderId = Convert.ToInt32(data[0].Trim());
            string custEmail = data[1].Trim();
            string restId = data[2].Trim();

            if (!restaurantMap.ContainsKey(restId))
            {
                Console.WriteLine($"Warning: Restaurant {restId} not found for order {orderId}");
                continue;
            }

            Restaurant restaurant = restaurantMap[restId];

            string delivDate = data[3].Trim();
            string delivTime = data[4].Trim();
            DateTime delivDateTime = DateTime.ParseExact($"{delivDate} {delivTime}", "d/M/yyyy H:m", CultureInfo.InvariantCulture);
            string delivAddr = data[5].Trim();
            DateTime createdDateTime = DateTime.ParseExact(data[6].Trim(), "d/M/yyyy H:m", CultureInfo.InvariantCulture);
            double orderTotal = Convert.ToDouble(data[7].Trim());
            string orderStatus = data[8].Trim();
            string paymentMethod = data[10].Trim();

            Order o = new Order(orderId, createdDateTime, orderTotal, orderStatus, delivDateTime, delivAddr, paymentMethod, true);

            string itemsStr = data[9].Trim().Trim('"');
            string[] itemChunks = itemsStr.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (string chunk in itemChunks)
            {
                string[] parts = chunk.Split(',');
                if (parts.Length < 2) continue;
                string itemName = parts[0].Trim();
                if (!int.TryParse(parts[1].Trim(), out int qty))
                    continue;

                FoodItem fi = null;
                foreach (Menu m in restaurant.menuList)
                {
                    foreach (FoodItem item in m.foodItemList)
                    {
                        if (item.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                        {
                            fi = item;
                            break;
                        }
                    }
                    if (fi != null) break;
                }

                if (fi != null)
                {
                    o.itemList.Add(new OrderedFoodItem(fi.ItemName, fi.ItemDesc, fi.ItemPrice, qty));
                }
            }

            if (customerMap.ContainsKey(custEmail))
            {
                Customer customer = customerMap[custEmail];
                customer.AddOrder(o);
                orderToCustomerMap[o] = customer;
            }

            restaurant.orderQueue.Enqueue(o);
            orderToRestaurantMap[o] = restaurant;

            if (orderId >= nextOrderId)
            {
                nextOrderId = orderId + 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Error parsing line {i} in orders.csv: {ex.Message}");
        }
    }
}

// FEATURE 3 - List all restaurants and menu items (Yi Kai)
void DisplayRestaurantMenuItem()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant restaurant in restaurantList)
    {
        Console.WriteLine($"\n{restaurant.ToString()}");
        foreach (Menu menu in restaurant.menuList)
        {
            foreach (FoodItem item in menu.foodItemList)
            {
                Console.WriteLine($" {item.ToString()}");
            }
        }
    }
}

// FEATURE 4 - List all orders (Q)
void DisplayAllOrders()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine($"{"Order ID",-12}{"Customer",-25}{"Restaurant",-20}{"Delivery Date/Time",-25}{"Amount",-10}{"Status"}");
    Console.WriteLine($"{"--------",-12}{"----------",-25}{"-------------",-20}{"------------------",-25}{"------",-10}{"---------"}");

    foreach (Customer c in customerList)
    {
        foreach (Order o in c.orderList)
        {
            // Find restaurant for this order
            Restaurant r = orderToRestaurantMap.ContainsKey(o) ? orderToRestaurantMap[o] : null;
            string restaurantName = r != null ? r.restaurantName : "Unknown";

            Console.WriteLine($"{o.OrderId,-12}{c.customerName,-25}{restaurantName,-20}{o.DeliveryDateTime,-25:dd/MM/yyyy HH:mm}{o.OrderTotal,-10:C}{o.OrderStatus}");
        }
    }
}

// FEATURE 5 - Create a new order (Yi Kai)
void CreateNewOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");

    // Get and validate customer email
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];

    // Get and validate restaurant ID
    Console.Write("Enter Restaurant ID: ");
    string restId = Console.ReadLine().Trim();

    if (!restaurantMap.ContainsKey(restId))
    {
        Console.WriteLine("Error: Restaurant not found.");
        return;
    }

    Restaurant restaurant = restaurantMap[restId];

    // Get delivery date and time
    DateTime delivDateTime;
    while (true)
    {
        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        string delivDateStr = Console.ReadLine().Trim();
        Console.Write("Enter Delivery Time (hh:mm): ");
        string delivTimeStr = Console.ReadLine().Trim();

        try
        {
            delivDateTime = DateTime.ParseExact($"{delivDateStr} {delivTimeStr}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            if (delivDateTime < DateTime.Now)
            {
                Console.WriteLine("Error: Delivery date/time must be in the future.");
                continue;
            }
            break;
        }
        catch
        {
            Console.WriteLine("Error: Invalid date/time format. Please try again.");
        }
    }

    // Get delivery address
    Console.Write("Enter Delivery Address: ");
    string delivAddr = Console.ReadLine().Trim();

    if (string.IsNullOrWhiteSpace(delivAddr))
    {
        Console.WriteLine("Error: Delivery address cannot be empty.");
        return;
    }

    // Display available food items
    Console.WriteLine("\nAvailable Food Items:");
    List<FoodItem> availableItems = restaurant.menuList[0].foodItemList;
    for (int i = 0; i < availableItems.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {availableItems[i].ItemName} - ${availableItems[i].ItemPrice:F2}");
    }

    // Select items
    Order newOrder = new Order();
    newOrder.OrderId = nextOrderId;
    newOrder.DeliveryDateTime = delivDateTime;
    newOrder.DeliveryAddress = delivAddr;
    newOrder.OrderDateTime = DateTime.Now;
    newOrder.OrderStatus = "Not Paid";

    while (true)
    {
        Console.Write("Enter item number (0 to finish): ");
        if (!int.TryParse(Console.ReadLine(), out int itemNum))
        {
            Console.WriteLine("Error: Please enter a valid number.");
            continue;
        }

        if (itemNum == 0) break;

        if (itemNum < 1 || itemNum > availableItems.Count)
        {
            Console.WriteLine("Error: Invalid item number.");
            continue;
        }

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
        {
            Console.WriteLine("Error: Please enter a valid quantity.");
            continue;
        }

        FoodItem selectedItem = availableItems[itemNum - 1];
        OrderedFoodItem orderedItem = new OrderedFoodItem(selectedItem.ItemName, selectedItem.ItemDesc, selectedItem.ItemPrice, qty);
        newOrder.AddOrderedFoodItem(orderedItem);
    }

    if (newOrder.itemList.Count == 0)
    {
        Console.WriteLine("Error: No items selected. Order cancelled.");
        return;
    }

    // Special request
    while (true)
    {
        Console.Write("Add special request? [Y/N]: ");
        string specialReqChoice = Console.ReadLine().Trim();

        if (specialReqChoice == "Y")
        {
            Console.Write("Enter special request: ");
            string specialRequest = Console.ReadLine().Trim();
            // Note: Special request would be stored if Order class had that attribute
            // For now, we acknowledge it but can't store it per class diagram
            break;
        }
        else if (specialReqChoice == "N")
        {
            break;
        }
        else
        {
            Console.WriteLine("Error: Invalid input. Please enter 'Y' or 'N' only.");
        }
    }

    // Calculate total
    double orderTotal = newOrder.CalculateOrderTotal();
    Console.WriteLine($"\nOrder Total: ${orderTotal - 5:F2} + $5.00 (delivery) = ${orderTotal:F2}");

    // Payment
    while (true)
    {
        Console.Write("Proceed to payment? [Y/N]: ");
        string payChoice = Console.ReadLine().Trim();

        if (payChoice == "Y")
        {
            break; // Proceed to payment
        }
        else if (payChoice == "N")
        {
            Console.WriteLine("Order cancelled.");
            return;
        }
        else
        {
            Console.WriteLine("Error: Invalid input. Please enter 'Y' or 'N' only.");
        }
    }

    string paymentMethod = "";
    while (true)
    {
        Console.Write("\nPayment method:\n[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string payMethodChoice = Console.ReadLine().Trim();

        if (payMethodChoice == "CC" || payMethodChoice == "PP" || payMethodChoice == "CD")
        {
            paymentMethod = payMethodChoice;
            break;
        }
        else
        {
            Console.WriteLine("Error: Invalid payment method. Please enter 'CC', 'PP', or 'CD' only.");
        }
    }

    newOrder.OrderPaymentMethod = paymentMethod;
    newOrder.OrderStatus = "Pending";
    newOrder.OrderPaid = true;

    // Add to customer and restaurant
    customer.AddOrder(newOrder);
    restaurant.orderQueue.Enqueue(newOrder);

    // Store mappings
    orderToCustomerMap[newOrder] = customer;
    orderToRestaurantMap[newOrder] = restaurant;

    // Append to orders.csv
    // Build itemsStr using loop instead of string.Join and LINQ
    string itemsStr = "";
    for (int i = 0; i < newOrder.itemList.Count; i++)
    {
        if (i > 0) itemsStr += "|";
        itemsStr += newOrder.itemList[i].ItemName + ", " + newOrder.itemList[i].QtyOrdered;
    }
    string orderLine = $"{newOrder.OrderId},{custEmail},{restId},{delivDateTime:dd/MM/yyyy},{delivDateTime:HH:mm},{delivAddr},{newOrder.OrderDateTime:dd/MM/yyyy HH:mm},{orderTotal:F1},{newOrder.OrderStatus},{itemsStr},{paymentMethod}";
    File.AppendAllText("orders.csv", "\n" + orderLine);

    nextOrderId++;
    Console.WriteLine($"\nOrder {newOrder.OrderId} created successfully! Status: {newOrder.OrderStatus}");
}

// FEATURE 6 - Process an order (Q)
void ProcessOrder()
{
    Console.WriteLine("Process Order");
    Console.WriteLine("=============");
    Console.Write("Enter Restaurant ID: ");
    string rId = Console.ReadLine().Trim();

    if (!restaurantMap.ContainsKey(rId))
    {
        Console.WriteLine("Error: Restaurant not found.");
        return;
    }

    Restaurant restaurant = restaurantMap[rId];

    if (restaurant.orderQueue.Count == 0)
    {
        Console.WriteLine("No orders to process for this restaurant.");
        return;
    }

    // Create temporary queue to process orders
    Queue<Order> tempQueue = new Queue<Order>();

    while (restaurant.orderQueue.Count > 0)
    {
        Order current = restaurant.orderQueue.Dequeue();

        // Find customer for this order
        Customer customer = orderToCustomerMap.ContainsKey(current) ? orderToCustomerMap[current] : null;
        string customerName = customer != null ? customer.customerName : "Unknown";

        Console.WriteLine($"\nOrder {current.OrderId}:");
        Console.WriteLine($"Customer: {customerName}");
        Console.WriteLine("Ordered Items:");
        current.DisplayOrderedFoodItems();
        Console.WriteLine($"Delivery date/time: {current.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${current.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {current.OrderStatus}");

        Console.Write("\n[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string option = Console.ReadLine().Trim();

        if (option == "C")
        {
            if (current.OrderStatus == "Pending")
            {
                current.OrderStatus = "Preparing";
                Console.WriteLine($"\nOrder {current.OrderId} confirmed. Status: Preparing");
            }
            else
            {
                Console.WriteLine($"Error: Can only confirm orders with 'Pending' status.");
            }
        }
        else if (option == "R")
        {
            if (current.OrderStatus == "Pending")
            {
                current.OrderStatus = "Rejected";
                refundStack.Push(current);
                Console.WriteLine($"\nOrder {current.OrderId} rejected. Refund of ${current.OrderTotal:F2} processed.");
            }
            else
            {
                Console.WriteLine($"Error: Can only reject orders with 'Pending' status.");
            }
        }
        else if (option == "S")
        {
            Console.WriteLine($"\nOrder {current.OrderId} skipped.");
        }
        else if (option == "D")
        {
            if (current.OrderStatus == "Preparing")
            {
                current.OrderStatus = "Delivered";
                Console.WriteLine($"\nOrder {current.OrderId} delivered. Status: Delivered");
            }
            else
            {
                Console.WriteLine($"Error: Can only deliver orders with 'Preparing' status.");
            }
        }
        else
        {
            Console.WriteLine("Error: Invalid option. Please enter 'C', 'R', 'S', or 'D' only. Order skipped.");
        }

        tempQueue.Enqueue(current);
    }

    // Restore queue
    while (tempQueue.Count > 0)
    {
        restaurant.orderQueue.Enqueue(tempQueue.Dequeue());
    }
}

// FEATURE 7 - Modify an existing order (Yi Kai)
void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];
    // Get pending orders using foreach instead of .Where()
    List<Order> pendingOrders = new List<Order>();
    foreach (Order o in customer.orderList)
    {
        if (o.OrderStatus == "Pending")
        {
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (var order in pendingOrders)
    {
        Console.WriteLine(order.OrderId);
    }

    Console.Write("Enter Order ID: ");
    if (!int.TryParse(Console.ReadLine(), out int orderId))
    {
        Console.WriteLine("Error: Invalid Order ID.");
        return;
    }

    // Find order using foreach instead of .FirstOrDefault()
    Order targetOrder = null;
    foreach (Order o in pendingOrders)
    {
        if (o.OrderId == orderId)
        {
            targetOrder = o;
            break;
        }
    }
    if (targetOrder == null)
    {
        Console.WriteLine("Error: Order not found or not pending.");
        return;
    }

    Console.WriteLine("Order Items:");
    targetOrder.DisplayOrderedFoodItems();
    Console.WriteLine($"Address:\n{targetOrder.DeliveryAddress}");
    Console.WriteLine($"Delivery Date/Time:\n{targetOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");

    Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
    string modChoice = Console.ReadLine().Trim();


    if (modChoice == "1")
    {
        // Modify items
        Restaurant restaurant = orderToRestaurantMap.ContainsKey(targetOrder) ? orderToRestaurantMap[targetOrder] : null;
        if (restaurant == null)
        {
            Console.WriteLine("Error: Cannot find restaurant for this order.");
            return;
        }

        Console.WriteLine("\nAvailable Food Items:");
        List<FoodItem> availableItems = restaurant.menuList[0].foodItemList;
        for (int i = 0; i < availableItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {availableItems[i].ItemName} - ${availableItems[i].ItemPrice:F2}");
        }

        targetOrder.itemList.Clear();
        while (true)
        {
            Console.Write("Enter food item number (or 0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int itemNum))
            {
                Console.WriteLine("Error: Please enter a valid number.");
                continue;
            }

            if (itemNum == 0) break;

            if (itemNum < 1 || itemNum > availableItems.Count)
            {
                Console.WriteLine("Error: Invalid item number.");
                continue;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Console.WriteLine("Error: Please enter a valid quantity.");
                continue;
            }

            FoodItem selectedItem = availableItems[itemNum - 1];
            OrderedFoodItem orderedItem = new OrderedFoodItem(selectedItem.ItemName, selectedItem.ItemDesc, selectedItem.ItemPrice, qty);
            targetOrder.AddOrderedFoodItem(orderedItem);
        }

        double oldTotal = targetOrder.OrderTotal;
        double newTotal = targetOrder.CalculateOrderTotal();

        if (newTotal > oldTotal)
        {
            Console.WriteLine($"Additional payment required: ${newTotal - oldTotal:F2}");

            while (true)
            {
                Console.Write("Proceed to payment? [Y/N]: ");
                string payChoice = Console.ReadLine().Trim();

                if (payChoice == "Y")
                {
                    break; // Proceed
                }
                else if (payChoice == "N")
                {
                    Console.WriteLine("Modification cancelled.");
                    return;
                }
                else
                {
                    Console.WriteLine("Error: Invalid input. Please enter 'Y' or 'N' only.");
                }
            }
        }

        Console.WriteLine($"\nOrder {targetOrder.OrderId} updated. New Total: ${newTotal:F2}");
    }
    else if (modChoice == "2")
    {
        // Modify address
        Console.Write("Enter new Delivery Address: ");
        string newAddr = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(newAddr))
        {
            targetOrder.DeliveryAddress = newAddr;
            Console.WriteLine($"Order {targetOrder.OrderId} updated. New Address: {newAddr}");
        }
        else
        {
            Console.WriteLine("Error: Address cannot be empty.");
        }
    }
    else if (modChoice == "3")
    {
        // Modify delivery time
        Console.Write("Enter new Delivery Time (hh:mm): ");
        string newTimeStr = Console.ReadLine().Trim();
        try
        {
            DateTime newDateTime = DateTime.ParseExact(
                $"{targetOrder.DeliveryDateTime:dd/MM/yyyy} {newTimeStr}",
                "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture);
            targetOrder.DeliveryDateTime = newDateTime;
            Console.WriteLine($"\nOrder {targetOrder.OrderId} updated. New Delivery Time: {newTimeStr}");
        }
        catch
        {
            Console.WriteLine("Error: Invalid time format.");
        }
    }
    else
    {
        Console.WriteLine("Error: Invalid option. Please enter '1', '2', or '3' only.");
    }
}


// FEATURE 8 - Delete an existing order (Q)
void DeleteOrder()
{
    Console.WriteLine("Delete Order");
    Console.WriteLine("============");
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];
    // Get pending orders using foreach instead of .Where()
    List<Order> pendingOrders = new List<Order>();
    foreach (Order o in customer.orderList)
    {
        if (o.OrderStatus == "Pending")
        {
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (var order in pendingOrders)
    {
        Console.WriteLine(order.OrderId);
    }

    Console.Write("Enter Order ID: ");
    if (!int.TryParse(Console.ReadLine(), out int orderId))
    {
        Console.WriteLine("Error: Invalid Order ID.");
        return;
    }

    // Find order using foreach instead of .FirstOrDefault()
    Order targetOrder = null;
    foreach (Order o in pendingOrders)
    {
        if (o.OrderId == orderId)
        {
            targetOrder = o;
            break;
        }
    }
    if (targetOrder == null)
    {
        Console.WriteLine("Error: Order not found or not pending.");
        return;
    }

    string customerName = customer.customerName;

    Console.WriteLine($"\nCustomer: {customerName}");
    Console.WriteLine($"Ordered Items: ");
    targetOrder.DisplayOrderedFoodItems();
    Console.WriteLine($"Delivery date/time: {targetOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Total Amount: ${targetOrder.OrderTotal:F2}");
    Console.WriteLine($"Order Status: {targetOrder.OrderStatus}");

    while (true)
    {
        Console.Write("Confirm deletion? [Y/N]: ");
        string confirm = Console.ReadLine().Trim();

        if (confirm == "Y")
        {
            targetOrder.OrderStatus = "Cancelled";
            refundStack.Push(targetOrder);
            Console.WriteLine($"\nOrder {targetOrder.OrderId} cancelled. Refund of ${targetOrder.OrderTotal:F2} processed.");
            break;
        }
        else if (confirm == "N")
        {
            Console.WriteLine("\nDeletion cancelled.");
            break;
        }
        else
        {
            Console.WriteLine("Error: Invalid input. Please enter 'Y' or 'N' only.");
        }
    }
}

// ADVANCED FEATURE (a) - Bulk processing of unprocessed orders for current day (Yi Kai)
void BulkProcessOrders()
{
    Console.WriteLine("Bulk Process Unprocessed Orders");
    Console.WriteLine("================================");

    // Get today's date
    DateTime today = DateTime.Today;
    Console.WriteLine($"Processing orders for: {today:dd/MM/yyyy}\n");

    // Count pending orders
    int totalPendingOrders = 0;
    int processedCount = 0;
    int preparingCount = 0;
    int rejectedCount = 0;

    // Process each restaurant's queue
    foreach (Restaurant restaurant in restaurantList)
    {
        Queue<Order> tempQueue = new Queue<Order>();

        while (restaurant.orderQueue.Count > 0)
        {
            Order order = restaurant.orderQueue.Dequeue();

            if (order.OrderStatus == "Pending")
            {
                totalPendingOrders++;

                // Check if delivery time is less than 1 hour from now
                TimeSpan timeUntilDelivery = order.DeliveryDateTime - DateTime.Now;

                if (timeUntilDelivery.TotalHours < 1)
                {
                    // Reject - not enough time
                    order.OrderStatus = "Rejected";
                    refundStack.Push(order);
                    rejectedCount++;
                    processedCount++;
                    Console.WriteLine($"Order {order.OrderId} - REJECTED (Delivery time < 1 hour): Refund ${order.OrderTotal:F2}");
                }
                else
                {
                    // Confirm - enough time
                    order.OrderStatus = "Preparing";
                    preparingCount++;
                    processedCount++;
                    Console.WriteLine($"Order {order.OrderId} - CONFIRMED: Status changed to Preparing");
                }
            }

            tempQueue.Enqueue(order);
        }

        // Restore queue
        while (tempQueue.Count > 0)
        {
            restaurant.orderQueue.Enqueue(tempQueue.Dequeue());
        }
    }

    // Display summary statistics
    Console.WriteLine("\n========== Summary Statistics ==========");
    Console.WriteLine($"Total Pending Orders Found: {totalPendingOrders}");
    Console.WriteLine($"Orders Processed: {processedCount}");
    Console.WriteLine($"  - Preparing: {preparingCount}");
    Console.WriteLine($"  - Rejected: {rejectedCount}");

    // Calculate total orders in system
    // Calculate total orders in system using loop instead of LINQ
    int totalOrdersInSystem = 0;
    for (int i = 0; i < customerList.Count; i++)
    {
        totalOrdersInSystem += customerList[i].orderList.Count;
    }

    if (totalOrdersInSystem > 0)
    {
        double percentage = (processedCount * 100.0) / totalOrdersInSystem;
        Console.WriteLine($"\nPercentage of orders automatically processed: {percentage:F2}%");
        Console.WriteLine($"  ({processedCount} processed out of {totalOrdersInSystem} total orders)");
    }
    else
    {
        Console.WriteLine("\nNo orders in the system.");
    }
}
// ADVANCED FEATURE (b) - Display total order amount (Q)
void DisplayTotalOrderAmount()
{
    // Title of the report
    Console.WriteLine("Total Order Amounts and Revenue Report");
    Console.WriteLine("======================================\n");

    // Fixed values used for calculation
    double GRUBEROO_COMMISSION = 0.30; // Gruberoo earns 30% from food sales
    double DELIVERY_FEE = 5.00;        // Fixed delivery fee per order

    // Variables to store overall totals across ALL restaurants
    double grandDeliveredFoodSales = 0; // Total food sales (excluding delivery fees)
    double grandTotalRefunds = 0;      // Total refunds from all restaurants
    int grandDeliveredCount = 0;       // Total number of delivered orders

    // Loop through each restaurant in the system
    for (int r = 0; r < restaurantList.Count; r++)
    {
        Restaurant restaurant = restaurantList[r];

        // Display restaurant header
        Console.WriteLine($"\n{restaurant.restaurantName} ({restaurant.restaurantId})");
        Console.WriteLine(new string('-', 50));

        // Variables to track this restaurant’s totals
        double restaurantDeliveredFoodSales = 0; // Food sales only (no delivery fee)
        double restaurantTotalRefunds = 0;      // Refund total for this restaurant
        int deliveredCount = 0;                 // Count of delivered orders
        int refundedCount = 0;                  // Count of refunded orders

        // Loop through all orders stored in the restaurant mapping dictionary
        foreach (KeyValuePair<Order, Restaurant> kvp in orderToRestaurantMap)
        {
            Order order = kvp.Key;
            Restaurant mappedRestaurant = kvp.Value;

            // Skip orders that do not belong to the current restaurant
            if (mappedRestaurant.restaurantId != restaurant.restaurantId)
                continue;

            // If the order was successfully delivered
            if (order.OrderStatus == "Delivered")
            {
                // Remove delivery fee to get only the food sales portion
                double foodPart = order.OrderTotal - DELIVERY_FEE;

                // Safety check to prevent negative values
                if (foodPart < 0)
                {
                    foodPart = 0;
                }

                // Add to this restaurant’s delivered food sales
                restaurantDeliveredFoodSales += foodPart;
                deliveredCount++;
            }
            // If the order was rejected or cancelled (refunded)
            else if (order.OrderStatus == "Rejected" || order.OrderStatus == "Cancelled")
            {
                // Add full order amount to refund total
                restaurantTotalRefunds += order.OrderTotal;
                refundedCount++;
            }
        }

        // Display this restaurant’s results
        Console.WriteLine($"Delivered Orders: {deliveredCount}");
        Console.WriteLine($"  Total Order Amount (less delivery fee): ${restaurantDeliveredFoodSales:F2}");

        Console.WriteLine($"\nRefunded Orders: {refundedCount}");
        Console.WriteLine($"  Total Refunds: ${restaurantTotalRefunds:F2}");

        // Add this restaurant’s values to the overall totals
        grandDeliveredFoodSales += restaurantDeliveredFoodSales;
        grandTotalRefunds += restaurantTotalRefunds;
        grandDeliveredCount += deliveredCount;
    }

    // Display overall summary for all restaurants
    Console.WriteLine("\n" + new string('=', 50));
    Console.WriteLine("OVERALL SUMMARY");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine($"Total Order Amount (less delivery fee): ${grandDeliveredFoodSales:F2}");
    Console.WriteLine($"Total Refunds: ${grandTotalRefunds:F2}");

    // Calculate Gruberoo's final earnings
    // Earnings = 30% commission from food sales + all delivery fees
    double totalDeliveryFees = grandDeliveredCount * DELIVERY_FEE;
    double gruberooCommission = grandDeliveredFoodSales * GRUBEROO_COMMISSION;
    double finalGruberooEarnings = gruberooCommission + totalDeliveryFees;

    // Display Gruberoo earnings breakdown
    Console.WriteLine($"\nFinal Amount Gruberoo Earns: ${finalGruberooEarnings:F2}");
    Console.WriteLine($"  (Commission: ${gruberooCommission:F2} + Delivery Fees: ${totalDeliveryFees:F2})");
}


// ADVANCED FEATURE (c) - Display customer order history and statistics (Yi Kai)
void DisplayCustomerOrderHistory()
{
    Console.WriteLine("Customer Order History & Statistics");
    Console.WriteLine("===================================\n");

    // Get customer email
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];

    if (customer.orderList.Count == 0)
    {
        Console.WriteLine("No orders found for this customer.");
        return;
    }

    // Initialize counters
    int deliveredCount = 0;
    int pendingCount = 0;
    int preparingCount = 0;
    int cancelledCount = 0;
    int rejectedCount = 0;

    double totalSpent = 0;
    double pendingValue = 0;

    // Dictionary to count orders per restaurant
    Dictionary<string, int> restaurantOrderCount = new Dictionary<string, int>();

    // Display header
    Console.WriteLine($"\nOrder History for {customer.customerName} ({customer.emailAddress})");
    Console.WriteLine(new string('-', 90));
    Console.WriteLine($"{"Order ID",-10} {"Restaurant",-30} {"Delivery Date",-20} {"Total",-10} {"Status",-15}");
    Console.WriteLine(new string('-', 90));

    // Process each order
    foreach (Order order in customer.orderList)
    {
        // Get restaurant info
        Restaurant rest = null;
        if (orderToRestaurantMap.ContainsKey(order))
        {
            rest = orderToRestaurantMap[order];
        }
        string restName = rest != null ? rest.restaurantName : "Unknown";

        // Display order
        Console.WriteLine($"{order.OrderId,-10} {restName,-30} {order.DeliveryDateTime:dd/MM/yyyy HH:mm}    ${order.OrderTotal,-8:F2} {order.OrderStatus,-15}");

        // Count by status
        if (order.OrderStatus == "Delivered")
        {
            deliveredCount++;
            totalSpent += order.OrderTotal;
        }
        else if (order.OrderStatus == "Pending")
        {
            pendingCount++;
            pendingValue += order.OrderTotal;
        }
        else if (order.OrderStatus == "Preparing")
        {
            preparingCount++;
        }
        else if (order.OrderStatus == "Cancelled")
        {
            cancelledCount++;
        }
        else if (order.OrderStatus == "Rejected")
        {
            rejectedCount++;
        }

        // Count orders per restaurant
        if (rest != null)
        {
            if (restaurantOrderCount.ContainsKey(restName))
            {
                restaurantOrderCount[restName]++;
            }
            else
            {
                restaurantOrderCount[restName] = 1;
            }
        }
    }

    // Display statistics
    Console.WriteLine("\n" + new string('=', 90));
    Console.WriteLine("ORDER STATISTICS");
    Console.WriteLine(new string('=', 90));
    Console.WriteLine($"Total Orders Placed: {customer.orderList.Count}");

    Console.WriteLine($"\nBreakdown by Status:");
    Console.WriteLine($"  Delivered: {deliveredCount}");
    Console.WriteLine($"  Pending: {pendingCount}");
    Console.WriteLine($"  Preparing: {preparingCount}");
    Console.WriteLine($"  Cancelled: {cancelledCount}");
    Console.WriteLine($"  Rejected: {rejectedCount}");

    Console.WriteLine($"\nFinancial Summary:");
    Console.WriteLine($"  Total Spent (Delivered Orders): ${totalSpent:F2}");

    if (deliveredCount > 0)
    {
        double avgOrder = totalSpent / deliveredCount;
        Console.WriteLine($"  Average Order Value: ${avgOrder:F2}");
    }
    else
    {
        Console.WriteLine($"  Average Order Value: $0.00 (No delivered orders)");
    }

    Console.WriteLine($"  Pending Order Value: ${pendingValue:F2}");

    // Find most frequent restaurant
    string mostFrequentRest = "";
    int maxCount = 0;

    foreach (var kvp in restaurantOrderCount)
    {
        if (kvp.Value > maxCount)
        {
            maxCount = kvp.Value;
            mostFrequentRest = kvp.Key;
        }
    }

    if (!string.IsNullOrEmpty(mostFrequentRest))
    {
        Console.WriteLine($"\nMost Ordered Restaurant: {mostFrequentRest} ({maxCount} orders)");
    }

    Console.WriteLine("\n" + new string('=', 90));
}


// ADVANCED FEATURE (e) - Customer Notifs (Q)
void DisplayCustomerNotifications()
{
    Console.WriteLine("Customer Notifications & ETA");
    Console.WriteLine("============================\n");

    // Step 1: Ask for customer email
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    // Step 2: Validate customer exists
    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];

    // Step 3: Check if customer has any orders
    if (customer.orderList.Count == 0)
    {
        Console.WriteLine("No orders found for this customer.");
        return;
    }

    // Step 4: Collect only active orders (Pending or Preparing)
    List<Order> activeOrders = new List<Order>();

    for (int i = 0; i < customer.orderList.Count; i++) // Loop through ALL orders the customer has made
    {
        string status = customer.orderList[i].OrderStatus; // Get the status of the current order

        if (status == "Pending" || status == "Preparing") //condition of orders to add into activeOrders 
        {
            activeOrders.Add(customer.orderList[i]);
        }
    }

    // Step 5: If no active orders
    if (activeOrders.Count == 0)
    {
        Console.WriteLine("No active orders (Pending/Preparing) found.");
        return;
    }

    // Step 6: Print table header
    Console.WriteLine($"\nActive Orders for {customer.customerName} ({customer.emailAddress})");
    Console.WriteLine(new string('-', 100));
    Console.WriteLine($"{"Order ID",-10} {"Restaurant",-25} {"Delivery Time",-20} {"Status",-12} {"ETA(min)",-10} {"Urgency",-10}");
    Console.WriteLine(new string('-', 100));

    // Counters for summary statistics
    int pendingCount = 0;
    int preparingCount = 0;
    int urgentCount = 0;
    int overdueCount = 0;

    // Step 7: Loop through each active order
    for (int i = 0; i < activeOrders.Count; i++)
    {
        Order order = activeOrders[i];

        // Step 7A: Find restaurant of this order using mapping dictionary
        Restaurant rest = null;

        if (orderToRestaurantMap.ContainsKey(order))
        {
            rest = orderToRestaurantMap[order];
        }

        // Step 7B: Get restaurant name safely (no ternary)
        string restName; // declare variable to store restaurant name 

        if (rest != null)
        {
            restName = rest.restaurantName; // if found, store the actual name 
        }
        else
        {
            restName = "Unknown";
        }

        // Step 7C: Calculate ETA in minutes
        TimeSpan diff = order.DeliveryDateTime - DateTime.Now;
        int etaMins = (int)Math.Round(diff.TotalMinutes);

        // Step 7D: Determine urgency label
        string urgency = "";

        if (etaMins < 0)
        {
            urgency = "OVERDUE";
            overdueCount++;
        }
        else if (etaMins < 60)
        {
            urgency = "URGENT";
            urgentCount++;
        }
        else if (etaMins <= 180)
        {
            urgency = "SOON";
        }
        else
        {
            urgency = "SCHEDULED";
        }

        // Step 7E: Count order status
        if (order.OrderStatus == "Pending")
        {
            pendingCount++;
        }
        else if (order.OrderStatus == "Preparing")
        {
            preparingCount++;
        }

        // Step 7F: Display order row
        Console.WriteLine($"{order.OrderId,-10} {restName,-25} {order.DeliveryDateTime,-20:dd/MM/yyyy HH:mm} {order.OrderStatus,-12} {etaMins,-10} {urgency,-10}");

        // Step 7G: Simulated notification message
        Console.WriteLine("  Notification:");

        if (order.OrderStatus == "Pending")
        {
            Console.WriteLine($"  \"Your order {order.OrderId} is pending confirmation by {restName}.\"");
        }
        else if (order.OrderStatus == "Preparing")
        {
            if (etaMins < 0)
            {
                Console.WriteLine($"  \"Your order {order.OrderId} may be delayed. Please contact support if needed.\"");
            }
            else
            {
                Console.WriteLine($"  \"Good news! {restName} is preparing your order {order.OrderId}. Estimated arrival in {etaMins} minutes.\"");
            }
        }

        Console.WriteLine(); // spacing between orders
    }

    // Step 8: Summary statistics (like your friend's feature style)
    Console.WriteLine(new string('=', 100));
    Console.WriteLine("NOTIFICATION SUMMARY");
    Console.WriteLine(new string('=', 100));
    Console.WriteLine($"Total Active Orders: {activeOrders.Count}");
    Console.WriteLine($" Pending: {pendingCount}");
    Console.WriteLine($" Preparing: {preparingCount}");
    Console.WriteLine(new string('=', 10));
    Console.WriteLine($"Urgent (< 60 mins): {urgentCount}");
    Console.WriteLine($"Overdue: {overdueCount}");

    // Step 9: Suggestion message
    if (overdueCount > 0)
    {
        Console.WriteLine("\nSuggestion: Some orders are overdue. Consider contacting the restaurant or support.");
    }
    else if (urgentCount > 0)
    {
        Console.WriteLine("\nSuggestion: Urgent orders are arriving soon. Ensure someone is available to receive them.");
    }

    Console.WriteLine(new string('=', 100));
}


// Save queue and stack on exit
void SaveQueueAndStack()
{
    // Save queue
    using (StreamWriter sw = new StreamWriter("queue.csv"))
    {
        sw.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items,PaymentMethod");

        foreach (Restaurant r in restaurantList)
        {
            foreach (Order o in r.orderQueue)
            {
                // Find customer and restaurant for this order
                Customer cust = orderToCustomerMap.ContainsKey(o) ? orderToCustomerMap[o] : null;
                Restaurant rest = orderToRestaurantMap.ContainsKey(o) ? orderToRestaurantMap[o] : null;

                string custEmail = cust != null ? cust.emailAddress : "";
                string restId = rest != null ? rest.restaurantId : "";

                // Build itemsStr using loop instead of string.Join and LINQ
                string itemsStr = "";
                for (int i = 0; i < o.itemList.Count; i++)
                {
                    if (i > 0) itemsStr += "|";
                    itemsStr += o.itemList[i].ItemName + ", " + o.itemList[i].QtyOrdered;
                }

                sw.WriteLine($"{o.OrderId},{custEmail},{restId},{o.DeliveryDateTime:dd/MM/yyyy},{o.DeliveryDateTime:HH:mm},{o.DeliveryAddress},{o.OrderDateTime:dd/MM/yyyy HH:mm},{o.OrderTotal:F1},{o.OrderStatus},{itemsStr},{o.OrderPaymentMethod}");
            }
        }
    }

    // Save stack
    using (StreamWriter sw = new StreamWriter("stack.csv"))
    {
        sw.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items,PaymentMethod");

        foreach (Order o in refundStack)
        {
            // Find customer and restaurant for this order
            Customer cust = orderToCustomerMap.ContainsKey(o) ? orderToCustomerMap[o] : null;
            Restaurant rest = orderToRestaurantMap.ContainsKey(o) ? orderToRestaurantMap[o] : null;

            string custEmail = cust != null ? cust.emailAddress : "";
            string restId = rest != null ? rest.restaurantId : "";

            string itemsStr = "";
            for (int i = 0; i < o.itemList.Count; i++)
            {
                if (i > 0) itemsStr += "|";
                itemsStr += o.itemList[i].ItemName + ", " + o.itemList[i].QtyOrdered;
            }

            sw.WriteLine($"{o.OrderId},{custEmail},{restId},{o.DeliveryDateTime:dd/MM/yyyy},{o.DeliveryDateTime:HH:mm},{o.DeliveryAddress},{o.OrderDateTime:dd/MM/yyyy HH:mm},{o.OrderTotal:F1},{o.OrderStatus},{itemsStr},{o.OrderPaymentMethod}");
        }
    }

    Console.WriteLine("\nQueue and stack data saved successfully!");
}

