// <copyright file="FileTypeAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.AspNetCore.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.Adapters
{
    internal class FileTypeAttributeAdapter : AttributeAdapterBase<FileTypeAttribute>
    {
        public FileTypeAttributeAdapter(FileTypeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string[] validFileTypeNames = Attribute.FileTypes.Select(ft => ft.ToString("G")).ToArray();
            string validFileTypeNamesString = string.Join(",", validFileTypeNames);

            AddAttribute(context.Attributes, "data-val", "true");
            AddAttribute(context.Attributes, "data-val-filetype", GetErrorMessage(context));
            AddAttribute(context.Attributes, "data-val-filetype-validtypes", validFileTypeNamesString);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();

            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, Attribute.ValidFileTypeNamesString);
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
