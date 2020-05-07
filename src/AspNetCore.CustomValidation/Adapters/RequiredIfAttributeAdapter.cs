// <copyright file="RequiredIfAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.CustomValidation.Adapters
{
    internal class RequiredIfAttributeAdapter : AttributeAdapterBase<RequiredIfAttribute>
    {
        public RequiredIfAttributeAdapter(RequiredIfAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-requiredif-other-property", Attribute.OtherPropertyName);
            AddAttribute(context.Attributes, "data-val-requiredif-other-property-value", Attribute.OtherPropertyValue.ToString());

            string errorMessage = GetErrorMessage(context);
            AddAttribute(context.Attributes, "data-val-requiredif", errorMessage);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();
            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName);
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
