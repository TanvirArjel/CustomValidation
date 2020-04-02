using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to check whether the property value is smaller than the specified <see cref="MinDate"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MinDateAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// This constructor takes the <see cref="MinDate"/> value in <paramref name="year"/>, <paramref name="month"/> and <paramref name="day"/> format.
        /// </summary>
        /// <param name="year">A calendar year like 1988,2019 etc,</param>
        /// <param name="month">A calendar month number. The value should be in 1 to 12.</param>
        /// <param name="day">A calendar date. The value should be in 1 to 31.</param>
        public MinDateAttribute(int year, int month, int day)
        {
            MinDate = new DateTime(year, month, day);
            ErrorMessage = ErrorMessage ?? "The {0} cannot be smaller than {1}.";
        }

        /// <summary>
        /// This constructor takes the <see cref="MinDate"/> value in <see cref="string"/> with a specified <see cref="DateTime"/> format.
        /// </summary>
        /// <param name="minDate">The <see cref="string"/> representation of the minDate value.</param>
        /// <param name="format">Format of the supplied string minDate value.</param>
        public MinDateAttribute(string minDate, string format)
        {
            MinDate = DateTime.ParseExact(minDate, format, CultureInfo.InvariantCulture);
        }

        public DateTime MinDate { get; }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();
            var errorMessage = FormatErrorMessage(propertyDisplayName);

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-valid-date-format", "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");
            AddAttribute(context.Attributes, "data-val-mindate", errorMessage);

            var minDate = MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-mindate-date", minDate);
        }

        public override string FormatErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, ErrorMessage, displayName, MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));
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
                throw new ArgumentException($"The {nameof(MinDateAttribute)} is not valid on property type {propertyType}." +
                                            $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}");
            }

            if (value != null)
            {
                var inputDate = (DateTime)value;

                if (inputDate < MinDate)
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
