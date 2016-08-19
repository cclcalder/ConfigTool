using System;
using Model.DataAccess;
 
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Model.Language
{
   
    public class CurrentLanguageSet
    {
        public string LanguageCode { get; set; }
        public ObservableCollection<LanguageSet> AppLabels { get; set; }

        public CurrentLanguageSet()
        { }

        public CurrentLanguageSet(string lang)
        {
            LanguageCode = lang;
            var xDoc = LoginAccess.GetLanguageSet(lang);
            //var list = new List<LanguageSet>();
           //var res  = xDoc.Descendants("AppLabels").ToDictionary(x => x.Descendants("LabelKey").First().Value, x => x.Descendants("LabelValue").First().Value);
            AppLabels = new ObservableCollection<LanguageSet>(xDoc.Descendants("AppLabels").Select(r => new LanguageSet(r)));

            var yDoc = new LanguageAccess().GetAllMessages(lang);
            var msgs = new ObservableCollection<LanguageSet>(yDoc.Descendants("SQLMessages").Select(r => new LanguageSet(r)));
            AppLabels.AddRange(msgs);

        }

        /// <summary>
        /// Return langauge value or fallback
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public string GetValue(string code, string fallback)
        {
            var x = AppLabels.FirstOrDefault(r => r.Code == code); 
            return x == null ? fallback : x.Name;
        }

        public string GetValue(string code)
        {
            var x = AppLabels.FirstOrDefault(r => r.Code == code);
            return x == null ? "No translation" : x.Name;
        }

        public bool Contains(string code)
        {
            return AppLabels.Any(r => r.Code == code);
        }
    }
}
