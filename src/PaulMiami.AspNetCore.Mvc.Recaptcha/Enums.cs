#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public enum RecaptchaSize
    {
        Normal,
        Compact
    }

    public enum RecaptchaType
    {
        Image,
        Audio
    }

    public enum RecaptchaTheme
    {
        Light,
        Dark
    }
}
