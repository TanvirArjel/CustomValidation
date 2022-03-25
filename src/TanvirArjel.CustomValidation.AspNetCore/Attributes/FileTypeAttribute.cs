// <copyright file="FileTypeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using TanvirArjel.CustomValidation.AspNetCore.Extensions;

namespace TanvirArjel.CustomValidation.AspNetCore.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate file type of <see cref="IFormFile"/> object.
    /// </summary>
    public sealed class FileTypeAttribute : ValidationAttribute
    {
        /// <summary>
        /// This <see cref="ValidationAttribute"/> is used to validate file type of <see cref="IFormFile"/> object.
        /// </summary>
        /// <param name="fileTypes">An <see cref="Array"/> of <see cref="FileType"/>.</param>
        public FileTypeAttribute(params FileType[] fileTypes)
            : base("The {0} should be in {1} formats.")
        {
            FileTypes = fileTypes;
            string[] validFileTypeNames = FileTypes.Select(ft => ft.ToString("G")).ToArray();
            ValidFileTypeNamesString = string.Join(",", validFileTypeNames);
        }

        /// <summary>
        /// Get an <see cref="Array"/> of allowed file types.
        /// </summary>
        public FileType[] FileTypes { get; }

        /// <summary>
        /// Valid file types string
        /// </summary>
        internal string ValidFileTypeNamesString { get; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, ValidFileTypeNamesString);
        }

        /// <summary>
        /// To check whether the input <see cref="IFormFile"/> is type of the specified type.
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
                    if (FileTypes != null && FileTypes.Length > 0)
                    {
                        string[] validFileTypes = FileTypes.Select(ft => ft.ToDescriptionString().ToUpperInvariant()).ToArray();
                        validFileTypes = validFileTypes.SelectMany(vft => vft.Split(',')).ToArray();
                        if (!validFileTypes.Contains(inputFile.ContentType.ToUpperInvariant()))
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                }
                else
                {
                    return new ValidationResult("The selected file is empty.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
