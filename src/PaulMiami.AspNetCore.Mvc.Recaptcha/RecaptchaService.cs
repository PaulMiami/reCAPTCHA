#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaService
    {
        private RecaptchaOptions _options;

        public RecaptchaService(IOptions<RecaptchaOptions> options)
        {
            options.CheckArgumentNull(nameof(options));

            _options = options.Value;

            _options.ResponseValidationEndpoint.CheckMandatoryOption(nameof(_options.ResponseValidationEndpoint));

            _options.SiteKey.CheckMandatoryOption(nameof(_options.SiteKey));

            _options.SecretKey.CheckMandatoryOption(nameof(_options.SecretKey));
        }

        public string GetSiteKey()
        {
            return _options.SiteKey;
        }

        public async Task ValidateResponseAsync(string response, string remoteIp)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, RecaptchaDefaults.ResponseValidationEndpoint);
            var paramaters = new Dictionary<string, string>();
            paramaters["secret"] = _options.SecretKey;
            paramaters["response"] = response;
            paramaters["remoteip"] = remoteIp;
            request.Content = new FormUrlEncodedContent(paramaters);

            HttpClient backchannel = new HttpClient();

            var resp = await backchannel.SendAsync(request);

            var responseText = await resp.Content.ReadAsStringAsync();

            var jsonTokenResponse = JObject.Parse(responseText);

            if (!jsonTokenResponse.Value<bool>("success"))
                throw new Exception();//throw new RecaptchaException();
        }
    }
}
