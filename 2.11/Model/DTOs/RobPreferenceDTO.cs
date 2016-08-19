namespace Model
{
    using System;
    using System.Collections.Generic;

    public class RobPreferenceDTO
    {        
        public IEnumerable<string> StatusIDs { get; set; }

        public IEnumerable<string> CustomerIDs { get; set; }

        public IEnumerable<string> ProductIDs { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }
    }
}