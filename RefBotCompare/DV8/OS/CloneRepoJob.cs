using System;

namespace RefBotCompare.DV8.OS
{
    public class CloneRepoJob : Job
    {
        public CloneRepoJob(
            string exec,
            string repoDir,
            string repoName,
            Uri repoUrl,
            JobOutputHandler handler
            ) : base(exec, $"clone {repoUrl} {repoName}", repoDir, handler) { }
    }
}
