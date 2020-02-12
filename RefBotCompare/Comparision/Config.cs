using System.Collections.Generic;

namespace RefBotCompare
{
    public class Config
    {
        public string SessionCookie { get; set; }
        public string CacheDir { get; set; }
        public int MaxRefBotRequests { get; set; }
        public IEnumerable<ProjectConfig> Projects { get; set; }
    }
}
