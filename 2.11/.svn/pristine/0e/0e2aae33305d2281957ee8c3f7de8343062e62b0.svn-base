using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity
{
    public class ClaimUserDefaults
    {
        public string DateSearchPreference { get; set; }
        public DateTime ClaimStartDate { get; set; }
        public DateTime ClaimEndDate { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string SalesOrg_Idx { get; set; }

        public static ClaimUserDefaults FromXml(XElement element)
        {
            //ch 2014-02-18 changed to fix
            //const string dateFormat = "yyyyMMdd";         
            const string dateFormat = "yyyy-MM-dd";
            //const string dateSearchPreferenceElement = "<Date_Search_Preference";
            const string dateSearchPreferenceElement = "Date_Search_Preference";
            const string claimStartDateElement = "ClaimStartDate";
            const string claimEndDateElement = "ClaimEndDate";
            const string eventStartDateElement = "EventStartDate";
            const string eventEndDateElement = "EventEndDate";
            const string salesOrg_Idx = "SalesOrg_Idx";

            return new ClaimUserDefaults
            {
                DateSearchPreference = element.GetValue<string>(dateSearchPreferenceElement),
                ClaimStartDate = DateTime.ParseExact(element.GetValue<string>(claimStartDateElement), dateFormat, null),
                ClaimEndDate = DateTime.ParseExact(element.GetValue<string>(claimEndDateElement), dateFormat, null),
                EventStartDate = DateTime.ParseExact(element.GetValue<string>(eventStartDateElement), dateFormat, null),
                EventEndDate = DateTime.ParseExact(element.GetValue<string>(eventEndDateElement), dateFormat, null),
                SalesOrg_Idx = element.GetValue<string>(salesOrg_Idx),
            };
        }
    }
}
