// <copyright file="FileAttributeAdapter.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AspNetCore.CustomValidation.Attributes;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace AspNetCore.CustomValidation.Adapters
{
    internal class FileAttributeAdapter : AttributeAdapterBase<FileAttribute>
    {
        private readonly IStringLocalizer _stringLocalizer;

        public FileAttributeAdapter(FileAttribute attribute, IStringLocalizer stringLocalizer)
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

            string[] validFileTypeNames = Attribute.FileTypes.Select(ft => ft.ToString("G")).ToArray();
            string validFileTypeNamesString = string.Join(",", validFileTypeNames);

            AddAttribute(context.Attributes, "data-val", "true");

            AddAttribute(context.Attributes, "data-val-filetype", GetErrorMessage(context));
            AddAttribute(context.Attributes, "data-val-filetype-validtypes", validFileTypeNamesString);

            string propertyDisplayName = context.ModelMetadata.GetDisplayName();

            if (Attribute.MinSize > 0)
            {
                string localizedErrorMessage = _stringLocalizer[Attribute.FileMinSizeErrorMessage];
                var formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, localizedErrorMessage, propertyDisplayName, Attribute.MinSizeAndUnit);
                AddAttribute(context.Attributes, "data-val-file-minsize", formattedErrorMessage);
                AddAttribute(context.Attributes, "data-val-file-minsize-value", Attribute.MinSize.ToString(CultureInfo.InvariantCulture));
            }

            if (Attribute.MaxSize > 0)
            {
                string localizedErrorMessage = _stringLocalizer[Attribute.FileMaxSizeErrorMessage];
                var formattedErrorMessage = string.Format(CultureInfo.InvariantCulture, localizedErrorMessage, propertyDisplayName, Attribute.MaxSizeAndUnit);
                AddAttribute(context.Attributes, "data-val-file-maxsize", formattedErrorMessage);
                AddAttribute(context.Attributes, "data-val-file-maxsize-value", Attribute.MaxSize.ToString(CultureInfo.InvariantCulture));
            }
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();

            string[] validFileTypeNames = Attribute.FileTypes.Select(ft => ft.ToString("G")).ToArray();
            string validFileTypeNamesString = string.Join(",", validFileTypeNames);
            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, validFileTypeNamesString);
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
