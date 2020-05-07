// <copyright file="FileMinSizeAttributeAdapter.cs" company="TanvirArjel">
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
    internal class FileMinSizeAttributeAdapter : AttributeAdapterBase<FileMinSizeAttribute>
    {
        public FileMinSizeAttributeAdapter(FileMinSizeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            AddAttribute(context.Attributes, "data-val", "true");

            AddAttribute(context.Attributes, "data-val-file-minsize", GetErrorMessage(context));
            string minSize = Attribute.MinSize.ToString(CultureInfo.InvariantCulture);
            AddAttribute(context.Attributes, "data-val-file-minsize-value", minSize);
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            string propertyDisplayName = validationContext.ModelMetadata.GetDisplayName();
            int minSize = Attribute.MinSize;
            string minSizeAndUnit = minSize >= 1024 ? Math.Round(minSize / 1024M, 2) + " MB" : minSize + " KB";
            return GetErrorMessage(validationContext.ModelMetadata, propertyDisplayName, minSizeAndUnit);
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
