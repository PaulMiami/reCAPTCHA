#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class ValidateRecaptchaFilter : IAsyncAuthorizationFilter
    {
        private IRecaptchaValidationService _service;
        private ILogger<ValidateRecaptchaFilter> _logger;

        public ValidateRecaptchaFilter(IRecaptchaValidationService service, ILoggerFactory loggerFactory)
        {
            service.CheckArgumentNull(nameof(service));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _service = service;
            _logger = loggerFactory.CreateLogger<ValidateRecaptchaFilter>();
        }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            context.CheckArgumentNull(nameof(context));
            context.HttpContext.CheckArgumentNull(nameof(context.HttpContext));

            if (ShouldValidate(context))
            {
                var form = await context.HttpContext.Request.ReadFormAsync();
                var response = form["g-recaptcha-response"];
                var remoteIp = context.HttpContext.Connection?.RemoteIpAddress?.ToString();

                try
                {
                    await _service.ValidateResponseAsync(response, remoteIp);
                }
                catch (RecaptchaValidationException ex)
                {
                    _logger.ValidationException(ex.Message, ex);
                    context.Result = new BadRequestResult();
                }
            }
        }

        protected  bool ShouldValidate(AuthorizationFilterContext context)
        {
            return string.Equals("POST", context.HttpContext.Request.Method, StringComparison.OrdinalIgnoreCase);
        }
    }
}
