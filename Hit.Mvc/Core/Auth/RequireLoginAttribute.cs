using System;
using System.Web.Mvc;

namespace Hit.Mvc
{
    /// <summary>
    /// 标志 需要登陆
    /// </summary>
    public class RequireLoginAttribute : FilterAttribute, IAuthorizationFilter
    {
        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            var authFlag = filterContext.Controller.ViewBag.__AuthFlag == null ? false : (bool)filterContext.Controller.ViewBag.__AuthFlag;
            if (authFlag)
                return;

            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }
            if (filterContext.ActionDescriptor.IsDefined(typeof(HTMLAttribute), true))
            {
                string url = Config.Cfg.GetLoginPage((string)filterContext.RouteData.DataTokens["area"])
                    + "?redirectURL=" + filterContext.HttpContext.Request.Url;
                filterContext.Result = new RedirectResult(url);
            }
            else
                filterContext.Result = new JsonNetResult() { Data = new JSON_Data { success = false, msg = "未登陆", code = 0 } };
        }
    }
}
