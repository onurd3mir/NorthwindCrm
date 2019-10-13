using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebUI.Models
{
    public class PageModel
    {
        public int page { get; set; }

        public int Page_Count { get; set; }

        public int List_Count { get; set; }

        public string Page_Url { get; set; }

    }
}
