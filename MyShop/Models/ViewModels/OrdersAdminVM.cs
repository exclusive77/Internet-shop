using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models.ViewModels
{
    public class OrdersAdminVM
    {
        [Display(Name = "Номер ордера")]
        public int OrderId { get; set; }
        [Display(Name = "Имя Пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Сумма")]
        public decimal Total { get; set; }
        public Dictionary<string,int> ProductQTY { get; set; }
        [Display(Name = "Дата")]
        public DateTime CreateAtOrder { get; set; }
    }
}