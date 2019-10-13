using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Entities
{
    public class Admin
    {
        public int Admin_ID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public DateTime date { get; set; }
    }
}
