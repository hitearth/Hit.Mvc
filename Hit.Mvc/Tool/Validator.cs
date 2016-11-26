using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Hit.Mvc.Tool
{
    /// <summary>
    /// MVC 数据验证
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// 验证实例 model 数据
        /// </summary>
        /// <typeparam name="T">实例的类型</typeparam>
        /// <param name="model">实例</param>
        /// <param name="validationResult">错误信息</param>
        /// <returns>验证通过 true 不通过 false</returns>
        public static bool Validate<T>(T model, out List<ModelValidationResult> validationResult)
        {
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(T));
            validationResult = ModelValidator.GetModelValidator(modelMetadata, new ControllerContext()).Validate(null).ToList();
            return validationResult.Count == 0;
        }
        /// <summary>
        /// 验证实例 model 数据
        /// </summary>
        /// <typeparam name="T">实例的类型</typeparam>
        /// <param name="model">实例</param>
        /// <returns>验证通过 true 不通过 false</returns>
        public static bool Validate<T>(T model)
        {
            List<ModelValidationResult> validationResult;
            return Validate(model, out validationResult);
        }
    }
}
