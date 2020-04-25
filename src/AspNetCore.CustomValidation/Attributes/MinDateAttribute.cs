// <copyright file="MinDateAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to check whether the property value is smaller than the specified <see cref="MinDate"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MinDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinDateAttribute"/> class.
        /// This constructor takes the <see cref="MinDate"/> value in <paramref name="year"/>, <paramref name="month"/> and <paramref name="day"/> format.
        /// </summary>
        /// <param name="year">A calendar year like 1988,2019 etc.</param>
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

        /// <summary>
        /// Get the allowed min date value.
        /// </summary>
        public DateTime MinDate { get; }

        ////public override string FormatErrorMessage(string displayName)
        ////{
        ////    return string.Format(CultureInfo.InvariantCulture, ErrorMessage, displayName, MinDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture));
        ////}

        /// <summary>
        /// To check whether the input date violates the specified min date constraint.
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

            Type propertyType = propertyInfo.PropertyType;

            if (propertyType != typeof(DateTime) && propertyType != typeof(DateTime?))
            {
                throw new ArgumentException($"The {nameof(MinDateAttribute)} is not valid on property type {propertyType}." +
                                            $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}");
            }

            if (value != null)
            {
                DateTime inputDate = (DateTime)value;

                if (inputDate < MinDate)
                {
                    string errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
