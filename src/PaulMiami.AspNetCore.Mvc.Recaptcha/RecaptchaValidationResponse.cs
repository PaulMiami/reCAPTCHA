#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha
{
    public class RecaptchaValidationResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }

    }
}
