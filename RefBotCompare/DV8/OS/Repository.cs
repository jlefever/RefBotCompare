using System;
using System.IO;

namespace RefBotCompare.DV8.OS
{
    // A handle for a git repository on the filesystem
    public class Repository
    {
        public string Name { get; }
        public string Dir { get; }
        public Uri Url { get; }

        public Repository(string name, string dir, Uri url)
        {
            Name = name;
            Dir = dir;
            Url = url;
        }

        public bool IsCloned()
        {
            return DirectoryExists(GitDir);
        }

        public bool HasDependancyInfo()
        {
            return FileExists(DependancyInfoSuffix);
        }

        public bool HasHistoryLog()
        {
            return FileExists(HistoryLogSuffix);
        }

        public bool HasHistoryDSM()
        {
            return FileExists(HistoryDSMSuffix);
        }

        public bool HasStructuralDSM()
        {
            return FileExists(StructuralDSMSuffix);
        }

        public bool HasMergedDSM()
        {
            return FileExists(MergedDSMSuffix);
        }

        public bool HasFlaws()
        {
            return DirectoryExists(FlawsDir);
        }

        public bool HasTextualFlaws()
        {
            throw new NotImplementedException();
        }

        private bool FileExists(string suffix)
        {
            return File.Exists(Path.Combine(Dir, Name, Name + suffix));
        }

        private bool DirectoryExists(string dir)
        {
            return Directory.Exists(Path.Combine(Dir, Name, dir));
        }

        private const string GitDir = ".git";
        private const string FlawsDir = ".flaws";
        private const string DependancyInfoSuffix = "-depends.json";
        private const string HistoryLogSuffix = "-git-history.txt";
        private const string HistoryDSMSuffix = "-history.dv8-dsm";
        private const string StructuralDSMSuffix = "-structure.dv8-dsm";
        private const string MergedDSMSuffix = "-merge.dv8-dsm";
    }
}
