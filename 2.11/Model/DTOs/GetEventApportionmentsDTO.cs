using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class GetEventApportionmentsDTO
    {
        public string EventId { get; set; }
        public IList<string> ClaimIds { get; set; }
    }
}
