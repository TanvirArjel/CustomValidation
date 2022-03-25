using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate the WYSIWYG editor text min length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class TextEditorMinLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorMinLengthAttribute"/> class.
        /// </summary>
        public TextEditorMinLengthAttribute()
            : base("{0} should be at least {1} characters long.")
        { }

        /// <summary>
        /// Get and set the minimum length of the text editor field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MinLength { get; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinLength);
        }

        /// <summary>
        /// To check whether the input text violates the required constraint.
        /// </summary>
        /// <param name="value">Type of <see cref="DateTime"/>.</param>
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

            if (propertyInfo.PropertyType != typeof(string))
            {
                throw new ArgumentException($"The {nameof(TextEditorMinLengthAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                            $"This Attribute is only valid on {typeof(string)}");
            }

            string requiredErrorMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessageString, validationContext.DisplayName);

            if (value == null)
            {
                return MinLength > 0 ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) : ValidationResult.Success;
            }

            string inputValue = value.ToString();
            string inputValueWithoutHtml = Regex.Replace(inputValue, "<.*?>|&nbsp;", string.Empty);

            if (string.IsNullOrWhiteSpace(inputValueWithoutHtml))
            {
                return new ValidationResult(requiredErrorMessage);
            }

            if (MinLength > 0 && inputValueWithoutHtml.Length < MinLength)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
