#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    public class RecaptchaScriptTagHelper : TagHelper
    {
        private RecaptchaService _service;

        private const string _scriptSnippet = "var {0}=function(e){{var r=$('#{1}');r.length&&r.hide()}};$.validator.setDefaults({{submitHandler:function(){{var e=this,r=''!==grecaptcha.getResponse(),a='{2}',t=$('#{1}');return a&&(r?t.length&&t.hide():(e.errorList.push({{message:a}}),$(e.currentForm).triggerHandler('invalid-form',[e]),t.length&&(t.html(a),t.show()))),r}}}});";

        private const string JqueryValidationAttributeName = "jquery-validation";
        private const string ValidationMessageElementIdAttributeName = "validation-message-element-id";

        public RecaptchaScriptTagHelper(RecaptchaService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _service = service;
        }

        [HtmlAttributeName(JqueryValidationAttributeName)]
        public bool? JqueryValidation { get; set; }

        [HtmlAttributeName(ValidationMessageElementIdAttributeName)]
        public string ValidationMessageElementID { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("src", _service.JavaScriptUrl);
            output.Attributes.Add("async", string.Empty);
            output.Attributes.Add("defer", string.Empty);

            if (JqueryValidation ?? true)
            {
                var script = new TagBuilder("script");
                script.TagRenderMode = TagRenderMode.Normal;
                script.InnerHtml.AppendHtml(string.Format(_scriptSnippet,
                    RecaptchaTagHelper.RecaptchaValidationJSCallBack, ValidationMessageElementID, _service.ValidationMessage));

                output.PostElement.AppendHtml(script);
            }
        }
    }
}
