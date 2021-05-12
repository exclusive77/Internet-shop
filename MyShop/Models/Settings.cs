using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class Settings
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Email для отправки сообщений о покупке пользователями")]

        public string EmailMassege { get; set; }
        [Display(Name = "Отправлять сообщения о покупке")]
        public bool AllowMessages { get; set; }
        [Required]
        [Display(Name = "Количество товаров на страницу при просмотре пользователем")]

        public int PageSizeUser { get; set; }
        [Required]
        [Display(Name = "Количество товаров на страницу при просмотре в админ меню")]

        public int PageSizeAdmin { get; set; }
        public Settings() { }
        public Settings(Settings settings)
        {
            Id = settings.Id;


            EmailMassege = settings.EmailMassege;
            AllowMessages = settings.AllowMessages;
            PageSizeUser = settings.PageSizeUser;
            PageSizeAdmin = settings.PageSizeAdmin;


        }
    }
}