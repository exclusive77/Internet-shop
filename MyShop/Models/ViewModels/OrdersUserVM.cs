using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.Models.ViewModels
{
    public class OrdersUserVM
    {
        [Display(Name = "Номер ордера")]
        public int OrderId { get; set; }
       
        [Display(Name = "Сумма")]
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductQTY { get; set; }
        [Display(Name = "Дата")]
        public DateTime CreateAtOrder { get; set; }
        [AllowHtml]
        [Display(Name = "Примечание/Доставка")]
        public string NoteUser { get; set; }
    }
}