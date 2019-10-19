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
        /// <param name="fieldName">A property name of the same object.</param>
        public GreaterThanAttribute(string fieldName)
        {
            OtherFieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            Type propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (propertyType == null)
            {
                throw new ArgumentException($"The type of {validationContext.MemberName} is null.");
            }

            Type otherPropertyType = validationContext.ObjectType.GetProperty(OtherFieldName)?.PropertyType;

            if (otherPropertyType == null)
            {
                throw new ArgumentNullException($"The type of {OtherFieldName} is null");
            }

            if (propertyType == otherPropertyType)
            {
                var validationResult = TriggerValueComparison();
                return validationResult;
            }
            else
            {
                try
                {
                    object changeType = Convert.ChangeType(value, otherPropertyType, CultureInfo.InvariantCulture);

                    var validationResult = TriggerValueComparison();
                    return validationResult;
                }
                catch (Exception)
                {
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not convertible to type of {OtherFieldName}");
                }

            }

            ValidationResult TriggerValueComparison()
            {
                var otherPropertyValue = validationContext.ObjectType.GetProperty(OtherFieldName)?.GetValue(validationContext.ObjectInstance, null);

                if (otherPropertyValue == null)
                {
                    return ValidationResult.Success;
                }

                if (value.IsNumber())
                {
                    if (Convert.ToDecimal(value, CultureInfo.InvariantCulture) < Convert.ToDecimal(otherPropertyValue, CultureInfo.InvariantCulture))
                    {
                        return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {OtherFieldName}.");
                    }
                }

                if (value.IsDateTime())
                {
                    if ((DateTime)value < (DateTime)otherPropertyValue)
                    {
                        return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {OtherFieldName}.");
                    }
                }

                if (value is string)
                {
                    if (value.ToString().Length < otherPropertyValue.ToString().Length)
                    {
                        return new ValidationResult(ErrorMessage ?? $"The {validationContext.MemberName} should be greater than {OtherFieldName}.");
                    }
                }

                return ValidationResult.Success;
            }

        }

        private string OtherFieldName { get; set; }


    }
}
