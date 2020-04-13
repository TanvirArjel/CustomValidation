// <copyright file="MinAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// min age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MinAgeAttribute : ValidationAttribute
    {
        /// <summary>
        /// This constructor takes the permitted min age value in <see cref="years"/>, <see cref="months"/> and <see cref="days"/> format.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> number.</param>
        /// <param name="months">A positive <see cref="int"/> value ranging from 0 to 11.</param>
        /// <param name="days">A positive <see cref="int"/> value ranging from 0 to 31.</param>
        public MinAgeAttribute(int years, int months, int days)
        {
            Years = years < 0 ? 0 : years;
            Months = months < 0 ? 0 : months;
            Days = days < 0 ? 0 : days;

            ErrorMessage = ErrorMessage ?? $"Minimum age should be {(Years > 0 ? "{0}" + " years" : string.Empty)} {(Months > 0 ? "{1}" + " months" : string.Empty)} {(Days > 0 ? "{2}" + " days" : string.Empty)}";
        }

        public int Years { get; }

        public int Months { get; }

        public int Days { get; }

        public override string FormatErrorMessage(string displayName)
        {
            return string.Format(CultureInfo.InvariantCulture, ErrorMessage, Years, Months, Days);
        }

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
