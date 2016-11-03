using System.Web.Mvc;
using System;

namespace Hit.Mvc
{
    public class RequireMobileAttribute : FilterAttribute, IAuthorizationFilter
    {
        Func<string, string> GetRedirect;
        public RequireMobileAttribute()
        {
            GetRedirect = _ => "/m";
        }
        public RequireMobileAttribute(Func<string, string> getRedirect)
        {
            GetRedirect = getRedirect;
        }
        public void OnAuthorization(AuthorizationContext filterContext)
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
