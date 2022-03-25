// <copyright file="CompareToAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.Adapters
{
    internal class CompareToAttributeAdapter : AttributeAdapterBase<CompareToAttribute>
    {
        public CompareToAttributeAdapter(CompareToAttribute attribute, IStringLocalizer stringLocalizer)
            : base(new CompareToAttributeWrapper(attribute), stringLocalizer)
        {
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

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-input-type-compare", $"{propertyDisplayName} is not comparable to {comparePropertyName}");

            comparePropertyName = "*." + comparePropertyName;

            AddAttribute(context.Attributes, "data-val-input-type-compare-property", comparePropertyName);

            if (comparisonType == ComparisonType.Equal)
            {
                AddAttribute(context.Attributes, "data-val-comparison-equal", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-equal-property", comparePropertyName);
            }

            if (comparisonType == ComparisonType.NotEqual)
            {
                AddAttribute(context.Attributes, "data-val-comparison-not-equal", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-not-equal-property", comparePropertyName);
            }

            if (comparisonType == ComparisonType.GreaterThan)
            {
                AddAttribute(context.Attributes, "data-val-comparison-greater-than", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-greater-than-property", comparePropertyName);
            }

            if (comparisonType == ComparisonType.GreaterThanOrEqual)
            {
                AddAttribute(context.Attributes, "data-val-comparison-greater-than-or-equal", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-greater-than-or-equal-property", comparePropertyName);
            }

            if (comparisonType == ComparisonType.SmallerThan)
            {
                AddAttribute(context.Attributes, "data-val-comparison-smaller-than", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-smaller-than-property", comparePropertyName);
            }

            if (comparisonType == ComparisonType.SmallerThanOrEqual)
            {
                AddAttribute(context.Attributes, "data-val-comparison-smaller-than-or-equal", GetErrorMessage(context));
                AddAttribute(context.Attributes, "data-val-comparison-smaller-than-or-equal-property", comparePropertyName);
            }
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();
            string comparePropertyDisplayName = validationContext.ModelMetadata.ContainerMetadata.Properties
                .Single(p => p.PropertyName == Attribute.ComparePropertyName).GetDisplayName();

            ((CompareToAttributeWrapper)Attribute).ComparePropertyDisplayName = comparePropertyDisplayName;

            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, comparePropertyDisplayName);
        }

        private static void AddAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }

        // The attribute wrapper is needed to override the FormatErrorMessage method
        // See https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.DataAnnotations/src/CompareAttributeAdapter.cs
        private sealed class CompareToAttributeWrapper : CompareToAttribute
        {
            public CompareToAttributeWrapper(CompareToAttribute attribute)
                : base(attribute.ComparePropertyName, attribute.ComparisonType)
            {
                if (!string.IsNullOrEmpty(attribute.ErrorMessage) ||
                    !string.IsNullOrEmpty(attribute.ErrorMessageResourceName) ||
                    attribute.ErrorMessageResourceType != null)
                {
                    ErrorMessage = attribute.ErrorMessage;
                    ErrorMessageResourceName = attribute.ErrorMessageResourceName;
                    ErrorMessageResourceType = attribute.ErrorMessageResourceType;
                }
            }

            /// <summary>
            /// Display name of the property which against the comparison will be done.
            /// </summary>
            public string ComparePropertyDisplayName { get; set; }

            public override string FormatErrorMessage(string name)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, ComparePropertyDisplayName ?? ComparePropertyName);
            }
        }
    }
}
