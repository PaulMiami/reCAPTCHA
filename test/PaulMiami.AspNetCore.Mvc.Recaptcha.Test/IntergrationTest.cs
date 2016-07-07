#region License
//Copyright(c) Paul Biccherai
//Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PaulMiami.AspNetCore.Mvc.Recaptcha.Test
{
    public class IntergrationTest : IClassFixture<IntegrationTestFixture>
    {
        public IntergrationTest(IntegrationTestFixture fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ValidResponse()
        {
            var formParams = new Dictionary<string, string>();
            formParams["g-recaptcha-response"] = "Good";

            var response = await Client.PostAsync("http://localhost", new FormUrlEncodedContent(formParams));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("OK", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SkipValidation()
        {
            var response = await Client.GetAsync("http://localhost");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("HELLO", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task UnvalidResponse()
        {
            var formParams = new Dictionary<string, string>();
            formParams["g-recaptcha-response"] = "Bad";

            var response = await Client.PostAsync("http://localhost", new FormUrlEncodedContent(formParams));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("FAIL", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task MissingResponse()
        {
            var formParams = new Dictionary<string, string>();

            var response = await Client.PostAsync("http://localhost", new FormUrlEncodedContent(formParams));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("FAIL", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task BadRequest()
        {
            var formParams = new Dictionary<string, string>();
            formParams["g-recaptcha-response"] = "somethingWrongWithTheServer";

            var response = await Client.PostAsync("http://localhost", new FormUrlEncodedContent(formParams));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task BadRequestNoForm()
        {
            var response = await Client.PostAsync("http://localhost", new StringContent("somethingbad"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestControlRender()
        {
            var response = await Client.GetAsync("http://localhost/Home/RenderControl");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("<div class=\"g-recaptcha\" data-sitekey=\"SiteKey\" data-callback=\"recaptchaValidated\"></div>", await response.Content.ReadAsStringAsync());
        }
    }
}
