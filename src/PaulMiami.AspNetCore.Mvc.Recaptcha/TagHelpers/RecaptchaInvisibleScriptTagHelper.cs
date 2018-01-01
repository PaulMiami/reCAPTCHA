using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.TagHelpers
{
    [HtmlTargetElement("recaptcha-invisible-script", Attributes = FormIdAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RecaptchaInvisibleScriptTagHelper : TagHelper
    {
        private const string FormIdAttributeName = "form-id";
        private const string JqueryValidationAttributeName = "jquery-validation";

        private const string ScriptSnippetJValidate = "function {0}(token) {{ document.getElementById('{1}').submit(); }} function {2}(e) {{ e.preventDefault(); $('#{1}').validate(); if ($('#{1}').valid()) grecaptcha.execute(); }}";
        private const string ScriptSnippetNoValidate = "function {0}(token) {{ document.getElementById('{1}').submit(); }} function {2}(e) {{ e.preventDefault(); grecaptcha.execute(); }}";

        private readonly IRecaptchaConfigurationService _service;

        [HtmlAttributeName(FormIdAttributeName)]
        public string FormId { get; set; }

        [HtmlAttributeName(JqueryValidationAttributeName)]
        public bool? JqueryValidation { get; set; }

        public RecaptchaInvisibleScriptTagHelper(RecaptchaService service)
        {
            service.CheckArgumentNull(nameof(service));

            _service = service;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_service.Enabled)
            {
                output.TagName = null;
                return;
            }

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(GetScript());

            base.Process(context, output);
        }

        private string GetScript()
        {
            if (JqueryValidation ?? true)
            {
                return string.Format(ScriptSnippetJValidate, GetOnSubmitFunctionName(FormId), FormId, GetOnClickFunctionName(FormId));
            }
            else
            {
                return string.Format(ScriptSnippetNoValidate, GetOnSubmitFunctionName(FormId), FormId, GetOnClickFunctionName(FormId));
            }
        }

        internal static string GetOnSubmitFunctionName(string formId)
        {
            return GetFunctionNameForFormId(formId, "onSubmit");
        }

        internal static string GetOnClickFunctionName(string formId)
        {
            return GetFunctionNameForFormId(formId, "onClick");
        }

        private static string GetFunctionNameForFormId(string formId, string prefix)
        {
            var functionNameBuilder = new StringBuilder(prefix);

            foreach (var match in Regex.Matches(formId, "[A-Za-z0-9_]+"))
            {
                var pascalCasedPart = match.ToString().First().ToString().ToUpper() + match.ToString().Substring(1);
                functionNameBuilder.Append(pascalCasedPart);
            }

            return functionNameBuilder.ToString();
        }
    }
}