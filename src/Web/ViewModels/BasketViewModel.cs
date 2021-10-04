using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class BasketViewModel
    {
        public List<BasketItemVievModel> Items { get; set; } = new List<BasketItemVievModel>();
        public decimal TotalPrice { get; set; }
    }
}
