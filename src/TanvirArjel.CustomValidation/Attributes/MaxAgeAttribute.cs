﻿// <copyright file="MaxAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// max age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MaxAgeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAgeAttribute"/> class.
        /// </summary>
        /// <param name="years">A positive <see cref="int"/> value.</param>
        /// <param name="months">A <see cref="int"/> value in between 0 and 11.</param>
        /// <param name="days">A <see cref="int"/> value in between 0 and 31.</param>
        public MaxAgeAttribute(int years, int months, int days)
        {
            Years = years < 0 ? 0 : years;
            Months = years < 0 ? 0 : months;
            Days = days < 0 ? 0 : days;

            ErrorMessage = ErrorMessage ?? $"The Maximum age can be {(Years > 0 ? years + " years" : string.Empty)} {(Months > 0 ? months + " months" : string.Empty)} {(Days > 0 ? days + " days." : string.Empty)}";
        }

        /// <summary>
        /// Get the year value of the max allowed age.
        /// </summary>
        public int Years { get; }

        /// <summary>
        /// Get the month value of the max allowed age.
        /// </summary>
        public int Months { get; }

        /// <summary>
        /// Get the day value of the max allowed age.
        /// </summary>
        public int Days { get; }

        ////public override string FormatErrorMessage(string displayName)
        ////{
        ////    return string.Format(CultureInfo.InvariantCulture, this.ErrorMessage, this.Years, this.Months, this.Days);
        ////}

        /// <summary>
        /// To check whether the input date violates the specified max age constraint.
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
                DateTime dateOfBirth = (DateTime)value;

                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult($"{validationContext.DisplayName} can not be greater than today's date.");
                }

                DateTime dateNow = DateTime.Now;
                TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
                DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

                DateTime maxAgeDateTime = DateTime.MinValue.AddYears(Years).AddMonths(Months).AddDays(Days);

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
    }
}
