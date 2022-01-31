using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace AspNetCore.MyCustomAttributes.Adapters
{
    public class FooAtrributeAdapter : AttributeAdapterBase<FooAtrribute>
    {
        public FooAtrributeAdapter(FooAtrribute atrribute, IStringLocalizer stringLocalizer)
            : base(atrribute, stringLocalizer)
        {
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return GetErrorMessage(validationContext.ModelMetadata);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            AddAttribute(context.Attributes, "data-val-foo", "This is foo error message");
        }

        private static void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
