#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test.TagHelpers
{
    public class RecaptchaTagHelperTest
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

        private TagHelperOutput ProcessTagHelper(RecaptchaService service, TagHelperAttributeList attributes, Action<RecaptchaTagHelper> config = null)
        {
            var tagHelper = new RecaptchaTagHelper(service);

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
            var ex = Assert.Throws<ArgumentNullException>(() => new RecaptchaTagHelper(null));
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
        public void NoOptions()
        {
            var service = new RecaptchaService(GetOptions());

            var output = ProcessTagHelper(service, new TagHelperAttributeList());

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["class"]);
            Assert.Equal("g-recaptcha", output.Attributes["class"].Value);
            Assert.NotNull(output.Attributes["data-sitekey"]);
            Assert.Equal(_siteKey, output.Attributes["data-sitekey"].Value);
            Assert.NotNull(output.Attributes["data-callback"]);
            Assert.Equal("recaptchaValidated", output.Attributes["data-callback"].Value);
            Assert.Null(output.Attributes["data-theme"]);
            Assert.Null(output.Attributes["data-type"]);
            Assert.Null(output.Attributes["data-size"]);
            Assert.Null(output.Attributes["data-tabindex"]);
        }

        [Fact]
        public void ThemeDarkOptions()
        {
            var service = new RecaptchaService(GetOptions());

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), (th)=> th.Theme = RecaptchaTheme.Dark);

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["class"]);
            Assert.Equal("g-recaptcha", output.Attributes["class"].Value);
            Assert.NotNull(output.Attributes["data-sitekey"]);
            Assert.Equal(_siteKey, output.Attributes["data-sitekey"].Value);
            Assert.NotNull(output.Attributes["data-callback"]);
            Assert.Equal("recaptchaValidated", output.Attributes["data-callback"].Value);
            Assert.NotNull(output.Attributes["data-theme"]);
            Assert.Equal("dark", output.Attributes["data-theme"].Value);
            Assert.Null(output.Attributes["data-type"]);
            Assert.Null(output.Attributes["data-size"]);
            Assert.Null(output.Attributes["data-tabindex"]);
        }

        [Fact]
        public void TypeAudioOptions()
        {
            var service = new RecaptchaService(GetOptions());

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), (th) => th.Type = RecaptchaType.Audio);

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["class"]);
            Assert.Equal("g-recaptcha", output.Attributes["class"].Value);
            Assert.NotNull(output.Attributes["data-sitekey"]);
            Assert.Equal(_siteKey, output.Attributes["data-sitekey"].Value);
            Assert.NotNull(output.Attributes["data-callback"]);
            Assert.Equal("recaptchaValidated", output.Attributes["data-callback"].Value);
            Assert.Null(output.Attributes["data-theme"]);
            Assert.NotNull(output.Attributes["data-type"]);
            Assert.Equal("audio", output.Attributes["data-type"].Value);
            Assert.Null(output.Attributes["data-size"]);
            Assert.Null(output.Attributes["data-tabindex"]);
        }

        [Fact]
        public void SizeCompactOptions()
        {
            var service = new RecaptchaService(GetOptions());

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), (th) => th.Size = RecaptchaSize.Compact);

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["class"]);
            Assert.Equal("g-recaptcha", output.Attributes["class"].Value);
            Assert.NotNull(output.Attributes["data-sitekey"]);
            Assert.Equal(_siteKey, output.Attributes["data-sitekey"].Value);
            Assert.NotNull(output.Attributes["data-callback"]);
            Assert.Equal("recaptchaValidated", output.Attributes["data-callback"].Value);
            Assert.Null(output.Attributes["data-theme"]);
            Assert.Null(output.Attributes["data-type"]);
            Assert.NotNull(output.Attributes["data-size"]);
            Assert.Equal("compact", output.Attributes["data-size"].Value);
            Assert.Null(output.Attributes["data-tabindex"]);
        }

        [Fact]
        public void TabIndexOptions()
        {
            var service = new RecaptchaService(GetOptions());
            var ran = new Random(DateTime.Now.Millisecond);
            var index = ran.Next(1000);

            var output = ProcessTagHelper(service, new TagHelperAttributeList(), (th) => th.TabIndex = index);

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.NotNull(output.Attributes["class"]);
            Assert.Equal("g-recaptcha", output.Attributes["class"].Value);
            Assert.NotNull(output.Attributes["data-sitekey"]);
            Assert.Equal(_siteKey, output.Attributes["data-sitekey"].Value);
            Assert.NotNull(output.Attributes["data-callback"]);
            Assert.Equal("recaptchaValidated", output.Attributes["data-callback"].Value);
            Assert.Null(output.Attributes["data-theme"]);
            Assert.Null(output.Attributes["data-type"]);
            Assert.Null(output.Attributes["data-size"]);
            Assert.NotNull(output.Attributes["data-tabindex"]);
            Assert.Equal(index, output.Attributes["data-tabindex"].Value);
        }
    }
}
