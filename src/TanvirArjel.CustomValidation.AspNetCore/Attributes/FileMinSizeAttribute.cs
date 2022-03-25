// <copyright file="FileMinSizeAttribute.cs" company="TanvirArjel">
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
    /// This <see cref="ValidationAttribute"/> is used to validate allowed minimum size of a file.
    /// </summary>
    public sealed class FileMinSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMinSizeAttribute"/> class.
        /// </summary>
        /// <param name="minSize">Allowed <see cref="MinSize"/> of the file in KB.</param>
        public FileMinSizeAttribute(int minSize)
            : base("{0} should be at least {1}.")
        {
            MinSize = minSize;
        }

        /// <summary>
        /// Get allowed <see cref="MinSize"/> of the file. The unit of the size is KB.
        /// </summary>
        public int MinSize { get; }

        /// <summary>
        /// Get allowed <see cref="MinSize"/> of the file with appropriate unit.
        /// </summary>
        internal string MinSizeAndUnit => MinSize >= 1024 ? Math.Round(MinSize / 1024M, 2) + " MB" : MinSize + " KB";

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinSizeAndUnit);
        }

        /// <summary>
        /// To check whether the input <see cref="IFormFile"/> is smaller than the specified size.
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

                    if (MinSize > 0 && fileLengthInKByte < MinSize)
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
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
