using System;
using System.Collections.Generic;

namespace Exceedra.Common.Entity
{
    public class Contract
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> CustomersIds { get; set; }
    }
}
