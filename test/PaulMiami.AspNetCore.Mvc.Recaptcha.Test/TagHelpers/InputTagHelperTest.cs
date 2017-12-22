using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test.TagHelpers
{
    public class InputTagHelperTest
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

        private TagHelperOutput ProcessTagHelper(RecaptchaService service, TagHelperAttributeList attributes, Action<InputTagHelper> config = null)
        {
            var tagHelper = new InputTagHelper(service);

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
            var ex = Assert.Throws<ArgumentNullException>(() => new InputTagHelper(null));
        }

        [Fact]
        public void DoNotRenderIfDisabled()
        {
            var options = GetOptions();
            options.Value.Enabled = false;
            var service = new RecaptchaService(options);

            var output = ProcessTagHelper(service, new TagHelperAttributeList());

            Assert.False(output.Attributes.ContainsName("onclick"));
            Assert.True(output.PostElement.IsEmptyOrWhiteSpace);
        }

        [Fact]
        public void CreatesOnClickAttribute()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);
            var formId = "frm";

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = formId);

            Assert.True(output.Attributes.ContainsName("onclick"));
            Assert.Equal("onClickFrm(event)", output.Attributes["onclick"].Value);
        }

        [Fact]
        public void OnClickAttributeStripsHyphen()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);
            var formId = "frm-id";

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = formId);

            Assert.True(output.Attributes.ContainsName("onclick"));
            Assert.Equal("onClickFrmId(event)", output.Attributes["onclick"].Value);
        }

        [Fact]
        public void CreatesRecaptchaDivWithCorrectAttributes()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);
            var formId = "frm";
            var expectedFunction = "onSubmitFrm";
            var expected = $"<div class=\"g-recaptcha\" data-callback=\"{expectedFunction}\" data-sitekey=\"{options.Value.SiteKey}\" data-size=\"invisible\"></div>";

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = formId);

            Assert.False(output.PostElement.IsEmptyOrWhiteSpace);
            Assert.Equal(expected, output.PostElement.GetContent());
        }

        [Fact]
        public void OnSubmitAttributeStripsHyphen()
        {
            var options = GetOptions();
            var service = new RecaptchaService(options);
            var formId = "frm-id";
            var expectedFunction = "onSubmitFrmId";
            var expected = $"<div class=\"g-recaptcha\" data-callback=\"{expectedFunction}\" data-sitekey=\"{options.Value.SiteKey}\" data-size=\"invisible\"></div>";

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), th => th.FormId = formId);

            Assert.False(output.PostElement.IsEmptyOrWhiteSpace);
            Assert.Equal(expected, output.PostElement.GetContent());
        }
    }
}
