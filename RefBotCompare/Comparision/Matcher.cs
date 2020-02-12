using RefBotCompare.DV8;
using RefBotCompare.RefBot;
using System.Collections.Generic;

namespace RefBotCompare.Comparision
{
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
                name = name.Remove(0, prefix.Length);
            }

            return name.Replace('/', '.');
        }
    }
}
