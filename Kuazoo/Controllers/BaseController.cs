using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.kuazoo;

namespace Kuazoo.Controllers
{
    public class BaseController : Controller
    {
        protected override void ExecuteCore()
        {
            //init config settings
            GeneralService.InitConfigSettings();
            base.ExecuteCore();
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model)
            };
            filterContext.ExceptionHandled = true;
            string ip = GetUserIP();
            string url = HttpContext.Request.Url.AbsoluteUri;
            GeneralService.LoggingException(ip, url, model.Exception.ToString(), model.Exception.Message);
            //base.OnException(filterContext);
        }
        protected void CustomException(Exception ex)
        {
            string ip = GetUserIP();
            string url = HttpContext.Request.Url.AbsoluteUri;
            GeneralService.LoggingException(ip, url, ex.ToString(), ex.Message);
        }
        private string GetUserIP()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
