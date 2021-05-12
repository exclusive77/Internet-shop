using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MyShop.Models;

namespace MyShop.Models
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext ctx)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));

            //Роли приложения
            var role1 = new IdentityRole("admin");
            var role2 = new IdentityRole("user");
            var role3 = new IdentityRole("managerShop");

            //Добавление ролей в БД
            rm.Create(role1);
            rm.Create(role2);
            rm.Create(role3);




            ctx.SaveChanges();
            //Пользователи
            var admin = new ApplicationUser
            {
                Email = "adminShop@gmail.com",
                UserName = "adminShop@gmail.com",

                FirstName = "admin",
                LastName = "admin",
                DateOfBirth = DateTime.Now.Date,

                Phone = "80505667788",
                Address = "Украина",
                Status = 0
            };
            ;
            var managerShop = new ApplicationUser
            {
                Email = "manager@gmail.com",
                UserName = "manager@gmail.com",

                FirstName = "Олег",
                LastName = "Иванов",
                DateOfBirth = DateTime.Now.Date,
                Phone = "80975667433",
                Address = "Украина",
                Status = Status.Manager
            };
            var um = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));
            var result = um.Create(managerShop, "managerok");
            if (result.Succeeded)
            {
                //Добавление ролей к пользователям

                um.AddToRole(managerShop.Id, role2.Name);
                um.AddToRole(managerShop.Id, role3.Name);
            }
            var user1 = new ApplicationUser
            {
                Email = "user@gmail.com",
                UserName = "user@gmail.com",

                FirstName = "Андрей",
                LastName = "Папов",
                DateOfBirth = DateTime.Now.Date,
                Phone = "80975667433",
                Address = "Украина",
                Status = Status.User
            };

            var result2 = um.Create(user1, "user");
            if (result2.Succeeded)
            {
                //Добавление ролей к пользователям


                um.AddToRole(user1.Id, role2.Name);
            }


            var result1 = um.Create(admin, "admin77");
            if (result1.Succeeded)
            {
                //Добавление ролей к пользователям
                um.AddToRole(admin.Id, role1.Name);
                um.AddToRole(admin.Id, role2.Name);
                um.AddToRole(admin.Id, role3.Name);
            }

            ctx.SaveChanges();


            Category g1 = new Category
            {
                Name = "Одежда",

            };
            Category g2 = new Category
            {
                Name = "Бытовая техника",

            };
            Category g3 = new Category
            {
                Name = "Товары для дома",

            };
            Category g4 = new Category
            {
                Name = "Бытовая химия",

            };

            ctx.Categorys.AddRange(new Category[] { g1, g2, g3, g4 });
            ctx.SaveChanges();
            Settings st1 = new Settings
            {
                EmailMassege = "exclusive_77@i.ua",
                AllowMessages = false,
                PageSizeUser=6,
                PageSizeAdmin=3
            };

            ctx.Settings.Add(st1);
            ctx.SaveChanges();
            ;
        }
    }
}