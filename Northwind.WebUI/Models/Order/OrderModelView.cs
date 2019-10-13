using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebUI.Models.Order
{
    public class OrderModelView
    {
        public int page { get; set; }
        public string Bastar { get; set; }
        public string Bittar { get; set; }
        public string City { get; set; }
        public string Customer { get; set; }

        public List<OrderListView> orderListViews { get; set; }

        public PageModel pageModel { get; set; }
    }
}
