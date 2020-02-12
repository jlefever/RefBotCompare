using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public sealed class RefBotClient : IRefBotClient, IDisposable
    {
        private readonly HttpClient _httpClient;

        public RefBotClient(string session)
        {
            _httpClient = CreateHttpClient(session);
        }

        public async Task<string> FetchProjectHtml(int id)
        {
            var uri = new Uri(Schema + Domain + ProjectPath + id);
            var res = await _httpClient.GetAsync(uri);

            if (res.RequestMessage.RequestUri.AbsolutePath != uri.AbsolutePath)
            {
                throw new Exception("Invalid session cookie.");
            }

            return await res.Content.ReadAsStringAsync();
        }

        private HttpClient CreateHttpClient(string session)
        {
            var cookie = new Cookie(CookieName, session, Path, Domain);
            var container = new CookieContainer();
            container.Add(cookie);
            var handler = new HttpClientHandler() { CookieContainer = container };
            return new HttpClient(handler);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }        

        private const string CookieName = "JSESSIONID";
        private const string Path = "/";
        private const string Schema = "http://";
        private const string Domain = "refactor.iselab.us";
        private const string ProjectPath = "/report/project/?projectid=";
    }
}
