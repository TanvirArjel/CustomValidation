using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AspNetCore.CustomValidation.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
    public class CompareToAttribute : ValidationAttribute, IClientModelValidator
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
        }

        private string EqualityErrorMessage => this.ErrorMessage ?? "The {0} is not equal to {1}.";

        private string NotEqualityErrorMessage => this.ErrorMessage ?? "The {0} can not be equal to {1}.";

        private string GreaterThanErrorMessage => this.ErrorMessage ?? "The {0} should be greater than {1}.";

        private string GreaterThanOrEqualErrorMessage => this.ErrorMessage ?? "The {0} should be greater than or equal {1}.";

        private string SmallerThanErrorMessage => this.ErrorMessage ?? "The {0} should be smaller than {1}.";

        private string SmallerThanOrEqualErrorMessage => this.ErrorMessage ?? "The {0} should be smaller than or equal {1}.";

        private string ComparePropertyName { get; }

        private ComparisonType ComparisonType { get; }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var propertyName = context.ModelMetadata.GetDisplayName();

            this.AddAttribute(context.Attributes, "data-val", "true");
            this.AddAttribute(context.Attributes, "data-val-input-type-compare", $"{propertyName} is not comparable to {ComparePropertyName}");
            this.AddAttribute(context.Attributes, "data-val-input-type-compare-property", ComparePropertyName);

            if (ComparisonType == ComparisonType.Equal)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-equal", GetFormattedErrorMessage(EqualityErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-equal-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.NotEqual)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-not-equal", GetFormattedErrorMessage(NotEqualityErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-not-equal-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.GreaterThan)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-greater-than", GetFormattedErrorMessage(GreaterThanErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-greater-than-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.GreaterThanOrEqual)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-greater-than-or-equal", GetFormattedErrorMessage(GreaterThanOrEqualErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-greater-than-or-equal-property", ComparePropertyName);
            }

            if (this.ComparisonType == ComparisonType.SmallerThan)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-smaller-than", GetFormattedErrorMessage(SmallerThanErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-smaller-than-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.SmallerThanOrEqual)
            {
                this.AddAttribute(context.Attributes, "data-val-comparison-smaller-than-or-equal", GetFormattedErrorMessage(SmallerThanOrEqualErrorMessage, propertyName, ComparePropertyName));
                this.AddAttribute(context.Attributes, "data-val-comparison-smaller-than-or-equal-property", ComparePropertyName);
            }
        }

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

                if (this.ComparisonType == ComparisonType.Equal)
                {
                    var errorMessage = this.GetFormattedErrorMessage(this.EqualityErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
                    var errorMessage = GetFormattedErrorMessage(NotEqualityErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
                    var errorMessage = GetFormattedErrorMessage(GreaterThanErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
                    var errorMessage = GetFormattedErrorMessage(GreaterThanOrEqualErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
                    var errorMessage = GetFormattedErrorMessage(SmallerThanErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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
                    var errorMessage = GetFormattedErrorMessage(SmallerThanOrEqualErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        private string GetFormattedErrorMessage(string errorMessage, string propertyName, string comparePropertyName)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName, comparePropertyName);
        }
    }
}
