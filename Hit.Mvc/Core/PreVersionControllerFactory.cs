using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hit.Mvc
{
    public class PreVersionControllerFactory : DefaultControllerFactory
    {
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
