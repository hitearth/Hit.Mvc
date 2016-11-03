using System;
using System.ComponentModel.DataAnnotations;

namespace Hit.Mvc
{
    public sealed class ArrayLengthAttribute : ValidationAttribute
    {
        public ArrayLengthAttribute()
        {
        }

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