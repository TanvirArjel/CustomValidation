// <copyright file="MaxAgeAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
            : base("The {0} cannot be smaller than {1}.")
        {
            MaxAgeDateTime = DateTime.Today.AddYears(years < 0 ? 0 : -years).AddMonths(months < 0 ? 0 : -months).AddDays(days < 0 ? 0 : -days);
        }

        /// <summary>
        /// Get the allowed min date value.
        /// </summary>
        public DateTime MaxAgeDateTime { get; }

        /// <summary>
        /// Gets the format of the <see cref="MaxAgeDateTime"/> that will be used in <see cref="FormatErrorMessage"/>
        /// </summary>
        public string ErrorMessageMaxAgeDateTimeFormat { get; set; } = "dd-MM-yyyy";

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxAgeDateTime.ToString(ErrorMessageMaxAgeDateTimeFormat, CultureInfo.CurrentCulture));
        }

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

                //DateTime dateNow = DateTime.Now;
                //TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
                //DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

                if (dateOfBirth > MaxAgeDateTime)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }
}
