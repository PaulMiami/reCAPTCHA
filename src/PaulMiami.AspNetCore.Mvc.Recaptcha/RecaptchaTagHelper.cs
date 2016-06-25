#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaTagHelper : TagHelper
    {
        private RecaptchaService _service;

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
            output.Attributes.Add("data-sitekey", _service.GetSiteKey());
        }
    }
}
