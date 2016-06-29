#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public interface IRecaptchaValidationService
    {
        Task ValidateResponseAsync(string response, string remoteIp);
    }
}
