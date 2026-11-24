using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;


[assembly: OwinStartup(typeof(Proyecto_Estadias.App_Start.Startup))]

namespace Proyecto_Estadias.App_Start
{
   
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(
                new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions()
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Auth/Login"),
                    ExpireTimeSpan = TimeSpan.FromMinutes(30), //Expiracion en 30 minutos
                    SlidingExpiration = true, //renueva la sesion al interactuar 
                    CookieHttpOnly = true, //Previene accesos desde javaScript
                    CookieSecure = CookieSecureOption.SameAsRequest,//HTTPS si la peticion es segura
                    CookieSameSite = Microsoft.Owin.SameSiteMode.Strict

                });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
} 