using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public sealed class CachedRefBotClient : IRefBotClient
    {
        private readonly IRefBotClient _client;
        private readonly ICache<string> _cache;

        public CachedRefBotClient(IRefBotClient client, ICache<string> cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<string> FetchProjectHtml(int id)
        {
            return await _cache.GetAsync(id.ToString(), () => _client.FetchProjectHtml(id));
        }
    }
}
