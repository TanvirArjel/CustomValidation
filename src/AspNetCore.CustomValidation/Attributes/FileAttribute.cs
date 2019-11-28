using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AspNetCore.CustomValidation.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the value of <see cref="IFormFile"/> type property value from different
    /// parameters like <see cref="FileType"/>, <see cref="MaxSize"/> and <see cref="MinSize"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FileAttribute : ValidationAttribute, IClientModelValidator
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
                        if (!validFileTypes.Contains(inputFile.ContentType.ToUpperInvariant()))
                        {
                            string[] validFileTypeNames = FileTypes.Select(ft => ft.ToString("G")).ToArray();
                            string validFileTypeNamesString = string.Join(",", validFileTypeNames);
                            var fileTypeErrorMessage = GetFileTypeErrorMessage(FileTypeErrorMessage, validFileTypeNamesString, validFileTypeNames.Length);
                            return new ValidationResult(fileTypeErrorMessage);

                        }
                    }

                    var fileLengthInKByte = inputFile.Length / 1000;

                    if (MinSize > 0 && fileLengthInKByte < MinSize)
                    {
                        return new ValidationResult(FileMinSizeErrorMessage);
                    }

                    if (MaxSize > 0 && fileLengthInKByte > MaxSize)
                    {
                        return new ValidationResult(FileMaxSizeErrorMessage);
                    }
                }
                else
                {
                    return new ValidationResult("Selected file is empty.");
                }
            }

            return ValidationResult.Success;
        }

        private FileType[] FileTypes { get; }

        /// <summary>
        /// Set the File <see cref="MinSize"/> in KB.
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// Set the File <see cref="MaxSize"/> in KB.
        /// </summary>
        public int MaxSize { get; set; }

        private static string FileTypeErrorMessage = "The file should be in {0} {1}.";
        private string FileMinSizeErrorMessage => $"File size should be at least {(MinSize >= 1024 ? Math.Round(MinSize / 1024M,2) + " MB" : MinSize + " KB")}.";
        private string FileMaxSizeErrorMessage => $"File size should not be more than {(MaxSize >= 1024 ? Math.Round(MaxSize / 1024M,2) + " MB" : MaxSize + " KB")}.";

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            string[] validFileTypeNames = FileTypes.Select(ft => ft.ToString("G")).ToArray();
            string validFileTypeNamesString = string.Join(",", validFileTypeNames);

            var fileTypeErrorMessage = GetFileTypeErrorMessage(FileTypeErrorMessage, validFileTypeNamesString, validFileTypeNames.Length);

            AddAttribute(context.Attributes, "data-val", "true");

            AddAttribute(context.Attributes, "data-val-filetype", fileTypeErrorMessage);
            AddAttribute(context.Attributes, "data-val-filetype-validtypes", validFileTypeNamesString);

            if (MinSize > 0)
            {
                AddAttribute(context.Attributes, "data-val-file-minsize", FileMinSizeErrorMessage);
                AddAttribute(context.Attributes, "data-val-file-minsize-value", MinSize.ToString(CultureInfo.InvariantCulture));
            }

            if (MaxSize > 0)
            {
                AddAttribute(context.Attributes, "data-val-file-maxsize", FileMaxSizeErrorMessage);
                AddAttribute(context.Attributes, "data-val-file-maxsize-value", MaxSize.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private string GetFileTypeErrorMessage(string errorMessageString, string fileTypeNamesString, int fileTypeCount)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessageString, fileTypeNamesString, fileTypeCount > 1 ? "formats" : "format");
        }
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

    
}
