// <copyright file="IServiceCollectionExtensions.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.CustomValidation.AspNetCore.AdapterProviders;

namespace TanvirArjel.CustomValidation.AspNetCore.Extensions
{
    /// <summary>
    /// Contains necessary <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add services for unobtrusive client side validation support for ASP.NET Core Custom Validation.
        /// </summary>
        /// <param name="services">Extend the type <see cref="IServiceCollection"/>.</param>
        public static void AddAspNetCoreCustomValidation(this IServiceCollection services)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, TanvirArjelAttributeAdapterProvider>();
        }
    }
}
