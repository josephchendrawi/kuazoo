using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using com.kuazoo;
using System.Web.Caching;

namespace Kuazoo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static CacheItemRemovedCallback OnCacheRemove = null;
        private static DateTime lastDailyTask;
        private const string dailyTask = "DailyTask";

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
        //    {
        //        UriBuilder builder = new UriBuilder(Request.Url);
        //        builder.Host = "www." + Request.Url.Host;
        //        Response.StatusCode = 301;
        //        Response.AddHeader("Location", builder.ToString());
        //        Response.End();
        //    }
        //}
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            AddTask(dailyTask, 60);
        }

        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }
        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            switch (k)
            {
                case dailyTask:
                    DoDailyTask();
                    break;
            }
            AddTask(k, Convert.ToInt32(v));
        }


        private void DoDailyTask()
        {
            DateTime now = DateTime.UtcNow;
            if (now > lastDailyTask)
            {
                ScheduledTasks.CheckFlashDealNotifEmail();
                lastDailyTask = now.AddHours(1);
            }
        }
    }
}