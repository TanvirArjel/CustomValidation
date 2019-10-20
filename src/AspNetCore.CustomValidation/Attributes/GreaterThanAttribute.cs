using AspNetCore.CustomValidation.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to check whether the value of the property is greater than the value of the specified property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
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

            var comparePropertyValue = validationContext.ObjectType.GetProperty(PropertyName)?.GetValue(validationContext.ObjectInstance, null);

            if (value == null || comparePropertyValue == null)
            {
                return ValidationResult.Success;
            }


            Type propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (propertyType == null)
            {
                throw new ArgumentException($"The type of {validationContext.MemberName} is null.");
            }

            Type otherPropertyType = validationContext.ObjectType.GetProperty(PropertyName)?.PropertyType;

            if (otherPropertyType == null)
            {
                throw new ArgumentNullException($"The type of {PropertyName} is null");
            }

            if (propertyType == otherPropertyType)
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
