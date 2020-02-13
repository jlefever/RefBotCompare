using System;

namespace RefBotCompare.Configuration
{
    public class ProjectConfig
    {
        public string Name { get; set; }
        public int RefBotId { get; set; }
        public Uri GitUrl { get; set; }
    }
}
