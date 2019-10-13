using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Cache;
using Northwind.Dal;
using Northwind.Entities;
using Northwind.WebUI.Models;
using Northwind.WebUI.Models.Order;

namespace Northwind.WebUI.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {

        internal readonly IRepositoryDal repositoryDal;
        internal readonly IRepositoryCache repositoryCache;


        public AdminController(IRepositoryDal _repositoryDal,IRepositoryCache _repositoryCache)
        {
            repositoryDal = _repositoryDal;
            repositoryCache = _repositoryCache;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Product()
        {

            string sql = @"
                            SET NOCOUNT ON;
                            select p.ProductID,c.CategoryID,p.ProductName,c.CategoryName,
                            p.UnitPrice,p.UnitsInStock from Products p (nolock)
                            left join Categories c on p.CategoryID=c.CategoryID
                        ";

            var result = repositoryDal.GetQuery<ProductListView>(sql);



            var categoryList = repositoryCache.AddCache<Category>("ProductCategoryList",1, "SELECT CategoryID,CategoryName FROM categories (Nolock)");


            return View(result.ToList());
        }

        [Authorize(Roles ="admin")]
        public IActionResult Order(OrderModelView orderModelView)
        {

            #region sql

            string sql = @"
                            SET NOCOUNT ON;
                            Select o.OrderID,o.CustomerID,o.EmployeeID,o.RequiredDate,o.OrderDate,
                            o.ShippedDate,o.ShipName,o.ShipAddress,o.ShipCity,o.ShipCountry,e.FirstName,e.LastName,c.ContactName from Orders o (nolock)
                            left join Employees e (nolock) on e.EmployeeID=o.EmployeeID
                            left join Customers c (nolock) on c.CustomerID=o.CustomerID WHERE 1=1
                        ";
           


            if (!string.IsNullOrEmpty(orderModelView.Customer))
            {
                sql += $" AND  c.ContactName LIKE '%{orderModelView.Customer}%' ";
            }

            if (!string.IsNullOrEmpty(orderModelView.City))
            {
                sql += $" AND  o.ShipCity LIKE '%{orderModelView.City}%' ";
            }

            if (!string.IsNullOrEmpty(orderModelView.Bastar))
            {
                sql += $" AND  convert(varchar(8),o.OrderDate,112)>='{BC_parse_date(orderModelView.Bastar)}' ";
            }

            if (!string.IsNullOrEmpty(orderModelView.Bittar))
            {
                sql += $" AND  convert(varchar(8),o.OrderDate,112)<='{BC_parse_date(orderModelView.Bittar)}' ";
            }

            #endregion

            var result = repositoryDal.GetQuery<OrderListView>(sql);

            if (string.IsNullOrEmpty(orderModelView.page.ToString()) || orderModelView.page < 1 ) orderModelView.page = 1;

            PageModel pm = new PageModel();
            pm.List_Count = result.Count();
            pm.page = orderModelView.page;
            pm.Page_Count = 20;
            pm.Page_Url = $"/admin/order?bastar={orderModelView.Bastar}&bittar={orderModelView.Bittar}&city={orderModelView.City}&customer={orderModelView.Customer}";

            orderModelView.pageModel = pm;

           
            orderModelView.page = (orderModelView.page - 1) * 20;

            result = result.Skip(orderModelView.page).Take(20);

            orderModelView.orderListViews =  result.ToList();

            return View(orderModelView);
        }

        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Admin admin)
        {
            if(ModelState.IsValid)
            {

                var result = repositoryDal.GetAdmin().FirstOrDefault(a => a.email == admin.email && a.Password == admin.Password);

                if(result==null)
                {
                    ModelState.AddModelError("", "Kullanıcı Giriş Başarısız");
                    return View();
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, result.UserName));
                identity.AddClaim(new Claim("UserId", result.Admin_ID.ToString()));

                //identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                //identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

                //foreach (var role in user.Roles)
                //{
                //    identity.AddClaim(new Claim(ClaimTypes.Role, role.Role));
                //}

                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(40),

                });

                return RedirectToAction("index", "admin");


            }
           
            return View(admin);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }



    }
}