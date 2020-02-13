using System.Collections.Generic;

namespace RefBotCompare.Configuration
{
    public class Config
    {
        public string SessionCookie { get; set; }
        public string CacheDir { get; set; }
        public string RepoDir { get; set; }
        public string GitExec { get; set; }
        public int MaxRefBotRequests { get; set; }
        public ExecutablesConfig Executables { get; set; }
        public IEnumerable<ProjectConfig> Projects { get; set; }
    }
}
