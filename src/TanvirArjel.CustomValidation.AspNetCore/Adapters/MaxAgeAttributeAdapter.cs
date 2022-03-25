﻿// <copyright file="MaxAgeAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.Adapters
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

            AddAttribute(context.Attributes, "data-val-valid-date-format",
                "The input date/datetime format is not valid! Please prefer: '01-Jan-2019' format.");

            AddAttribute(context.Attributes, "data-val-currenttime",
                $"{propertyDisplayName} can not be greater than today's date.");

            AddAttribute(context.Attributes, "data-val-maxage", errorMessage);

            string maxAgeDateTime = Attribute.MaxAgeDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-maxage-maxagedatetime", maxAgeDateTime);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();

            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, Attribute.MaxAgeDateTime.ToString(Attribute.ErrorMessageMaxAgeDateTimeFormat, CultureInfo.CurrentCulture));
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
