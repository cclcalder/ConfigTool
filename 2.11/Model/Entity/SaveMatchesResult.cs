using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class SaveMatchesResult
    {
        public string PreExisting { get; set; }
        public string Submitted { get; set; }
        public string SavedMatches { get; set; }
        public string DeletedMatches { get; set; }
        public string NotSavedMatches { get; set; }
        public string NotDeletedMatches { get; set; }
        public string Message { get; set; }
        public string DisplayMessage 
        { 
            get 
            {
                string.Format("Pre existing matches:{0} \n Submitted matches:{1} \n Saved matches:{2} \n Deletd matches:{3} \n Not Saved :matches{4} \n Not Deleted matches:{5}", PreExisting, Submitted, SavedMatches, DeletedMatches, NotSavedMatches, NotDeletedMatches);
                return string.Empty; 
            } 
        }

        public static SaveMatchesResult FromXml(XElement element)
        {
            const string matchesPreExistingElement = "Matches_PreExisting";
            const string matchesSubmittedElement = "Matches_Submitted";
            const string matchesSavedElement = "Matches_Saved";
            const string matchesDeletedElement = "Matches_Deleted";
            const string matchesNotSavedElement = "Matches_Not_Saved";
            const string matchesNotDeletedElement = "Matches_Not_Deleted";
            //const string messageElement = "Outcome";
            return new SaveMatchesResult() {
                SavedMatches = element.GetValue<string>(matchesSavedElement),
                DeletedMatches = element.GetValue<string>(matchesDeletedElement),
                PreExisting = element.GetValue<string>(matchesPreExistingElement),
                Submitted = element.GetValue<string>(matchesSubmittedElement),
                NotSavedMatches = element.GetValue<string>(matchesNotSavedElement),
                NotDeletedMatches = element.GetValue<string>(matchesNotDeletedElement),
                Message = element.Value
            };
        }
    }
}
