//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace RefBotCompare
//{
//    public class RunConfig
//    {
//        public IDictionary<string, string> Cookies { get; }
//        public string ProjectUrl { get; set; }
//    }

//    public class HttpClientFactory
//    {
//        public HttpClient CreateHttpClient(IDictionary<string, string> cookies)
//        {
//            var container = new CookieContainer();
            
//            foreach(var cookie in cookies)
//            {
//                container.Add(new Cookie(cookie.Key, cookie.Value));
//            }

//            return new HttpClient(new HttpClientHandler() { CookieContainer = container });
//        }
//    }

//    public class CachedRefBotClient
//    {

//    }

//    public class RefBotClient
//    {
//        private readonly HttpClient _httpClient;

//        public RefBotClient(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }
//    }

//    public interface ICache<T>
//    {
//        Task<T> TryGetAsync(string key, Func<Task<T>> loadFunc);
//    }

//    public class FileSystemCache : ICache<string>
//    {
//        private readonly string _directory;

//        public FileSystemCache(string directory)
//        {
//            _directory = directory;
//        }

//        public async Task<string> TryGetAsync(string key, Func<Task<string>> loadFunc)
//        {
//            var filename = key + ".txt";
//            var path = Path.Combine(_directory, filename);

//            if (File.Exists(path))
//            {
//                return File.ReadAllText(path);
//            }

//            if (!Directory.Exists(_directory))
//            {
//                Directory.CreateDirectory(_directory);
//            }

//            var text = await loadFunc();
//            File.WriteAllText(path, text);
//            return text;
//        }
//    }
//}
