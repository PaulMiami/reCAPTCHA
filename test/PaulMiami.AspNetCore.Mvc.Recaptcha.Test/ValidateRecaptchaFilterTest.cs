#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion
/*
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Linq;

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

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, NullLoggerFactory.Instance);

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

            Assert.Null(context.Result);
            Assert.True(context.ModelState.IsValid);
            Assert.Empty(context.ModelState);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostSucessNoIp(string httpMethod)
        {
            var recaptchaResponse = Guid.NewGuid().ToString();

            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(recaptchaResponse, null))
                .Returns(Task.FromResult(0))
                .Verifiable();

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, NullLoggerFactory.Instance);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = httpMethod;
            httpContext.Request.HttpContext.Connection.RemoteIpAddress = null;
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "g-recaptcha-response", recaptchaResponse }
            });

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            recaptchaService.Verify();

            Assert.Null(context.Result);
            Assert.True(context.ModelState.IsValid);
            Assert.Empty(context.ModelState);
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
                .Throws(new RecaptchaValidationException(errorMessage, false))
                .Verifiable();

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, loggerFactory);

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
            var httpBadRequest = Assert.IsType<BadRequestResult>(context.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, httpBadRequest.StatusCode);
            Assert.True(context.ModelState.IsValid);
            Assert.Empty(context.ModelState);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostFailInvalidResponse(string httpMethod)
        {
            var recaptchaResponse = Guid.NewGuid().ToString();
            var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            var errorMessage = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();

            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(recaptchaResponse, ipAddress.ToString()))
                .Throws(new RecaptchaValidationException(errorMessage, true))
                .Verifiable();

            recaptchaService
                .Setup(a => a.ValidationMessage)
                .Returns(validationMessage)
                .Verifiable();


            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, loggerFactory);

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
            Assert.Null(context.Result);
            Assert.False(context.ModelState.IsValid);
            Assert.NotEmpty(context.ModelState);
            Assert.NotNull(context.ModelState["g-recaptcha-response"]);
            Assert.Equal(1, context.ModelState["g-recaptcha-response"].Errors.Count);
            Assert.Equal(validationMessage, context.ModelState["g-recaptcha-response"].Errors.First().ErrorMessage);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostWrongContentType(string httpMethod)
        {
            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var sink = new TestSink();
            var loggerFactory = new TestLoggerFactory(sink, enabled: true);

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, loggerFactory);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = httpMethod;
            httpContext.Request.ContentType = "Wrong content type";

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            recaptchaService.Verify(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            await filter.OnAuthorizationAsync(context);

            Assert.Empty(sink.Scopes);
            Assert.Single(sink.Writes);
            Assert.Equal($"Recaptcha validation failed. The content type is 'Wrong content type', it should be form content.", sink.Writes[0].State?.ToString());
            var httpBadRequest = Assert.IsType<BadRequestResult>(context.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, httpBadRequest.StatusCode);
            Assert.True(context.ModelState.IsValid);
            Assert.Empty(context.ModelState);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("post")]
        public async Task TestPostFailMissingResponse(string httpMethod)
        {
            var recaptchaResponse = string.Empty;
            var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            var errorMessage = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();

            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);
            recaptchaService
                .Setup(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(0))
                .Verifiable();

            recaptchaService
                .Setup(a => a.ValidationMessage)
                .Returns(validationMessage)
                .Verifiable();

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, NullLoggerFactory.Instance);

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

            recaptchaService.Verify(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            Assert.Null(context.Result);
            Assert.False(context.ModelState.IsValid);
            Assert.NotEmpty(context.ModelState);
            Assert.NotNull(context.ModelState["g-recaptcha-response"]);
            Assert.Equal(1, context.ModelState["g-recaptcha-response"].Errors.Count);
            Assert.Equal(validationMessage, context.ModelState["g-recaptcha-response"].Errors.First().ErrorMessage);
        }

        [Fact]
        public async Task DoNotValidateIfDisabled()
        {
            var recaptchaResponse = string.Empty;
            var ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            var recaptchaService = new Mock<IRecaptchaValidationService>(MockBehavior.Strict);

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(false)
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, NullLoggerFactory.Instance);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "POST";
            httpContext.Request.HttpContext.Connection.RemoteIpAddress = ipAddress;
            httpContext.Request.ContentType = "application/x-www-form-urlencoded";
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "g-recaptcha-response", recaptchaResponse }
            });

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            Assert.Null(context.Result);

            configurationService.Verify();
            recaptchaService.Verify();
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

            var configurationService = new Mock<IRecaptchaConfigurationService>(MockBehavior.Strict);
            configurationService
                .Setup(a => a.Enabled)
                .Returns(true)
                .Verifiable();

            var filter = new ValidateRecaptchaFilter(recaptchaService.Object, configurationService.Object, NullLoggerFactory.Instance);

            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            actionContext.HttpContext.Request.Method = httpMethod;

            var context = new AuthorizationFilterContext(actionContext, new[] { filter });

            await filter.OnAuthorizationAsync(context);

            recaptchaService.Verify(a => a.ValidateResponseAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

            Assert.Null(context.Result);
            Assert.True(context.ModelState.IsValid);
            Assert.Empty(context.ModelState);
        }
    }
}
*/