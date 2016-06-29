#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging.Testing;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class ValidateRecaptchaFilterTest
    {

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostSucess(string httpMethod)
        {
            var recaptchaResponse = Guid.NewGuid().ToString();
            var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });

            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(recaptchaResponse, ipAddress.ToString()))
                .Returns(Task.FromResult(0))
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, NullLoggerFactory.Instance);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = httpMethod;
            httpContext.Request.HttpContext.Connection.RemoteIpAddress = ipAddress;
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "g-recaptcha-response", recaptchaResponse }
            });

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            recaptchaService.Verify();
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostFail(string httpMethod)
        {
            var recaptchaResponse = Guid.NewGuid().ToString();
            var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            var errorMessage = Guid.NewGuid().ToString();

            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(recaptchaResponse, ipAddress.ToString()))
                .Throws(new RecaptchaValidationException(errorMessage))
                .Verifiable();

            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, loggerFactory);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = httpMethod;
            httpContext.Request.HttpContext.Connection.RemoteIpAddress = ipAddress;
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "g-recaptcha-response", recaptchaResponse }
            });

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            recaptchaService.Verify();

            Assert.Empty(sink.Scopes);
            Assert.Single(sink.Writes);
            Assert.Equal($"Recaptcha validation failed. {errorMessage}", sink.Writes[0].State?.ToString());
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("HEAD")]
        [InlineData("TRACE")]
        [InlineData("OPTIONS")]
        public async Task TestPostSkipping(string httpMethod)
        {
            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(0))
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, NullLoggerFactory.Instance);

            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            actionContext.HttpContext.Request.Method = httpMethod;

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            recaptchaService.Verify(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }
}
