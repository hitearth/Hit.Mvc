using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hit.Mvc
{
    /// <summary>
    /// 可以访问其他版本的控制器的DefaultControllerFactory
    /// </summary>
    public class PreVersionControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// 检索指定名称和请求上下文的控制器类型。
        /// </summary>
        /// <param name="requestContext">HTTP 请求的上下文，其中包括 HTTP 上下文和路由数据。</param>
        /// <param name="controllerName">控制器的名称。</param>
        /// <returns>控制器类型。</returns>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type result = base.GetControllerType(requestContext, controllerName);
            if (result != null) return result;
            object fallbackToNamespaceObj;
            if (requestContext != null && requestContext.RouteData.DataTokens.TryGetValue("PreVersion", out fallbackToNamespaceObj))
            {
                requestContext.RouteData.DataTokens["Namespaces"] = fallbackToNamespaceObj;
                requestContext.RouteData.Values["UsePreVersion"] = true;
                return base.GetControllerType(requestContext, controllerName);
            }
            return null;
        }
    }
}
