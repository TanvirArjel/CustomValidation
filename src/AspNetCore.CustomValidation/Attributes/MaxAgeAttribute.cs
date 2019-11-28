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
    /// max age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxAgeAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// This constructor takes the permitted max age value in <see cref="years"/>, <see cref="months"/> and <see cref="days"/> format.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> value.</param>
        /// <param name="months">A <see cref="int"/> value in between 0 and 11.</param>
        /// <param name="days">A <see cref="int"/> value in between 0 and 31.</param>
        public MaxAgeAttribute(int years, int months, int days)
        {
            Years = years < 0 ? 0 : years;
            Months = years < 0 ? 0 : months;
            Days = days < 0 ? 0 : days;

            ErrorMessage = ErrorMessage ?? $"Maximum age can be {(Years > 0 ? "{0}" + " years" : "")} {(Months > 0 ? "{1}" + " months" : "")} {(Days > 0 ? "{2}" + " days" : "")}";
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

            if (propertyInfo.PropertyType != typeof(DateTime) && propertyInfo.PropertyType != typeof(DateTime?))
            {
                throw new ArgumentException($"The {nameof(MaxAgeAttribute)} is not valid on property type {propertyInfo.PropertyType}." +
                                            $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}.");
            }

            if (value != null)
            {
                var dateOfBirth = (DateTime)value;

                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult($"{validationContext.DisplayName} can not be greater than today's date.");
                }

                var dateNow = DateTime.Now;
                TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
                DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

                var maxAgeDateTime = DateTime.MinValue.AddYears(Years).AddMonths(Months).AddDays(Days);

                if (Years > 0 || Months > 0 || Days > 0)
                {
                    if (ageDateTime > maxAgeDateTime)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
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

            AddAttribute(context.Attributes,"data-val-valid-date-format", "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");
            AddAttribute(context.Attributes, "data-val-currenttime", $"{propertyDisplayName} can not be greater than today's date.");
            AddAttribute(context.Attributes, "data-val-maxage", errorMessage);

            var years = Years.ToString(CultureInfo.InvariantCulture);
            var months = Months.ToString(CultureInfo.InvariantCulture);
            var days = Days.ToString(CultureInfo.InvariantCulture);

            AddAttribute(context.Attributes, "data-val-maxage-years", years);
            AddAttribute(context.Attributes, "data-val-maxage-months", months);
            AddAttribute(context.Attributes, "data-val-maxage-days", days);
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
            return string.Format(CultureInfo.InvariantCulture,ErrorMessage, Years, Months, Days);
        }
    }
}
