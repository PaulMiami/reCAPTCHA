using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test.TagHelpers
{
    public class RecaptchaInvisibleScriptTagHelperTest
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

        private TagHelperOutput ProcessTagHelper(RecaptchaService service, TagHelperAttributeList attributes, Action<RecaptchaInvisibleScriptTagHelper> config = null)
        {
            var tagHelper = new RecaptchaInvisibleScriptTagHelper(service);

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
            var ex = Assert.Throws<ArgumentNullException>(() => new RecaptchaInvisibleScriptTagHelper(null));
        }

        [Fact]
        public void DoNotRenderIfDisabled()
        {
            var options = GetOptions();
            options.Value.Enabled = false;
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList());

            Assert.Null(output.TagName);
            Assert.False(output.IsContentModified);
            Assert.Empty(output.Attributes);
        }

        [Fact]
        public void HasOnClickFunction()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = "frm");

            Assert.Contains("function onClickFrm(e)", output.Content.GetContent());
        }

        [Fact]
        public void OnClickStripsHyphens()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = "frm-id");

            Assert.Contains("function onClickFrmId(e)", output.Content.GetContent());
        }

        [Fact]
        public void HasOnSubmitFunction()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = "frm");

            Assert.Contains("function onSubmitFrm(token)", output.Content.GetContent());
        }

        [Fact]
        public void OnSubmitStripsHyphens()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = "frm-id");

            Assert.Contains("function onSubmitFrmId(token)", output.Content.GetContent());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(null)]
        public void CallsJqueryValidate(bool? jQueryValidateAttributeValue)
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => { th.FormId = "frm"; th.JqueryValidation = jQueryValidateAttributeValue; });

            Assert.Contains("$('", output.Content.GetContent());
            Assert.Contains(".validate()", output.Content.GetContent());
            Assert.Contains(".valid()", output.Content.GetContent());
        }

        [Fact]
        public void DoesNotCallJqueryValidateIfEnabled()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => { th.FormId = "frm"; th.JqueryValidation = false; });

            Assert.DoesNotContain("$('", output.Content.GetContent());
            Assert.DoesNotContain(".validate()", output.Content.GetContent());
            Assert.DoesNotContain(".valid()", output.Content.GetContent());
        }
    }
}
