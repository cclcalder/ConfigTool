using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic; 
using System.Text;
using Model;

namespace WPF.Test
{
    [TestClass]
    public class Claims
    {
        [TestMethod]
        public void ReadXML()
        {
            var xml = @"<UserDefault><ClaimStartDate>2010-12-01</ClaimStartDate><ClaimEndDate>2014-12-31</ClaimEndDate><EventStartDate>2013-01-01</EventStartDate><EventEndDate>2013-12-31</EventEndDate><Date_Search_Preference>0</Date_Search_Preference></UserDefault>";
            XElement element = GetElement(xml);
            
            const string dateFormat = "yyyy-MM-dd";
            const string dateSearchPreferenceElement = "Date_Search_Preference";
            const string claimStartDateElement = "ClaimStartDate";
            const string claimEndDateElement = "ClaimEndDate";
            const string eventStartDateElement = "EventStartDate";
            const string eventEndDateElement = "EventEndDate";

            //var res =   new Model.Entity.ClaimUserDefaults
            //{
              var  DateSearchPreference = element.GetValue<string>(dateSearchPreferenceElement);
              var ClaimStartDate = DateTime.ParseExact(element.GetValue<string>(claimStartDateElement), dateFormat, null);
              var ClaimEndDate = DateTime.ParseExact(element.GetValue<string>(claimEndDateElement), dateFormat, null);
              var EventStartDate = DateTime.ParseExact(element.GetValue<string>(eventStartDateElement), dateFormat, null);
              var EventEndDate = DateTime.ParseExact(element.GetValue<string>(eventEndDateElement), dateFormat, null);
            //};


              Assert.IsTrue(DateSearchPreference.ToString() == "0");
              Assert.IsTrue(ClaimStartDate  == new  DateTime(2010,12,01));
              Assert.IsTrue(ClaimEndDate == new  DateTime(2014,12,31));
              Assert.IsTrue(EventStartDate  == new  DateTime(2013,01,01));
              Assert.IsTrue(EventEndDate  == new  DateTime(2013,12,31));
        }


        private static XElement GetElement(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.Root;
        }

    }
}
