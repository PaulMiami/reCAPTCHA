#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
	public interface IRecaptchaConfigurationService
	{
		bool Enabled { get; }

		string JavaScriptUrl { get; }

		string ValidationMessage { get; }

		string SiteKey { get; }

		RecaptchaControlSettings ControlSettings { get; }

		string LanguageCode { get; }
	}
}
