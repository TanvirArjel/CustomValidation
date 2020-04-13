// <copyright file="TextEditorRequiredAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to make popular TinyMCE online editor field required along with an option for setting minimum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class TextEditorRequiredAttribute : ValidationAttribute
    {
        public TextEditorRequiredAttribute()
        {
            ErrorMessage = ErrorMessage ?? "{0} field is required.";
            MinLengthErrorMessage = MinLengthErrorMessage ?? "{0} should be at least {1} characters long.";
            MaxLengthErrorMessage = MaxLengthErrorMessage ?? "{0} cannot be more than {1} characters long.";
        }

        /// <summary>
        /// You can set <see cref="MinLength"/> of the TinyMCE field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// You can set <see cref="MaxLength"/> of the TinyMCE field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MaxLength { get; set; }

        public string MinLengthErrorMessage { get; set; }

        public string MaxLengthErrorMessage { get; set; }

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

            if (propertyInfo.PropertyType != typeof(string))
            {
                throw new ArgumentException($"The {nameof(TextEditorRequiredAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                            $"This Attribute is only valid on {typeof(string)}");
            }

            string requiredErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName);

            if (value == null)
            {
                return new ValidationResult(requiredErrorMessage);
            }

            string inputValue = value.ToString();
            string inputValueWithoutHtml = Regex.Replace(inputValue, "<.*?>", string.Empty);

            if (string.IsNullOrWhiteSpace(inputValueWithoutHtml))
            {
                return new ValidationResult(requiredErrorMessage);
            }

            if (MinLength > 0 && inputValueWithoutHtml.Length < MinLength)
            {
                string minLengthErrorMessage = string.Format(CultureInfo.InvariantCulture, MinLengthErrorMessage, validationContext.DisplayName, MinLength);
                return new ValidationResult(minLengthErrorMessage);
            }

            if (MaxLength > 0 && inputValueWithoutHtml.Length > MaxLength)
            {
                string maxLengthErrorMessage = string.Format(CultureInfo.InvariantCulture, MaxLengthErrorMessage, validationContext.DisplayName, MaxLength);
                return new ValidationResult(maxLengthErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
