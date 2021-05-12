using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /*  routes.MapRoute(
                name: "Catalog",
                url: "{Home}/{Сatalog}/{name}",
                defaults: new { controller = "Home", action = "Index", name = UrlParameter.Optional }

              );*/
           /* routes.MapRoute("User", "A/{action}/{name}", new { controller = "Admin", action = "UserDetails", name = UrlParameter.Optional },
               new[] { "MyShop.Controllers" });*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
