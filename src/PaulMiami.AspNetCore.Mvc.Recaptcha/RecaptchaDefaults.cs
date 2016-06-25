#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaDefaults
    {
        public const string ResponseValidationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
        public const string JavaScriptUrl = "https://www.google.com/recaptcha/api.js";
    }
}
