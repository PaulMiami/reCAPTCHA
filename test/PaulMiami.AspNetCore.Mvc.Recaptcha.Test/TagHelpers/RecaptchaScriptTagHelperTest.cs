#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Moq;
using PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test.TagHelpers
{
    public class RecaptchaScriptTagHelperTest
    {
        private const string _script = "<script>var {0}=function(e){{var r=$('#{1}');r.length&&r.hide()}};$.validator.setDefaults({{submitHandler:function(){{var e=this,r=''!==grecaptcha.getResponse(),a='{2}',t=$('#{1}');return a&&(r?t.length&&t.hide():(e.errorList.push({{message:a}}),$(e.currentForm).triggerHandler('invalid-form',[e]),t.length&&(t.html(a),t.show()))),r}}}});</script>";
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

        private TagHelperOutput ProcessTagHelper(RecaptchaService service, IHttpContextAccessor httpContextAccessor, TagHelperAttributeList attributes, Action<RecaptchaScriptTagHelper> config = null)
        {
            var tagHelper = new RecaptchaScriptTagHelper(service, httpContextAccessor);

            config?.Invoke(tagHelper);

            var tagHelperContext = new TagHelperContext(
               allAttributes: new TagHelperAttributeList(),
               items: new Dictionary<object, object>(),
               uniqueId: "test");

            var output = new TagHelperOutput(
                tagName: "doesntmatter",
                attributes: attributes,
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
                });

            tagHelper.Process(tagHelperContext, output);

            return output;
        }

        [Fact]
        public void MissingService()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new RecaptchaScriptTagHelper(null, new HttpContextAccessor()));
        }

        [Fact]
        public void MissingContext()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var ex = Assert.Throws<ArgumentNullException>(() => new RecaptchaScriptTagHelper(service, null));
        }

        [Fact]
        public void DoNotRenderIfDisabled()
        {
            var options = GetOptions();
            options.Value.Enabled = false;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList());

            Assert.Null(output.TagName);
            Assert.False(output.IsContentModified);
            Assert.Empty(output.Attributes);
        }

        [Fact]
        public void NoOptions()
        {
            var options = GetOptions();
            var src = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();
            options.Value.JavaScriptUrl = src;
            options.Value.ValidationMessage = validationMessage;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList());

            Assert.Equal("script", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["src"]);
            Assert.Equal(src, output.Attributes["src"].Value);
            Assert.NotNull(output.Attributes["async"]);
            Assert.Equal(string.Empty, output.Attributes["async"].Value);
            Assert.NotNull(output.Attributes["defer"]);
            Assert.Equal(string.Empty, output.Attributes["defer"].Value);
            Assert.Equal(string.Format(_script, "recaptchaValidated", string.Empty, validationMessage), output.PostElement.GetContent());

            httpContextAccessor.Verify();
        }

        [Fact]
        public void JqueryValidationDisabledOptions()
        {
            var options = GetOptions();
            var src = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();
            options.Value.JavaScriptUrl = src;
            options.Value.ValidationMessage = validationMessage;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList(), (th)=> th.JqueryValidation = false);

            Assert.Equal("script", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["src"]);
            Assert.Equal(src, output.Attributes["src"].Value);
            Assert.NotNull(output.Attributes["async"]);
            Assert.Equal(string.Empty, output.Attributes["async"].Value);
            Assert.NotNull(output.Attributes["defer"]);
            Assert.Equal(string.Empty, output.Attributes["defer"].Value);
            Assert.Equal(string.Empty, output.PostElement.GetContent());

            httpContextAccessor.Verify();
        }

        [Fact]
        public void ValidationMessageElementIdDisabledOptions()
        {
            var options = GetOptions();
            var src = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();
            var validationMessageElementId = Guid.NewGuid().ToString();
            options.Value.JavaScriptUrl = src;
            options.Value.ValidationMessage = validationMessage;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList(), (th)=>th.ValidationMessageElementId = validationMessageElementId);

            Assert.Equal("script", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["src"]);
            Assert.Equal(src, output.Attributes["src"].Value);
            Assert.NotNull(output.Attributes["async"]);
            Assert.Equal(string.Empty, output.Attributes["async"].Value);
            Assert.NotNull(output.Attributes["defer"]);
            Assert.Equal(string.Empty, output.Attributes["defer"].Value);
            Assert.Equal(string.Format(_script, "recaptchaValidated", validationMessageElementId, validationMessage), output.PostElement.GetContent());

            httpContextAccessor.Verify();
        }

        [Fact]
        public void LanguageCodeOptions()
        {
            var options = GetOptions();
            var src = Guid.NewGuid().ToString();
            var languageCode = Guid.NewGuid().ToString(); 
            var validationMessage = Guid.NewGuid().ToString();
            options.Value.JavaScriptUrl = src;
            options.Value.ValidationMessage = validationMessage;
            options.Value.LanguageCode = languageCode;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList());

            Assert.Equal("script", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["src"]);
            Assert.Equal($"{src}?hl={languageCode}", output.Attributes["src"].Value);
            Assert.NotNull(output.Attributes["async"]);
            Assert.Equal(string.Empty, output.Attributes["async"].Value);
            Assert.NotNull(output.Attributes["defer"]);
            Assert.Equal(string.Empty, output.Attributes["defer"].Value);
            Assert.Equal(string.Format(_script, "recaptchaValidated", string.Empty, validationMessage), output.PostElement.GetContent());

            httpContextAccessor.Verify();
        }

        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("fr")]
        public void RequestCultureFeature(string culture)
        {
            var options = GetOptions();
            var src = Guid.NewGuid().ToString();
            var languageCode = Guid.NewGuid().ToString();
            var validationMessage = Guid.NewGuid().ToString();
            options.Value.JavaScriptUrl = src;
            options.Value.ValidationMessage = validationMessage;
            var service = new RecaptchaService(options);
            var httpContext = new DefaultHttpContext();
            var requestCultureFeature = new Mock<IRequestCultureFeature>(MockBehavior.Strict);
            httpContext.Features.Set<IRequestCultureFeature>(requestCultureFeature.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpContextAccessor
                .Setup(a => a.HttpContext)
                .Returns(httpContext)
                .Verifiable();

            requestCultureFeature
                .Setup(a => a.RequestCulture)
                .Returns(new RequestCulture(culture))
                .Verifiable();

            var output = ProcessTagHelper(service, httpContextAccessor.Object, new TagHelperAttributeList());

            Assert.Equal("script", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["src"]);
            Assert.Equal($"{src}?hl={culture}", output.Attributes["src"].Value);
            Assert.NotNull(output.Attributes["async"]);
            Assert.Equal(string.Empty, output.Attributes["async"].Value);
            Assert.NotNull(output.Attributes["defer"]);
            Assert.Equal(string.Empty, output.Attributes["defer"].Value);
            Assert.Equal(string.Format(_script, "recaptchaValidated", string.Empty, validationMessage), output.PostElement.GetContent());

            httpContextAccessor.Verify();
            requestCultureFeature.Verify();
        }
    }
}
