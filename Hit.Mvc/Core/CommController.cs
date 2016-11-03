using System.Web.Mvc;
using System.Linq;
using System;
using System.Globalization;
using System.Web.WebPages;
using CacheManager.Core.Internal;

namespace DHW.MVC.Controllers
{
    [AllowAnonymous]
    public class CController : Controller
    {
        public static string[] FileExtensions = new[] { "cshtml", };

        /// <summary>
        /// 404
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            return Content("NotFound");
        }
        /// <summary>
        /// 500
        /// </summary>
        /// <returns></returns>
        public ActionResult error()
        {
            return Content("error");
        }
        public ActionResult moudle()
        {
            if (Request.Url.Host == "localhost")
            {
                return Content("[\"" + string.Join("\",\"", HttpContext.ApplicationInstance.Modules.AllKeys) + "\"]", "application/json");
            }
            return Content("[]", "application/json");
        }
        public void cachestats()
        {
            var cache = CacheManager.Web.CacheManagerOutputCacheProvider.Cache;

            foreach (var handle in cache.CacheHandles)
            {
                var stats = handle.Stats;
                Response.Write(string.Format(
                         "Items: {0}, Hits: {1}, Miss: {2}, Remove: {3}, ClearRegion: {4}, Clear: {5}, Adds: {6}, Puts: {7}, Gets: {8}",
                             stats.GetStatistic(CacheStatsCounterType.Items),
                             stats.GetStatistic(CacheStatsCounterType.Hits),
                             stats.GetStatistic(CacheStatsCounterType.Misses),
                             stats.GetStatistic(CacheStatsCounterType.RemoveCalls),
                             stats.GetStatistic(CacheStatsCounterType.ClearRegionCalls),
                             stats.GetStatistic(CacheStatsCounterType.ClearCalls),
                             stats.GetStatistic(CacheStatsCounterType.AddCalls),
                             stats.GetStatistic(CacheStatsCounterType.PutCalls),
                             stats.GetStatistic(CacheStatsCounterType.GetCalls)
                         ));
                Response.Write("<br>");
            }

        }
        public void se()
        {
            throw new Exception("se");
        }

    }



}
