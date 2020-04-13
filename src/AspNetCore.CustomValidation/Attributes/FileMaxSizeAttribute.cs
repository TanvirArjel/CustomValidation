// <copyright file="FileMaxSizeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CustomValidation.Attributes
{
    public sealed class FileMaxSizeAttribute : ValidationAttribute
    {
        public FileMaxSizeAttribute(int maxSize)
        {
            MaxSize = maxSize;
            ErrorMessage = ErrorMessage ?? "{0} should be not more than {1}.";
        }

        public int MaxSize { get;  }

        private string MaxSizeAndUnit => MaxSize >= 1024 ? Math.Round(MaxSize / 1024M, 2) + " MB" : MaxSize + " KB";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{validationContext.MemberName}'");
            }

            if (propertyInfo.PropertyType != typeof(IFormFile))
            {
                throw new ArgumentException($"The {nameof(FileAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                            $"This Attribute is only valid on {typeof(IFormFile)}");
            }

            if (value != null)
            {
                IFormFile inputFile = (IFormFile)value;

                if (inputFile.Length > 0)
                {
                    var fileLengthInKByte = inputFile.Length / 1024;

                    if (MaxSize > 0 && fileLengthInKByte > MaxSize)
                    {
                        string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName, MaxSizeAndUnit);
                        return new ValidationResult(formattedErrorMessage);
                    }
                }
                else
                {
                    return new ValidationResult("Selected file is empty.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
