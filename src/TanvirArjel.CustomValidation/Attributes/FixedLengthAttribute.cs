﻿// <copyright file="FixedLengthAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate the length of a <see cref="string"/> field against the specified
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
            ErrorMessage = ErrorMessage ?? "The {0} should be exactly {1} characters long.";
        }

        /// <summary>
        /// Get the value of specified fixed length.
        /// </summary>
        public int FixedLength { get; }

        /// <summary>
        /// To check whether the input value has exactly same as specified.
        /// </summary>
        /// <param name="value">Type of <see cref="string"/>.</param>
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

        private static string GetFormattedErrorMessage(string errorMessage, string propertyName, int fixedLength)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, fixedLength);
        }
    }
}
