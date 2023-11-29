using System.Collections.Generic;
namespace XConfig 
{
public class Card
{
private Card(){ }
public class Data
{
public int id;
public string icon;
public int probability_player;
public int probability_ai;
}
Dictionary<int, Data> _Items = new Dictionary<int, Data>()
{
[1] = new Data()
{
id = 1,
probability_player = 300,
probability_ai = 300,
},

[2] = new Data()
{
id = 2,
probability_player = 150,
probability_ai = 150,
},

[3] = new Data()
{
id = 3,
probability_player = 150,
probability_ai = 150,
},

[4] = new Data()
{
id = 4,
probability_player = 100,
probability_ai = 100,
},

[5] = new Data()
{
id = 5,
probability_player = 100,
probability_ai = 100,
},

[6] = new Data()
{
id = 6,
probability_player = 50,
probability_ai = 50,
},

[7] = new Data()
{
id = 7,
probability_player = 50,
probability_ai = 50,
},

[8] = new Data()
{
id = 8,
probability_player = 30,
probability_ai = 30,
},

[9] = new Data()
{
id = 9,
probability_player = 20,
probability_ai = 20,
},
};

static Card __item;
public static Dictionary<int, Data> Items{ get{ if (__item == null) __item = new Card();return __item._Items; }}

}
}