using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RefBotCompare
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

    public enum FlawKind
    {
        Clique,
        Crossing,
        ModularityViolation,
        PackageCycle,
        UnhealthyInheritance,
        UnstableInterface
    }

    public class Flaw
    {
        public FlawKind FlawKind { get; set; }
        public int FlawIndex { get; set; }
        public string Name { get; set; }
        public IList<string> FileNames { get; set; }
    }

    public class DV8Matrix
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("variables")]
        public IList<string> Variables { get; set; }
    }

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

    public class RefBotDocument
    {
        private readonly IHtmlDocument _document;

        public RefBotDocument(IHtmlDocument document)
        {
            _document = document;
        }

        public Project FindProject()
        {
            return new Project(FindSolutions());
        }

        public IEnumerable<Solution> FindSolutions()
        {
            var table = _document.GetElementById(TableId) as IHtmlTableElement;

            foreach (var row in table.Rows.Skip(RowStartIndex))
            {
                var solutionIdText = row.Children[SolutionIdColIndex].TextContent;
                var solutionId = Convert.ToInt32(solutionIdText);

                var clusterIdText = row.Children[ClusterIdColIndex].TextContent;
                var clusterId = Convert.ToInt32(clusterIdText);

                var filesText = row.Children[FilesColIndex].TextContent;
                var files = filesText.Split(',').Select(f => f.Trim());

                yield return new Solution(solutionId, clusterId, files);
            }
        }

        private const string TableId = "html_table_all_solutions";
        private const int RowStartIndex = 1;
        private const int SolutionIdColIndex = 0;
        private const int ClusterIdColIndex = 2;
        private const int FilesColIndex = 17;
    }

    public class Matcher
    {
        private readonly IEnumerable<Solution> _solutions;
        private readonly IEnumerable<Flaw> _flaws;
        private readonly IDictionary<string, File> _files;

        public Matcher(IEnumerable<Solution> solutions, IEnumerable<Flaw> flaws)
        {
            _solutions = solutions;
            _flaws = flaws;
            _files = new Dictionary<string, File>();
        }

        public void FindFiles()
        {
            foreach (var flaw in _flaws)
            {
                foreach (var fileName in flaw.FileNames)
                {
                    var name = FlawFileNameToSolutionFileName(fileName);

                    if (!_files.ContainsKey(name))
                    {
                        _files.Add(name, new File(name));
                    }

                    _files[name].Flaws.Add(flaw);
                }
            }

            foreach (var solution in _solutions)
            {
                foreach (var fileName in solution.FileNames)
                {
                    if (!_files.ContainsKey(fileName))
                    {
                        _files.Add(fileName, new File(fileName));
                    }

                    _files[fileName].Solutions.Add(solution);
                }
            }
        }

        public IEnumerable<File> GetFiles()
        {
            return _files.Values;
        }

        public IEnumerable<File> GetIntersectingFiles()
        {
            foreach (var file in _files)
            {
                if (file.Value.HasBoth())
                {
                    yield return file.Value;
                }
            }
        }

        private string FlawFileNameToSolutionFileName(string name)
        {
            var prefix = "src/main/";
            var suffix = ".java";

            if (name.EndsWith(suffix))
            {
                name = name.Remove(name.Length - suffix.Length);
            }

            if (name.StartsWith(prefix))
            {
                name = name.Remove(0, prefix.Count());
            }

            return name.Replace('/', '.');
        }
    }

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
                        Cliques.Add(flaw.FlawIndex);
                        break;
                    case FlawKind.Crossing:
                        Crossings.Add(flaw.FlawIndex);
                        break;
                    case FlawKind.ModularityViolation:
                        ModularityViolations.Add(flaw.FlawIndex);
                        break;
                    case FlawKind.PackageCycle:
                        PackageCycles.Add(flaw.FlawIndex);
                        break;
                    case FlawKind.UnhealthyInheritance:
                        UnhealthyInheritances.Add(flaw.FlawIndex);
                        break;
                    case FlawKind.UnstableInterface:
                        UnstableInterfaces.Add(flaw.FlawIndex);
                        break;
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var html = System.IO.File.ReadAllText(@"C:\Users\jtl86\Desktop\refbot_apache_ant.html");

            var document = new HtmlParser().ParseDocument(html);

            var project = new RefBotDocument(document).FindProject();

            var matrices = ReadMatrices(@"C:\Users\jtl86\Documents\ant_analysis\ant_flaws\json\");

            var flaws = ToFlaws(matrices);

            var matcher = new Matcher(project.Solutions, flaws);
            
            matcher.FindFiles();

            var files = matcher.GetFiles();

            var rows = files.Select(f => new FileRow(f));

            var csvFileName = @"C:\Users\jtl86\Documents\ant_analysis\ant_flaws\comparison.csv";

            using (var writer = new StreamWriter(csvFileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(rows);
                }
            }
        }

        public static IEnumerable<Flaw> ToFlaws(IEnumerable<DV8Matrix> matrices)
        {
            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            foreach (var matrix in matrices)
            {
                var suffix = "-merge";

                var name = matrix.Name.Remove(matrix.Name.Length - suffix.Length);

                var index = name.IndexOfAny(digits);

                var flawIndex = int.Parse(name.Substring(index));

                var kind = ToFlawKind(name.Substring(0, index));

                yield return new Flaw
                {
                    FlawKind = kind,
                    FlawIndex = flawIndex,
                    Name = name,
                    FileNames = matrix.Variables
                };
            }
        }

        public static FlawKind ToFlawKind(string name)
        {
            if (name == "Clique")
            {
                return FlawKind.Clique;
            }
            else if (name == "Crossing")
            {
                return FlawKind.Crossing;
            }
            else if (name == "ModularityViolation")
            {
                return FlawKind.ModularityViolation;
            }
            else if (name == "PackageCycle")
            {
                return FlawKind.PackageCycle;
            }
            else if (name == "UnhealthyInheritance")
            {
                return FlawKind.UnhealthyInheritance;
            }
            else if (name == "UnstableInterface")
            {
                return FlawKind.UnstableInterface;
            }

            throw new Exception();
        }

        public static IEnumerable<DV8Matrix> ReadMatrices(string directory)
        {
            var paths = Directory.GetFiles(directory);

            foreach (var path in paths)
            {
                var text = System.IO.File.ReadAllText(path);
                yield return JsonConvert.DeserializeObject<DV8Matrix>(text);
            }
        }

        public static void PrintCounts(Project project)
        {
            var counts = project.CountUniqueMentions();

            foreach (var count in counts.OrderByDescending(c => c.Value))
            {
                Console.WriteLine(count.Key + "\t\t\t" + count.Value);
            }
        }
    }
}
