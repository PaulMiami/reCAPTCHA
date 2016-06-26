#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.Options;
using System;
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

            Assert.Equal(options.Value.JavaScriptUrl, service.GetJavaScriptUrl());
        }

        [Fact]
        public void GetSiteKeySuccess()
        {
            var options = GetOptions();

            var service = new RecaptchaService(options);

            Assert.Equal(options.Value.SiteKey, service.GetSiteKey());
        }
    }
}
