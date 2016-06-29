#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    public class RecaptchaTagHelper : TagHelper
    {
        private RecaptchaService _service;

        internal const string RecaptchaValidationJSCallBack = "recaptchaValidated";

        private const string ThemeAttributeName = "theme";
        private const string TypeAttributeName = "type";
        private const string SizeAttributeName = "size";
        private const string TabindexAttributeName = "tabindex";

        [HtmlAttributeName(ThemeAttributeName)]
        public RecaptchaTheme? Theme { get; set; }

        [HtmlAttributeName(TypeAttributeName)]
        public RecaptchaType? Type { get; set; }

        [HtmlAttributeName(SizeAttributeName)]
        public RecaptchaSize? Size { get; set; }

        [HtmlAttributeName(TabindexAttributeName)]
        public int TabIndex { get; set; }

        public RecaptchaTagHelper(RecaptchaService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _service = service;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "g-recaptcha");
            output.Attributes.Add("data-sitekey", _service.SiteKey);
            output.Attributes.Add("data-callback", RecaptchaValidationJSCallBack);

            var controlSettings = _service.ControlSettings;

            if ((Theme ?? controlSettings.Theme) == RecaptchaTheme.Dark)
            {
                output.Attributes.Add("data-theme", "dark");
            }

            if ((Type ?? controlSettings.Type) == RecaptchaType.Audio)
            {
                output.Attributes.Add("data-type", "audio");
            }

            if ((Size ?? controlSettings.Size) == RecaptchaSize.Compact)
            {
                output.Attributes.Add("data-size", "compact");
            }

            if (TabIndex != 0)
            {
                output.Attributes.Add("data-tabindex", TabIndex);
            }
        }
    }
}
