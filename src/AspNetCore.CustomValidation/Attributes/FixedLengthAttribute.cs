using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate length of a <see cref="string"/> field against the specified
    /// fixed length value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FixedLengthAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// This private field store the fixed length value provided by the caller.
        /// </summary>
        private readonly int _fixedLength;

        /// <summary>
        ///  Initializes a new instance of the <see cref="FixedLengthAttribute"/> class.
        /// </summary>
        /// <param name="fixedLength">A positive <see cref="int"/> value.</param>
        public FixedLengthAttribute(int fixedLength)
        {
            _fixedLength = fixedLength;
            ErrorMessage = ErrorMessage ?? "{0} should be exactly {1} characters long.";
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();
            string errorMessage = GetFormattedErrorMessage(ErrorMessage, propertyDisplayName, _fixedLength);

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-fixed-length", errorMessage);

            string fixedLengthValue = _fixedLength.ToString(CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-fixed-length-value", fixedLengthValue);
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

            if (propertyInfo.PropertyType != typeof(string))
            {
                throw new ArgumentException($"The {nameof(FixedLengthAttribute)} is not valid on property type {propertyInfo.PropertyType}." +
                                            $" This Attribute is only valid on {typeof(string)} type.");
            }

            if (value != null)
            {
                var inputValue = value.ToString();

                if (inputValue.Length != _fixedLength)
                {
                    string errorMessage = GetFormattedErrorMessage(ErrorMessage, validationContext.DisplayName, _fixedLength);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private string GetFormattedErrorMessage(string errorMessage, string propertyName, int fixedLength)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, fixedLength);
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
