using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hit.Mvc
{
    public sealed class JsonNetValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");

            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request
                return null;
            }

            StreamReader reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            string bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }
            if (!bodyText.StartsWith("{")) return null;

            return new JsonNetValueProvider(JObject.Parse(bodyText) as JContainer);
        }
    }

    public class JsonNetValueProvider : IValueProvider
    {
        private JContainer _jcontainer;

        public JsonNetValueProvider(JContainer jcontainer)
        {
            _jcontainer = jcontainer;
        }

        public bool ContainsPrefix(string prefix)
        {
            return _jcontainer.SelectToken(prefix) != null;
        }

        public ValueProviderResult GetValue(string key)
        {
            var jtoken = _jcontainer.SelectToken(key);
            if (jtoken == null || jtoken.Type == JTokenType.Object) return null;
            if (jtoken.Type == JTokenType.Array)
            {
                var d = jtoken as JArray;
                if (d != null && d.Count == 0) return new ArraySp1ProviderResult(d, jtoken.ToString(), CultureInfo.CurrentCulture);
                return null;
            }
            return new ValueProviderResult(jtoken.ToObject<object>(), jtoken.ToString(), CultureInfo.CurrentCulture);
        }
    }
    public class ArraySp1ProviderResult : ValueProviderResult
    {
        public ArraySp1ProviderResult(object rawValue, string attemptedValue, CultureInfo culture)
            : base(rawValue, attemptedValue, culture)
        {
        }
        public override object ConvertTo(Type type, CultureInfo culture)
        {
            JArray jtoken = RawValue as JArray;
            if (jtoken == null) return null;
            var d = Array.CreateInstance(type.GetElementType(), 0);
            return d;
        }
    }
    public class JsonNetValueProviderResult : ValueProviderResult
    {
        public JsonNetValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture)
            : base(rawValue, attemptedValue, culture)
        {
        }
        public override object ConvertTo(Type type, CultureInfo culture)
        {
            JToken jtoken = RawValue as JToken;
            if (jtoken == null) return null;
            using (JTokenReader jr = new JTokenReader(jtoken))
            {
                return JsonSerializer.Create(null).Deserialize(jr, type);
            }
        }
    }
}