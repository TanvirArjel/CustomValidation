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
    /// <summary>
    /// Contains possible comparison types for <see cref="CompareToAttribute"/>.
    /// </summary>
    public enum ComparisonType
    {
        /// <summary>
        /// Use to check equality.
        /// </summary>
        Equal,

        /// <summary>
        /// Use to check not equality.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Use to check greater than.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Use to check greater than or equality.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Use to check smaller than.
        /// </summary>
        SmallerThan,

        /// <summary>
        /// Use to check smaller than or equal.
        /// </summary>
        SmallerThanOrEqual
    }

    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to compare the decorated property value against the another property value of the same object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CompareToAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareToAttribute"/> class.
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

        /// <summary>
        /// Name of the property which against the comparison will be done.
        /// </summary>
        public string ComparePropertyName { get; }

        /// <summary>
        /// The comparison type. For example: GreaterThan, GreaterThanOrEqual etc.
        /// </summary>
        public ComparisonType ComparisonType { get; }

        /// <summary>
        /// Perform the compare to validation check and returns the <see cref="ValidationResult"/>.
        /// </summary>
        /// <param name="value">Value of the input field.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>Retuns <see cref="ValidationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="validationContext"/> is null.</exception>
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

            object comparePropertyValue = compareProperty.GetValue(validationContext.ObjectInstance, null);

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
                ValidationResult validationResult = TriggerValueComparison();
                return validationResult;
            }
            else
            {
                if (value.IsNumber() && comparePropertyValue.IsNumber())
                {
                    ValidationResult validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else if (value is DateTime && comparePropertyValue is DateTime)
                {
                    ValidationResult validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else if (value is TimeSpan && comparePropertyValue is TimeSpan)
                {
                    ValidationResult validationResult = TriggerValueComparison();
                    return validationResult;
                }
                else
                {
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not comparable to type of {ComparePropertyName}");
                }
            }

            ValidationResult TriggerValueComparison()
            {
                string propertyDisplayName = validationContext.DisplayName;
                DisplayAttribute comparePropertyDisplayAttribute = compareProperty.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                string comparePropertyDisplayName = comparePropertyDisplayAttribute?.GetName() ?? this.ComparePropertyName;

                string errorMessage = this.GetFormattedErrorMessage(this.ErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
