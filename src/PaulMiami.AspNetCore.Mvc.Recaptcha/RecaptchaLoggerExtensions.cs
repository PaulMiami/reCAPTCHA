#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.Logging;
using System;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public static class RecaptchaLoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _validationException;

        static RecaptchaLoggerExtensions()
        {
            _validationException = LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                Resources.Log_ValidationException);
        }

        public static void ValidationException(this ILogger<ValidateRecaptchaFilter> logger, string message, Exception ex)
        {
            _validationException(logger, message, ex);
        }
    }
}
