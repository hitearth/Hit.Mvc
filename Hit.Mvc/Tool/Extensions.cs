using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Hit.Mvc
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class ModelError { public string Key { get; set; } public ModelState Value { get; set; } }

    public static class Extensions
    {
        public static List<ModelError> GetError(this ModelStateDictionary model)
        {
            var d = new List<ModelError>();
            foreach (var item in model)
            {
                if (item.Value.Errors.Count > 0)
                    d.Add(new ModelError { Key = item.Key, Value = item.Value });
            }
            return d;
        }
        public static string[] GetErrorMessage(this ModelStateDictionary model)
        {
            return model.SelectMany(it => it.Value.Errors.Select(er => er.ErrorMessage)).ToArray();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
