// <copyright file="TextEditorRequiredAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to make online text editor field, like TinyMCE, required along with an option for setting minimum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class TextEditorAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditorAttribute"/> class.
        /// </summary>
        public TextEditorAttribute()
        {
            ErrorMessage = ErrorMessage ?? "{0} field is required.";
            MinLengthErrorMessage = MinLengthErrorMessage ?? "{0} should be at least {1} characters long.";
            MaxLengthErrorMessage = MaxLengthErrorMessage ?? "{0} cannot be more than {1} characters long.";
        }

        /// <summary>
        /// Get and set the minimum length of the text editor field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Get and set the maximum length of the text editor field. The value should be a positive <see cref="int"/> number.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Get and set the error message if the <c>MinLength</c> validation fails.
        /// </summary>
        public string MinLengthErrorMessage { get; set; }

        /// <summary>
        /// Get and set the error message if the <c>MaxLength</c> validaton fails.
        /// </summary>
        public string MaxLengthErrorMessage { get; set; }

        /// <summary>
        /// To check whether the input date violates the required constraint.
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
                throw new ArgumentException($"The {nameof(TextEditorAttribute)} is not valid on property type {propertyInfo.PropertyType}" +
                                            $"This Attribute is only valid on {typeof(string)}");
            }

            string requiredErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName);

            if (value == null)
            {
                return new ValidationResult(requiredErrorMessage);
            }

            string inputValue = value.ToString();
            string inputValueWithoutHtml = Regex.Replace(inputValue, "<.*?>|&nbsp;", string.Empty);

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
