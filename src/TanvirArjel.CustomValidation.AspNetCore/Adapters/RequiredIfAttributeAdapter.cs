// <copyright file="RequiredIfAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;
using TanvirArjel.CustomValidation.Extensions;

namespace TanvirArjel.CustomValidation.AspNetCore.Adapters
{
    internal class RequiredIfAttributeAdapter : AttributeAdapterBase<RequiredIfAttribute>
    {
        public RequiredIfAttributeAdapter(RequiredIfAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            var modelType = context.ModelMetadata.ContainerType;
            var otherPropertyInfo = modelType.GetProperty(Attribute.OtherPropertyName);

            if (otherPropertyInfo == null)
            {
                throw new ApplicationException($"Model does not contain any property named {Attribute.OtherPropertyName}");
            }

            var otherPropertyType = otherPropertyInfo.PropertyType;

            if (otherPropertyType.IsDateTimeType())
            {
                AddAttribute(context.Attributes, "data-val-requiredif-other-property-type", "datetime");
            }
            else if (otherPropertyType.IsNumericType())
            {
                AddAttribute(context.Attributes, "data-val-requiredif-other-property-type", "number");
            }
            else if (otherPropertyType == typeof(string))
            {
                AddAttribute(context.Attributes, "data-val-requiredif-other-property-type", "string");
            }
            else if (otherPropertyType.IsTimeSpanType())
            {
                AddAttribute(context.Attributes, "data-val-requiredif-other-property-type", "timespan");
            }
            else
            {
                throw new ApplicationException($"This attribute does not support type {otherPropertyType}");
            }

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-requiredif-other-property", Attribute.OtherPropertyName);
            AddAttribute(context.Attributes, "data-val-requiredif-comparison-type", Attribute.ComparisonType.ToString());
            AddAttribute(context.Attributes, "data-val-requiredif-other-property-value", Attribute.OtherPropertyValue?.ToString() ?? string.Empty);

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
