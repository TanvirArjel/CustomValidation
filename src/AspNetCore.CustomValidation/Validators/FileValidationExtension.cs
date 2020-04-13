// <copyright file="FileValidationExtension.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AspNetCore.CustomValidation.Attributes;
using AspNetCore.CustomValidation.Extensions;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CustomValidation.Validators
{
    public static class FileValidationExtension
    {
        /// <summary>
        /// This extension method is used to validate <see cref="IFormFile"/> input against dynamic values from database,
        /// configuration file or any external source.
        /// </summary>
        /// <param name="validationContext">The type to be extended.</param>
        /// <param name="propertyName">Name of the <see cref="IFormFile"/> type property.</param>
        /// <param name="fileOptions">Validation options.</param>
        /// <returns>Returns <see cref="ValidationResult"/></returns>
        public static ValidationResult ValidateFile(
            this ValidationContext validationContext,
            string propertyName,
            FileOptions fileOptions)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"The object does not contain the property '{propertyName}'");
            }

            var propertyType = propertyInfo.PropertyType;

            if (propertyType != typeof(IFormFile))
            {
                throw new ArgumentException($"Property '{propertyName}' must be {typeof(IFormFile)} type.");
            }

            var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            IFormFile inputFile = (IFormFile)propertyValue;

            if (inputFile == null || fileOptions == null)
            {
                return ValidationResult.Success;
            }

            if (inputFile.Length > 0)
            {
                if (fileOptions.FileTypes != null && fileOptions.FileTypes.Length > 0)
                {
                    string[] validFileTypes = fileOptions.FileTypes.Select(ft => ft.ToDescriptionString().ToUpperInvariant()).ToArray();
                    validFileTypes = validFileTypes.SelectMany(vft => vft.Split(',')).ToArray();
                    if (!validFileTypes.Contains(inputFile.ContentType.ToUpperInvariant()))
                    {
                        string[] validFileTypeNames = fileOptions.FileTypes.Select(ft => ft.ToString("G")).ToArray();
                        string validFileTypeNamesString = string.Join(",", validFileTypeNames);

                        return new ValidationResult(
                            $"The file should be in {validFileTypeNamesString.ToUpperInvariant()}" +
                                                    $" {(validFileTypeNames.Length > 1 ? "formats" : "format")}.", new[] { propertyName });
                    }
                }

                var fileLengthInKByte = inputFile.Length / 1024;

                if (fileOptions.MinSize > 0 && fileLengthInKByte < fileOptions.MinSize)
                {
                    return new ValidationResult($"File size should be at least {(fileOptions.MinSize >= 1024 ? Math.Round(fileOptions.MinSize / 1024M, 2) + " MB" : fileOptions.MinSize + " KB")}.", new[] { propertyName });
                }

                if (fileOptions.MaxSize > 0 && fileLengthInKByte > fileOptions.MaxSize)
                {
                    return new ValidationResult($"File size should not be more than {(fileOptions.MaxSize >= 1024 ? Math.Round(fileOptions.MaxSize / 1024M, 2) + " MB" : fileOptions.MaxSize + " KB")}.", new[] { propertyName });
                }
            }
            else
            {
                return new ValidationResult("Selected file is empty.", new[] { propertyName });
            }

            return ValidationResult.Success;
        }
    }

    public class FileOptions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Not applicable here")]
        public FileType[] FileTypes { get; set; }

        /// <summary>
        /// Set allowed minimum size in KB.
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// Set allowed maximum size in KB.
        /// </summary>
        public int MaxSize { get; set; }
    }
}
