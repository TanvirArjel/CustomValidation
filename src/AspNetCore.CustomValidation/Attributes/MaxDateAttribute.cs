using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to check whether the property value is smaller than the specified <see cref="MaxDate"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// This constructor takes the <see cref="MaxDate"/> value in <paramref name="year"/>, <paramref name="month"/> and <paramref name="day"/> format.
        /// </summary>
        /// <param name="year">A calendar year like 1988,2019 etc,</param>
        /// <param name="month">A calendar month number. The value should be in 1 to 12.</param>
        /// <param name="day">A calendar date. The value should be in 1 to 31.</param>
        public MaxDateAttribute(int year, int month, int day)
        {
            MaxDate = new DateTime(year, month, day);
        }

        /// <summary>
        /// This constructor takes the <see cref="MaxDate"/> value in <see cref="string"/> with a specified <see cref="DateTime"/> format.
        /// </summary>
        /// <param name="maxDate">The <see cref="string"/> representation of the minDate value.</param>
        /// <param name="format">Format of the supplied string minDate value.</param>
        public MaxDateAttribute(string maxDate, string format)
        {
            MaxDate = DateTime.ParseExact(maxDate, format, CultureInfo.InvariantCulture);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"The object does not contain any property with name '{validationContext.MemberName}'");
            }

            Type propertyType = propertyInfo.PropertyType;

            if (propertyType != typeof(DateTime) && propertyType != typeof(DateTime?))
            {
                throw new ArgumentException($"The {nameof(MaxDateAttribute)} is not valid on property type {propertyType}." +
                                            $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}");
            }

            if (value == null)
            {
                return ValidationResult.Success;
            }

            var inputDate = (DateTime)value;

            if (inputDate > MaxDate)
            {
                var errorMessage = ErrorMessage ?? $"The {validationContext.MemberName} cannot be larger than {MaxDate:dd-MMM-yyyy}.";
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        public DateTime MaxDate { get; }
    }
}
