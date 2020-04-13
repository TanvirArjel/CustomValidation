// <copyright file="MaxDateAttributeAdapter.cs" company="TanvirArjel">
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
    internal class MaxDateAttributeAdapter : AttributeAdapterBase<MaxDateAttribute>
    {
        public MaxDateAttributeAdapter(MaxDateAttribute attribute, IStringLocalizer stringLocalizer)
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
            AddAttribute(context.Attributes, "data-val-valid-date-format", "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");
            AddAttribute(context.Attributes, "data-val-maxdate", errorMessage);
            var maxDate = Attribute.MaxDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-maxdate-date", maxDate);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            var propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();
            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, Attribute.MaxDate);
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
