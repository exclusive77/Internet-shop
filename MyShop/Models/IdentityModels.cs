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
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Дата рождения")]
 
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Телефон")]
        [RegularExpression("^[+]?\\d*\\(?\\d{3}\\)?\\d{3}-?\\d{2}-?\\d{2}$", ErrorMessage = "Проверьте правильность ввода телефона")]
        public string Phone { get; set; }
        [Display(Name = "Адрес(Город)")]
       
        public string Address { get; set; }

        [Display(Name = "Cтатус")]
        public Status Status { get; set; } = Status.User;
    
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
       
    
     
     

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> Categorys { get; set; }
       
        public DbSet<Product> Products { get; set; }
     public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Delivery> Deliverys { get; set; }
        
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
