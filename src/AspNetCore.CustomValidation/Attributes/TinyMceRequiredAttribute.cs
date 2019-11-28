using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to make popular TinyMCE online editor field required along with an option for setting minimum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TinyMceRequiredAttribute : ValidationAttribute, IClientModelValidator
    {
        public TinyMceRequiredAttribute()
        {
            ErrorMessage = ErrorMessage ?? "The {0} field is required {1}.";
        }
        /// <summary>
        /// You can set <see cref="MinLength"/> of the TinyMCE field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MaxLength { get; set; }
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

            if (propertyInfo.PropertyType != typeof(String))
            {
                throw new ArgumentException($"The {nameof(TinyMceRequiredAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                            $"This Attribute is only valid on {typeof(String)}");
            }

            if (value == null)
            {
                return new ValidationResult(GetRequiredErrorMessage(validationContext.DisplayName));
            }

            string inputValue = value.ToString();
            string  inputValueWithoutHtml = Regex.Replace(inputValue, "<.*?>", String.Empty);

            if (string.IsNullOrWhiteSpace(inputValueWithoutHtml))
            {
                return new ValidationResult(GetRequiredErrorMessage(validationContext.DisplayName));
            }

            if (MinLength > 0 && inputValueWithoutHtml.Length < MinLength)
            {
                return new ValidationResult(GetMinLengthErrorMessage(validationContext.DisplayName));
            }

            if (MaxLength > 0 && inputValueWithoutHtml.Length > MaxLength)
            {
                return new ValidationResult(GetMaxLengthErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var propertyDisplayName = context.ModelMetadata.GetDisplayName();
            
            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-tinymce-required", GetRequiredErrorMessage(propertyDisplayName));

            if (MinLength > 0)
            {
                AddAttribute(context.Attributes, "data-val-tinymce-minlength", GetMinLengthErrorMessage(propertyDisplayName));
                AddAttribute(context.Attributes, "data-val-tinymce-minlength-value", MinLength.ToString(CultureInfo.InvariantCulture));
            }

            if (MaxLength > 0)
            {
                AddAttribute(context.Attributes, "data-val-tinymce-maxlength", GetMaxLengthErrorMessage(propertyDisplayName));
                AddAttribute(context.Attributes, "data-val-tinymce-maxlength-value", MaxLength.ToString(CultureInfo.InvariantCulture));
            }

            
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private static string MinLengthErrorMessage => "The {0} should be at least {1} characters long.";
        private static string MaxLengthErrorMessage => "The {0} cannot be more than {1} characters long.";

        private string GetRequiredErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, ErrorMessage, displayName,string.Empty);
        }

        private string GetMinLengthErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, MinLengthErrorMessage, displayName, MinLength);
        }

        private string GetMaxLengthErrorMessage( string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, MaxLengthErrorMessage, displayName, MaxLength);
        }
    }
}
