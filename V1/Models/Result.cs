using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiffingApi.V1.Models
{
    public class Result
    {
        public string DiffResultType { get; set; }
        public List<Difference> Diffs { get; set; }
    }
}
