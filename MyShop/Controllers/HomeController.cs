using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static MyShop.Models.ApplicationUser;
using System.Threading.Tasks;
using PagedList.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using System.Data.Entity;
using MyShop.Models.ViewModels;
using System.Text.RegularExpressions;

namespace MyShop.Controllers
{
    public class HomeController : Controller
    {

        static string searchNameConst = null;
        static Category categorySelect = new Category();
        ApplicationDbContext ctx = new ApplicationDbContext();
        static Settings SettingsStat = new Settings();

        //Главная
        public ActionResult Index()
        {
            SettingsStat = ctx.Settings.FirstOrDefault(c => c.Id == 1);

            return View();
        }
        //Каталог товаров
        [HttpGet]
        public async Task<ActionResult> Сatalog(int? page, int? id)
        {
            SettingsStat = ctx.Settings.FirstOrDefault(c => c.Id == 1);

            List<Product> productsList = new List<Product>();
            categorySelect = ctx.Categorys.FirstOrDefault(q => q.Id == id);
            ;
            if (categorySelect != null)
            {
                if (id != null)
                {

                    searchNameConst = null;
                }

           ;
                if (searchNameConst != null)
                {
                    if (Regex.IsMatch(searchNameConst, @"^\d+$"))
                    {
                        int searchNameOrderINT = Convert.ToInt32(searchNameConst);
                        productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Id== searchNameOrderINT).ToListAsync();
                        ;
                        
                    }
                    else
                    {
                        productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Name.Contains(searchNameConst)).ToListAsync();
                        
                    }

                   
                }
                else
                {

                    productsList = await ctx.Products.Include(p => p.Category).Where(v => id == null || id == 0 || v.CategoryId == id).ToListAsync();

                }
            }

            else
            {
                productsList = await ctx.Products.Include(p => p.Category).ToListAsync();
            }
           ;
            //   return View(productsList);
            int pageSize = SettingsStat.PageSizeUser;
            int pageNumber = (page ?? 1);
            ;
            return View(productsList.ToPagedList(pageNumber, pageSize));

        }
        [HttpPost]
        public async Task<ActionResult> Сatalog(string searchName, int? id, int? page)
        {
            int pageSize;
            int pageNumber;
            List<Product> productsList = new List<Product>();

            if (id != null)
            {
                categorySelect = ctx.Categorys.FirstOrDefault(q => q.Id == id);
            }
            ;
            if (searchName != null)
            {
                searchNameConst = searchName;
                if (Regex.IsMatch(searchNameConst, @"^\d+$"))
                {
                    int searchNameOrderINT = Convert.ToInt32(searchNameConst);
                    productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Id == searchNameOrderINT).ToListAsync();
                    ;

                }
                else
                {
                    productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Name.Contains(searchNameConst)).ToListAsync();

                }

              pageSize = SettingsStat.PageSizeUser;
                 pageNumber = (page ?? 1);
                ;
                return View(productsList.ToPagedList(pageNumber, pageSize));
            }
        pageSize = SettingsStat.PageSizeUser;
           pageNumber = (page ?? 1);
            ;
            return View(productsList.ToPagedList(pageNumber, pageSize));
        }
        //Формирование меню католог товаров
        public ActionResult CategoryMenu()
        {
            var category = ctx.Categorys.ToList();
            return PartialView("_CategoryMenu", category);
        }
     /*   [HttpPost]
        public ActionResult СatalogFilter(string searchName, int? page)
        {
            List<Product> productsList = new List<Product>();
            /*
                 int pageSize = 3; // количество объектов на страницу
                var productsPerPages = ctx.Products.Include(p => p.Category).ToList().Skip((page - 1) * pageSize).Take(pageSize);
                 PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = productsPerPages.Count() };
            ViewModelPage ivm = new ViewModelPage { PageInfo = pageInfo, Products = productsPerPages };
                 return View(ivm);
        
            if (searchName != null)
            {
                productsList = ctx.Products.Include(p => p.Category).Where(p => p.Name.Contains(searchName)).ToList();
            }
            else
            {
                productsList = ctx.Products.Include(p => p.Category).ToList();
            }

            //   return View(productsList);
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            ;
            return PartialView(productsList.ToPagedList(pageNumber, pageSize));
        }
*/
       //Детали о продукте
        [HttpGet]
        public ActionResult DetailsProductUser(int? id)
        {
            if (id is null) return HttpNotFound();

            {


                Product detailsProductUser = ctx.Products.Include(p => p.Category).FirstOrDefault(c => c.Id == id);
                SelectList SelectCategory1 = new SelectList(ctx.Categorys, "id", "Name");
                ViewBag.CategorysShow1 = SelectCategory1;
                ProductVM productDetails = new ProductVM(detailsProductUser);
                if (detailsProductUser is null) return HttpNotFound();

                productDetails.GaleryImages = Directory.EnumerateFiles(Server.MapPath("~/Files/Products/" + detailsProductUser.Id + "/Gallery")).Select(fn => Path.GetFileName(fn));

                return View(productDetails);
            }
        }
        [Authorize]
        //Получение списка товаров user
        public ActionResult OrdersUser()
        {
            string userId = User.Identity.GetUserId();
            List<OrdersUserVM> ordersUser = new List<OrdersUserVM>();

            List<OrderVM> orders = ctx.Orders.Where(x => x.UserOrderId == userId).ToArray().Select(z => new OrderVM(z)).OrderByDescending(v => v.CreateAtOrder).ToList();
        
            foreach (var orderItem in orders)
            {
                Dictionary<string, int> productQty = new Dictionary<string, int>();
                decimal total = 0m;
                List<OrderDetails> orderDetailsListUser = ctx.OrderDetails.Where(s => s.OrderId == orderItem.Id).ToList();

                foreach (var orderDetailsItem in orderDetailsListUser)
                {
                    Product product = ctx.Products.FirstOrDefault(z => z.Id == orderDetailsItem.ProductId);
                    productQty.Add(product.Name, orderDetailsItem.Quantity);
                    //получаем общюю стоимость товаров
                    total += orderDetailsItem.Quantity * product.Price;

                }


                ordersUser.Add(new OrdersUserVM
                {
                    OrderId = orderItem.Id,
                    Total = total,
                    ProductQTY = productQty,
                    CreateAtOrder = orderItem.CreateAtOrder

                });
            }
            return View(ordersUser);

        }
        //Контакты 
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //Реклама карусель

        public ActionResult Carousel()
        {
            var carousel = ctx.Carousels.ToList();
            return PartialView("_Carousel", carousel);
        }
        //Реклама Новости
        public ActionResult News()
        {
            var news = ctx.News.ToList();
            return PartialView("_News", news);
        }
        //Доставка и оплата
        public ActionResult PaymentAndDelivery()
        {
           

            return View();
        }
    } 
}