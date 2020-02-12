using Newtonsoft.Json;
using System.Collections.Generic;

namespace RefBotCompare.DV8
{
    public class RawMatrix
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("variables")]
        public IList<string> Variables { get; set; }
    }
}
