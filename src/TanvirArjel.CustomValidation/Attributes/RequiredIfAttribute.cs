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
    [AttributeUsage(AttributeTargets.Property)]
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

            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);

            if (otherPropertyInfo == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{OtherPropertyName}'");
            }

            Type otherPropertyType = otherPropertyInfo.PropertyType;

            object otherPropertyContextValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (otherPropertyContextValue == null)
            {
                return ValidationResult.Success;
            }

            if (ComparisonType == ComparisonType.Equal)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue == otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().ToUpperInvariant() == OtherPropertyValue.ToString().ToUpperInvariant())
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) == Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue == (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }
            else if (ComparisonType == ComparisonType.NotEqual)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue != otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().ToUpperInvariant() != OtherPropertyValue.ToString().ToUpperInvariant())
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) != Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue != (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }
            else if (ComparisonType == ComparisonType.GreaterThan)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue > otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().Length > OtherPropertyValue.ToString().Length)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) > Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue > (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }
            else if (ComparisonType == ComparisonType.GreaterThanOrEqual)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue >= otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().Length >= OtherPropertyValue.ToString().Length)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) >= Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue >= (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }
            else if (ComparisonType == ComparisonType.SmallerThan)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue < otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().Length < OtherPropertyValue.ToString().Length)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) < Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue < (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }
            else if (ComparisonType == ComparisonType.SmallerThanOrEqual)
            {
                if (otherPropertyType == typeof(DateTime) || otherPropertyType == typeof(DateTime?))
                {
                    DateTime otherPropertyValue = DateTime.Parse(OtherPropertyValue.ToString(), CultureInfo.InvariantCulture);
                    if ((DateTime)otherPropertyContextValue <= otherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(string))
                {
                    if (otherPropertyContextValue.ToString().Length <= OtherPropertyValue.ToString().Length)
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType.IsNumericType())
                {
                    if (Convert.ToDecimal(otherPropertyContextValue, CultureInfo.InvariantCulture) <= Convert.ToDecimal(OtherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return IsRequired(value, validationContext);
                    }
                }

                if (otherPropertyType == typeof(TimeSpan) || otherPropertyType == typeof(TimeSpan?))
                {
                    if ((TimeSpan)otherPropertyContextValue <= (TimeSpan)OtherPropertyValue)
                    {
                        return IsRequired(value, validationContext);
                    }
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult IsRequired(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                string formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, ErrorMessage, validationContext.DisplayName);
                return new ValidationResult(formattedErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
