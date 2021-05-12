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
using PagedList.Mvc;
using MyShop.Models.ViewModels;
using PagedList;
using System.Net.Mail;
using System.Net;

namespace MyShop.Controllers
{
    public class BasketController : Controller
    {
        ApplicationDbContext ctx = new ApplicationDbContext();
        // GET: Basket
        //Корзина список товаров
        [Authorize]
        public ActionResult BasketList()
        {
            List<BasketVM> basketList = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            if (basketList.Count == 0 || Session["basket"] == null)
            {
                ViewBag.MessegeBasket = "Корзина пуста";
                return View();
            }
            else
            {
                ViewBag.MessegeBasket = " ";
            }
            decimal total = 0m;
            foreach (var item in basketList)
            {
                total += item.Total;
            }

            ViewBag.SumTotal = total;
            ;
            return View(basketList);
        }
        //добавление товара в корзину
        [Authorize]
        public ActionResult AddToBasketPartial(int id)
        {
            int quitity = 0;
            decimal price = 0m;
            List<BasketVM> basket = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            BasketVM model = new BasketVM();
            //получаем добавленный продукт
            Product productAdd = ctx.Products.FirstOrDefault(d => d.Id == id);
            //Поиск продукта в корзине
            if (productAdd.Quantity != 0)
            {
                productAdd.Quantity--;
                ctx.SaveChanges();
                var productInbasket = basket.FirstOrDefault(s => s.ProductId == productAdd.Id);
                if (productInbasket == null)
                {
                    basket.Add(new BasketVM()
                    {
                        ProductId = productAdd.Id,
                        Quantity = 1,
                        ProductName = productAdd.Name,
                        Image = productAdd.PathImage,
                        Price = productAdd.Price
                    });
                }
                else
                {
                    productInbasket.Quantity++;
                }
            }
            foreach (var item in basket)
            {

                quitity += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Price = price;
            model.Quantity = quitity;
            Session["basket"] = basket;
            ;
            return PartialView("_AddToBasketPartial", model);
        }
      /*  public JsonResult IncrementProduct22(int productId)
        {
            List<BasketVM> basket = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            BasketVM model = new BasketVM();

            Product productAdd = ctx.Products.FirstOrDefault(d => d.Id == productId);
            //Поиск продукта в корзине
            var productInbasket = basket.FirstOrDefault(s => s.ProductId == productId);
            //добавляем в корзину если есть в наличии
            /*  if (productAdd.Quantity != 0)
               {
                   productAdd.Quantity--;
                   ctx.SaveChanges();

                 

            productInbasket.Quantity++;

            ;
            var result = new { qty = productInbasket.Quantity, price = productInbasket.Price };
            return Json(result, JsonRequestBehavior.AllowGet);

        }*/
        //Добавить единицу товар в корзину
        public ActionResult IncrementProduct(int? id)
        {
            int quitity = 0;
            decimal price = 0m;
            List<BasketVM> basket = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            BasketVM model = new BasketVM();
            ;
            Product productAdd = ctx.Products.FirstOrDefault(d => d.Id == id);
            //Поиск продукта в корзине
            var productInbasket = basket.FirstOrDefault(s => s.ProductId == id);
            //добавляем в корзину если есть в наличии
            /*  if (productAdd.Quantity != 0)
              {
                  productAdd.Quantity--;
                  ctx.SaveChanges();

                 */

            productInbasket.Quantity++;
            foreach (var item in basket)
            {

                quitity += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Price = price;
            model.Quantity = quitity;
            Session["basket"] = basket;
            ;
            return RedirectToAction("BasketList");

        }
        //Удалить единицу товара в корзину
        public ActionResult DecrementProduct(int? id)
        {
            int quitity = 0;
            decimal price = 0m;
            List<BasketVM> basket = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            BasketVM model = new BasketVM();
            ;
            Product productAdd = ctx.Products.FirstOrDefault(d => d.Id == id);
            //Поиск продукта в корзине
            var productInbasket = basket.FirstOrDefault(s => s.ProductId == id);
            //добавляем в корзину если есть в наличии
            if (productInbasket.Quantity > 1)
            {
                productAdd.Quantity++;
                ctx.SaveChanges();



                productInbasket.Quantity--;
                foreach (var item in basket)
                {

                    quitity += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Price = price;
                model.Quantity = quitity;
            }
            Session["basket"] = basket;
            ;
            return RedirectToAction("BasketList");

        }
        //Удаление товара из корзины
        public ActionResult DelProduct(int? id)
        {
            int quitity = 0;
            decimal price = 0m;
            List<BasketVM> basket = Session["basket"] as List<BasketVM> ?? new List<BasketVM>();
            BasketVM model = new BasketVM();
            ;
            Product productAdd = ctx.Products.FirstOrDefault(d => d.Id == id);
            //Поиск продукта в корзине
            var productInbasket = basket.FirstOrDefault(s => s.ProductId == id);
            //добавляем в корзину если есть в наличии
            if (productInbasket.Quantity >= 1)
            {

                int qt_prod = productInbasket.Quantity;
                productAdd.Quantity += qt_prod;
                ctx.SaveChanges();
                basket.Remove(productInbasket);

                foreach (var item in basket)
                {

                    quitity += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Price = price;
                model.Quantity = quitity;
            }
            Session["basket"] = basket;
            ;
            return RedirectToAction("BasketList");

        }
        //иконка корзины на главной страницы
        public ActionResult BasketPartial()
        {
            BasketVM basketModel = new BasketVM();
            //кол-во товаров и цены
            int quelity = 0;
            decimal price = 0;
            if (Session["basket"] != null)
            {
                var listbasket = (List<BasketVM>)Session["basket"];
                foreach (var item in listbasket)
                {
                    quelity += item.Quantity;
                    price += item.Quantity * item.Price;

                }
                basketModel.Quantity = quelity;
                basketModel.Price = price;
            }
            else
            {//если корзина пуста
                basketModel.Price = 0;
                basketModel.Quantity = 0;

            }
            return PartialView("_BasketPartial", basketModel);
        }
        public ActionResult PaypalPartial()
        {
            List<BasketVM> basket = Session["basket"] as List<BasketVM>;
            return PartialView(basket);
        }
//оформление заказа и доставки
        public ActionResult PlaceOrder()
        {
          
           
            string UserBasketId = User.Identity.GetUserId();
            string UserBasketName = User.Identity.GetUserName();
           
             
            ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));

            ApplicationUser UserDelivery =  um1.FindById(UserBasketId);
            Delivery delivery = new Delivery();
         //   delivery.OrderId = order.Id;
            delivery.FirstName = UserDelivery.FirstName;
            delivery.LastName = UserDelivery.LastName;
            delivery.Phone = UserDelivery.Phone;
            delivery.Email = UserDelivery.Email;
            delivery.City = UserDelivery.Address;
            delivery.DeliveryCompany = "Новая почта";
          
            ;
            return View(delivery);
        }
        //оформление заказа и доставки
        [HttpPost]
        public ActionResult PlaceOrder(Delivery delivery)
        {
            if (String.IsNullOrEmpty(delivery.FirstName) || String.IsNullOrEmpty(delivery.LastName) ||
                String.IsNullOrEmpty(delivery.Phone) || String.IsNullOrEmpty(delivery.Email) ||
                String.IsNullOrEmpty(delivery.DeliveryCompany) || String.IsNullOrEmpty(delivery.BranchNumber))
            {
                ViewBag.Massege = "Не все поля заполнены!!!";
                return View(delivery);
            }
            else
            {
                ViewBag.Massege = " ";

                List<BasketVM> basket = Session["basket"] as List<BasketVM>;
                string UserBasketId = User.Identity.GetUserId();
                string UserBasketName = User.Identity.GetUserName();
                Order order = new Order();
                order.UserOrderId = UserBasketId;
                order.CreateAtOrder = DateTime.Now;
                order.UserName = UserBasketName;
                ctx.Orders.Add(order);
                ctx.SaveChanges();
                OrderDetails orderDetails = new OrderDetails();
                ;
                foreach (var item in basket)
                {
                    orderDetails.OrderId = order.Id;
                    orderDetails.UserId = UserBasketId;
                    orderDetails.ProductId = item.ProductId;
                    orderDetails.Quantity = item.Quantity;
                    ctx.OrderDetails.Add(orderDetails);
                    ctx.SaveChanges();
                }
                Delivery deliveryEdit = new Delivery();
                deliveryEdit.OrderId = order.Id;
                deliveryEdit.FirstName = delivery.FirstName;
                deliveryEdit.LastName = delivery.LastName;
                deliveryEdit.Phone = delivery.Phone;
                deliveryEdit.Email = delivery.Email;
                deliveryEdit.City = delivery.City;
                deliveryEdit.DeliveryCompany = delivery.DeliveryCompany;
                deliveryEdit.BranchNumber = delivery.BranchNumber;
                deliveryEdit.DeliveryInvoiceNumber = delivery.DeliveryInvoiceNumber;
                ctx.Deliverys.Add(deliveryEdit);
                ctx.SaveChanges();
           
                Settings settings = ctx.Settings.FirstOrDefault(s => s.Id == 1);
                //отправка сообщения администратору о покупке ,если разрешено в настройках
                if (settings.AllowMessages)
                {
                    MailAddress from = new MailAddress("testshop1977@gmail.com", "Server MySHOP message");
                    MailAddress to = new MailAddress(settings.EmailMassege);

                    MailMessage msg = new MailMessage(from, to);
                    msg.Subject = "New Order";
                    msg.IsBodyHtml = true;

                   
                    msg.Body = $"<p>У вас новый заказ номер :{order.Id},от пользователя:{UserBasketName}</p>";

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("testshop1977@gmail.com", "passhop77");

                    client.Send(msg);
                }
                Session["basket"] = null;
                return RedirectToAction("OrdersUser", "Home")
           ;
        }
           
        }
        //информация о доставке
        public ActionResult DeliverysInfo(int? Id)

        {
            Delivery model = new Delivery();
            if (Id != null)
            {
                model = ctx.Deliverys.FirstOrDefault(p => p.OrderId == Id);
                ;
            }
            return View(model);
        }
        //Примечание к доставке
        public ActionResult PlaceOrderAddNote(int? Id)

        {
            Order model = new Order();
            if (Id != null)
            {
                model = ctx.Orders.FirstOrDefault(p => p.Id == Id);
            }
            return View(model);
        }
        //Примечание к доставке
        [HttpPost]
        public ActionResult PlaceOrderAddNote(Order order)

        {
            Order Editmodel = new Order();
            ;
            if (order != null)
            {
                
                Editmodel = ctx.Orders.FirstOrDefault(p => p.Id == order.Id);
                ;
                Editmodel.NoteUser = order.NoteUser;
                ctx.SaveChanges();
            }
            return RedirectToAction("OrdersUser", "Home");
        }
    }
}