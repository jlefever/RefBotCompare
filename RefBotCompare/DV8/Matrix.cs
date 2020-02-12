using System;
using System.Collections.Generic;

namespace RefBotCompare.DV8
{
    public class Matrix
    {
        public string Name { get; }
        public string BaseName { get; }
        public int Index { get; }
        public IList<string> FileNames { get; }

        public Matrix(string name, IList<string> variables)
        {
            Name = GetNameWithoutSuffix(name);
            FileNames = variables;
            var index = FindFirstDigit(Name);
            BaseName = Name.Substring(0, index);
            Index = int.Parse(Name.Substring(index));
        }

        private static string GetNameWithoutSuffix(string name)
        {
            return name.Substring(0, name.LastIndexOf('-'));
        }

        private static int FindFirstDigit(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]))
                {
                    return i;
                }
            }

            throw new Exception("Matrix has no index.");
        }
    }
}
