using System;
using System.Collections.Generic;

namespace Hit.Mvc
{
    /// <summary>
    /// 已编译视图的IVirtualPathFactory
    /// </summary>
    public class CompileViewVirtualPathFactory : System.Web.WebPages.IVirtualPathFactory
    {
        private Dictionary<string, Func<object>> pathFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFactory"></param>
        public CompileViewVirtualPathFactory(Dictionary<string, Func<object>> pathFactory)
        {
            this.pathFactory = pathFactory;
        }
        object System.Web.WebPages.IVirtualPathFactory.CreateInstance(string virtualPath)
        {
            if (pathFactory.ContainsKey(virtualPath)) return pathFactory[virtualPath]();
            return null;
        }
        bool System.Web.WebPages.IVirtualPathFactory.Exists(string virtualPath)
        {
            return pathFactory.ContainsKey(virtualPath);
        }
    }
}
