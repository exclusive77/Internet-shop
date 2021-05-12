using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class Carousel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название картинки")]
        public string Name { get; set; }
        [Display(Name = "Фото")]

        public string PathImage { get; set; }

        public Carousel(Carousel carousel)
        {
            Id = carousel.Id;


            Name = carousel.Name;
            PathImage = carousel.PathImage;


          

        }
        public Carousel() { }
    }
}