#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaValidationException : Exception
    {
        public bool InvalidResponse { get; }

        public RecaptchaValidationException(string message, bool invalidResponse):base(message)
        {
            InvalidResponse = invalidResponse;
        }
    }
}
