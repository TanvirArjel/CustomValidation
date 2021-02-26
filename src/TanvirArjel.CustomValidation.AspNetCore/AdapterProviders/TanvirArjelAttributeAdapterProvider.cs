// <copyright file="TanvirArjelAttributeAdapterProvider.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.AspNetCore.Adapters;
using TanvirArjel.CustomValidation.AspNetCore.Attributes;
using TanvirArjel.CustomValidation.Attributes;

namespace TanvirArjel.CustomValidation.AspNetCore.AdapterProviders
{
    /// <summary>
    /// Provider for supplying <see cref="IAttributeAdapter"/>.
    /// </summary>
    public class TanvirArjelAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        /// <summary>
        ///  Returns the <see cref="IAttributeAdapter"/> for a given <see cref="ValidationAttribute"/> attribute.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> for which <see cref="IAttributeAdapter"/> will be provided.</param>
        /// <param name="stringLocalizer">The <see cref="IStringLocalizer"/> which will be used to create messages.</param>
        /// <returns>An <see cref="IAttributeAdapter"/> for the given attribute.</returns>
        public virtual IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
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
                case RequiredIfAttribute requiredIfAttribute:
                    return new RequiredIfAttributeAdapter(requiredIfAttribute, stringLocalizer);
                case TextEditorRequiredAttribute textEditorRequiredAttribute:
                    return new TextEditorRequiredAttributeAdapter(textEditorRequiredAttribute, stringLocalizer);
                default:
                    return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
            }
        }
    }
}
