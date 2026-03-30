using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.DB
{
    public class DBClass
    {
        public static CoffeeShopDBEntities connect=new CoffeeShopDBEntities();
    }
}
