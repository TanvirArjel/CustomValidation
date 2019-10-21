using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AspNetCore.CustomValidation.Attributes
{
    /// <summary>
    /// This <see cref="Attribute"/> is used to validate the date of birth value of a <see cref="DateTime"/> field against the specified
    /// max age value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxAgeAttribute : ValidationAttribute
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

            var dateOfBirth = (DateTime)value;

            if (dateOfBirth > DateTime.Now)
            {
                return new ValidationResult($"{validationContext.MemberName} can not be greater than today's date.");
            }

            var dateNow = DateTime.Now;
            TimeSpan timeSpan = dateNow.Subtract(dateOfBirth);
            DateTime ageDateTime = DateTime.MinValue.Add(timeSpan);

            var maxAgeDateTime = DateTime.MinValue.AddYears(Years).AddMonths(Months).AddDays(Days);

            var errorMessage = ErrorMessage ?? $"Maximum age can be {(Years > 0 ? Years + " years" : "")} {(Months > 0 ? Months + " months" : "")} {(Days > 0 ? Days + " days" : "")}.";

            ValidationResult validationResult = new ValidationResult(errorMessage);

            if (Years > 0 || Months > 0 || Days > 0)
            {
                if (ageDateTime > maxAgeDateTime)
                {
                    return validationResult;
                }
            }

            return ValidationResult.Success;
        }

        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
    }
}
