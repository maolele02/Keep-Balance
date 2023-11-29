using System.Collections.Generic;
namespace XConfig 
{
public class Jewel
{
private Jewel(){ }
public class Data
{
public int id;
public string icon;
public float weight;
}
Dictionary<int, Data> _Items = new Dictionary<int, Data>()
{
[1] = new Data()
{
id = 1,
weight = 500f,
},

[2] = new Data()
{
id = 2,
weight = 500f,
},

[3] = new Data()
{
id = 3,
weight = 500f,
},

[4] = new Data()
{
id = 4,
weight = 500f,
},

[5] = new Data()
{
id = 5,
weight = 500f,
},

[6] = new Data()
{
id = 6,
weight = 500f,
},

[7] = new Data()
{
id = 7,
weight = 500f,
},

[8] = new Data()
{
id = 8,
weight = 500f,
},

[9] = new Data()
{
id = 9,
weight = 500f,
},
};

static Jewel __item;
public static Dictionary<int, Data> Items{ get{ if (__item == null) __item = new Jewel();return __item._Items; }}

}
}