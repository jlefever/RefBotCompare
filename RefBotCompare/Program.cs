using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
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
            return CountFiles(Solutions.SelectMany(s => s.Files));
        }

        public IDictionary<string, int> CountUniqueMentions()
        {
            return CountFiles(Solutions.SelectMany(s => s.Files.Distinct()));
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

    public class Solution
    {
        public int SolutionId { get; }
        public int ClusterId { get; }
        public IEnumerable<string> Files { get; }

        public Solution(int solutionId, int clusterId, IEnumerable<string> files)
        {
            SolutionId = solutionId;
            ClusterId = clusterId;
            Files = files;
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

    public class Program
    {
        public static void Main(string[] args)
        {
            var html = File.ReadAllText(@"C:\Users\jtl86\Desktop\refbot_apache_ant.html");

            var document = new HtmlParser().ParseDocument(html);

            var project = new RefBotDocument(document).FindProject();

            var counts = project.CountUniqueMentions();

            foreach (var count in counts.OrderByDescending(c => c.Value))
            {
                Console.WriteLine(count.Key + "\t\t\t" + count.Value);
            }
        }
    }
}
