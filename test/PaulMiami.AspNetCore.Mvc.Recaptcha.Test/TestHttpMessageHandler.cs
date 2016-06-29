#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, Task<HttpResponseMessage>> Sender { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (Sender != null)
            {
                return Sender(request);
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
