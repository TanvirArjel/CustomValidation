// <copyright file="FileAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using TanvirArjel.CustomValidation.AspNetCore.Extensions;

namespace TanvirArjel.CustomValidation.AspNetCore.Attributes
{
    /// <summary>
    /// An <see cref="Enum"/> of different file types.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a text file.
        /// </summary>
        [Description("text/plain")]
        Text,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a ms-word document file.
        /// </summary>
        [Description("application/msword")]
        Doc,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a ms-word document file.
        /// </summary>
        [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        DocX,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be an excel file.
        /// </summary>
        [Description("application/vnd.ms-excel")]
        Xls,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be an excel file.
        /// </summary>
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        Xlsx,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a power point file.
        /// </summary>
        [Description("application/vnd.ms-powerpoint")]
        Ppt,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a power point file.
        /// </summary>
        [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        Pptx,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a pdf file.
        /// </summary>
        [Description("application/pdf")]
        Pdf,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a jpg file.
        /// </summary>
        [Description("image/jpg")]
        Jpg,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a jpeg file.
        /// </summary>
        [Description("image/jpeg")]
        Jpeg,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a png file.
        /// </summary>
        [Description("image/png")]
        Png,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a mp4 file.
        /// </summary>
        [Description("video/mp4")]
        Mp4,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a mkv file.
        /// </summary>
        [Description("video/x-matroska")]
        Mkv,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a avi file.
        /// </summary>
        [Description("video/x-msvideo")]
        Avi,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a wmv file.
        /// </summary>
        [Description("video/x-ms-wmv")]
        Wmv,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a mp3 file.
        /// </summary>
        [Description("audio/mp3,audio/mpeg")]
        Mp3,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a rar file.
        /// </summary>
        [Description("application/x-rar-compressed,application/octet-stream")]
        Rar,

        /// <summary>
        /// Select if you want <see cref="IFormFile"/> should be a zip file.
        /// </summary>
        [Description("application/zip,application/octet-stream,application/x-zip-compressed,multipart/x-zip")]
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

        /// <summary>
        /// To check whether the input <see cref="IFormFile"/> is spefied file type.
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
                        string[] validFileTypes = FileTypes.Select(ft => ft.ToDescriptionString().ToUpperInvariant().Trim()).ToArray();
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
