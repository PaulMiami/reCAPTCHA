#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    public class RecaptchaScriptTagHelper : TagHelper
    {
        private RecaptchaService _service;

        public RecaptchaScriptTagHelper(RecaptchaService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _service = service;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("src", _service.GetJavaScriptUrl());
            output.Attributes.Add("async", string.Empty);
            output.Attributes.Add("defer", string.Empty);
        }
    }
}
