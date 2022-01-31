// <copyright file="ObjectExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;

namespace TanvirArjel.CustomValidation.Extensions
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

        internal static TimeSpan? ToTimeSpan(this object value)
        {
            TimeSpan? timeSpan = null;

            if (value != null)
            {
                if (TimeSpan.TryParse(value.ToString(), out TimeSpan outTimeSpan))
                {
                    timeSpan = outTimeSpan;
                }
                else
                {
                    throw new FormatException("The provided string is not a valid timespan.");
                }
            }

            return timeSpan;
        }

        internal static DateTime? ToDateTime(this object value)
        {
            DateTime? dateTime = null;

            if (value != null)
            {
                if (DateTime.TryParse(value.ToString(), out DateTime outDateTime))
                {
                    dateTime = outDateTime;
                }
                else
                {
                    throw new FormatException("The string was not recognized as a valid DateTime. String format should be: 01-Jan-2020 or 01-Jan-2020 10:00:00 AM");
                }
            }

            return dateTime;
        }
    }
}
