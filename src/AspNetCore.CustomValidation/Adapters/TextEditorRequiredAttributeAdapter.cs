// <copyright file="TextEditorRequiredAttributeAdapter.cs" company="TanvirArjel">
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
    internal class TextEditorRequiredAttributeAdapter : AttributeAdapterBase<TextEditorRequiredAttribute>
    {
        private IStringLocalizer _stringLocalizer;

        public TextEditorRequiredAttributeAdapter(TextEditorRequiredAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-texteditor-required", GetErrorMessage(context));

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();

            if (Attribute.MinLength > 0)
            {
                string minLengthErrorMessage = _stringLocalizer[Attribute.MinLengthErrorMessage];
                string formattedMinLengthErrorMessage = string.Format(CultureInfo.InvariantCulture, minLengthErrorMessage, propertyDisplayName, Attribute.MinLength);

                AddAttribute(context.Attributes, "data-val-texteditor-minlength", formattedMinLengthErrorMessage);
                AddAttribute(context.Attributes, "data-val-texteditor-minlength-value", Attribute.MinLength.ToString(CultureInfo.InvariantCulture));
            }

            if (Attribute.MaxLength > 0)
            {
                string maxLengthErrorMessage = _stringLocalizer[Attribute.MaxLengthErrorMessage];
                string formattedMaxLengthErrorMessage = string.Format(CultureInfo.InvariantCulture, maxLengthErrorMessage, propertyDisplayName, Attribute.MaxLength);

                AddAttribute(context.Attributes, "data-val-texteditor-maxlength", formattedMaxLengthErrorMessage);
                AddAttribute(context.Attributes, "data-val-texteditor-maxlength-value", Attribute.MaxLength.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

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
