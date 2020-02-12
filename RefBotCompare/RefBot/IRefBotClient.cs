using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public interface IRefBotClient
    {
        Task<string> FetchProjectHtml(int id);
    }
}
