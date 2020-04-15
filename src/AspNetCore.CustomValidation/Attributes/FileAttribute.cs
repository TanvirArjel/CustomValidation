// <copyright file="FileAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AspNetCore.CustomValidation.Extensions;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// An <see cref="Enum"/> of different file types.
    /// </summary>
    public enum FileType
    {
        [Description("text/plain")]
        Text,

        [Description("application/msword")]
        Doc,

        [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        DocX,

        [Description("application/vnd.ms-excel")]
        Xls,

        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        Xlsx,

        [Description("application/vnd.ms-powerpoint")]
        Ppt,

        [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        Pptx,

        [Description("application/pdf")]
        Pdf,

        [Description("image/jpg")]
        Jpg,

        [Description("image/jpeg")]
        Jpeg,

        [Description("image/png")]
        Png,

        [Description("video/mp4")]
        Mp4,

        [Description("video/x-matroska")]
        Mkv,

        [Description("video/x-msvideo")]
        Avi,

        [Description("video/x-ms-wmv")]
        Wmv,

        [Description("audio/mp3,audio/mpeg")]
        Mp3,

        [Description("application/x-rar-compressed, application/octet-stream")]
        Rar,

        [Description("application/zip, application/octet-stream, application/x-zip-compressed, multipart/x-zip")]
        Zip
    }

    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate the value of <see cref="IFormFile"/> type property value from different
    /// parameters like <see cref="FileType"/>, <see cref="MaxSize"/> and <see cref="MinSize"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FileAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttribute"/> class.
        /// This constructor is used to pass a single <see cref="FileType"/> value which against the <see cref="IFormFile"/> will be validated.
        /// </summary>
        /// <param name="fileType">A single <see cref="FileType"/> value.</param>
        public FileAttribute(FileType fileType)
        {
            FileTypes = new FileType[] { fileType };
            ErrorMessage = ErrorMessage ?? "{0} should be in {1} format.";
            FileMinSizeErrorMessage = FileMinSizeErrorMessage ?? "{0} should be at least {1}.";
            FileMaxSizeErrorMessage = FileMaxSizeErrorMessage ?? "{0} should be not more than {1}.";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAttribute"/> class.
        /// This constructor is used to pass an <see cref="Array"/> of <see cref="FileType"/> values which against the <see cref="IFormFile"/> will be validated.
        /// </summary>
        /// <param name="fileTypes">An <see cref="Array"/> of <see cref="FileType"/>.</param>
        public FileAttribute(FileType[] fileTypes)
        {
            FileTypes = fileTypes;
            ErrorMessage = ErrorMessage ?? "{0} should be in {1} formats.";
            FileMinSizeErrorMessage = FileMinSizeErrorMessage ?? "{0} should be at least {1}.";
            FileMaxSizeErrorMessage = FileMaxSizeErrorMessage ?? "{0} should be not more than {1}.";
        }

        /// <summary>
        /// Get <see cref="Array"/> of provided valid file types.
        /// </summary>
        public FileType[] FileTypes { get; }

        /// <summary>
        /// Get and set the File <see cref="MinSize"/> in KB.
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// Get allowed <see cref="MinSize"/> of the file with appropriate unit.
        /// </summary>
        public string MinSizeAndUnit => MinSize >= 1024 ? Math.Round(MinSize / 1024M, 2) + " MB" : MinSize + " KB";

        /// <summary>
        /// Set your own error message for <see cref="MinSize"/> violation.
        /// </summary>
        public string FileMinSizeErrorMessage { get; set; }

        /// <summary>
        /// Get and set the File <see cref="MaxSize"/> in KB.
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Get allowed <see cref="MaxSize"/> of the file with appropriate unit.
        /// </summary>
        public string MaxSizeAndUnit => MaxSize >= 1024 ? Math.Round(MaxSize / 1024M, 2) + " MB" : MaxSize + " KB";

        /// <summary>
        /// Set your own error message for <see cref="MaxSize"/> violation.
        /// </summary>
        public string FileMaxSizeErrorMessage { get; set; }

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
                            string[] validFileTypeNames = FileTypes.Select(ft => ft.ToString("G")).ToArray();
                            string validFileTypeNamesString = string.Join(",", validFileTypeNames);
                            string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName, validFileTypeNamesString);
                            return new ValidationResult(formattedErrorMessage);
                        }
                    }

                    long fileLengthInKByte = inputFile.Length / 1000;

                    if (MinSize > 0 && fileLengthInKByte < MinSize)
                    {
                        string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, FileMinSizeErrorMessage, validationContext.DisplayName, MinSizeAndUnit);
                        return new ValidationResult(formattedErrorMessage);
                    }

                    if (MaxSize > 0 && fileLengthInKByte > MaxSize)
                    {
                        string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, FileMaxSizeErrorMessage, validationContext.DisplayName, MaxSizeAndUnit);
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