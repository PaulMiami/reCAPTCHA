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
		private readonly IRecaptchaConfigurationService _configurationService;

		public ValidateRecaptchaFilter(IRecaptchaValidationService service, IRecaptchaConfigurationService configurationService, ILoggerFactory loggerFactory)
        {
            service.CheckArgumentNull(nameof(service));
			service.CheckArgumentNull(nameof(configurationService));
			loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _service = service;
			_configurationService = configurationService;
			_logger = loggerFactory.CreateLogger<ValidateRecaptchaFilter>();
        }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            context.CheckArgumentNull(nameof(context));
            context.HttpContext.CheckArgumentNull(nameof(context.HttpContext));

            if (ShouldValidate(context))
            {
                var formField = "g-recaptcha-response";
                Action invalidResponse = () => context.ModelState.AddModelError(formField, _service.ValidationMessage);

                try
                {
                    if (!context.HttpContext.Request.HasFormContentType)
                    {
                        throw new RecaptchaValidationException(string.Format(Resources.Exception_MissingFormContent, context.HttpContext.Request.ContentType), false);
                    }

                    var form = await context.HttpContext.Request.ReadFormAsync();
                    var response = form[formField];
                    var remoteIp = context.HttpContext.Connection?.RemoteIpAddress?.ToString();


                    if (string.IsNullOrEmpty(response))
                    {
                        invalidResponse();
                        return;
                    }
                
                    await _service.ValidateResponseAsync(response, remoteIp);
                }
                catch (RecaptchaValidationException ex)
                {
                    _logger.ValidationException(ex.Message, ex);

                    if (ex.InvalidResponse)
                    {
                        invalidResponse();
                        return;
                    }
                    else
                    {
                        context.Result = new BadRequestResult();
                    }
                }
            }
        }

        protected  bool ShouldValidate(AuthorizationFilterContext context)
        {
			return this._configurationService.Enabled && string.Equals("POST", context.HttpContext.Request.Method, StringComparison.OrdinalIgnoreCase);
        }
    }
}
