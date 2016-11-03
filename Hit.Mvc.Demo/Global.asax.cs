using Hit.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace demo
{
    public class CenterAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "aa";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "aa_default",
                "aa/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Hit.Mvc.Demo.Controllers" }

            );
        }
    }

    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "index", action = "Index", id = UrlParameter.Optional }
            );

            ConfigBuild.Config
                .UseAuth(_ => "aaa", null, area =>
                {
                    if (area == null) return ConfigBuild.Config.EmptyFilters; else return ConfigBuild.Config.RequireLoginFilters;
                })
                .UseAutofac(new Dictionary<string, object>() { { "Controllers", typeof(Global).Assembly } })
                .Start();

        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码

        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

    }
}
