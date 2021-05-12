using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyShop.Models;
using MyShop.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace MyShop.Controllers
{

 
    public class AdminController : Controller
    {
        static string searchNameAdmin = null;
        static Category categorySelectAdmin = new Category();
      
        static Settings SettingsStat = new Settings();
        // public List<Training> mytrainings = new List<Training>(2);
        ApplicationDbContext ctx = new ApplicationDbContext();
      static  List<OrdersAdminVM> ordersAdmin = new List<OrdersAdminVM>();
        static List<OrderAdminDeliveryVM> ordersAdminDelivery = new List<OrderAdminDeliveryVM>();
    
        private readonly object p;
     //Список пользователей и их ролей
        [Authorize(Roles = "managerShop")]
        public ActionResult user_list()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {

            

                var roles = ctx.Roles.ToList();
                var users = ctx.Users.ToList();
              



                var usersWithRoles = (from user in ctx.Users.Include(q => q.Status)

                                      select new
                                      {
                                          Username = user.UserName,
                                          Email = user.Email,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          DateOfBirth = user.DateOfBirth,
                                          Phone = user.Phone,
                                          Addres = user.Address,
                                          Id = user.Id,
                                          Status = user.Status,

                                          //  StatusName=user.status.StatusUser,
                                          RoleNames = (from userRole in user.Roles
                                                       join role in ctx.Roles on userRole.RoleId equals role.Id
                                                       select role.Name).ToList()
                                      }).ToList().Select(p => new UserView()

                                      {
                                          Id = p.Id,
                                          Username = p.Username,
                                          Email = p.Email,

                                          FirstName = p.FirstName,


                                          LastName = p.LastName,
                                          DateOfBirth = p.DateOfBirth,
                                          Phone = p.Phone,
                                          Address = p.Addres,
                                          Status = p.Status,


                                          // StatusName = p.status.StatusUser.ToString(),


                                          Roles = string.Join(",", p.RoleNames)


                                      });
                ;
                return View(usersWithRoles.ToList());

            }


        }
        //Редактирование пользователей
        //  [HttpGet]   
        [Authorize(Roles = "admin")]
        [Authorize]
        public async Task<ActionResult> EditUserAdmin(string id_edit)
        {

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));
                ;
                ApplicationUser userView = await um1.FindByIdAsync(id_edit);
                ;
                if (userView != null)
                {
                    //  Image editImage = ctx.Images.Include(p => p.category).FirstOrDefault(c => c.id == id);




                    SelectList statusUsers1 = new SelectList(Enum.GetNames(typeof(Status)));

                    ViewData["StatusLIST"] = statusUsers1;




                }
                ;
                return View(userView);
            }
        }
        //Редактирование информации о пользователе
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> EditUserAdmin(ApplicationUser userView)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));

                ApplicationUser editUserView = await um1.FindByEmailAsync(userView.Email);
                ApplicationUser adminUser = um1.FindByEmail("adminShop@gmail.com");
                ;
                SelectList statusUsers1 = new SelectList(Enum.GetNames(typeof(Status)));

                ViewData["StatusLIST"] = statusUsers1;
                IdentityResult result;
                if (editUserView != null)
                {
                    editUserView.Email = userView.Email;
                    editUserView.FirstName = userView.FirstName;
                    editUserView.LastName = userView.LastName;
                    editUserView.Phone = userView.Phone;
                    editUserView.Address = userView.Address;
                    editUserView.DateOfBirth = userView.DateOfBirth;
                    ;
                    editUserView.Status = userView.Status;
                    ctx.SaveChanges();
                    if (adminUser.Id != editUserView.Id)
                    {
                        switch (userView.Status)
                        {
                            case Status.Admin:
                                result = um1.AddToRole(editUserView.Id, "admin");
                                break;
                            case Status.Manager:
                                result = um1.RemoveFromRole(editUserView.Id, "admin");
                                result = um1.AddToRole(editUserView.Id, "managerShop");
                                break;
                            case Status.User:
                                result = um1.RemoveFromRole(editUserView.Id, "admin");
                                result = um1.RemoveFromRole(editUserView.Id, "managerShop");
                                result = um1.AddToRole(editUserView.Id, "user");
                                break;

                        }
                    }
                    result = await um1.UpdateAsync(editUserView);
                    ;
                    if (result.Succeeded)
                    {
                        return RedirectToAction("user_list", "Admin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Что-то пошло не так");
                    }

                }
            }
            return View(userView);
        }
        //редактирование ролей пользоватей
        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult edit_role(string id_user)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));

                ApplicationUser editUserRole = um1.FindById(id_user);
                ;
                if (editUserRole != null)
                {


                    IdentityResult result = um1.AddToRole(editUserRole.Id, "managerShop");
                    editUserRole.Status = Status.Manager;
                    ctx.SaveChanges();
                    if (result.Succeeded)
                    {
                        return RedirectToAction("user_list", "Admin");
                    }

                }
                return RedirectToAction("user_list", "Admin");
            }
        }
        //удаление роли менеджера
        [Authorize(Roles = "admin")]
        public ActionResult del_role(string id_user_del)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));

                ApplicationUser editUserRole = um1.FindById(id_user_del);
                ApplicationUser adminUser = um1.FindByEmail("adminShop@gmail.com");
                ;
                if (editUserRole != null && adminUser.Id != editUserRole.Id)
                {


                    IdentityResult result = um1.RemoveFromRole(editUserRole.Id, "managerShop");
                    editUserRole.Status = Status.User;
                    ctx.SaveChanges();
                    if (result.Succeeded)
                    {
                        return RedirectToAction("user_list", "Admin");
                    }

                }
                return RedirectToAction("user_list", "Admin");
            }
        }
        //удаление пользователя
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(string id_user)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));

                ApplicationUser delUser = um1.FindById(id_user);
                ApplicationUser adminUser = um1.FindByEmail("adminShop@gmail.com");
                ;
                if (delUser != null && adminUser.Id != delUser.Id)
                {



                    //удаление менеджера
                   
                    ;
                  


                    IdentityResult resultUser = um1.RemoveFromRole(delUser.Id, "user");

                    IdentityResult resultTR = um1.RemoveFromRole(delUser.Id, "managerShop");
                    um1.Delete(delUser);
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("user_list", "Admin");
        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        //категории

        [HttpGet]
        public ActionResult CreateCategory()
        {


            return View();
        }


        [Authorize]
        [HttpPost]
        public ActionResult CreateCategory(Category сategory)
        {
            if (ModelState.IsValid)
            {
                if (ctx.Categorys.Any(d => d.Name == сategory.Name))
                {
                    ModelState.AddModelError("", "Категория с таким названием уже существует");

                    return View(сategory);
                }
                else
                {
                    ViewBag.Messege = "   ";
                    ctx.Categorys.Add(сategory);
                    ctx.SaveChanges();
                    return RedirectToAction("ListCategorys");
                }

            }

            return View(сategory);


        }


        [Authorize(Roles = "managerShop")]

        public ActionResult ListCategorys()
        {
            var listCategorys = ctx.Categorys.ToList();



            return View(listCategorys);



        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpGet]
        public ActionResult EditCategory(int? idEditCategory)
        {
            if (idEditCategory is null) return HttpNotFound();

            {


                Category editCategory = ctx.Categorys.FirstOrDefault(c => c.Id == idEditCategory);
                if (editCategory is null) return HttpNotFound();



                return View(editCategory);
            }
        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult EditCategory(Category editCategory)
        {
            ;
            if (ModelState.IsValid)
            {
                if (ctx.Categorys.Where(x => x.Id != editCategory.Id).Any(d => d.Name == editCategory.Name))
                {
                    ModelState.AddModelError("", "Категория с таким названием уже существует");

                    return View(editCategory);
                }
                ;
                Category editCat = ctx.Categorys.FirstOrDefault(v => v.Name == editCategory.Name);
                if (editCat == null)
                {
                    ViewBag.Messege = "   ";
                    ctx.Entry(editCategory).State = EntityState.Modified;
                    ctx.SaveChanges();
                    return RedirectToAction("ListCategorys");
                }


            }




            return View(editCategory);

        }
        [Authorize(Roles = "managerShop")]
        public ActionResult DeleteCategory(int? id)
        {
            if (id is null) return HttpNotFound();

            {

                Category CategoryDel = ctx.Categorys
                    .Where(c => c.Id == id)
                    .FirstOrDefault();

                ctx.Categorys.Remove(CategoryDel);
                ctx.SaveChanges();


                return RedirectToAction("ListCategorys");


            }
        }
        //Продукты
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpGet]
        public ActionResult CreateProduct()
        {
            SelectList SelectCategory = new SelectList(ctx.Categorys, "id", "Name");

            ViewBag.CategorysShow = SelectCategory;
            ;
            return View();
        }

        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult CreateProduct(Product product, HttpPostedFileBase uploadImage)
        {
            ;
            SelectList SelectCategory = new SelectList(ctx.Categorys, "id", "Name");

            ViewBag.CategorysShow = SelectCategory;
            var path = Server.MapPath("~/Files");
            if (ModelState.IsValid)
            {
                Product productNew = ctx.Products.FirstOrDefault(v => v.Name == product.Name);
                if (productNew == null)
                {
                    TempData["MessegeProduct"] = null;
                    if (uploadImage != null)
                    {

                        uploadImage.SaveAs(Path.Combine(path, uploadImage.FileName));

                        string FileName = uploadImage.FileName;

                        ;

                        product.PathImage = FileName;
                    }
                    else { product.PathImage = null; }

                    ctx.Products.Add(product);
                    ctx.SaveChanges();
                    return RedirectToAction("ListProducts");
                }
                else
                {

                    ModelState.AddModelError("", "Товар с таким названием уже существует");
                    TempData["MessegeProduct"] = "Товар с таким названием уже существует";
                    ;
                    return View(product);
                }
            }
            return View(product);


        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpGet]
        public ActionResult CreateProductVM()
        {
            ProductVM model = new ProductVM();
            SelectList SelectCategory = new SelectList(ctx.Categorys, "id", "Name");

            ViewBag.CategorysShow = SelectCategory;
            ;
            return View();
        }

        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult CreateProductVM(ProductVM product, HttpPostedFileBase uploadImage)
        {

            SelectList SelectCategory = new SelectList(ctx.Categorys, "id", "Name");

            ViewBag.CategorysShow = SelectCategory;
            Product productDB = new Product();
            if (ModelState.IsValid)
            {
                Product productNew = ctx.Products.FirstOrDefault(v => v.Name == product.Name);
                if (productNew == null)
                {

                    int id_product;

                    productDB.Name = product.Name;
                    productDB.Description = product.Description;
                    productDB.Price = product.Price;
                    productDB.Quantity = product.Quantity;
                    productDB.CategoryId = product.CategoryId;
                    Category cat = ctx.Categorys.FirstOrDefault(s => s.Id == product.CategoryId);
                    productDB.Category = cat;
                    TempData["MessegeProduct"] = null;
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        string FileName = uploadImage.FileName;
                        productDB.PathImage = FileName;
                    }
                    else
                    {
                        productDB.PathImage = null;
                    }
                    ctx.Products.Add(productDB);
                    ctx.SaveChanges();
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        //  var productImg = ctx.Products.FirstOrDefault(r => r.Name == product.Name);
                        //   id_product = productImg.Id;
                        id_product = productDB.Id;
                        var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                        var path1 = Path.Combine(imageDirectory.ToString(), "Products");
                        var path2 = Path.Combine(imageDirectory.ToString(), "Products\\" + id_product.ToString());
                        var path3 = Path.Combine(imageDirectory.ToString(), "Products\\" + id_product.ToString() + "\\Gallery\\");
                        //  var path1 = Server.MapPath("~/Files");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);

                        }
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);

                        }
                        if (!Directory.Exists(path3))
                        {
                            Directory.CreateDirectory(path3);

                        }
                        string exten = uploadImage.ContentType.ToLower();
                        if (exten != "image/jpg" &&
                            exten != "image/pjepg" &&
                           exten != "image/jepg" &&
                           exten != "image/gif" &&
                            exten != "image/png" &&
                            exten != "image/x-png" &&
                            exten != "image/jpeg" &&
                           exten != "image/bmp" &&
                           exten != "image/tiff")
                        {
                            ;
                            ModelState.AddModelError("", "Фото товара не загружено-- Неверное расширение файла");
                            return View(product);
                        }
                        uploadImage.SaveAs(Path.Combine(path2, uploadImage.FileName));



                        ;


                    }



                    return RedirectToAction("ListProducts");
                }
                else
                {

                    ModelState.AddModelError("", "Товар с таким названием уже существует");
                    TempData["MessegeProduct"] = "Товар с таким названием уже существует";
                    ;
                    return View(product);
                }
            }
            return View(product);


        }
       
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpGet]
        public async Task<ActionResult> ListProducts(int? page, int? catId)
        {
           SettingsStat = ctx.Settings.FirstOrDefault(c => c.Id == 1);

            List<Product> productsList = new List<Product>();
            ViewBag.SelectlistCategory = new SelectList(ctx.Categorys.ToList(), "Id", "Name");
            ViewBag.SelectedCat = catId.ToString();
            categorySelectAdmin = ctx.Categorys.FirstOrDefault(q => q.Id == catId);
            ;
            if (categorySelectAdmin != null)
            {
                if (catId != null)
                {

                    searchNameAdmin = null;
                }

           ;
                if (searchNameAdmin != null)
                {
                    if (Regex.IsMatch(searchNameAdmin, @"^\d+$"))
                    {
                        int searchNameOrderINT = Convert.ToInt32(searchNameAdmin);
                        productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Id == searchNameOrderINT).ToListAsync();
                        ;

                    }
                    else
                    {
                        productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Name.Contains(searchNameAdmin)).ToListAsync();

                    }


                }
                else
                {

                    productsList = await ctx.Products.Include(p => p.Category).Where(v => catId == null || catId == 0 || v.CategoryId == catId).ToListAsync();

                }
            }

            else
            {
                productsList = await ctx.Products.Include(p => p.Category).ToListAsync();
            }
          ;
            //   return View(productsList);
            int pageSize = SettingsStat.PageSizeAdmin;
            int pageNumber = (page ?? 1);
            ;
            return View(productsList.ToPagedList(pageNumber, pageSize));

        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpPost]
        public async Task<ActionResult> ListProducts(string searchName, int? catId, int? page)
        {
            ViewBag.SelectlistCategory = new SelectList(ctx.Categorys.ToList(), "Id", "Name");
            ViewBag.SelectedCat = catId.ToString();
            int pageSize;
            int pageNumber;
            List<Product> productsList = new List<Product>();

            if (catId != null)
            {
                categorySelectAdmin = ctx.Categorys.FirstOrDefault(q => q.Id == catId);
            }
            ;
            if (searchName != null)
            {
                searchNameAdmin = searchName;
                if (Regex.IsMatch(searchNameAdmin, @"^\d+$"))
                {
                    int searchNameOrderINT = Convert.ToInt32(searchNameAdmin);
                    productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Id == searchNameOrderINT).ToListAsync();
                    ;

                }
                else
                {
                    productsList = await ctx.Products.Include(p => p.Category).Where(p => p.Name.Contains(searchNameAdmin)).ToListAsync();

                }

               pageSize = SettingsStat.PageSizeAdmin;
                pageNumber = (page ?? 1);
                ;
                return View(productsList.ToPagedList(pageNumber, pageSize));
            }
            pageSize = SettingsStat.PageSizeAdmin;
            pageNumber = (page ?? 1);
            ;
            return View(productsList.ToPagedList(pageNumber, pageSize));
        }
        [Authorize(Roles = "managerShop")]
        [HttpGet]
        public ActionResult EditProduct(int? id)
        {
            if (id is null) return HttpNotFound();

            {


                Product editProduct = ctx.Products.Include(p => p.Category).FirstOrDefault(c => c.Id == id);
                SelectList SelectCategory1 = new SelectList(ctx.Categorys, "id", "Name");
                ViewBag.CategorysShow1 = SelectCategory1;

                // if (editProduct is null) return HttpNotFound();

                if (editProduct is null) return Content("Данный продукт не доступен");
                ProductVM model = new ProductVM(editProduct);
                model.GaleryImages = Directory.EnumerateFiles(Server.MapPath("~/Files/Products/" + editProduct.Id + "/Gallery/")).
                    Select(filename => Path.GetFileName(filename));
                return View(model);
            }
        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult EditProduct(ProductVM editProduct, HttpPostedFileBase uploadImage)
        {
            SelectList SelectCategory = new SelectList(ctx.Categorys, "id", "Name");
            editProduct.GaleryImages = Directory.EnumerateFiles(Server.MapPath("~/Files/Products/" + editProduct.Id + "/Gallery/")).
                    Select(filename => Path.GetFileName(filename));
            ViewBag.CategorysShow = SelectCategory;
            if (ModelState.IsValid)
            {

                if (ctx.Products.Include(s => s.Category).Where(x => x.Id != editProduct.Id).Any(d => d.Name == editProduct.Name))
                {
                    ModelState.AddModelError("", "Товар с таким названием уже существует");
                    TempData["MessegeProduct"] = "Товар с таким названием уже существует";
                    ;
                    return View(editProduct);
                }
                else
                {
                    Product EditProduct = ctx.Products.First(c => c.Id == editProduct.Id);


                    EditProduct.Name = editProduct.Name;

                    EditProduct.Description = editProduct.Description;
                    EditProduct.Price = editProduct.Price;
                    EditProduct.Quantity = editProduct.Quantity;
                    EditProduct.CategoryId = editProduct.CategoryId;
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        string FileName = uploadImage.FileName;
                        EditProduct.PathImage = FileName;
                    }
                    else
                    {
                        EditProduct.PathImage = editProduct.PathImage;
                    }
                    // ctx.Entry(editTrainingName).State = EntityState.Modified
                    ctx.SaveChanges();
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        string exten = uploadImage.ContentType.ToLower();
                        if (exten != "image/jpg" &&
                            exten != "image/pjepg" &&
                           exten != "image/jepg" &&
                           exten != "image/gif" &&
                            exten != "image/png" &&
                            exten != "image/x-png" &&
                            exten != "image/jpeg" &&
                           exten != "image/bmp" &&
                           exten != "image/tiff")
                        {
                            ;
                            ModelState.AddModelError("", "Фото товара не загружено-- Неверное расширение файла");
                            return View(editProduct);
                        }
                        //  var productImg = ctx.Products.FirstOrDefault(r => r.Name == product.Name);
                        //   id_product = productImg.Id;
                        int id_product = editProduct.Id;
                        ;
                        var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                        var path1 = Path.Combine(imageDirectory.ToString(), "Products");
                        var path2 = Path.Combine(imageDirectory.ToString(), "Products\\" + id_product.ToString());
                        var path3 = Path.Combine(imageDirectory.ToString(), "Products\\" + id_product.ToString() + "\\Gallery\\");
                        DirectoryInfo directory1 = new DirectoryInfo(path2);
                        DirectoryInfo directoryGalery = new DirectoryInfo(path3);
                        foreach (var fileDel in directory1.GetFiles())
                        {
                            fileDel.Delete();

                        }
                        foreach (var fileDel2 in directoryGalery.GetFiles())
                        {
                            fileDel2.Delete();

                        }
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);

                        }
                        if (!Directory.Exists(path3))
                        {
                            Directory.CreateDirectory(path3);

                        }

                        uploadImage.SaveAs(Path.Combine(path2, uploadImage.FileName));



                        ;


                    }
                   

                    return RedirectToAction("ListProducts");
                }
            }
            return View(editProduct);
        }
        [Authorize(Roles = "managerShop")]
        public void SaveGalery(int? id)
        {
            foreach (string filename in Request.Files)
            {
                ;
                HttpPostedFileBase file = Request.Files[filename];
                if (file != null && file.ContentLength > 0)
                {
                    var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                    var path1 = Path.Combine(imageDirectory.ToString(), "Products");
                    var path2 = Path.Combine(imageDirectory.ToString(), "Products\\" + id.ToString());
                    var path3 = Path.Combine(imageDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\");
                    var pathGaleryFile = string.Format($"{path3}\\{ file.FileName}");

                    file.SaveAs(pathGaleryFile);
                }

            }
        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [Authorize]
        public void DeleteItemGalery(int? id, string imageName)
        {
            var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
            var pathImageG = Path.Combine(imageDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\" + imageName);

            var pathImageGg = Request.MapPath("~Files/Products/" + id.ToString() + "/Gallery/" + imageName);
            ;
            if (System.IO.File.Exists(pathImageG))
            {
                System.IO.File.Delete(pathImageG);
                ;
            }

        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpGet]
        public ActionResult DetailsProduct(int? id)
        {
            if (id is null) return HttpNotFound();

            {


                Product detailsProduct = ctx.Products.Include(p => p.Category).FirstOrDefault(c => c.Id == id);
                SelectList SelectCategory1 = new SelectList(ctx.Categorys, "id", "Name");
                ViewBag.CategorysShow1 = SelectCategory1;

                if (detailsProduct is null) return HttpNotFound();



                return View(detailsProduct);
            }
        }
        [Authorize(Roles = "managerShop")]
        [HttpPost]
        public ActionResult DetailsProduct(Product detailsProduct)
        {


            //  ctx.Entry(editTrainingName).State = EntityState.Modified;
            // ctx.SaveChanges();

            return View();

        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id is null) return HttpNotFound();

            {

                Product ProductDel = ctx.Products
                    .Where(c => c.Id == id)
                    .FirstOrDefault();
                int id_product = ProductDel.Id;
                ctx.Products.Remove(ProductDel);
                ctx.SaveChanges();
                var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                var path1 = Path.Combine(imageDirectory.ToString(), "Products");
                var path2 = Path.Combine(imageDirectory.ToString(), "Products\\" + id_product.ToString());
                if (Directory.Exists(path2))
                {
                    Directory.Delete(path2, true);
                }
                return RedirectToAction("ListProducts");


            }
        }
   //Список Заказов
 [HttpGet]
     
        [Authorize(Roles = "managerShop")]
        public ActionResult OrderAdminDelivery()
        {
            ordersAdminDelivery.Clear();

            List<OrderVM> orderVM = ctx.Orders.ToArray().Select(x => new OrderVM(x)).OrderByDescending(v=>v.CreateAtOrder).ToList();
            foreach (var orderItem in orderVM)
            {
                Dictionary<string, int> productQty = new Dictionary<string, int>();
                decimal total = 0m;
                List<OrderDetails> orderDetailsList = ctx.OrderDetails.Where(s => s.OrderId == orderItem.Id).ToList();
                ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));
                ApplicationUser userOrder = um1.FindById(orderItem.UserOrderId);

                foreach (var orderDetailsItem in orderDetailsList)
                {
                    Product product = ctx.Products.FirstOrDefault(z => z.Id == orderDetailsItem.ProductId);
                    productQty.Add(product.Name, orderDetailsItem.Quantity);
                    //получаем общюю стоимость товаров
                    total += orderDetailsItem.Quantity * product.Price;

                }
                Delivery deliveryUser = ctx.Deliverys.FirstOrDefault(q => q.OrderId == orderItem.Id);
                ;
                ordersAdminDelivery.Add(new OrderAdminDeliveryVM
                {
                    OrderId = orderItem.Id,
                    Total = total,
                    UserName = userOrder.UserName,
                    ProductQTY = productQty,
                    CreateAtOrder = orderItem.CreateAtOrder,
                    City = deliveryUser.City,
                    FirstName=deliveryUser.FirstName,
                    LastName=deliveryUser.LastName,
                    Email=deliveryUser.Email,
                    Phone=deliveryUser.Phone,
                    DeliveryCompany=deliveryUser.DeliveryCompany,
                    BranchNumber=deliveryUser.BranchNumber,
                    DeliveryInvoiceNumber=deliveryUser.DeliveryInvoiceNumber
                }
                    );
            }

            return View(ordersAdminDelivery);

        }
        [HttpPost]
        [Authorize(Roles = "managerShop")]
        public ActionResult OrderAdminDelivery(string searchNameOrder)
        {

            ;
            List<OrderAdminDeliveryVM> ordersAdminSearch = new List<OrderAdminDeliveryVM>();

            if (searchNameOrder != null && searchNameOrder != " ")
            {
                if (Regex.IsMatch(searchNameOrder, @"^\d+$"))
                {
                    int searchNameOrderINT = Convert.ToInt32(searchNameOrder);
                    ordersAdminSearch = ordersAdminDelivery.ToArray().Where(p => p.OrderId == searchNameOrderINT).ToList();
                    ;
                    return View(ordersAdminSearch);
                }
                else
                {

                    ordersAdminSearch = ordersAdminDelivery.ToArray().Where(p => p.UserName.Contains(searchNameOrder)||p.LastName.Contains(searchNameOrder)
                    ||p.City.Contains(searchNameOrder)).ToList();
                    ;
                    return View(ordersAdminSearch);
                }

            }
            else
            {
                return View(ordersAdminDelivery);
            }
    ;
        }
        [Authorize(Roles = "admin")]
        public ActionResult DeleteOrder(int? id)
        {
            if (id is null) return HttpNotFound();
            ;
            {

                Delivery DeliveryDel = ctx.Deliverys.Where(c => c.OrderId == id)
                    .FirstOrDefault();
                if (DeliveryDel != null) { 
                ctx.Deliverys.Remove(DeliveryDel);
                ctx.SaveChanges();
            }
                OrderDetails OrderDetailsDel = ctx.OrderDetails.Where(c => c.OrderId == id)
                    .FirstOrDefault();
                if (OrderDetailsDel != null)
                {
                    ctx.OrderDetails.Remove(OrderDetailsDel);
                    ctx.SaveChanges();
                }
                Order OrderDel = ctx.Orders.Where(c => c.Id == id).FirstOrDefault();
                if (OrderDetailsDel != null)
                {
                    ctx.Orders.Remove(OrderDel);
                    ctx.SaveChanges();
                }
                ;
                    return RedirectToAction("OrderAdminDelivery");


            }
        }
        //информация о доставке
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpGet]
        public ActionResult DeliverysInfoAdmin(int? Id)

        {
            Delivery model = new Delivery();
            if (Id != null)
            {
                model = ctx.Deliverys.FirstOrDefault(p => p.OrderId == Id);
                ;
            }
            return View(model);
        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpPost]
        public ActionResult DeliverysInfoAdmin(Delivery delivery)

        {
            Delivery deliveryEdit = new Delivery();
            ;
            if (delivery != null)
            {
                deliveryEdit = ctx.Deliverys.FirstOrDefault(d => d.Id == delivery.Id);
                if (deliveryEdit != null) { 
                deliveryEdit.FirstName = delivery.FirstName;
                deliveryEdit.LastName = delivery.LastName;
                deliveryEdit.Phone = delivery.Phone;
                deliveryEdit.Email = delivery.Email;
                deliveryEdit.City = delivery.City;
                deliveryEdit.DeliveryInvoiceNumber = delivery.DeliveryInvoiceNumber;
                deliveryEdit.DeliveryCompany = delivery.DeliveryCompany;
                deliveryEdit.BranchNumber = delivery.BranchNumber;
                deliveryEdit.OrderId = delivery.OrderId;


                ctx.SaveChanges();
                return RedirectToAction("OrderAdminDelivery", "Admin");
            }
           
            }
            return View(delivery);
        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        public ActionResult DeliveryL()

        {
        

            List<Delivery> model = ctx.Deliverys.ToList();
        
            return View(model);
        }
        //примечание к доставке админа
        [Authorize]
        [Authorize(Roles = "managerShop")]
        public ActionResult PlaceOrderAddNoteUser(int? Id)

        {
            Order model = new Order();
            if (Id != null)
            {
                model = ctx.Orders.FirstOrDefault(p => p.Id == Id);
            }
            return View(model);
        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        [HttpPost]
        public ActionResult PlaceOrderAddNoteUser(Order order)

        {
            Order Editmodel = new Order();
            ;
            if (order != null)
            {

                Editmodel = ctx.Orders.FirstOrDefault(p => p.Id == order.Id);
                ; Editmodel.NoteAdmin = order.NoteAdmin;
                Editmodel.NoteUser = order.NoteUser;
                ctx.SaveChanges();
            }
            return RedirectToAction("OrderAdminDelivery", "Admin");
        }


       //Реклама
        [Authorize]
        [Authorize(Roles = "managerShop")]
        public ActionResult CreateCarousel()
        {
            return View();

        }

        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult CreateCarousel(Carousel carousel, HttpPostedFileBase uploadImage)
        {




            Carousel carouselDB = new Carousel();
            if (ModelState.IsValid)
            {
                Carousel carouselNew = ctx.Carousels.FirstOrDefault(v => v.Name == carousel.Name);
                if (carouselNew == null)
                {

                    int id_carousel;

                    carouselDB.Name = carousel.Name;

                    TempData["MessegeProduct"] = null;
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        string FileName = uploadImage.FileName;
                        carouselDB.PathImage = FileName;
                    }
                  
                    ctx.Carousels.Add(carouselDB);
                    ctx.SaveChanges();
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        //  var productImg = ctx.Products.FirstOrDefault(r => r.Name == product.Name);
                        //   id_product = productImg.Id;
                        id_carousel = carouselDB.Id;
                        var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                        var path1 = Path.Combine(imageDirectory.ToString(), "Carousels");
                        var path2 = Path.Combine(imageDirectory.ToString(), "Carousels\\" + id_carousel.ToString());

                        //  var path1 = Server.MapPath("~/Files");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);

                        }
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);

                        }

                        string exten = uploadImage.ContentType.ToLower();
                        if (exten != "image/jpg" &&
                            exten != "image/pjepg" &&
                           exten != "image/jepg" &&
                           exten != "image/gif" &&
                            exten != "image/png" &&
                            exten != "image/x-png" &&
                            exten != "image/jpeg" &&
                           exten != "image/bmp" &&
                           exten != "image/tiff")
                        {
                            ;
                            ModelState.AddModelError("", "Фото товара не загружено-- Неверное расширение файла");
                            return View(carousel);
                        }
                        uploadImage.SaveAs(Path.Combine(path2, uploadImage.FileName));



                        ;


                    }



                    return RedirectToAction("ListCarousels");
                }
                else
                {

                    ModelState.AddModelError("", "Фото с таким названием уже существует");
                    TempData["MessegeProduct"] = "Фото с таким названием уже существует";
                    ;
                    return View(carousel);
                }
            }
            return View(carousel);

        }
        [Authorize(Roles = "managerShop")]
        public ActionResult ListCarousels()
        {
            var listCarousels = ctx.Carousels.ToList();



            return View(listCarousels);



        }
        [Authorize]
        [Authorize(Roles = "managerShop")]
        public ActionResult DeleteCarousels(int? id)
        {
            if (id is null) return HttpNotFound();

            {

                Carousel CarouselDel = ctx.Carousels
                    .Where(c => c.Id == id)
                    .FirstOrDefault();
                int id_сarousels = CarouselDel.Id;
                ctx.Carousels.Remove(CarouselDel);
                ctx.SaveChanges();
                var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                var path1 = Path.Combine(imageDirectory.ToString(), "Carousels");
                var path2 = Path.Combine(imageDirectory.ToString(), "Carousels\\" + id_сarousels.ToString());
                if (Directory.Exists(path2))
                {
                    Directory.Delete(path2, true);
                }
                return RedirectToAction("ListCarousels");


            }
        }
        [HttpGet]
        [Authorize(Roles = "managerShop")]
        public ActionResult EditCarousel(int? id)
        {
            if (id is null) return HttpNotFound();

            {


                Carousel editCarousel = ctx.Carousels.FirstOrDefault(c => c.Id == id);




                if (editCarousel is null) return Content("Данное фото не доступно");
                Carousel model = new Carousel(editCarousel);

                return View(model);
            }
        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult EditCarousel(Carousel editCarousel, HttpPostedFileBase uploadImage)
        {

            if (ctx.Carousels.Where(x => x.Id != editCarousel.Id).Any(d => d.Name == editCarousel.Name))
            {
                ModelState.AddModelError("", "Фото с таким названием уже существует");

                return View(editCarousel);
            }
        
         
                Carousel EditCarousel = ctx.Carousels.First(c => c.Id == editCarousel.Id);
                if (EditCarousel.Name != null)
                {

                    int id_carousel;

                    EditCarousel.Name = editCarousel.Name;

                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        string FileName = uploadImage.FileName;
                        EditCarousel.PathImage = FileName;
                    }
                    else
                    {
                        EditCarousel.PathImage = editCarousel.PathImage;
                    ctx.SaveChanges();
                    return RedirectToAction("ListCarousels");
                }
                    ;

                
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        //  var productImg = ctx.Products.FirstOrDefault(r => r.Name == product.Name);
                        //   id_product = productImg.Id;
                        id_carousel = EditCarousel.Id;
                        var imageDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Files"));
                        var path1 = Path.Combine(imageDirectory.ToString(), "Carousels");
                        var path2 = Path.Combine(imageDirectory.ToString(), "Carousels\\" + id_carousel.ToString());



                        string exten = uploadImage.ContentType.ToLower();
                        if (exten != "image/jpg" &&
                            exten != "image/pjepg" &&
                           exten != "image/jepg" &&
                           exten != "image/gif" &&
                            exten != "image/png" &&
                            exten != "image/x-png" &&
                            exten != "image/jpeg" &&
                           exten != "image/bmp" &&
                           exten != "image/tiff")
                        {
                            ;
                            ModelState.AddModelError("", "Фото товара не загружено-- Неверное расширение файла");
                            return View(editCarousel);
                        }
                        DirectoryInfo directory1 = new DirectoryInfo(path2);
                        foreach (var fileDel in directory1.GetFiles())
                        {
                            fileDel.Delete();

                        }

                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);

                        }


                        uploadImage.SaveAs(Path.Combine(path2, uploadImage.FileName));



                        ;





                        ;





                        return RedirectToAction("ListCarousels");

                    }
              
            }
            return View(editCarousel);
        }
        //Новости
        [Authorize(Roles = "managerShop")]
        public ActionResult CreateNews()
        {
            return View();

        }

        [Authorize(Roles = "managerShop")]
        [Authorize]
        [HttpPost]
        public ActionResult CreateNews(News news)
        {


            ;

            News newsDB = new News();
            if (ModelState.IsValid)
            {
                News carouselNew = ctx.News.FirstOrDefault(v => v.NameMessage == news.NameMessage);
                if (carouselNew == null)
                {


                    newsDB.NameMessage = news.NameMessage;
                    newsDB.Message = news.Message;


                    ctx.News.Add(newsDB);
                    ctx.SaveChanges();




                    return RedirectToAction("ListNews");
                }
                else
                {

                    ModelState.AddModelError("", "Сообщение с таким названием уже существует");

                    return View(news);
                }


            }
            return View(news);
        }
        [Authorize(Roles = "managerShop")]
        public ActionResult ListNews()
        {
            var listNews = ctx.News.ToList();



            return View(listNews);



        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        public ActionResult DeleteNews(int? id)
        {
            if (id is null) return HttpNotFound();

            {

                News newsDel = ctx.News
                    .Where(c => c.Id == id)
                    .FirstOrDefault();

                ctx.News.Remove(newsDel);
                ctx.SaveChanges();


            }
            return RedirectToAction("ListNews");
        }
        [Authorize(Roles = "managerShop")]
        [Authorize]
        public ActionResult EditNews(int? id)
        {
            if (id is null) return HttpNotFound();

            {


                News editNews = ctx.News.FirstOrDefault(c => c.Id == id);




                if (editNews is null) return Content("Данный сообщение не доступно");
                News model = new News(editNews);

                return View(model);
            }
        }
        [Authorize]
        [HttpPost]
        [Authorize(Roles = "managerShop")]
        public ActionResult EditNews(News editNews)
        {

            if (ctx.News.Where(x => x.Id != editNews.Id).Any(d => d.NameMessage == editNews.NameMessage))
            {
                ModelState.AddModelError("", "Сообщение таким названием уже существует");

                return View(editNews);
            }
            if (ModelState.IsValid)
            {
                News EditNews = ctx.News.First(c => c.Id == editNews.Id);
                if (EditNews != null)
                {



                    EditNews.NameMessage = editNews.NameMessage;
                    EditNews.Message = editNews.Message;



                    ctx.SaveChanges();
                    return RedirectToAction("ListNews");

                }

            }
            return View(editNews);
        }
        //информация о пользователе
        public ActionResult UserDetails(string name)
        {
            if (name is null) return HttpNotFound();
            ;
            ApplicationUserManager um1 = new ApplicationUserManager(new UserStore<ApplicationUser>(ctx));
            ApplicationUser userDetails = um1.FindByName(name);
                if (userDetails is null) return Content("Данный сообщение не доступно");
                return View(userDetails);
        }
        //Настройки
        [Authorize(Roles = "admin")]
        [Authorize]
        public ActionResult SettingsEdit()
        {
           


                Settings settings = ctx.Settings.FirstOrDefault(c => c.Id == 1);




                if (settings is null) return Content("Данный сообщение не доступно");
                Settings model = new Settings(settings);

                return View(model);
          
        }
[Authorize(Roles = "admin")]
        [Authorize]
        [HttpPost]
        public ActionResult SettingsEdit(Settings settingsEdit)
        {
        
            if (ModelState.IsValid)
            {
            Settings Settings = ctx.Settings.FirstOrDefault(c => c.Id == 1);
            if (Settings != null)
                {



                    Settings.EmailMassege = settingsEdit.EmailMassege;
                    Settings.AllowMessages = settingsEdit.AllowMessages;

                    Settings.PageSizeUser = settingsEdit.PageSizeUser;
                    Settings.PageSizeAdmin = settingsEdit.PageSizeAdmin;

                    ctx.SaveChanges();
                    return RedirectToAction("Index", "Home");

                }

            }
            return View(settingsEdit);
        }
        
    }
}