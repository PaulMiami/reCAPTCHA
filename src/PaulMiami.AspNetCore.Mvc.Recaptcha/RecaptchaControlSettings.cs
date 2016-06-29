#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaControlSettings
    {
        public RecaptchaTheme Theme { get; set; }

        public RecaptchaType Type { get; set; }

        public RecaptchaSize Size { get; set; }
    }
}
