using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.Models.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
      
        public string UserOrderId { get; set; }

        public ApplicationUser UserOrder { get; set; }

        public DateTime CreateAtOrder { get; set; }
        [AllowHtml]
        [Display(Name = "ПримечаниеUser/Доставка")]
        public string NoteUser { get; set; }
        [AllowHtml]
        [Display(Name = "Примечание/Доставка")]
        public string NoteAdmin { get; set; }
        public OrderVM() { }
        public OrderVM(Order row)
        {
            Id = row.Id;
            UserOrderId = row.UserOrderId;
            CreateAtOrder = row.CreateAtOrder;
            NoteUser = row.NoteUser;
            NoteAdmin = row.NoteAdmin;
        }
    }
}