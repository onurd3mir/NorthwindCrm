using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Dal;
using Microsoft.Extensions.Configuration;


namespace Northwind.WebUI.Controllers
{
    public class BaseController : Controller
    {

        public string BC_parse_date(string date)
        {
            string[] d = date.Split('.');

            string date_new = d[2] + "" + d[1] + "" + d[0];

            return date_new;
        }

    }
}