using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.Models
{
    public class Order
    {
        [Display(Name = "Номер заказа")]
        public int Id { get; set; }
        [Display(Name = "Пользователь ")]
        public string UserOrderId { get; set; }
     
        public ApplicationUser UserOrder { get; set; }
        [Display(Name = "Пользователь ")]
        public string UserName { get; set; }
        [Display(Name = "Дата оформления")]
        public DateTime CreateAtOrder{ get; set; }
        [AllowHtml]
        [Display(Name = "ПримечаниеUser")]
        public string NoteUser { get; set; }
        [AllowHtml]
        [Display(Name = "ПримечаниеАdmin")]
        public string NoteAdmin { get; set; }
    }
}