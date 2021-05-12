using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models.ViewModels
{
    public class OrderAdminDeliveryVM
    {
        [Display(Name = "Номер ордера")]
        public int OrderId { get; set; }
        [Display(Name = "Имя Пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Сумма")]
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductQTY { get; set; }
        [Display(Name = "Дата")]
        public DateTime CreateAtOrder { get; set; }
       
        [Display(Name = "Имя Получателя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия Получателя")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
       
        public string Phone { get; set; }
        [Display(Name = "Город")]

        public string City { get; set; }
        [Display(Name = "Фирма перевозчик")]
        public string DeliveryCompany { get; set; }
        [Display(Name = "Номер отделения(адрес отделения)")]
        public string BranchNumber { get; set; }
        [Display(Name = "Номер накладной доставки")]
        public string DeliveryInvoiceNumber { get; set; }
        public OrderAdminDeliveryVM() { }
        public OrderAdminDeliveryVM(OrderAdminDeliveryVM row)
        {
            OrderId = row.OrderId;
            UserName = row.UserName;
            Total = row.Total;
            ProductQTY = row.ProductQTY;
            CreateAtOrder = row.CreateAtOrder;
       
        FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
            Phone = row.Phone;
            City = row.City;
            DeliveryCompany = row.DeliveryCompany;
            BranchNumber = row.BranchNumber;
            DeliveryInvoiceNumber = row.DeliveryInvoiceNumber;


        }
    }
}
