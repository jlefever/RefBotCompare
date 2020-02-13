using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RefBotCompare.RefBot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AngleSharp.Html.Parser;
using RefBotCompare.DV8;

namespace RefBotCompare
{
    public class Program
    {
        public static void Main()
        {
            var path = @"C:\Users\jtl86\source\repos\RefBotCompare\RefBotCompare\config.json";
            var config = LoadConfig(path);
            var handler = new JobOutputHandler(Console.Out);
            var url = "https://github.com/jlefever/BlockDemo/";
            var p = new Job(config.GitExec, url, config.RepoDir, "blocks", handler);
            p.Exec();
            p.Dispose();
        }

        //public static void Main(string[] args)
        //{
        //    var path = @"C:\Users\jtl86\source\repos\RefBotCompare\RefBotCompare\config.json";

        //    var config = LoadConfig(path);

        //    var client = CreateRefBotClient(config);

        //    var extractor = new ProjectExtractor(new HtmlParser());

        //    foreach (var p in config.Projects)
        //    {
        //        var html = client.FetchProjectHtml(p.RefBotId).GetAwaiter().GetResult();
        //        var project = extractor.ExtractProject(html);
        //    }
        //}

        public static IRefBotClient CreateRefBotClient(Config config)
        {
            var cache = new FileSystemCache(config.CacheDir);

            var client = new RefBotClient(config.SessionCookie);

            var rateLimited = new RateLimitedRefBotClient(client, config.MaxRefBotRequests);

            return new CachedRefBotClient(rateLimited, cache);
        }

        public static Config LoadConfig(string path)
        {
            var text = File.ReadAllText(path);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                MissingMemberHandling = MissingMemberHandling.Error
            };

            return JsonConvert.DeserializeObject<Config>(text, settings);
        }

        //public static void Main(string[] args)
        //{
        //    var html = System.IO.File.ReadAllText(@"C:\Users\jtl86\Desktop\refbot_apache_ant.html");

        //    var document = new HtmlParser().ParseDocument(html);

        //    var project = new RefBotDocument(document).FindProject();

        //    var flaws = ReadFlaws(@"C:\Users\jtl86\Documents\ant_analysis\ant_flaws\json\").ToArray();

        //    var matcher = new Matcher(project.Solutions, flaws);

        //    matcher.FindFiles();

        //    var files = matcher.GetFiles();

        //    var rows = files.Select(f => new FileRow(f));

        //    var csvFileName = @"C:\Users\jtl86\Documents\ant_analysis\ant_flaws\comparison.csv";

        //    using (var writer = new StreamWriter(csvFileName))
        //    {
        //        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //        {
        //            csv.WriteRecords(rows);
        //        }
        //    }
        //}

        public static IEnumerable<Flaw> ReadFlaws(string directory)
        {
            var paths = Directory.GetFiles(directory);

            foreach (var path in paths)
            {
                var text = System.IO.File.ReadAllText(path);
                var obj = JsonConvert.DeserializeObject<Flaw>(text);
                yield return obj;
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
