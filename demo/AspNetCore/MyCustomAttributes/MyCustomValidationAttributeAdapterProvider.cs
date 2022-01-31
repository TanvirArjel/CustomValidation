using System.ComponentModel.DataAnnotations;
using AspNetCore.MyCustomAttributes.Adapters;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using TanvirArjel.CustomValidation.AspNetCore.AdapterProviders;
using TanvirArjel.CustomValidation.Attributes;

namespace AspNetCore.MyCustomAttributes
{
    ////public class MyCustomAttributeAdapterProvider : IValidationAttributeAdapterProvider
    ////{
    ////    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

    ////    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    ////    {
    ////        switch (attribute)
    ////        {
    ////            case FooAtrribute fooAtrribute:
    ////                return new FooAtrributeAdapter(fooAtrribute, stringLocalizer);
    ////            default:
    ////                return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    ////        }
    ////    }
    ////}

    public class MyCustomValidationAttributeAdapterProvider : TanvirArjelAttributeAdapterProvider
    {
        public override IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            switch (attribute)
            {
                case FooAtrribute fooAtrribute:
                    return new FooAtrributeAdapter(fooAtrribute, stringLocalizer);
                case CompareToAttribute compareToAttribute:
                    return new MyCompareToAttributeAdapter(compareToAttribute, stringLocalizer);
                default:
                    return base.GetAttributeAdapter(attribute, stringLocalizer);
            }
        }
    }
}
