using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        [Display(Name = "Номер заказа")]
        public int OrderId { get; set; }
        public Order OrderDelivery { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Телефон")]
        [RegularExpression("^[+]?\\d*\\(?\\d{3}\\)?\\d{3}-?\\d{2}-?\\d{2}$", ErrorMessage = "Проверьте правильность ввода телефона")]
        public string Phone { get; set; }
        [Display(Name = "Город")]

        public string City { get; set; }
        [Display(Name = "Фирма перевозчик")]
        public string DeliveryCompany { get; set; }
        [Display(Name = "Номер отделения(адрес отделения)")]
        public string BranchNumber { get; set; }
        [Display(Name = "Номер накладной доставки")]
        public string DeliveryInvoiceNumber { get; set; }
        public Delivery() { }
    /*    public Delivery(Delivery row)
        {
            Id = row.Id;
            OrderId = row.OrderId;
            OrderDelivery = row.OrderDelivery;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Email = row.Email;
            Phone = row.Phone;
            City = row.City;
            DeliveryCompany = row.DeliveryCompany;
            BranchNumber = row.BranchNumber;
            DeliveryInvoiceNumber = row.DeliveryInvoiceNumber;
            
            
        }*/
    }
}