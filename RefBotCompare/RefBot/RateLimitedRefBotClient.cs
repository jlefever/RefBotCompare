using System;
using System.Threading;
using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public sealed class RateLimitedRefBotClient : IRefBotClient, IDisposable
    {
        private readonly IRefBotClient _client;
        private readonly SemaphoreSlim _semaphore;

        public RateLimitedRefBotClient(IRefBotClient client, int max)
        {
            _client = client;
            _semaphore = new SemaphoreSlim(0, max);
        }

        public async Task<string> FetchProjectHtml(int id)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await _client.FetchProjectHtml(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
