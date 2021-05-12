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
using static MyShop.Models.ApplicationUser;
using System.Web.Mvc;

namespace MyShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Название категории")]
        [Required]
        public string Name { get; set; }
    }
}