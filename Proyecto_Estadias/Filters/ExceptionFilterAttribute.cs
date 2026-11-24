using NLog;
using System;
using System.Web.Mvc;



namespace Proyecto_Estadias.Filters
{
    public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            // Captura la excepcion 
            Exception ex = filterContext.Exception;
            // guarda la excepcion en Nlog 
            Logger.Error(ex,
                "Ocurrio una excepcion en el controlador: " +
                filterContext.RouteData.Values["controller"] +
                "Accion: " + filterContext.RouteData.Values["action"]);

            // configra la respuesta HTTP 
            filterContext.Result = new ViewResult
            {
                ViewName = "Error"  //Vista personalisada del error 
            };
            filterContext.ExceptionHandled = true;
        }
    }
}