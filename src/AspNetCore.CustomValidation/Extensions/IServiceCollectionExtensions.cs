// <copyright file="IServiceCollectionExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using AspNetCore.CustomValidation.AdapterProviders;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.CustomValidation.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddAspNetCoreCustomValidation(this IServiceCollection services)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CutomValidationAttributeAdapterProvider>();
        }
    }
}
