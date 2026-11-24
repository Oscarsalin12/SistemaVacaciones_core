using Proyecto_Estadias.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Proyecto_Estadias
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var owinStartup = typeof(Proyecto_Estadias.App_Start.Startup);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            // cponfiguracion de el automar para que se ejecute con la aplicacion 
            WebProfile.Run();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier =
                System.Security.Claims.ClaimTypes.NameIdentifier;
        }
    }
}
