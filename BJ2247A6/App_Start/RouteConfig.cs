using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BJ2247A5
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(name: "AddShow", url: "Actors/{id}/AddShow", defaults: new { controller = "Actors", action = "AddShow" });
            routes.MapRoute(name: "AddEpisode", url: "Shows/{id}/AddEpisode", defaults: new { controller = "Episodes", action = "AddEpisode" });
            routes.MapRoute(name: "EpisodeVideo", url: "Episodes/{id}/Video", defaults: new { controller = "Episodes", action = "GetVideo" });
            routes.MapRoute(name: "AddMediaItem", url: "Actors/{id}/AddMedia", defaults: new { controller = "Actors", action = "AddMedia" });
            routes.MapRoute(name: "DownloadMediaItem", url: "Actors/{id}/Download", defaults: new { controller = "Actors", action = "DownloadMediaItem" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
