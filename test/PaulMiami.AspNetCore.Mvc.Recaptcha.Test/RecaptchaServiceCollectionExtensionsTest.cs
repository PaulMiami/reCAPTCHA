#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class RecaptchaServiceCollectionExtensionsTest
    {
        [Fact]
        public void Success()
        {
            var services = new ServiceCollection();

            services.AddRecaptcha(new RecaptchaOptions());

            Assert.True(services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IOptions<RecaptchaOptions>)).Count() == 1);
            Assert.True(services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(RecaptchaService)).Count() == 1);
            Assert.True(services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IRecaptchaValidationService)).Count() == 1);
            Assert.True(services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IRecaptchaConfigurationService)).Count() == 1);
            Assert.True(services.Where(serviceDescriptor => serviceDescriptor.ServiceType == typeof(ValidateRecaptchaFilter)).Count() == 1);

            Assert.Equal(5, services.Count);
        }
    }
}
