using System;
using System.Web.Mvc;

namespace Hit.Mvc
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// 处理用户登录
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext == null) throw new Exception("filterContext");
            filterContext.Controller.ViewBag.__AuthFlag = Config.Cfg.Auth(filterContext);
        }
    }
}
