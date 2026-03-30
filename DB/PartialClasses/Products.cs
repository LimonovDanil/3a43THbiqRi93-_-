using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShop.DB
{
    public partial class Products
    {
        public string FullImagePath
        {
            get
            {
                if (ImagePath != "")
                {
                    return $"/Images/{ImagePath}";
                }
                else
                {
                    if (CategoryId == 1)
                    {
                        return $"/Images/zaglushCoffee.jpg";
                    }
                    else if (CategoryId == 2)
                    {
                        return $"/Images/zaglushTea.jpg";
                    }
                    else if (CategoryId == 3)
                    {
                        return $"/Images/zaglushDesert.jpg";
                    }
                    else if (CategoryId == 4)
                    {
                        return $"/Images/zaglushSand.jpg";
                    }
                    else
                    {
                        return $"/Images/zaglushSok.png";
                    }
                    //  switch (CategoryId)
                    // {
                    // case 1: return $"/Images/zaglushCoffee.jpg";
                    // case 2: return $"/Images/zaglushTea.jpg";
                    // case 3: return $"/Images/zaglushDesert.jpg";
                    // case 4: return $"/Images/zaglushSand.jpg";
                    // case 5: return $"/Images/zaglushSok.jpg";
                    //  }
                }
            }
        }
    }
}
