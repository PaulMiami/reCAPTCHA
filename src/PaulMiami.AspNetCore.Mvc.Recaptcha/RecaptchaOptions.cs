#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaOptions
    {

        public RecaptchaOptions()
        {
            ResponseValidationEndpoint = RecaptchaDefaults.ResponseValidationEndpoint;
            JavaScriptUrl = RecaptchaDefaults.JavaScriptUrl;
        }

        public string ResponseValidationEndpoint { get; set; }

        public string JavaScriptUrl { get; set; }

        public string SiteKey { get; set; }

        public string SecretKey { get; set; }
    }
}
