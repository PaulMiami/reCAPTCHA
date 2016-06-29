#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class RecaptchaServiceTest
    {
        private readonly string _siteKey = Guid.NewGuid().ToString();
        private readonly string _secretKey = Guid.NewGuid().ToString();

        public IOptions<RecaptchaOptions> GetOptions()
        {
            return new OptionsWrapper<RecaptchaOptions>(new RecaptchaOptions
            {
                SiteKey = _siteKey,
                SecretKey = _secretKey
            });
        }

        [Fact]
        public void MissingOptions()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RecaptchaService(null));
            Assert.Equal("options", ex.ParamName);
            
        }

        [Fact]
        public void MissingOptionResponseValidationEndpoint()
        {
            var options = GetOptions();
            options.Value.ResponseValidationEndpoint = null;

            var ex = Assert.Throws<ArgumentException>(() => new RecaptchaService(options));
            Assert.Equal("The 'ResponseValidationEndpoint' option must be provided.", ex.Message);
        }

        [Fact]
        public void MissingOptionJavaScriptUrl()
        {
            var options = GetOptions();
            options.Value.JavaScriptUrl = null;

            var ex = Assert.Throws<ArgumentException>(() => new RecaptchaService(options));
            Assert.Equal("The 'JavaScriptUrl' option must be provided.", ex.Message);
        }

        [Fact]
        public void MissingOptionSecretKey()
        {
            var options = GetOptions();
            options.Value.SecretKey = null;

            var ex = Assert.Throws<ArgumentException>(() => new RecaptchaService(options));
            Assert.Equal("The 'SecretKey' option must be provided.", ex.Message);
        }

        [Fact]
        public void MissingOptionSiteKey()
        {
            var options = GetOptions();
            options.Value.SiteKey = null;

            var ex = Assert.Throws<ArgumentException>(() => new RecaptchaService(options));
            Assert.Equal("The 'SiteKey' option must be provided.", ex.Message);
        }

        [Fact]
        public void GetJavaScriptUrlSuccess()
        {
            var options = GetOptions();

            var service = new RecaptchaService(options);

            Assert.Equal(options.Value.JavaScriptUrl, service.JavaScriptUrl);
        }

        [Fact]
        public void GetSiteKeySuccess()
        {
            var options = GetOptions();

            var service = new RecaptchaService(options);

            Assert.Equal(options.Value.SiteKey, service.SiteKey);
        }

        private RecaptchaService CreateTestService(System.Net.HttpStatusCode statusCode, RecaptchaValidationResponse result, string reponse, string ipAddress)
        {
            var options = GetOptions();
            options.Value.BackchannelHttpHandler = new TestHttpMessageHandler
            {
                Sender = async req =>
                {
                    var content = await req.Content.ReadAsStringAsync();
                    Assert.Equal($"secret={_secretKey}&response={reponse}&remoteip={ipAddress}", content);

                    if (req.RequestUri.AbsoluteUri == "https://www.google.com/recaptcha/api/siteverify")
                    {
                        var res = new HttpResponseMessage(statusCode);
                        var text = JsonConvert.SerializeObject(result);
                        res.Content = new StringContent(text, Encoding.UTF8, "application/json");
                        return res;
                    }

                    throw new NotImplementedException(req.RequestUri.AbsoluteUri);
                }
            };

            return new RecaptchaService(options);
        }

        [Fact]
        public async Task ValidateSuccess()
        {
            var captchaResponse = Guid.NewGuid().ToString();
            var ipAddress = Guid.NewGuid().ToString();

            var service = CreateTestService(System.Net.HttpStatusCode.OK, 
                new RecaptchaValidationResponse { Success =true}, captchaResponse, ipAddress);

            await service.ValidateResponseAsync(captchaResponse, ipAddress);
        }

        [Theory]
        [InlineData("missing-input-secret", "The secret parameter is missing.")]
        [InlineData("invalid-input-secret", "The secret parameter is invalid or malformed.")]
        [InlineData("missing-input-response", "The response parameter is missing.")]
        [InlineData("invalid-input-response", "The response parameter is invalid or malformed.")]
        public async Task ValidateMissingSecret(string serviceErrorCode, string exceptionMessage)
        {
            var captchaResponse = Guid.NewGuid().ToString();
            var ipAddress = Guid.NewGuid().ToString();

            var service = CreateTestService(System.Net.HttpStatusCode.OK, 
                new RecaptchaValidationResponse { Success = false, ErrorCodes=new List<string> { serviceErrorCode } }, captchaResponse, ipAddress);

            var ex = await Assert.ThrowsAsync<RecaptchaValidationException>(async () => await service.ValidateResponseAsync(captchaResponse, ipAddress));
            Assert.Equal(exceptionMessage, ex.Message);
        }
    }
}
