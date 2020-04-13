// <copyright file="FixedLengthAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate length of a <see cref="string"/> field against the specified
    /// fixed length value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FixedLengthAttribute : ValidationAttribute
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="FixedLengthAttribute"/> class.
        /// </summary>
        /// <param name="fixedLength">A positive <see cref="int"/> value.</param>
        public FixedLengthAttribute(int fixedLength)
        {
            FixedLength = fixedLength;
            ErrorMessage = ErrorMessage ?? "{0} should be exactly {1} characters long.";
        }

        public int FixedLength { get; }

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
                string inputValue = value.ToString();

                if (inputValue.Length != FixedLength)
                {
                    string errorMessage = GetFormattedErrorMessage(ErrorMessage, validationContext.DisplayName, FixedLength);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private string GetFormattedErrorMessage(string errorMessage, string propertyName, int fixedLength)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, fixedLength);
        }
    }
}
