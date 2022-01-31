using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.MyCustomAttributes.Adapters
{
    public class MyCompareToAttributeAdapter : AttributeAdapterBase<CompareToAttribute>
    {
        public MyCompareToAttributeAdapter(CompareToAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            throw new System.NotImplementedException();
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();
            string comparePropertyName = Attribute.ComparePropertyName;
            ComparisonType comparisonType = Attribute.ComparisonType;
            AddAttribute(context.Attributes, "my-data-val-input-type-compare-property", comparePropertyName);
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
