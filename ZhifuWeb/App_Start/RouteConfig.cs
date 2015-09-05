using System.Web.Mvc;
using System.Web.Routing;

namespace ZhifuWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Booking", action = "ChooseTicket", id = UrlParameter.Optional });
        }
    }
}