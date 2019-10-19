using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the value of <see cref="IFormFile"/> type property value from different
    /// parameters like <see cref="FileType"/>, <see cref="MaxSize"/> and <see cref="MinSize"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FileAttribute : ValidationAttribute
    {
        /// <summary>
        /// This constructor is used to pass a single <see cref="FileType"/> value which against the <see cref="IFormFile"/> will be validated.
        /// </summary>
        /// <param name="fileType">A single <see cref="FileType"/> value.</param>
        public FileAttribute(FileType fileType)
        {
            FileTypes = new FileType[]{ fileType };
        }

        /// <summary>
        /// This constructor is used to pass an <see cref="Array"/> of <see cref="FileType"/> values which against the <see cref="IFormFile"/> will be validated.
        /// </summary>
        /// <param name="fileTypes">An <see cref="Array"/> of <see cref="FileType"/></param>
        public FileAttribute(FileType[] fileTypes)
        {
            FileTypes = fileTypes;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "<Pending>")]
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType != typeof(IFormFile))
                {
                    throw new ArgumentException($"The {nameof(FileAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                                $"This Attribute is only valid on {typeof(IFormFile)}");
                }
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile inputFile = (IFormFile)value;

            if (inputFile.Length > 0)
            {
                if (FileTypes != null && FileTypes.Length > 0)
                {
                    string[] validFileTypes = FileTypes.Select(ft => ft.ToDescriptionString().ToUpperInvariant()).ToArray();
                    if (!validFileTypes.Contains(inputFile.ContentType.ToUpperInvariant()))
                    {
                        string[] validFileTypeNames = FileTypes.Select(ft => ft.ToString("G")).ToArray();
                        string validFileTypeNamesString = string.Join(",", validFileTypeNames);

                        if (validFileTypeNames.Length > 1)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The file should be in {validFileTypeNamesString.ToLowerInvariant()} formats.");
                        }

                        return new ValidationResult(ErrorMessage ?? $"The file should be in {validFileTypeNamesString.ToLowerInvariant()} format.");

                    }
                }

                var fileLengthInKByte = inputFile.Length / 1000;

                if (MinSize > 0 && fileLengthInKByte < MinSize)
                {
                    return new ValidationResult($"File size should be at least {MinSize} KB.");
                }

                if (MaxSize > 0 && fileLengthInKByte > MaxSize)
                {
                    return new ValidationResult($"File size should not be more than {MaxSize} KB.");
                }
            }
            else
            {
                return new ValidationResult("Selected file is empty.");
            }

            return ValidationResult.Success;
        }

        public FileType[] FileTypes { get; }

        /// <summary>
        /// Set the File <see cref="MinSize"/> in KB.
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// Set the File <see cref="MaxSize"/> in KB.
        /// </summary>
        public int MaxSize { get; set; }
    }

    /// <summary>
    /// An <see cref="Enum"/> of different file types.
    /// </summary>
    public enum FileType
    {
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
        [Description("audio/mp3")]
        Mp3
    }

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
