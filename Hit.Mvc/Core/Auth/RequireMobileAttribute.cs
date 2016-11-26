using System.Web.Mvc;
using System;

namespace Hit.Mvc
{
    /// <summary>
    /// 标志 移动端访问判断
    /// </summary>
    public class RequireMobileAttribute : FilterAttribute, IAuthorizationFilter
    {
        Func<string, string> GetRedirect;
        /// <summary>
        /// 构造函数
        /// </summary>
        public RequireMobileAttribute()
        {
            GetRedirect = _ => "/m";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="getRedirect">要跳转的移动页面 Func &lt; area, url &gt;</param>
        public RequireMobileAttribute(Func<string, string> getRedirect)
        {
            GetRedirect = getRedirect;
        }

        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(HTMLAttribute), true))
            {
                return;
            }

            if (filterContext.RequestContext.HttpContext.Request.Browser.IsMobileDevice)
            {
                filterContext.Result = new RedirectResult(GetRedirect((string)filterContext.RouteData.DataTokens["area"]));
            }
        }
    }
}
