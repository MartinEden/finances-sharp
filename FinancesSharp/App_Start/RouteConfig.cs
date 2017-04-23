using System.Web.Mvc;
using System.Web.Routing;

namespace FinancesSharp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cards", "Cards/", new { controller = "Card", action = "Index" });
            routes.MapRoute("Card", "Card/{action}/{cardNumber}", new { controller = "Card", action = "", cardNumber = "" });
            
            routes.MapRoute("Reports", "Reports/All/", new { controller = "Reports", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}