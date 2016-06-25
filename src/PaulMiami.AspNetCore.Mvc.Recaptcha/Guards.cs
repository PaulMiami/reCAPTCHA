#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    internal static class Guards
    {
        public static void CheckArgumentNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

        public static void CheckMandatoryOption(this string s, string name)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException(string.Format(Resources.Exception_OptionMustBeProvided, name));
        }
    }
}
