using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class MatchVisibility
    {
        public const string ShowAll = "Show All";
        public const string OnlySavedMatches = "Show Saved Matches";
        public const string OnlyUnsavedMatches = "Show UnSaved Matches";
        public string Id { get; set; }
        public string Name { get; set; }

        public static MatchVisibility FromXml(XElement element)
        {
            const string idElement = "Visiblity_Status_Idx";
            const string nameElement = "Visiblity_Status_Name";

            return new MatchVisibility
            {
                Id = element.GetValue<string>(idElement),
                Name = element.GetValue<string>(nameElement),
            };
        }
    }
}
