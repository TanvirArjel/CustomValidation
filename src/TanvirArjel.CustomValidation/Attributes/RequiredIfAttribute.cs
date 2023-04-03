// <copyright file="RequiredIfAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using TanvirArjel.CustomValidation.Extensions;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate required based on other property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RequiredIfAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">The other property to which against property with this attribute will mark required.</param>
        /// <param name="comparisonType">The <see cref="ComparisonType"/> type of the other property supplied value.</param>
        /// <param name="otherPropertyValue">The value against comparison will be done.</param>
        public RequiredIfAttribute(string otherPropertyName, ComparisonType comparisonType, object otherPropertyValue)
        {
            OtherPropertyName = otherPropertyName;
            ComparisonType = comparisonType;
            OtherPropertyValue = otherPropertyValue;
            ErrorMessage = ErrorMessage ?? "The {0} field is required.";
        }

        /// <summary>
        /// Gets other property name.
        /// </summary>
        public string OtherPropertyName { get; }

        /// <summary>
        /// Gets comparison type of the other property value.
        /// </summary>
        public ComparisonType ComparisonType { get; }

        /// <summary>
        /// Gets other property value.
        /// </summary>
        public object OtherPropertyValue { get; }

        /// <summary>
        /// To check wether the property safisfy the required validaton based on the other property value.
        /// </summary>
        /// <param name="value">Value of the property.</param>
        /// <param name="validationContext">The request validation context.</param>
        /// <returns>Returns <see cref="ValidationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="validationContext"/> is null.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if (string.IsNullOrWhiteSpace(OtherPropertyName))
            {
                throw new ArgumentException($"Other property name is empty");
            }

            object parentObj = validationContext.ObjectInstance;
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);

            if (otherPropertyInfo == null)
            {
                string[] propertyNames = OtherPropertyName.Split('.');
                otherPropertyInfo = validationContext.ObjectType.GetProperty(propertyNames[0]);

                if (otherPropertyInfo == null)
                {
                    throw new ArgumentException($"The object does not contain any property with name '{OtherPropertyName}'");
                }

                for (int i = 1; i < propertyNames.Length; i++)
                {
                    parentObj = otherPropertyInfo.GetValue(parentObj, null);
                    otherPropertyInfo = otherPropertyInfo.PropertyType.GetProperty(propertyNames[i]);

                    if (otherPropertyInfo == null)
                    {
                        throw new ArgumentException($"The object does not contain any property with name '{OtherPropertyName}'");
                    }
                }
            }

            Type otherPropertyType = otherPropertyInfo.PropertyType;

            object otherPropertyContextValue = otherPropertyInfo.GetValue(parentObj, null);

            // Cast value to the appropriate dynamic type.
            dynamic otherPropertyContextValueDynamic;
            dynamic otherPropertyValueDynamic;

            if (otherPropertyType.IsDateTimeType())
            {
                otherPropertyContextValueDynamic = otherPropertyContextValue.ToDateTime();
                otherPropertyValueDynamic = OtherPropertyValue.ToDateTime();
            }
            else if (otherPropertyType.IsNumericType())
            {
                otherPropertyContextValueDynamic = Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture);
                otherPropertyValueDynamic = Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture);
            }
            else if (otherPropertyType == typeof(string))
            {
                if (this.ComparisonType == ComparisonType.Equal || this.ComparisonType == ComparisonType.NotEqual)
                {
                    otherPropertyContextValueDynamic = otherPropertyContextValue?.ToString() ?? string.Empty;
                    otherPropertyValueDynamic = OtherPropertyValue?.ToString() ?? string.Empty;
                }
                else
                {
                    otherPropertyContextValueDynamic = otherPropertyContextValue?.ToString().Length ?? 0;
                    otherPropertyValueDynamic = OtherPropertyValue?.ToString().Length ?? 0;
                }
            }
            else if (otherPropertyType.IsTimeSpanType())
            {
                otherPropertyContextValueDynamic = otherPropertyContextValue.ToTimeSpan();
                otherPropertyValueDynamic = OtherPropertyValue.ToTimeSpan();
            }
            else if (otherPropertyType == typeof(bool))
            {
                if (ComparisonType != ComparisonType.Equal && ComparisonType != ComparisonType.NotEqual)
                {
                    throw new Exception($"The comparison type with boolean is not supported in {nameof(RequiredIfAttribute)}. Use Equal or NotEqual.");
                }

                otherPropertyContextValueDynamic = Convert.ToBoolean(otherPropertyContextValue, CultureInfo.InvariantCulture);
                otherPropertyValueDynamic = Convert.ToBoolean(OtherPropertyValue, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception($"The type is not supported in {nameof(RequiredIfAttribute)}.");
            }

            // Do comaprison and do the required validation.
            if (ComparisonType == ComparisonType.Equal)
            {
                if (otherPropertyContextValueDynamic == otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }
            else if (ComparisonType == ComparisonType.NotEqual)
            {
                if (otherPropertyContextValueDynamic != otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }
            else if (ComparisonType == ComparisonType.GreaterThan)
            {
                if (otherPropertyContextValueDynamic > otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }
            else if (ComparisonType == ComparisonType.GreaterThanOrEqual)
            {
                if (otherPropertyContextValueDynamic >= otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }
            else if (ComparisonType == ComparisonType.SmallerThan)
            {
                if (otherPropertyContextValueDynamic < otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }
            else if (ComparisonType == ComparisonType.SmallerThanOrEqual)
            {
                if (otherPropertyContextValueDynamic <= otherPropertyValueDynamic)
                {
                    return IsRequired(value, validationContext);
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult IsRequired(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName);
                return new ValidationResult(formattedErrorMessage, new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
