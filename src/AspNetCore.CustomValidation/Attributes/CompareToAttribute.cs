using AspNetCore.CustomValidation.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to compare the decorated property value against the another property value of the same object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompareToAttribute : ValidationAttribute, IClientModelValidator
    {
        private string ComparePropertyName { get; }
        private ComparisonType ComparisonType { get; }

        /// <summary>
        /// This constructor takes the <param name="comparePropertyName"></param> and <param name="comparisonType"></param> values.
        /// </summary>
        /// <param name="comparePropertyName">Name of the property which against the comparison will be done.</param>
        /// <param name="comparisonType">The <see cref="ComparisonType"/>.</param>
        public CompareToAttribute(string comparePropertyName, ComparisonType comparisonType)
        {
            ComparePropertyName = comparePropertyName;
            ComparisonType = comparisonType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo compareProperty = validationContext.ObjectType.GetProperty(ComparePropertyName);

            if (compareProperty == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{ComparePropertyName}'");
            }

            Type comparePropertyType = compareProperty.PropertyType;

            if (comparePropertyType == null)
            {
                throw new ArgumentNullException($"The type of {ComparePropertyName} is null");
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
                else
                {
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not comparable to type of {ComparePropertyName}");
                }
            }

            ValidationResult TriggerValueComparison()
            {
                var propertyDisplayName = validationContext.DisplayName;
                var comparePropertyDisplayAttribute = compareProperty.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                var comparePropertyDisplayName = comparePropertyDisplayAttribute?.GetName() ?? ComparePropertyName;

                if (ComparisonType == ComparisonType.Equality)
                {
                    var errorMessage = GetFormattedErrorMessage(EqualityErrorMessage, propertyDisplayName, comparePropertyDisplayName);

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

                    if (value is string)
                    {
                        if (value.ToString() != comparePropertyValue.ToString())
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

        private string EqualityErrorMessage => ErrorMessage ?? "The {0} is not equal to {1}.";
        private string GreaterThanErrorMessage => ErrorMessage ?? "The {0} should be greater than {1}.";
        private string SmallerThanErrorMessage => ErrorMessage ?? "The {0} should be smaller than {1}.";

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var propertyName = context.ModelMetadata.GetDisplayName();

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-input-type-compare", $"{propertyName} is not comparable to {ComparePropertyName}");
            AddAttribute(context.Attributes, "data-val-input-type-compare-property", ComparePropertyName);

            if (ComparisonType == ComparisonType.Equality)
            {
                AddAttribute(context.Attributes, "data-val-comparison-equality", GetFormattedErrorMessage(EqualityErrorMessage, propertyName, ComparePropertyName));
                AddAttribute(context.Attributes, "data-val-comparison-equality-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.GreaterThan)
            {
                AddAttribute(context.Attributes, "data-val-comparison-greater", GetFormattedErrorMessage(GreaterThanErrorMessage, propertyName, ComparePropertyName));
                AddAttribute(context.Attributes, "data-val-comparison-greater-property", ComparePropertyName);
            }

            if (ComparisonType == ComparisonType.SmallerThan)
            {
                AddAttribute(context.Attributes, "data-val-comparison-smaller", GetFormattedErrorMessage(SmallerThanErrorMessage, propertyName, ComparePropertyName));
                AddAttribute(context.Attributes, "data-val-comparison-smaller-property", ComparePropertyName);
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


    public enum ComparisonType
    {
        Equality,
        GreaterThan,
        SmallerThan
    }
}
