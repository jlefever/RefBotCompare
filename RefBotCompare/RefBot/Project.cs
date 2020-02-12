using System.Collections.Generic;
using System.Linq;

namespace RefBotCompare.RefBot
{
    public class Project
    {
        public IEnumerable<Solution> Solutions { get; }

        public Project(IEnumerable<Solution> solutions)
        {
            Solutions = solutions;
        }

        public IDictionary<string, int> CountTotalMentions()
        {
            return CountFiles(Solutions.SelectMany(s => s.FileNames));
        }

        public IDictionary<string, int> CountUniqueMentions()
        {
            return CountFiles(Solutions.SelectMany(s => s.FileNames.Distinct()));
        }

        private static IDictionary<string, int> CountFiles(IEnumerable<string> files)
        {
            var counts = new Dictionary<string, int>();

            foreach (var file in files)
            {
                counts.TryAdd(file, 0);
                counts[file] = counts[file] + 1;
            }

            return counts;
        }
    }
}
