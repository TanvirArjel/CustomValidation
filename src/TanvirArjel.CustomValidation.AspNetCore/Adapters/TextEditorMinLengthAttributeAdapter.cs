using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.Adapters
{
    internal class TextEditorMinLengthAttributeAdapter : AttributeAdapterBase<TextEditorMinLengthAttribute>
    {
        public TextEditorMinLengthAttributeAdapter(TextEditorMinLengthAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            AddAttribute(context.Attributes, "data-val", "true");

            if (Attribute.MinLength > 0)
            {
                AddAttribute(context.Attributes, "data-val-texteditor-minlength", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-texteditor-minlength-value", Attribute.MinLength.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();

            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, Attribute.MinLength);
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
