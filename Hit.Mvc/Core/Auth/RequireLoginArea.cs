using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace Hit.Mvc
{
    /// <summary>
    /// 根据Area获取Filter
    /// </summary>
    public sealed class AreaFilterCollection : IFilterProvider
    {
        Func<string, IEnumerable<Filter>> _getAuthFilterThunk;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="getAuthFilterThunk">根据Area获取Filter</param>
        public AreaFilterCollection(Func<string, IEnumerable<Filter>> getAuthFilterThunk) { _getAuthFilterThunk = getAuthFilterThunk; }

        IEnumerable<Filter> IFilterProvider.GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return _getAuthFilterThunk((string)controllerContext.RequestContext.RouteData.DataTokens["area"]);
        }
    }
}
