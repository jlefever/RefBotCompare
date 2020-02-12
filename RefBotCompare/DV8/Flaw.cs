using System;
using System.Collections.Generic;

namespace RefBotCompare.DV8
{
    public class Flaw : Matrix
    {
        public FlawKind FlawKind { get; }

        public Flaw(string name, IList<string> variables) : base(name, variables)
        {
            if (!Enum.TryParse(typeof(FlawKind), BaseName, out var kind))
            {
                throw new Exception("Not a valid flaw.");
            }

            FlawKind = (FlawKind)kind;
        }
    }
}
