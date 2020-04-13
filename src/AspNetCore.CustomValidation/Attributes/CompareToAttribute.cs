// <copyright file="CompareToAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AspNetCore.CustomValidation.Extensions;

namespace AspNetCore.CustomValidation.Attributes
{
    public enum ComparisonType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        SmallerThan,
        SmallerThanOrEqual
    }

    /// <summary>
    /// This <see cref="Attribute"/> is used to compare the decorated property value against the another property value of the same object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CompareToAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareToAttribute"/> class.
        /// This constructor takes the <param name="comparePropertyName"></param> and <param name="comparisonType"></param> values.
        /// </summary>
        /// <param name="comparePropertyName">Name of the property which against the comparison will be done.</param>
        /// <param name="comparisonType">The <see cref="ComparisonType"/>.</param>
        public CompareToAttribute(string comparePropertyName, ComparisonType comparisonType)
        {
            this.ComparePropertyName = comparePropertyName;
            this.ComparisonType = comparisonType;

            if (ErrorMessage == null)
            {
                SetErrorMessage(comparisonType);
            }
        }

        public string ComparePropertyName { get; }

        public ComparisonType ComparisonType { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo compareProperty = validationContext.ObjectType.GetProperty(this.ComparePropertyName);

            if (compareProperty == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{this.ComparePropertyName}'");
            }

            Type comparePropertyType = compareProperty.PropertyType;

            if (comparePropertyType == null)
            {
                throw new ArgumentNullException($"The type of {this.ComparePropertyName} is null");
            }

            var comparePropertyValue = compareProperty.GetValue(validationContext.ObjectInstance, null);

            if (value == null || comparePropertyValue == null)
            {
                return ValidationResult.Success;
            }

            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{validationContext.MemberName}'");
            }

            Type memberType = propertyInfo.PropertyType;

            if (memberType == null)
            {
                throw new ArgumentException($"The type of {validationContext.MemberName} is null.");
            }

            if (memberType == comparePropertyType)
            {
                var validationResult = TriggerValueComparison();
                return validationResult;
            }
            else
            {
                if (value.IsNumber() && comparePropertyValue.IsNumber())
                {
                    var validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else if (value is DateTime && comparePropertyValue is DateTime)
                {
                    var validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else if (value is TimeSpan && comparePropertyValue is TimeSpan)
                {
                    var validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else
                {
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not comparable to type of {ComparePropertyName}");
                }
            }

            ValidationResult TriggerValueComparison()
            {
                var propertyDisplayName = validationContext.DisplayName;
                var comparePropertyDisplayAttribute = compareProperty.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                var comparePropertyDisplayName = comparePropertyDisplayAttribute?.GetName() ?? this.ComparePropertyName;

                var errorMessage = this.GetFormattedErrorMessage(this.ErrorMessage, propertyDisplayName, comparePropertyDisplayName);

                if (this.ComparisonType == ComparisonType.Equal)
                {
                    if (value.IsNumber() && comparePropertyValue.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) != Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is DateTime memberValue)
                    {
                        if (memberValue != (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is TimeSpan timeSpanValue)
                    {
                        if (timeSpanValue != (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString() != comparePropertyValue.ToString())
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                if (this.ComparisonType == ComparisonType.NotEqual)
                {
                    if (value.IsNumber() && comparePropertyValue.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) == Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is DateTime memberValue)
                    {
                        if (memberValue == (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is TimeSpan timeSpanValue)
                    {
                        if (timeSpanValue == (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString() == comparePropertyValue.ToString())
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                if (ComparisonType == ComparisonType.GreaterThan)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) <= Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value <= (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsTimeSpan())
                    {
                        if ((TimeSpan)value <= (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length <= comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                if (ComparisonType == ComparisonType.GreaterThanOrEqual)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) < Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value < (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsTimeSpan())
                    {
                        if ((TimeSpan)value < (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length < comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                if (ComparisonType == ComparisonType.SmallerThan)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) >= Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value >= (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsTimeSpan())
                    {
                        if ((TimeSpan)value >= (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length >= comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                if (ComparisonType == ComparisonType.SmallerThanOrEqual)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) > Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value > (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value.IsTimeSpan())
                    {
                        if ((TimeSpan)value > (TimeSpan)comparePropertyValue)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length > comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(errorMessage);
                        }
                    }
                }

                return ValidationResult.Success;
            }
        }

        private string GetFormattedErrorMessage(string errorMessage, string propertyName, string comparePropertyName)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, comparePropertyName);
        }

        private void SetErrorMessage(ComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    ErrorMessage = "{0} is not equal to {1}.";
                    break;
                case ComparisonType.NotEqual:
                    ErrorMessage = "{0} can not be equal to {1}.";
                    break;
                case ComparisonType.GreaterThan:
                    ErrorMessage = "{0} should be greater than {1}.";
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    ErrorMessage = "{0} should be greater than or equal {1}.";
                    break;
                case ComparisonType.SmallerThan:
                    ErrorMessage = "{0} should be smaller than {1}.";
                    break;
                case ComparisonType.SmallerThanOrEqual:
                    ErrorMessage = "{0} should be smaller than or equal {1}.";
                    break;
                default:
                    throw new ArgumentNullException(nameof(comparisonType));
            }
        }
    }
}
