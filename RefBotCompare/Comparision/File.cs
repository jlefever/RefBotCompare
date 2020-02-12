using RefBotCompare.DV8;
using RefBotCompare.RefBot;
using System.Collections.Generic;

namespace RefBotCompare.Comparision
{
    public class File
    {
        public string Name { get; }
        public IList<Solution> Solutions { get; }
        public IList<Flaw> Flaws { get; }

        public File(string name)
        {
            Name = name;
            Solutions = new List<Solution>();
            Flaws = new List<Flaw>();
        }

        public bool HasBoth()
        {
            return Solutions.Count != 0 && Flaws.Count != 0;
        }
    }
}
