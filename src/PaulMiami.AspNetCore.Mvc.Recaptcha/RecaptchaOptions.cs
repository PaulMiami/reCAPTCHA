#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Net.Http;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaOptions
    {

        public string ResponseValidationEndpoint { get; set; } = RecaptchaDefaults.ResponseValidationEndpoint;

        public string JavaScriptUrl { get; set; } = RecaptchaDefaults.JavaScriptUrl;

        public string SiteKey { get; set; }

        public string SecretKey { get; set; }

        public HttpMessageHandler BackchannelHttpHandler { get; set; }

        public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public RecaptchaControlSettings ControlSettings { get; set; } = new RecaptchaControlSettings();

        public string ValidationMessage { get; set; }
    }
}
