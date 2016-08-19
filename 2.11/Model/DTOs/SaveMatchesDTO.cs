using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.DTOs
{
    public class ClaimEventMatch
    {
        public string ClaimId { get; set; }
        public string EventId { get; set; }
    }

    public class SaveMatchesDTO
    {
        public SaveMatchesDTO()
        {
            this.Matches = new List<ClaimEventMatch>();
            this.ClaimIds = new List<string>();
            this.EventIds = new List<string>(); 
        }
        public IList<string> ClaimIds { get; set; }
        public IList<string> EventIds { get; set; }
        public IList<ClaimEventMatch> Matches { get; set; }
    }
}
