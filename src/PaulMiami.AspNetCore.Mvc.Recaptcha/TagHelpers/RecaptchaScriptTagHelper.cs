#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Microsoft.AspNetCore.Localization;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    public class RecaptchaScriptTagHelper : TagHelper
    {
        private IRecaptchaConfigurationService _service;
        private IHttpContextAccessor _contextAccessor;

        private const string _scriptSnippet = "var {0}=function(e){{var r=$('#{1}');r.length&&r.hide()}};$.validator.setDefaults({{submitHandler:function(){{var e=this,r=''!==grecaptcha.getResponse(),a='{2}',t=$('#{1}', e.currentForm);if(t.length===0)return !0;return a&&(r?t.length&&t.hide():(e.errorList.push({{message:a}}),$(e.currentForm).triggerHandler('invalid-form',[e]),t.length&&(t.html(a),t.show()))),r}}}});";

        private const string JqueryValidationAttributeName = "jquery-validation";
        private const string ValidationMessageElementIdAttributeName = "validation-message-element-id";

        public RecaptchaScriptTagHelper(IRecaptchaConfigurationService service, IHttpContextAccessor contextAccessor)
        {
            service.CheckArgumentNull(nameof(service));
            contextAccessor.CheckArgumentNull(nameof(contextAccessor));

            _service = service;
            _contextAccessor = contextAccessor;
        }

        [HtmlAttributeName(JqueryValidationAttributeName)]
        public bool? JqueryValidation { get; set; }

        [HtmlAttributeName(ValidationMessageElementIdAttributeName)]
        public string ValidationMessageElementId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_service.Enabled)
            {
                output.TagName = null;
                return;
            }

            var requestCulture = _contextAccessor.HttpContext.Features.Get<IRequestCultureFeature>();
            var language = requestCulture?.RequestCulture?.UICulture?.Name ?? _service.LanguageCode;

            var javaScriptUrl = _service.JavaScriptUrl;

            if (!string.IsNullOrEmpty(language))
                javaScriptUrl = $"{javaScriptUrl}?hl={language}";

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("src", javaScriptUrl);
            output.Attributes.Add("async", string.Empty);
            output.Attributes.Add("defer", string.Empty);

            if (JqueryValidation ?? true)
            {
                var script = new TagBuilder("script");
                script.TagRenderMode = TagRenderMode.Normal;
                script.InnerHtml.AppendHtml(string.Format(_scriptSnippet,
                    RecaptchaTagHelper.RecaptchaValidationJSCallBack, ValidationMessageElementId, _service.ValidationMessage));

                output.PostElement.AppendHtml(script);
            }
        }
    }
}
