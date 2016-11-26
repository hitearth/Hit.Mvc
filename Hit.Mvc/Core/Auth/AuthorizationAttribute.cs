using System;
using System.Web.Mvc;

namespace Hit.Mvc
{
    /// <summary>
    /// 用户登录权限验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext == null) throw new Exception("filterContext");
            filterContext.Controller.ViewBag.__AuthFlag = Config.Cfg.Auth(filterContext);
        }
    }
}
