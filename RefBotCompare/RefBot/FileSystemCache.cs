using System;
using System.IO;
using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public class FileSystemCache : ICache<string>
    {
        private readonly string _directory;

        public FileSystemCache(string directory)
        {
            _directory = directory;
        }

        public async Task<string> GetAsync(string key, Func<Task<string>> loadFunc)
        {
            var filename = key + ".txt";
            var path = Path.Combine(_directory, filename);

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            var text = await loadFunc();
            File.WriteAllText(path, text);
            return text;
        }
    }
}
