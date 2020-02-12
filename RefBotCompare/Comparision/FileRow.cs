using RefBotCompare.DV8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RefBotCompare.Comparision
{
    public class FileRow
    {
        public string FileName { get; set; }
        public int NumOfSolutionMentions { get; set; }
        public int NumOfUniqueSolutionMentions { get; set; }
        public int NumOfFlaws { get; set; }

        public string Clique => string.Join(',', Cliques.OrderBy(a => a));
        public string Crossing => string.Join(',', Crossings.OrderBy(a => a));
        public string ModularityViolation => string.Join(',', ModularityViolations.OrderBy(a => a));
        public string PackageCycle => string.Join(',', PackageCycles.OrderBy(a => a));
        public string UnhealthyInheritance => string.Join(',', UnhealthyInheritances.OrderBy(a => a));
        public string UnstableInterface => string.Join(',', UnstableInterfaces.OrderBy(a => a));

        public IList<int> Cliques { get; set; }
        public IList<int> Crossings { get; set; }
        public IList<int> ModularityViolations { get; set; }
        public IList<int> PackageCycles { get; set; }
        public IList<int> UnhealthyInheritances { get; set; }
        public IList<int> UnstableInterfaces { get; set; }

        public FileRow(File file)
        {
            FileName = file.Name;
            NumOfSolutionMentions = file.Solutions.Count();
            NumOfUniqueSolutionMentions = file.Solutions.Distinct().Count();
            NumOfFlaws = file.Flaws.Count();

            Cliques = new List<int>();
            Crossings = new List<int>();
            ModularityViolations = new List<int>();
            PackageCycles = new List<int>();
            UnhealthyInheritances = new List<int>();
            UnstableInterfaces = new List<int>();

            foreach (var flaw in file.Flaws)
            {
                switch (flaw.FlawKind)
                {
                    case FlawKind.Clique:
                        Cliques.Add(flaw.Index);
                        break;
                    case FlawKind.Crossing:
                        Crossings.Add(flaw.Index);
                        break;
                    case FlawKind.ModularityViolation:
                        ModularityViolations.Add(flaw.Index);
                        break;
                    case FlawKind.PackageCycle:
                        PackageCycles.Add(flaw.Index);
                        break;
                    case FlawKind.UnhealthyInheritance:
                        UnhealthyInheritances.Add(flaw.Index);
                        break;
                    case FlawKind.UnstableInterface:
                        UnstableInterfaces.Add(flaw.Index);
                        break;
                }
            }
        }
    }
}
