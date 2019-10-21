using AspNetCore.CustomValidation.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to compare the decorated property value against the another property value of the same object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompareToAttribute : ValidationAttribute
    {
        /// <summary>
        /// This constructor takes the <param name="propertyName"></param> and <param name="comparisonType"></param> values.
        /// </summary>
        /// <param name="propertyName">Name of the property which against the comparison will be done.</param>
        /// <param name="comparisonType">The <see cref="ComparisonType"/>.</param>
        public CompareToAttribute(string propertyName, ComparisonType comparisonType)
        {
            PropertyName = propertyName;
            ComparisonType = comparisonType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo compareProperty = validationContext.ObjectType.GetProperty(PropertyName);

            if (compareProperty == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{PropertyName}'");
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

            Type comparePropertyType = compareProperty.PropertyType;

            if (comparePropertyType == null)
            {
                throw new ArgumentNullException($"The type of {PropertyName} is null");
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
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not comparable to type of {PropertyName}");
                }
            }

            ValidationResult TriggerValueComparison()
            {
                if (ComparisonType == ComparisonType.GreaterThan)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) < Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {PropertyName}.");
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value < (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {PropertyName}.");
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length < comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {PropertyName}.");
                        }
                    }
                }

                if (ComparisonType == ComparisonType.SmallerThan)
                {
                    if (value.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) > Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be smaller than {PropertyName}.");
                        }
                    }

                    if (value.IsDateTime())
                    {
                        if ((DateTime)value > (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be smaller than {PropertyName}.");
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString().Length > comparePropertyValue.ToString().Length)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be smaller than {PropertyName}.");
                        }
                    }
                }

                if (ComparisonType == ComparisonType.Equality)
                {
                    if (value.IsNumber() && comparePropertyValue.IsNumber())
                    {
                        if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) != Convert.ToDecimal(comparePropertyValue, CultureInfo.InvariantCulture))
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be smaller than {PropertyName}.");
                        }
                    }

                    if (value is DateTime memberValue)
                    {
                        if (memberValue != (DateTime)comparePropertyValue)
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} is not equal to {PropertyName}.");
                        }
                    }

                    if (value is string)
                    {
                        if (value.ToString() != comparePropertyValue.ToString())
                        {
                            return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} is not equal to {PropertyName}.");
                        }
                    }
                    
                }

                return ValidationResult.Success;
            }
        }

        private string PropertyName { get; }
        private ComparisonType ComparisonType { get; }
    }


    public enum ComparisonType
    {
        GreaterThan,
        SmallerThan,
        Equality
    }
}
