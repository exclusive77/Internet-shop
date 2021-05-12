using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название сообщения")]
        public string NameMessage { get; set; }
        [Display(Name = "Сообщение")]
        [Required]
        [AllowHtml]
        public string Message { get; set; }
        public News(News news)
        {
            Id = news.Id;


            NameMessage = news.NameMessage;
            Message = news.Message;




        }
        public News() { }
    }
}