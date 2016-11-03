using System.Web.Mvc;

namespace Hit.Mvc
{
    public class AllowOriginAttribute : ActionFilterAttribute
    {
        public string Origin { get; set; }
        public AllowOriginAttribute()
        {
            Origin = "*";
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", Origin);
        }
    }
}