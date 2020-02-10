using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RefBotCompare
{
    public class FlawImporter
    {
        public static IEnumerable<string> FindFilesWithSuffix(string directory, string suffix)
        {
            var files = new List<string>();

            foreach (string file in Directory.GetFiles(directory))
            {
                if (file.EndsWith(suffix))
                {
                    files.Add(file);
                }
            }

            foreach (string dir in Directory.GetDirectories(directory))
            {
                files.AddRange(FindFilesWithSuffix(dir, suffix));
            }

            return files;
        }

        public static IEnumerable<string> FindMatrices(string directory)
        {
            return FindFilesWithSuffix(directory, MergeDv8DsmSuffix);
        }

        public static IEnumerable<string> FindJson(string directory)
        {
            return FindFilesWithSuffix(directory, ".json");
        }

        public static void ConvertToJson(string file)
        {
            Console.WriteLine("Attempting " + file + "...");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    Arguments = "/C dv8-console core:export-matrix " + file
                }
            };

            process.Start();
            process.WaitForExit();
        }

        public static void Main(string[] args)
        {
            foreach (var matrixFile in FindMatrices("C:\\Users\\jtl86\\Documents\\ant_analysis\\ant_flaws"))
            {
                ConvertToJson(matrixFile);
            }
        }

        private const string MergeDv8DsmSuffix = "-merge.dv8-dsm";
    }
}
