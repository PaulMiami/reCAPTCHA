#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class ValidateRecaptchaFilter : IAsyncAuthorizationFilter
    {
        private RecaptchaService _service;

        public ValidateRecaptchaFilter(RecaptchaService service)
        {
            service.CheckArgumentNull(nameof(service));
            _service = service;
        }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            context.CheckArgumentNull(nameof(context));
            context.HttpContext.CheckArgumentNull(nameof(context.HttpContext));

            var form = await context.HttpContext.Request.ReadFormAsync();
            var response = form["g-recaptcha-response"];
            var remoteIp = context.HttpContext.Connection?.RemoteIpAddress?.ToString();

            try
            {
                await _service.ValidateResponseAsync(response, remoteIp);
            }
            catch (Exception)
            {
                // log ex
                context.Result = new BadRequestResult();
            }
        }
    }
}
