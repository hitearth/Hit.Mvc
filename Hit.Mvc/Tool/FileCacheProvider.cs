//代码来自于 http://kb.cnblogs.com/page/114736/
using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Caching;

namespace Hit.Mvc
{

    public class FileCacheProvider : OutputCacheProvider
    {
        public override void Initialize(string name, NameValueCollection attributes)
        {
            base.Initialize(name, attributes);
            CachePath = HttpContext.Current.Server.MapPath(attributes["cachePath"]);
        }

        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            Object obj = Get(key);
            if (obj != null)
            {
                return obj;
            }
            Set(key, entry, utcExpiry);
            return entry;
        }

        public override object Get(string key)
        {
            string path = ConvertKeyToPath(key);
            if (!File.Exists(path))
            {
                return null;
            }
            CacheItem item = null;
            using (FileStream file = File.OpenRead(path))
            {
                var formatter = new BinaryFormatter();
                item = (CacheItem)formatter.Deserialize(file);
            }

            if (item.ExpiryDate <= DateTime.Now.ToUniversalTime())
            {

                Remove(key);
                return null;
            }
            return item.Item;
        }


        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            CacheItem item = new CacheItem(entry, utcExpiry);
            string path = ConvertKeyToPath(key);
            using (FileStream file = File.OpenWrite(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, item);
            }
        }

        public override void Remove(string key)
        {
            string path = ConvertKeyToPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        public string CachePath
        {
            get;
            set;
        }

        private string ConvertKeyToPath(string key)
        {
            string file = key.Replace('/', '-');
            file += ".txt";
            return Path.Combine(CachePath, file);
        }
    }

    [Serializable]
    public class CacheItem
    {
        public DateTime ExpiryDate;
        public object Item;

        public CacheItem(object entry, DateTime utcExpiry)
        {
            Item = entry;
            ExpiryDate = utcExpiry;
        }
    }
}