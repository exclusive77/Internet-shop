using System;
using System.Collections.Generic;
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
namespace MyShop.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название товара")]
        public string Name { get; set; }
        [Display(Name = "Фото товара")]

        public string PathImage { get; set; }
        [Display(Name = "Описание")]
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Цена")]
       
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Количество")]
        [Range(0, 10000, ErrorMessage = "Недопустимое значение")]
        public int Quantity { get; set; }
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Display(Name = "Галерея")]
        public IEnumerable<string> GaleryImages { get; set; }
        public ProductVM() { }

        public ProductVM(Product prod)
        {
            Id = prod.Id;


            Name = prod.Name;
            PathImage = prod.PathImage;


            Description = prod.Description;
            Price = prod.Price;


            Quantity = prod.Quantity;

            CategoryId = prod.CategoryId;
            Category = prod.Category;
       
    }
    }
    
}