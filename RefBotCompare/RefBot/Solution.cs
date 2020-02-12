using System.Collections.Generic;

namespace RefBotCompare.RefBot
{
    public class Solution
    {
        public int SolutionId { get; }
        public int ClusterId { get; }
        public IEnumerable<string> FileNames { get; }

        public Solution(int solutionId, int clusterId, IEnumerable<string> files)
        {
            SolutionId = solutionId;
            ClusterId = clusterId;
            FileNames = files;
        }
    }
}
