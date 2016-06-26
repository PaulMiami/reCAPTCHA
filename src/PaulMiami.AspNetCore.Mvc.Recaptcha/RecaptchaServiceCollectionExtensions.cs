#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public static class RecaptchaServiceCollectionExtensions
    {
        public static void AddRecaptcha(this IServiceCollection services, RecaptchaOptions configureOptions)
        {
            services.TryAddSingleton(Options.Create(configureOptions));
            services.TryAddSingleton<RecaptchaService>();
            services.TryAddSingleton<ValidateRecaptchaFilter>();
        }
    }
}
