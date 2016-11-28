using System.Web.Mvc;

namespace Hit.Mvc
{
    /// <summary>
    /// 允许跨域
    /// </summary>
    public class AllowOriginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 初始化 Hit.Mvc.AllowOriginAttribute 类的新实例。
        /// </summary>
        public AllowOriginAttribute()
        {
            Origin = "*";
        }
        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", Origin);
        }
    }
}