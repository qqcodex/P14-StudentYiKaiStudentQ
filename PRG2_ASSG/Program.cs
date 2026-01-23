//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

using PRG2_ASSG;
using System.Numerics;

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
        double iprice = Convert.ToDouble(details[3]);

        FoodItem fi = new FoodItem(iname, idesc, iprice);
        Restaurant r = restaurantMap[rid]; //link fooditem to restaurant

        r.menuList[0].AddFoodItem(fi);
    }
}

InitialiseFoodItem();

//qn4 - Q 

//qn6 - Q

//qn8 - Q 


//qn2 - Yi Kai

//qn3 - Yi Kai

//qn5 - Yi Kai

//qn7 - Yi Kai