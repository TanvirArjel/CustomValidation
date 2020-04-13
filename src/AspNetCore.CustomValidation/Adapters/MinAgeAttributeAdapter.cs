// <copyright file="MinAgeAttributeAdapter.cs" company="TanvirArjel">
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
    internal class MinAgeAttributeAdapter : AttributeAdapterBase<MinAgeAttribute>
    {
        public MinAgeAttributeAdapter(MinAgeAttribute attribute, IStringLocalizer stringLocalizer)
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
            var errorMessage = GetErrorMessage(context);

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-valid-date-format", "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");
            AddAttribute(context.Attributes, "data-val-currenttime", $"{propertyDisplayName} can not be greater than today's date.");

            AddAttribute(context.Attributes, "data-val-minage", errorMessage);

            var years = Attribute.Years.ToString(CultureInfo.InvariantCulture);
            var months = Attribute.Months.ToString(CultureInfo.InvariantCulture);
            var days = Attribute.Days.ToString(CultureInfo.InvariantCulture);

            AddAttribute(context.Attributes, "data-val-minage-years", years);
            AddAttribute(context.Attributes, "data-val-minage-months", months);
            AddAttribute(context.Attributes, "data-val-minage-days", days);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata, Attribute.Years, Attribute.Months, Attribute.Years);
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
