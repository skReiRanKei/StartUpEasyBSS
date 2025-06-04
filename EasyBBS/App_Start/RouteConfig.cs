using System.Web.Mvc;
using System.Web.Routing;

namespace EasyBBS
{
    /// <summary>
    /// URL設定
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Base", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
