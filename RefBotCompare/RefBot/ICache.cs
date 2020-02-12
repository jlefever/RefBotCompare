using System;
using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public interface ICache<T>
    {
        Task<T> GetAsync(string key, Func<Task<T>> loadFunc);
    }
}
