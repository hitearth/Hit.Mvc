using System.Web.Mvc;
using System.Web.WebPages;

namespace Hit.Mvc
{
    public static class Helper
    {
        /// <summary>
        /// 根据viewName,获取视图生成的html
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewName"></param>
        /// <param name="dic"></param>
        /// <param name="tempDic"></param>
        /// <returns></returns>
        public static string GetHtml(ControllerContext context, string viewName, ViewDataDictionary dic, TempDataDictionary tempDic)
        {
            string html = string.Empty;
            IView view = ViewEngines.Engines.FindView(context, viewName, string.Empty).View;
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                ViewContext vc = new ViewContext(context, view, dic, tempDic, sw);
                vc.View.Render(vc, sw);
                html = sw.ToString();
            }
            return html;
        }

        public static ViewResult GetPrecompilingViewResult(this Controller source, WebViewPage instance, string viewPath, object model = null)
        {
            if (model != null)
            {
                source.ViewData.Model = model;
            }
            return new ViewResult
            {
                MasterName = null,
                ViewData = source.ViewData,
                TempData = source.TempData,
                ViewEngineCollection = source.ViewEngineCollection,
                View = new PrecompilingView(instance, viewPath)
            };
        }

    }

    public class PrecompilingView : IView
    {
        private WebViewPage webViewPage;
        private string virtualPath;
        public PrecompilingView(WebViewPage instance, string viewPath)
        {
            webViewPage = instance; virtualPath = viewPath;
        }
        public static string[] FileExtensions = new[] { "cshtml", };
        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            webViewPage.VirtualPath = virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;

            WebPageRenderingBase startPage = StartPage.GetStartPage(webViewPage, "_ViewStart", FileExtensions);
            webViewPage.ExecutePageHierarchy(new WebPageContext(context: viewContext.HttpContext, page: null, model: null), writer, startPage);
        }
    }
}
