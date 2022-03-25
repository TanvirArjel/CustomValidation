// <copyright file="MinAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

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
            : base("The {0} cannot be larger than {1}.")
        {
            MinAgeDateTime = DateTime.Today.AddYears(years < 0 ? 0 : -years).AddMonths(months < 0 ? 0 : -months).AddDays(days < 0 ? 0 : -days);
        }

        /// <summary>
        /// Get the allowed max date value.
        /// </summary>
        public DateTime MinAgeDateTime { get; }

        /// <summary>
        /// Gets the format of the <see cref="MinAgeDateTime"/> that will be used in <see cref="FormatErrorMessage"/>
        /// </summary>
        public string ErrorMessageMinAgeDateTimeFormat { get; set; } = "dd-MM-yyyy";

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinAgeDateTime.ToString(ErrorMessageMinAgeDateTimeFormat, CultureInfo.CurrentCulture));
        }

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

                //DateTime dateNow = DateTime.Now;
                //TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
                //DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

                if (MinAgeDateTime > dateOfBirth)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }
}
