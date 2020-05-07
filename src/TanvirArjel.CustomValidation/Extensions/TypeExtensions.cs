// <copyright file="TypeExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace TanvirArjel.CustomValidation.Extensions
{
    internal static class TypeExtensions
    {
        private static HashSet<Type> _numericTypes = new HashSet<Type>
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

        internal static bool IsNumericType(this Type type)
        {
            return _numericTypes.Contains(type) || _numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }
    }
}
