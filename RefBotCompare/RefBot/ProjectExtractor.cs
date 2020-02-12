using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefBotCompare.RefBot
{
    public class ProjectExtractor
    {
        private readonly IHtmlParser _parser;

        public ProjectExtractor(IHtmlParser parser)
        {
            _parser = parser;
        }

        public async Task<Project> ExtractProject(string html)
        {
            var document = await _parser.ParseDocumentAsync(html);
            return new Project(ExtractSolutions(document));
        }

        private IEnumerable<Solution> ExtractSolutions(IHtmlDocument document)
        {
            var table = document.GetElementById(TableId) as IHtmlTableElement;

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
}
