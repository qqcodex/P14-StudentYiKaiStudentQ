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
using System.Linq;

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
LoadSpecialOffers(); // Load special offers for advanced feature
Console.Write();

Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
Console.WriteLine($"{restaurantList.Count} restaurants loaded!");
int totalFoodItems = restaurantList.Sum(r => r.menuList.Sum(m => m.foodItemList.Count));
Console.WriteLine($"{totalFoodItems} food items loaded!");
Console.WriteLine($"{customerList.Count} customers loaded!");
int totalOrders = customerList.Sum(c => c.orderList.Count);
Console.WriteLine($"{totalOrders} orders loaded!\n");

// Main menu loop
bool exit = false;
while (!exit)
{
    DisplayMainMenu();
    string choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            DisplayRestaurantMenuItem();
            break;
        case "2":
            DisplayAllOrders();
            break;
        case "3":
            CreateNewOrder();
            break;
        case "4":
            ProcessOrder();
            break;
        case "5":
            ModifyOrder();
            break;
        case "6":
            DeleteOrder();
            break;
        case "7":
            BulkProcessOrders();
            break;
        case "8":
            DisplayTotalOrderAmount();
            break;
        case "9":
            ApplySpecialOffer();
            break;
        case "0":
            SaveQueueAndStack();
            Console.WriteLine("Thank you for using Gruberoo!");
            exit = true;
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }

    if (!exit)
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}

// FEATURE 1 - Load restaurants and food items
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

// FEATURE 2 - Load customers and orders
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

        string[] data = lines[i].Split(',');

        if (data.Length < 11)
        {
            Console.WriteLine($"Warning: Skipping malformed line {i} in orders.csv");
            continue;
        }

        try
        {
            int orderId = Convert.ToInt32(data[0].Trim());
            string custEmail = data[1].Trim();
            string restId = data[2].Trim();
            string delivDate = data[3].Trim();
            string delivTime = data[4].Trim();
            DateTime delivDateTime = DateTime.ParseExact($"{delivDate} {delivTime}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            string delivAddr = data[5].Trim();
            DateTime createdDateTime = DateTime.ParseExact(data[6].Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            double orderTotal = Convert.ToDouble(data[7].Trim());
            string orderStatus = data[8].Trim();
            string items = data[9].Trim();
            string paymentMethod = data[10].Trim();

            // Create order with only the 8 attributes
            Order o = new Order(orderId, createdDateTime, orderTotal, orderStatus, delivDateTime, delivAddr, paymentMethod, true);

            // Parse items and add to order
            string[] itemPairs = items.Split('|');
            foreach (string itemPair in itemPairs)
            {
                string[] parts = itemPair.Split(',');
                if (parts.Length < 2) continue;

                string itemName = parts[0].Trim();
                int qty = Convert.ToInt32(parts[1].Trim());

                // Find the food item in restaurant menu
                if (restaurantMap.ContainsKey(restId))
                {
                    Restaurant rest = restaurantMap[restId];
                    foreach (Menu menu in rest.menuList)
                    {
                        FoodItem foodItem = menu.foodItemList.FirstOrDefault(f => f.ItemName == itemName);
                        if (foodItem != null)
                        {
                            OrderedFoodItem orderedItem = new OrderedFoodItem(foodItem.ItemName, foodItem.ItemDesc, foodItem.ItemPrice, qty);
                            o.AddOrderedFoodItem(orderedItem);
                            break;
                        }
                    }
                }
            }

            // Store mappings
            if (customerMap.ContainsKey(custEmail))
            {
                Customer customer = customerMap[custEmail];
                customer.AddOrder(o);
                orderToCustomerMap[o] = customer;
            }

            if (restaurantMap.ContainsKey(restId))
            {
                Restaurant restaurant = restaurantMap[restId];
                restaurant.orderQueue.Enqueue(o);
                orderToRestaurantMap[o] = restaurant;
            }

            // Update nextOrderId
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

// Display main menu
void DisplayMainMenu()
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("Basic Features:");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("\nAdvanced Features:");
    Console.WriteLine("7. Bulk process unprocessed orders");
    Console.WriteLine("8. Display total order amounts and revenue");
    Console.WriteLine("9. Apply special offer to order (Bonus)");
    Console.WriteLine("\n0. Exit");
    Console.Write("Enter your choice: ");
}

// FEATURE 3 - List all restaurants and menu items
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

// FEATURE 4 - List all orders
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

            Console.WriteLine($"{o.OrderId,-12}{c.customerName,-25}{restaurantName,-20}{o.DeliveryDateTime:dd/MM/yyyy HH:mm,-25}${o.OrderTotal,-9:F2}{o.OrderStatus}");
        }
    }
}

// FEATURE 5 - Create a new order
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
    string restId = Console.ReadLine().Trim().ToUpper();

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
    Console.Write("Add special request? [Y/N]: ");
    string specialReqChoice = Console.ReadLine().Trim().ToUpper();
    if (specialReqChoice == "Y")
    {
        Console.Write("Enter special request: ");
        string specialRequest = Console.ReadLine().Trim();
        // Note: Special request would be stored if Order class had that attribute
        // For now, we acknowledge it but can't store it per class diagram
    }

    // Calculate total
    double orderTotal = newOrder.CalculateOrderTotal();
    Console.WriteLine($"\nOrder Total: ${orderTotal - 5:F2} + $5.00 (delivery) = ${orderTotal:F2}");

    // Payment
    Console.Write("Proceed to payment? [Y/N]: ");
    string payChoice = Console.ReadLine().Trim().ToUpper();
    if (payChoice != "Y")
    {
        Console.WriteLine("Order cancelled.");
        return;
    }

    string paymentMethod = "";
    while (true)
    {
        Console.Write("Payment method:\n[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string payMethodChoice = Console.ReadLine().Trim().ToUpper();

        if (payMethodChoice == "CC" || payMethodChoice == "PP" || payMethodChoice == "CD")
        {
            paymentMethod = payMethodChoice;
            break;
        }
        else
        {
            Console.WriteLine("Error: Invalid payment method.");
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
    string itemsStr = string.Join("|", newOrder.itemList.Select(i => $"{i.ItemName}, {i.QtyOrdered}"));
    string orderLine = $"{newOrder.OrderId},{custEmail},{restId},{delivDateTime:dd/MM/yyyy},{delivDateTime:HH:mm},{delivAddr},{newOrder.OrderDateTime:dd/MM/yyyy HH:mm},{orderTotal:F1},{newOrder.OrderStatus},{itemsStr},{paymentMethod}";
    File.AppendAllText("orders.csv", "\n" + orderLine);

    nextOrderId++;
    Console.WriteLine($"\nOrder {newOrder.OrderId} created successfully! Status: {newOrder.OrderStatus}");
}

// FEATURE 6 - Process an order
void ProcessOrder()
{
    Console.WriteLine("Process Order");
    Console.WriteLine("=============");
    Console.Write("Enter Restaurant ID: ");
    string rId = Console.ReadLine().Trim().ToUpper();

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
        current.DisplayOrderedFoodItems();
        Console.WriteLine($"Delivery date/time: {current.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${current.OrderTotal:F2}");
        Console.WriteLine($"Order Status: {current.OrderStatus}");

        Console.Write("\n[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string option = Console.ReadLine().Trim().ToUpper();

        if (option == "C")
        {
            if (current.OrderStatus == "Pending")
            {
                current.OrderStatus = "Preparing";
                Console.WriteLine($"Order {current.OrderId} confirmed. Status: Preparing");
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
                Console.WriteLine($"Order {current.OrderId} rejected. Refund of ${current.OrderTotal:F2} processed.");
            }
            else
            {
                Console.WriteLine($"Error: Can only reject orders with 'Pending' status.");
            }
        }
        else if (option == "S")
        {
            Console.WriteLine($"Order {current.OrderId} skipped.");
        }
        else if (option == "D")
        {
            if (current.OrderStatus == "Preparing")
            {
                current.OrderStatus = "Delivered";
                Console.WriteLine($"Order {current.OrderId} delivered. Status: Delivered");
            }
            else
            {
                Console.WriteLine($"Error: Can only deliver orders with 'Preparing' status.");
            }
        }
        else
        {
            Console.WriteLine("Invalid option. Order skipped.");
        }

        tempQueue.Enqueue(current);
    }

    // Restore queue
    while (tempQueue.Count > 0)
    {
        restaurant.orderQueue.Enqueue(tempQueue.Dequeue());
    }
}

// FEATURE 7 - Modify an existing order
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
    List<Order> pendingOrders = customer.orderList.Where(o => o.OrderStatus == "Pending").ToList();

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

    Order targetOrder = pendingOrders.FirstOrDefault(o => o.OrderId == orderId);
    if (targetOrder == null)
    {
        Console.WriteLine("Error: Order not found or not pending.");
        return;
    }

    Console.WriteLine("\nOrder Items:");
    targetOrder.DisplayOrderedFoodItems();
    Console.WriteLine($"\nAddress:\n{targetOrder.DeliveryAddress}");
    Console.WriteLine($"\nDelivery Date/Time:\n{targetOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");

    Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
    string modChoice = Console.ReadLine().Trim();

    switch (modChoice)
    {
        case "1":
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
                targetOrder.AddOrderedFoodItem(orderedItem);
            }

            double oldTotal = targetOrder.OrderTotal;
            double newTotal = targetOrder.CalculateOrderTotal();

            if (newTotal > oldTotal)
            {
                Console.WriteLine($"Additional payment required: ${newTotal - oldTotal:F2}");
                Console.Write("Proceed to payment? [Y/N]: ");
                string payChoice = Console.ReadLine().Trim().ToUpper();
                if (payChoice != "Y")
                {
                    Console.WriteLine("Modification cancelled.");
                    return;
                }
            }

            Console.WriteLine($"Order {targetOrder.OrderId} updated. New Total: ${newTotal:F2}");
            break;

        case "2":
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
            break;

        case "3":
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
                Console.WriteLine($"Order {targetOrder.OrderId} updated. New Delivery Time: {newTimeStr}");
            }
            catch
            {
                Console.WriteLine("Error: Invalid time format.");
            }
            break;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

// FEATURE 8 - Delete an existing order
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
    List<Order> pendingOrders = customer.orderList.Where(o => o.OrderStatus == "Pending").ToList();

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

    Order targetOrder = pendingOrders.FirstOrDefault(o => o.OrderId == orderId);
    if (targetOrder == null)
    {
        Console.WriteLine("Error: Order not found or not pending.");
        return;
    }

    string customerName = customer.customerName;

    Console.WriteLine($"\nCustomer: {customerName}");
    targetOrder.DisplayOrderedFoodItems();
    Console.WriteLine($"Delivery date/time: {targetOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Total Amount: ${targetOrder.OrderTotal:F2}");
    Console.WriteLine($"Order Status: {targetOrder.OrderStatus}");

    Console.Write("\nConfirm deletion? [Y/N]: ");
    string confirm = Console.ReadLine().Trim().ToUpper();

    if (confirm == "Y")
    {
        targetOrder.OrderStatus = "Cancelled";
        refundStack.Push(targetOrder);
        Console.WriteLine($"Order {targetOrder.OrderId} cancelled. Refund of ${targetOrder.OrderTotal:F2} processed.");
    }
    else
    {
        Console.WriteLine("Deletion cancelled.");
    }
}

// ADVANCED FEATURE (a) - Bulk processing of unprocessed orders for current day
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
    int totalOrdersInSystem = customerList.Sum(c => c.orderList.Count);

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

// ADVANCED FEATURE (b) - Display total order amount
void DisplayTotalOrderAmount()
{
    Console.WriteLine("Total Order Amounts and Revenue Report");
    Console.WriteLine("======================================\n");

    double grandTotalRevenue = 0;
    double grandTotalRefunds = 0;
    const double GRUBEROO_COMMISSION = 0.30; // 30%
    const double DELIVERY_FEE = 5.00;

    // Process each restaurant
    foreach (Restaurant restaurant in restaurantList)
    {
        Console.WriteLine($"\n{restaurant.restaurantName} ({restaurant.restaurantId})");
        Console.WriteLine(new string('-', 50));

        double restaurantTotalRevenue = 0;
        double restaurantTotalRefunds = 0;
        int deliveredCount = 0;
        int refundedCount = 0;

        // Get all orders for this restaurant
        List<Order> restaurantOrders = new List<Order>();
        foreach (var kvp in orderToRestaurantMap)
        {
            if (kvp.Value.restaurantId == restaurant.restaurantId)
            {
                restaurantOrders.Add(kvp.Key);
            }
        }

        // Calculate delivered orders revenue
        foreach (Order order in restaurantOrders)
        {
            if (order.OrderStatus == "Delivered")
            {
                // Order total includes delivery fee, subtract it for restaurant revenue
                double orderAmountNoDelivery = order.OrderTotal - DELIVERY_FEE;
                restaurantTotalRevenue += orderAmountNoDelivery;
                deliveredCount++;
            }
        }

        // Calculate refunded orders (from all orders in customer lists)
        foreach (Customer customer in customerList)
        {
            foreach (Order order in customer.orderList)
            {
                if (orderToRestaurantMap.ContainsKey(order) &&
                    orderToRestaurantMap[order].restaurantId == restaurant.restaurantId)
                {
                    if (order.OrderStatus == "Rejected" || order.OrderStatus == "Cancelled")
                    {
                        restaurantTotalRefunds += order.OrderTotal;
                        refundedCount++;
                    }
                }
            }
        }

        Console.WriteLine($"Delivered Orders: {deliveredCount}");
        Console.WriteLine($"  Total Revenue (excl. delivery): ${restaurantTotalRevenue:F2}");
        Console.WriteLine($"\nRefunded Orders: {refundedCount}");
        Console.WriteLine($"  Total Refunds: ${restaurantTotalRefunds:F2}");

        double netRevenue = restaurantTotalRevenue - restaurantTotalRefunds;
        Console.WriteLine($"\nNet Revenue: ${netRevenue:F2}");

        grandTotalRevenue += restaurantTotalRevenue;
        grandTotalRefunds += restaurantTotalRefunds;
    }

    // Display grand totals
    Console.WriteLine("\n" + new string('=', 50));
    Console.WriteLine("OVERALL SUMMARY");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine($"Total Revenue (all restaurants): ${grandTotalRevenue:F2}");
    Console.WriteLine($"Total Refunds (all restaurants): ${grandTotalRefunds:F2}");
    Console.WriteLine($"Net Revenue: ${grandTotalRevenue - grandTotalRefunds:F2}");

    // Calculate Gruberoo's earnings
    double gruberooEarnings = (grandTotalRevenue - grandTotalRefunds) * GRUBEROO_COMMISSION;
    Console.WriteLine($"\nGruberoo Commission (30%): ${gruberooEarnings:F2}");

    // Calculate total delivery fees collected
    int totalDeliveredOrders = 0;
    foreach (Customer customer in customerList)
    {
        totalDeliveredOrders += customer.orderList.Count(o => o.OrderStatus == "Delivered");
    }
    double totalDeliveryFees = totalDeliveredOrders * DELIVERY_FEE;
    Console.WriteLine($"Delivery Fees Collected: ${totalDeliveryFees:F2}");

    double totalGruberooEarnings = gruberooEarnings + totalDeliveryFees;
    Console.WriteLine($"\n*** TOTAL GRUBEROO EARNINGS: ${totalGruberooEarnings:F2} ***");
}

// ADVANCED FEATURE (c) - BONUS: Apply special offer to order
void ApplySpecialOffer()
{
    Console.WriteLine("Apply Special Offer to Order");
    Console.WriteLine("============================\n");

    // First, load special offers if not already loaded
    LoadSpecialOffers();

    // Get customer email
    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine().Trim();

    if (!customerMap.ContainsKey(custEmail))
    {
        Console.WriteLine("Error: Customer not found.");
        return;
    }

    Customer customer = customerMap[custEmail];

    // Display pending orders
    List<Order> pendingOrders = customer.orderList.Where(o => o.OrderStatus == "Pending").ToList();

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (var order in pendingOrders)
    {
        Restaurant rest = orderToRestaurantMap.ContainsKey(order) ? orderToRestaurantMap[order] : null;
        string restName = rest != null ? rest.restaurantName : "Unknown";
        Console.WriteLine($"  {order.OrderId} - {restName} - ${order.OrderTotal:F2}");
    }

    // Get order ID
    Console.Write("\nEnter Order ID: ");
    if (!int.TryParse(Console.ReadLine(), out int orderId))
    {
        Console.WriteLine("Error: Invalid Order ID.");
        return;
    }

    Order targetOrder = pendingOrders.FirstOrDefault(o => o.OrderId == orderId);
    if (targetOrder == null)
    {
        Console.WriteLine("Error: Order not found or not pending.");
        return;
    }

    // Get restaurant for this order
    Restaurant restaurant = orderToRestaurantMap.ContainsKey(targetOrder) ? orderToRestaurantMap[targetOrder] : null;
    if (restaurant == null)
    {
        Console.WriteLine("Error: Cannot find restaurant for this order.");
        return;
    }

    // Display available special offers for this restaurant
    if (restaurant.specialOfferList.Count == 0)
    {
        Console.WriteLine($"\nNo special offers available for {restaurant.restaurantName}.");
        return;
    }

    Console.WriteLine($"\nAvailable Special Offers for {restaurant.restaurantName}:");
    for (int i = 0; i < restaurant.specialOfferList.Count; i++)
    {
        SpecialOffer offer = restaurant.specialOfferList[i];
        Console.WriteLine($"{i + 1}. {offer.offerCode} - {offer.offerDesc}");
        if (offer.discount > 0)
        {
            Console.WriteLine($"   Discount: {offer.discount}%");
        }
    }

    // Select offer
    Console.Write("\nEnter offer number (0 to cancel): ");
    if (!int.TryParse(Console.ReadLine(), out int offerNum) || offerNum < 0 || offerNum > restaurant.specialOfferList.Count)
    {
        Console.WriteLine("Cancelled.");
        return;
    }

    if (offerNum == 0)
    {
        Console.WriteLine("Cancelled.");
        return;
    }

    SpecialOffer selectedOffer = restaurant.specialOfferList[offerNum - 1];

    // Apply discount
    double originalTotal = targetOrder.OrderTotal;
    double discount = 0;

    if (selectedOffer.discount > 0)
    {
        // Percentage discount (apply to items only, not delivery fee)
        double itemsTotal = originalTotal - 5.00; // Subtract delivery fee
        discount = itemsTotal * (selectedOffer.discount / 100.0);
        targetOrder.OrderTotal = originalTotal - discount;
    }
    else
    {
        // Free delivery offer
        if (selectedOffer.offerDesc.ToLower().Contains("free delivery"))
        {
            double itemsTotal = originalTotal - 5.00;
            if (itemsTotal >= 30) // Only if order is over $30
            {
                discount = 5.00;
                targetOrder.OrderTotal = originalTotal - discount;
                Console.WriteLine("Free delivery applied! (Order over $30)");
            }
            else
            {
                Console.WriteLine($"Error: Order must be at least $30 for free delivery. Current: ${itemsTotal:F2}");
                return;
            }
        }
    }

    Console.WriteLine($"\nSpecial Offer Applied: {selectedOffer.offerCode}");
    Console.WriteLine($"Original Total: ${originalTotal:F2}");
    Console.WriteLine($"Discount: -${discount:F2}");
    Console.WriteLine($"New Total: ${targetOrder.OrderTotal:F2}");
    Console.WriteLine($"\nOrder {targetOrder.OrderId} updated successfully!");
}

void LoadSpecialOffers()
{
    try
    {
        var lines = File.ReadAllLines("specialoffers.csv");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] data = lines[i].Split(',');
            if (data.Length < 3) continue;

            string restId = data[0].Trim();
            string offerCode = data[1].Trim();
            string offerDesc = data[2].Trim();

            double discount = 0;
            if (data.Length >= 4 && !string.IsNullOrWhiteSpace(data[3]))
            {
                double.TryParse(data[3].Trim(), out discount);
            }

            if (restaurantMap.ContainsKey(restId))
            {
                SpecialOffer offer = new SpecialOffer(offerCode, offerDesc, discount);
                restaurantMap[restId].AddSpecialOffer(offer);
            }
        }
    }
    catch (Exception)
    {
        // Special offers file might not exist yet
    }
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

                string itemsStr = string.Join("|", o.itemList.Select(i => $"{i.ItemName}, {i.QtyOrdered}"));
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

            string itemsStr = string.Join("|", o.itemList.Select(i => $"{i.ItemName}, {i.QtyOrdered}"));
            sw.WriteLine($"{o.OrderId},{custEmail},{restId},{o.DeliveryDateTime:dd/MM/yyyy},{o.DeliveryDateTime:HH:mm},{o.DeliveryAddress},{o.OrderDateTime:dd/MM/yyyy HH:mm},{o.OrderTotal:F1},{o.OrderStatus},{itemsStr},{o.OrderPaymentMethod}");
        }
    }

    Console.WriteLine("\nQueue and stack data saved successfully!");
}
