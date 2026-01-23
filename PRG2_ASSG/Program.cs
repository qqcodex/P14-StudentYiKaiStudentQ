//==========================================================
// Student Number : S10272951
// Student Name : Dg Muhammad Aqil Bin Md Alias
// Partner Name : Tan Yi Kai
//==========================================================

using PRG2_ASSG;
using System.Numerics;

//qn1 - Q
List<Restaurant> restaurants = new List<Restaurant>();
void InitialiseRestaurant()
{
    List<string> csvList = File.ReadAllLines("restaurants.csv").ToList();

    for (int i = 1; i < csvList.Count; i++)
    {
        string[] details = csvList[i].Split(',');
        string rid = details[0];
        string rn = details[1];
        string re = details[2];

        Restaurant r = new Restaurant(rid, rn, re);
        restaurants.Add(r);
    }
}

InitialiseRestaurant();
Console.WriteLine(restaurants.Count);


//qn4 - Q 

//qn6 - Q

//qn8 - Q 


//qn2 - Yi Kai

//qn3 - Yi Kai

//qn5 - Yi Kai

//qn7 - Yi Kai