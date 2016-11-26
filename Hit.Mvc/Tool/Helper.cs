using System.Web.Mvc;
using System.Web.WebPages;

namespace Hit.Mvc
{
    /// <summary>
    /// Helper
    /// </summary>
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
        /// <summary>
        /// 根据instance,获取ViewResult
        /// </summary>
        /// <param name="source"></param>
        /// <param name="instance"></param>
        /// <param name="viewPath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
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
}
