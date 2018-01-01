using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForRecaptchaFormAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("button", Attributes = ForRecaptchaFormAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class InputTagHelper : TagHelper
    {
        private const string ForRecaptchaFormAttributeName = "for-recaptcha-form";
        private const string DataCallBackAttributeName = "data-callback";
        private const string DataSizeAttributeName = "data-size";
        private const string DataSiteKeyAttributeName = "data-sitekey";
        private const string OnClickAttributeName = "onclick";
        private const string CssClassAttribute = "class";

        private readonly IRecaptchaConfigurationService _service;

        [HtmlAttributeName(ForRecaptchaFormAttributeName)]
        public string FormId { get; set; }

        public InputTagHelper(IRecaptchaConfigurationService service)
        {
            service.CheckArgumentNull(nameof(service));

            _service = service;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_service.Enabled)
            {
                base.Process(context, output);
                return;
            }

            output.Attributes.Add(OnClickAttributeName, RecaptchaInvisibleScriptTagHelper.GetOnClickFunctionName(FormId) + "(event)");
            output.PostElement.AppendHtml(CreateRecaptchaDivTag());

            base.Process(context, output);
        }

        private TagBuilder CreateRecaptchaDivTag()
        {
            var div = new TagBuilder("div");

            div.TagRenderMode = TagRenderMode.Normal;
            div.Attributes.Add(CssClassAttribute, "g-recaptcha");
            div.Attributes.Add(DataSiteKeyAttributeName, _service.SiteKey);
            div.Attributes.Add(DataCallBackAttributeName, RecaptchaInvisibleScriptTagHelper.GetOnSubmitFunctionName(FormId));
            div.Attributes.Add(DataSizeAttributeName, "invisible");

            return div;
        }
    }
}
