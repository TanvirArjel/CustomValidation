using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to make popular TinyMCE online editor field required along with an option for setting minimum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TinyMceRequiredAttribute : ValidationAttribute
    {
        /// <summary>
        /// You can set <see cref="MinLength"/> of the TinyMCE field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MinLength { get; set; } = 0;
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
                return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} is required.");
            }

            string inputValue = value.ToString();
            string  inputValueWithoutHtml = Regex.Replace(inputValue, "<.*?>", String.Empty);
            if (string.IsNullOrWhiteSpace(inputValueWithoutHtml))
            {
                return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} is required.");
            }

            if (MinLength > 0 && inputValueWithoutHtml.Length < MinLength)
            {
                return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be at least {MinLength} characters long.");
            }
            return ValidationResult.Success;
        }
    }
}
