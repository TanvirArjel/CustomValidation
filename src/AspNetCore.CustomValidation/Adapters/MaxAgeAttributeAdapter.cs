// <copyright file="MaxAgeAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.CustomValidation.Adapters
{
    internal class MaxAgeAttributeAdapter : AttributeAdapterBase<MaxAgeAttribute>
    {
        public MaxAgeAttributeAdapter(MaxAgeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();
            string errorMessage = GetErrorMessage(context);

            AddAttribute(context.Attributes, "data-val", "true");

            AddAttribute(context.Attributes, "data-val-valid-date-format", "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");
            AddAttribute(context.Attributes, "data-val-currenttime", $"{propertyDisplayName} can not be greater than today's date.");
            AddAttribute(context.Attributes, "data-val-maxage", errorMessage);

            string years = Attribute.Years.ToString(CultureInfo.InvariantCulture);
            string months = Attribute.Months.ToString(CultureInfo.InvariantCulture);
            string days = Attribute.Days.ToString(CultureInfo.InvariantCulture);

            AddAttribute(context.Attributes, "data-val-maxage-years", years);
            AddAttribute(context.Attributes, "data-val-maxage-months", months);
            AddAttribute(context.Attributes, "data-val-maxage-days", days);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, Attribute.Years, Attribute.Months, Attribute.Days);
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
