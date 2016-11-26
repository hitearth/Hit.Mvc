using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Hit.Mvc
{
    /// <summary>
    /// 监测文件的变更
    /// </summary>
    public class Watcher
    {
        private JToken data;
        private int cacheDependencyFlag = 0;

        private string path;
        Action<string> ilog;
        Action<JToken> callback;
        private string watcherKey;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="ilog"></param>
        public Watcher(string path, string key, Action<JToken> callback, Action<string> ilog)
        {
            this.path = path;
            this.ilog = ilog;
            this.callback = callback;
            watcherKey = "__configkey_" + key;
        }
        /// <summary>
        /// 加载文件
        /// </summary>
        public void Load()
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    data = JObject.Load(new JsonTextReader(sr)) as JToken;
                }
                callback(data);
            }
            catch (Exception ex)
            {
                ilog(ex.ToString());
            }


            int flag = System.Threading.Interlocked.CompareExchange(ref cacheDependencyFlag, 1, 0);
            if (flag == 0)
            {
                CacheDependency dep = new CacheDependency(path);
                HttpRuntime.Cache.Insert(watcherKey, "c000 " + DateTime.Now.ToString("yyyyMMddHHmmss"), dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, RunOptionsUpdateCallback);
            }
        }

        private void RunOptionsUpdateCallback(
            string key, CacheItemUpdateReason reason,
            out object expensiveObject,
            out CacheDependency dependency,
            out DateTime absoluteExpiration,
            out TimeSpan slidingExpiration)
        {
            expensiveObject = "c001" + DateTime.Now.ToString("yyyyMMddHHmmss");
            dependency = new CacheDependency(path);
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = Cache.NoSlidingExpiration;

            Load();

        }


    }






}
