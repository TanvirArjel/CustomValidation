// <copyright file="FileMaxSizeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace TanvirArjel.CustomValidation.AspNetCore.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate allowed maximum size of a file.
    /// </summary>
    public sealed class FileMaxSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMaxSizeAttribute"/> class.
        /// </summary>
        /// <param name="maxSize">Allowed <see cref="MaxSize"/> of the file in KB.</param>
        public FileMaxSizeAttribute(int maxSize)
        {
            MaxSize = maxSize;
            ErrorMessage = ErrorMessage ?? "{0} should be not more than {1}.";
        }

        /// <summary>
        /// Get allowed <see cref="MaxSize"/> of the file. The unit of the size is KB.
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// Get allowed <see cref="MaxSize"/> of the file with appropriate unit.
        /// </summary>
        private string MaxSizeAndUnit => MaxSize >= 1024 ? Math.Round(MaxSize / 1024M, 2) + " MB" : MaxSize + " KB";

        /// <summary>
        /// To check whether the input <see cref="IFormFile"/> is larger than the specified size.
        /// </summary>
        /// <param name="value">Type of <see cref="IFormFile"/>.</param>
        /// <param name="validationContext">The request validation context.</param>
        /// <returns>Returns <see cref="ValidationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="validationContext"/> is null.</exception>
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
                    long fileLengthInKByte = inputFile.Length / 1024;

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
