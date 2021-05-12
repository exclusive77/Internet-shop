using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using  System.ComponentModel.DataAnnotations;

namespace MyShop.Models {
    public class UserView
    {
        public string Id { get; set; }
        public string Username { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]

        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата рождения")]


        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Display(Name = "Адресс")]
        public string Address { get; set; }

        [Display(Name = "Статус")]


        public Status Status { get; set; }


        public string Roles { get; set; }
    }
}