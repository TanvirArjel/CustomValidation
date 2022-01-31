// <copyright file="MinAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// min age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MinAgeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinAgeAttribute"/> class.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> number.</param>
        /// <param name="months">A positive <see cref="int"/> value ranging from 0 to 11.</param>
        /// <param name="days">A positive <see cref="int"/> value ranging from 0 to 31.</param>
        public MinAgeAttribute(int years, int months, int days)
        {
            Years = years < 0 ? 0 : years;
            Months = months < 0 ? 0 : months;
            Days = days < 0 ? 0 : days;

            ErrorMessage = ErrorMessage ?? $"The Minimum age should be {(Years > 0 ? years + " years" : string.Empty)} {(Months > 0 ? months + " months" : string.Empty)} {(Days > 0 ? days + " days." : string.Empty)}";
        }

        /// <summary>
        /// Get the year value of the allowed min age.
        /// </summary>
        public int Years { get; }

        /// <summary>
        /// Get the month value of the allowed min age.
        /// </summary>
        public int Months { get; }

        /// <summary>
        /// Get the day value of the allowed min age.
        /// </summary>
        public int Days { get; }

        ////public override string FormatErrorMessage(string displayName)
        ////{
        ////    return string.Format(CultureInfo.InvariantCulture, ErrorMessage, Years, Months, Days);
        ////}

        /// <summary>
        /// To check whether the input date violates the specified min age constraint.
        /// </summary>
        /// <param name="value">Type of <see cref="DateTime"/>.</param>
        /// <param name="validationContext">The request validation context.</param>
        /// <returns>Returns <see cref="ValidationResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="validationContext"/> is null.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            Type propertyType = validationContext.ObjectType.GetProperty(validationContext.MemberName)?.PropertyType;

            if (propertyType != null)
            {
                if (propertyType != typeof(DateTime) && propertyType != typeof(DateTime?))
                {
                    throw new ArgumentException($"The {nameof(MinAgeAttribute)} is not valid on property type {propertyType}." +
                                                $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}.");
                }
            }

            if (value != null)
            {
                DateTime dateOfBirth = (DateTime)value;

                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult($"{validationContext.DisplayName} can not be greater than today's date.");
                }

                DateTime dateNow = DateTime.Now;
                TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
                DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

                DateTime minAgeDateTime = DateTime.MinValue.AddYears(Years).AddMonths(Months).AddDays(Days);

                if (Years > 0 || Months > 0 || Days > 0)
                {
                    if (minAgeDateTime > ageDateTime)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
