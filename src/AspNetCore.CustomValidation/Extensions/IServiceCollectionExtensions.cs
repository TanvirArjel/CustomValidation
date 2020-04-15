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
        /// <summary>
        /// Add services for unobtrusive client side validation support for ASP.NET Core Custom Validation.
        /// </summary>
        /// <param name="services">Extend the type <see cref="IServiceCollection"/>.</param>
        public static void AddAspNetCoreCustomValidation(this IServiceCollection services)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CutomValidationAttributeAdapterProvider>();
        }
    }
}
