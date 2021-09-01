using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiffingApi.V1.Models
{
    public class Result
    {
        [JsonProperty("diffResultType")]
        public string DiffResultType { get; set; }

        [JsonProperty("diffs")]
        public List<Difference> Diffs { get; set; }
    }
}
