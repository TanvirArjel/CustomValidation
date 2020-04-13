// <copyright file="MaxAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// max age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MaxAgeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAgeAttribute"/> class.
        /// This constructor takes the permitted max age value in <see cref="years"/>, <see cref="months"/> and <see cref="days"/> format.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> value.</param>
        /// <param name="months">A <see cref="int"/> value in between 0 and 11.</param>
        /// <param name="days">A <see cref="int"/> value in between 0 and 31.</param>
        public MaxAgeAttribute(int years, int months, int days)
        {
            this.Years = years < 0 ? 0 : years;
            this.Months = years < 0 ? 0 : months;
            this.Days = days < 0 ? 0 : days;

            this.ErrorMessage = this.ErrorMessage ?? $"Maximum age can be {(this.Years > 0 ? "{0}" + " years" : string.Empty)} {(this.Months > 0 ? "{1}" + " months" : string.Empty)} {(this.Days > 0 ? "{2}" + " days" : string.Empty)}";
        }

        public int Years { get; }

        public int Months { get; }

        public int Days { get; }

        public override string FormatErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, this.ErrorMessage, this.Years, this.Months, this.Days);
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

                var maxAgeDateTime = DateTime.MinValue.AddYears(this.Years).AddMonths(this.Months).AddDays(this.Days);

                if (this.Years > 0 || this.Months > 0 || this.Days > 0)
                {
                    if (ageDateTime > maxAgeDateTime)
                    {
                        return new ValidationResult(this.ErrorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
