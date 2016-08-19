using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class AutomaticMatchDTO
    {
        public IList<string> ClaimIds { get; set; }
        public IList<string> EventIds { get; set; }
    }
}
