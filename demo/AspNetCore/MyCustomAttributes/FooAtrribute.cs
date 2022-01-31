using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.MyCustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class FooAtrribute : ValidationAttribute
    {
        public FooAtrribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return new ValidationResult($"{validationContext.DisplayName} is invalid");
        }
    }
}
