using System;

namespace AspNetCore.CustomValidation.Extensions
{
    internal static class ObjectExtensions
    {
        internal static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }

        internal static bool IsDateTime(this object value)
        {
            return value is DateTime;
        }
    }
}
