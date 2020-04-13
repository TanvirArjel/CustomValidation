// <copyright file="FixedLengthAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using AspNetCore.CustomValidation.Attributes;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace AspNetCore.CustomValidation.Adapters
{
    internal class FixedLengthAttributeAdapter : AttributeAdapterBase<FixedLengthAttribute>
    {
        public FixedLengthAttributeAdapter(FixedLengthAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string errorMessage = GetErrorMessage(context);

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-fixed-length", errorMessage);

            string fixedLengthValue = Attribute.FixedLength.ToString(CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-fixed-length-value", fixedLengthValue);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return GetErrorMessage(validationContext.ModelMetadata, validationContext.ModelMetadata.GetDisplayName(), Attribute.FixedLength);
        }

        private void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
