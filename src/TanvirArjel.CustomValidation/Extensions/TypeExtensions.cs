// <copyright file="TypeExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace TanvirArjel.CustomValidation.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>
        {
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        /// <summary>
        /// To check if the target type is numeric value type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>Returns <see langword="true"/> or <see langword="false"/>.</returns>
        public static bool IsNumericType(this Type type)
        {
            return _numericTypes.Contains(type) || _numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        /// <summary>
        /// To check if the target type is a <see cref="DateTime"/> value type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>Returns <see langword="true"/> or <see langword="false"/>.</returns>
        public static bool IsDateTimeType(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(DateTime?);
        }

        /// <summary>
        /// To check if the target type is <see cref="TimeSpan"/> value type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        /// <returns>Returns <see langword="true"/> or <see langword="false"/>.</returns>
        public static bool IsTimeSpanType(this Type type)
        {
            return type == typeof(TimeSpan) || type == typeof(TimeSpan?);
        }
    }
}
