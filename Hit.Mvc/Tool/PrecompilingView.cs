using System.Web.Mvc;
using System.Web.WebPages;

namespace Hit.Mvc
{
    /// <summary>
    /// 预编译视图
    /// </summary>
    public class PrecompilingView : IView
    {
        private WebViewPage webViewPage;
        private string virtualPath;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="instance">视图实例</param>
        /// <param name="viewPath">视图路径</param>
        public PrecompilingView(WebViewPage instance, string viewPath)
        {
            webViewPage = instance; virtualPath = viewPath;
        }
        /// <summary>
        /// _ViewStart 的 扩展名
        /// </summary>
        public static string[] FileExtensions = new[] { "cshtml", };
        void IView.Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            webViewPage.VirtualPath = virtualPath;
            webViewPage.ViewContext = viewContext;
            webViewPage.ViewData = viewContext.ViewData;

            WebPageRenderingBase startPage = StartPage.GetStartPage(webViewPage, "_ViewStart", FileExtensions);
            webViewPage.ExecutePageHierarchy(new WebPageContext(context: viewContext.HttpContext, page: null, model: null), writer, startPage);
        }
    }
}
