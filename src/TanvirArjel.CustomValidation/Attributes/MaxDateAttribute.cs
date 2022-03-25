// <copyright file="MaxDateAttribute.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace TanvirArjel.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="ValidationAttribute"/> is used to check whether the property value is smaller than the specified <see cref="MaxDate"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MaxDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxDateAttribute"/> class.
        /// This constructor takes the <see cref="MaxDate"/> value in <paramref name="year"/>, <paramref name="month"/> and <paramref name="day"/> format.
        /// </summary>
        /// <param name="year">A calendar year like 1988,2019 etc.</param>
        /// <param name="month">A calendar month number. The value should be in 1 to 12.</param>
        /// <param name="day">A calendar date. The value should be in 1 to 31.</param>
        public MaxDateAttribute(int year, int month, int day)
            : base("The {0} cannot be larger than {1}.")
        {
            MaxDate = new DateTime(year, month, day);
        }

        /// <summary>
        /// This constructor takes the <see cref="MaxDate"/> value in <see cref="string"/> with a specified <see cref="DateTime"/> format.
        /// </summary>
        /// <param name="maxDate">The <see cref="string"/> representation of the minDate value.</param>
        /// <param name="format">Format of the supplied string minDate value.</param>
        public MaxDateAttribute(string maxDate, string format)
            : base("The {0} cannot be larger than {1}.")
        {
            MaxDate = DateTime.ParseExact(maxDate, format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get the allowed max date value.
        /// </summary>
        public DateTime MaxDate { get; }

        /// <summary>
        /// Gets the format of the <see cref="MaxDate"/> that will be used in <see cref="FormatErrorMessage"/>
        /// </summary>
        public string ErrorMessageMaxDateFormat { get; set; } = "dd-MM-yyyy";

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxDate.ToString(ErrorMessageMaxDateFormat, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// To check whether the input date violates the specified max date constraint.
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
                throw new ArgumentException($"The {nameof(MaxDateAttribute)} is not valid on property type {propertyType}." +
                                            $" This Attribute is only valid on {typeof(DateTime)} and {typeof(DateTime?)}");
            }

            if (value != null)
            {
                DateTime inputDate = (DateTime)value;

                if (inputDate > MaxDate)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }
    }
}
