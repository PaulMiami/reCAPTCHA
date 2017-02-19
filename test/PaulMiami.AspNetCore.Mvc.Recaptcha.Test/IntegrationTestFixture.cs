#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class IntegrationTestFixture: IDisposable
    {
        private readonly TestServer _server;

        public IntegrationTestFixture()
        {
            var contentRoot = Path.Combine("..", "..", "..", "..", "WebSites", "TestSite");

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseStartup(typeof(TestSite.Startup));

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }


        protected void InitializeServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRecaptcha(config =>
            {
                config.SecretKey = "SecretKey";
                config.SiteKey = "SiteKey";
                config.BackchannelHttpHandler= new TestHttpMessageHandler
                {
                    Sender = async req =>
                    {
                        var content = await req.Content.ReadAsStringAsync();

                        if (req.RequestUri.AbsoluteUri == "https://www.google.com/recaptcha/api/siteverify")
                        {
                            RecaptchaValidationResponse result;
                            if (content == $"secret=SecretKey&response=Good&remoteip=")
                                result = new RecaptchaValidationResponse { Success = true };
                            else if (content == $"secret=SecretKey&response=Bad&remoteip=")
                                result = new RecaptchaValidationResponse { Success = false, ErrorCodes = new List<string> { "invalid-input-response" } };
                            else
                                result = new RecaptchaValidationResponse { Success = false };

                            var res = new HttpResponseMessage(HttpStatusCode.OK);
                            var text = JsonConvert.SerializeObject(result);
                            res.Content = new StringContent(text, Encoding.UTF8, "application/json");
                            return res;
                        }

                        throw new NotImplementedException(req.RequestUri.AbsoluteUri);
                    }
                };
            });
        }
    }
}
