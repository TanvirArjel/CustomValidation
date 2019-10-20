using AspNetCore.CustomValidation.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AspNetCore.CustomValidation.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CompareToAttribute : ValidationAttribute
    {
        public CompareToAttribute(string propertyName, CompareType compareType)
        {
            PropertyName = propertyName;
            CompareType = compareType;
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


            Type memberType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (memberType == null)
            {
                throw new ArgumentException($"The type of {validationContext.MemberName} is null.");
            }

            Type comparePropertyType = validationContext.ObjectType.GetProperty(PropertyName)?.PropertyType;

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
                else
                {
                    throw new ArgumentException($"The type of {validationContext.MemberName} is not comparable to type of {PropertyName}");
                }
            }

            ValidationResult TriggerValueComparison()
            {
                if (CompareType == CompareType.GreaterThan)
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

                if (CompareType == CompareType.SmallerThan)
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

                if (CompareType == CompareType.Equality)
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
        private CompareType CompareType { get; }
    }


    public enum CompareType
    {
        GreaterThan,
        SmallerThan,
        Equality
    }
}
