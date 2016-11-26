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
    /// <summary>
    /// 把JSON字符串转成对象
    /// </summary>
    public class StringToObjectAttribute : CustomModelBinderAttribute
    {
        /// <summary>
        /// 检索关联的模型联编程序。
        /// </summary>
        /// <returns>对实现 System.Web.Mvc.IModelBinder 接口的对象的引用。</returns>
        public override IModelBinder GetBinder()
        {
            return new StringToObjectBinder();
        }
    }
    /// <summary>
    /// 把JSON字符串转成对象
    /// </summary>
    public class StringToObjectBinder : IModelBinder
    {
        /// <summary>
        /// 使用指定的控制器上下文和绑定上下文将模型绑定到一个值。
        /// </summary>
        /// <param name="controllerContext">控制器上下文。</param>
        /// <param name="bindingContext">绑定上下文。</param>
        /// <returns>绑定值。</returns>
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
