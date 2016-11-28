using System;
using System.ComponentModel.DataAnnotations;

namespace Hit.Mvc
{
    /// <summary>
    /// 数组为空判断
    /// </summary>
    public sealed class ArrayLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// 初始化 Hit.Mvc.ArrayLengthAttribute 类的新实例。
        /// </summary>
        public ArrayLengthAttribute()
        {
        }
        /// <summary>
        /// 确定对象的指定值是否有效。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            if (value is Array)
            {
                return ((Array)value).Length > 0;
            }
            return false;
        }
    }
}