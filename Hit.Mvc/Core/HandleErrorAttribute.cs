using log4net;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;


namespace Hit.Mvc
{
    /// <summary>
    /// 错误处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class HandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }

            if (filterContext.ExceptionHandled)
            {
                return;
            }

            Exception exception = filterContext.Exception;

            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                return;
            }

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            log4net.ILog log = LogManager.GetLogger("FilterError");
            var request = filterContext.RequestContext.HttpContext.Request;
            Newtonsoft.Json.Linq.JObject jt = new Newtonsoft.Json.Linq.JObject();
            jt.Add("controllerName", controllerName);
            jt.Add("actionName", actionName);
            jt.Add("url", request.Url.OriginalString);
            if (request.UrlReferrer != null) jt.Add("UrlReferrer", request.UrlReferrer.OriginalString);

            var sv = HttpContext.Current.Request.ServerVariables;
            if (sv["HTTP_X_FORWARDED_FOR"] != null) jt.Add("HTTP_X_FORWARDED_FOR", sv["HTTP_X_FORWARDED_FOR"]);
            if (sv["REMOTE_ADDR"] != null) jt.Add("REMOTE_ADDR", sv["REMOTE_ADDR"]);

            string bodyText;

            request.InputStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(request.InputStream);
            bodyText = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(bodyText))
                jt.Add("body", bodyText);

            int statusCode = 500;
            if (exception is System.ArgumentException && exception.TargetSite.Name == "ExtractParameterFromDictionary")
            {
                statusCode = 404;
                filterContext.Result = new RedirectResult("/c/notfound?ArgumentException");
                jt.Add("msg", filterContext.Exception.Message);
                log.Warn(jt.ToString());
            }
            else
            {
                string ep = Config.Cfg.GetErrorPage(request.Url.OriginalString);
                if (request.Url.AbsolutePath != ep)
                    filterContext.Result = new RedirectResult(ep);
                else
                    filterContext.Result = new ContentResult() { Content = "error" };
                log.Error(jt.ToString(), filterContext.Exception);
            }
            if (Config.Cfg.Debug)
            {
                filterContext.Result = new JsonNetResult() { Data = new JSON_Data { success = false, code = -1, msg = filterContext.Exception.Message } };
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

    }
}
