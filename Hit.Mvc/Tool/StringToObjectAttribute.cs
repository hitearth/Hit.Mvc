using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web;
using Newtonsoft.Json.Converters;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Hit.Mvc
{
    public class StringToObjectAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new StringToObjectBinder();
        }
    }
    public class StringToObjectBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            var prefix = bindingContext.ModelName;
            if (prefix == null)
            {
                return null;
            }
            var vr = bindingContext.ValueProvider.GetValue(prefix);
            if (vr == null) return null;
            var obj = vr.ConvertTo(typeof(string));
            string jsonString = (string)obj;
            if (jsonString != null)
            {
                var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType);
                return result;
            }
            else
            {
                return null;
            }


        }
    }
}
