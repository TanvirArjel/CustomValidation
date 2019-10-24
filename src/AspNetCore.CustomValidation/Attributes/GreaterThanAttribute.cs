using AspNetCore.CustomValidation.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to check whether the value of the property is greater than the value of the specified property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    [Obsolete("This attribute has been obsolete and will be removed in future version. Use CompareToAttribute instead.", true)]
    public class GreaterThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// This constructor takes the name of field or property which against the comparison will be done.
        /// </summary>
        /// <param name="propertyName">A property name of the same object.</param>
        public GreaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
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

            Type propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (propertyType == null)
            {
                throw new ArgumentException($"The type of {validationContext.MemberName} is null.");
            }

            Type comparePropertyType = compareProperty.PropertyType;

            if (comparePropertyType == null)
            {
                throw new ArgumentNullException($"The type of {PropertyName} is null");
            }

            if (propertyType == comparePropertyType)
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

                return ValidationResult.Success;
            }

        }

        private string PropertyName { get;}


    }
}
