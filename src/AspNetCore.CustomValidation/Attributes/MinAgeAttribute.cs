using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// min age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MinAgeAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// This constructor takes the permitted min age value in <see cref="years"/>, <see cref="months"/> and <see cref="days"/> format.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> number.</param>
        /// <param name="months">A positive <see cref="int"/> value ranging from 0 to 11.</param>
        /// <param name="days">A positive <see cref="int"/> value ranging from 0 to 31.</param>
        public MinAgeAttribute(int years, int months, int days)
        {
            Years = years < 0 ? 0: years;
            Months = months < 0 ? 0 : months;
            Days = days < 0 ? 0 : days;

            ErrorMessage = ErrorMessage ?? $"Minimum age should be {(Years > 0 ? "{0}" + " years" : "")} {(Months > 0 ? "{1}" + " months" : "")} {(Days > 0 ? "{2}" + " days" : "")}";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (propertyType != null)
            {
                if (propertyType != typeof(DateTime) && propertyType != typeof(DateTime?))
                {
                    throw new ArgumentException($"The {nameof(MinAgeAttribute)} is not valid on property type {propertyType}." +
                                                $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}.");
                }
            }
            
            var dateOfBirth = (DateTime) value;

            if (dateOfBirth > DateTime.Now)
            {
                return new ValidationResult($"{validationContext.DisplayName} can not be greater than today's date.");
            }

            var dateNow = DateTime.Now;
            TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
            DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

            var minAgeDateTime = DateTime.MinValue.AddYears(Years).AddMonths(Months).AddDays(Days);


            if (Years > 0 || Months > 0 || Days > 0 )
            {
                if (minAgeDateTime > ageDateTime)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }


        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
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
            AddAttribute(context.Attributes, "data-val-currenttime", $"{propertyDisplayName} can not be greater than today's date.");

            AddAttribute(context.Attributes, "data-val-minage", errorMessage);

            var years = Years.ToString(CultureInfo.InvariantCulture);
            var months = Months.ToString(CultureInfo.InvariantCulture);
            var days = Days.ToString(CultureInfo.InvariantCulture);

            AddAttribute(context.Attributes, "data-val-minage-years", years);
            AddAttribute(context.Attributes, "data-val-minage-months", months);
            AddAttribute(context.Attributes, "data-val-minage-days", days);
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        public override string FormatErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, ErrorMessage, Years, Months, Days);
        }
    }
}
