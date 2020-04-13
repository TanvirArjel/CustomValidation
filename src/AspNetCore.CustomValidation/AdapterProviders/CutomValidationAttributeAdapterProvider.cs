// <copyright file="CutomValidationAttributeAdapterProvider.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using AspNetCore.CustomValidation.Adapters;
using AspNetCore.CustomValidation.Attributes;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace AspNetCore.CustomValidation.AdapterProviders
{
    internal class CutomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            switch (attribute)
            {
                case CompareToAttribute compareToAttribute:
                    return new CompareToAttributeAdapter(compareToAttribute, stringLocalizer);
                case FileAttribute fileAttribute:
                    return new FileAttributeAdapter(fileAttribute, stringLocalizer);
                case FileMaxSizeAttribute fileMaxSizeAttribute:
                    return new FileMaxSizeAttributeAdapter(fileMaxSizeAttribute, stringLocalizer);
                case FileMinSizeAttribute fileMinSizeAttribute:
                    return new FileMinSizeAttributeAdapter(fileMinSizeAttribute, stringLocalizer);
                case FileTypeAttribute fileTypeAttribute:
                    return new FileTypeAttributeAdapter(fileTypeAttribute, stringLocalizer);
                case FixedLengthAttribute fixedLengthAttribute:
                    return new FixedLengthAttributeAdapter(fixedLengthAttribute, stringLocalizer);
                case MaxAgeAttribute maxAgeAttribute:
                    return new MaxAgeAttributeAdapter(maxAgeAttribute, stringLocalizer);
                case MaxDateAttribute maxDateAttribute:
                    return new MaxDateAttributeAdapter(maxDateAttribute, stringLocalizer);
                case MinAgeAttribute minAgeAttribute:
                    return new MinAgeAttributeAdapter(minAgeAttribute, stringLocalizer);
                case MinDateAttribute minDateAttribute:
                    return new MinDateAttributeAdapter(minDateAttribute, stringLocalizer);
                case TextEditorRequiredAttribute textEditorRequiredAttribute:
                    return new TextEditorRequiredAttributeAdapter(textEditorRequiredAttribute, stringLocalizer);
                default:
                    return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
            }
        }
    }
}
