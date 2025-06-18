using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Routes.API;
using Routes.Domain.ViewModels;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Routes.Data.Utils;

namespace Routes.Tests.Integration.Fixtures
{
    public class TestContext
    {
        private IWebHostBuilder _webHostBuilder;
        public HttpClient _client { get; set; }
        private TestServer _host;
        public UsuarioViewModel GetUserAdmin = new UsuarioViewModel { CPF = "11122233398", Senha = "YWRtaW4=" };
        public TestContext()
        { }
        public async Task SetupClient()
        {
            string curDir = Directory.GetCurrentDirectory();
            _webHostBuilder = new WebHostBuilder().UseContentRoot(curDir).UseStartup<Program>();
            _host = new TestServer(_webHostBuilder);
            _client = _host.CreateClient();
            await AddAuthentication();
        }
        private async Task AddAuthentication()
        {
            await Task.Run(() =>
            {
                var json = GetUserAdmin.ToJson();
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = _client.PostAsync("/api/User/Login", stringContent).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                TokenViewModel token = JsonConvert.DeserializeObject<TokenViewModel>(result);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            });
        }
    }
}