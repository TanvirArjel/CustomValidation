// <copyright file="FileTypeExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System.ComponentModel;
using TanvirArjel.CustomValidation.AspNetCore.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.Extensions
{
    internal static class FileTypeExtensions
    {
        public static string ToDescriptionString(this FileType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
