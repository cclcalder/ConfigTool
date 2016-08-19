using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity
{
    public class PromotionSaveResults
    {
        //<Results>
        //  <Promo_Idx>4</Promo_Idx>
        //  <Msg>Success or Disaster Warning Message</Msg>  Just display this to the user
        //  <ValidationStatus>0</ValidationStatus>          0 = everything fine, 1 = error 
        //   <IsAmendable>0</IsAmendable>          0 = everything fine, 1 = error 
        //  <WizardPages>
        //    <Tab>
        //      <WizardTab_Code>Customer</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>                     0 = not started, 1 = green tick, 2 = previously saved but now needs review
        //    </Tab>
        //    <Tab>
        //      <WizardTab_Code>Dates</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>
        //    </Tab>
        //    <Tab>
        //      <WizardTab_Code>Products</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>
        //    </Tab>
        //    <Tab>
        //      <WizardTab_Code>Attributes</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>
        //    </Tab>
        //    <Tab>
        //      <WizardTab_Code>Volumes</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>
        //    </Tab>
        //    <Tab>
        //      <WizardTab_Code>Financials</WizardTab_Code>
        //      <IsCompleted>1</IsCompleted>
        //    </Tab>
        //  </WizardPages>
        // <StatusID>1</StatusID>
        //  <IsAmendable>1</IsAmendable>
        //  <CurrentlyViewingUsers>
        //      <Users>
        //          <User_DisplayName>Test User, Exceedra</User_DisplayName>
        //          <LastSeenTime>2015-01-19T19:22:15.777</LastSeenTime>
        //      </Users>
        //  </CurrentlyViewingUsers>
        //</Results>

        public string Message { get; set; }
        public ValidationStatus ValidationStatus { get; set; }
        public bool IsAmendable { get; set; }
        public string CodeAndName { get; set; }

        public List<PromotionViewingUser> ViewingUsers { get; set; }

        public List<PromotionTab> WizardPages { get; set; }

        public DateTime? LastSaved { get; set; }

        public PromotionSaveResults()
        {
        }

        public PromotionSaveResults(XElement xml)
        {
            if (xml != null)
            {
                var din = xml.Element("LastSaved").MaybeValue();

                DateTime? dt;

                if (!string.IsNullOrEmpty(din))
                {
                    dt = DateTime.Parse(din);
                }
                else
                {
                    dt = null;
                }

                LastSaved = dt;
                Message = xml.GetValue<string>("Msg");
                ValidationStatus = ValidationStatusGetter.Get(xml.Element("ValidationStatus"));
                IsAmendable = (xml.Element("IsAmendable").MaybeValue() == "1");
                WizardPages = new List<PromotionTab>();
                WizardPages =
                    xml.Element("WizardPages").MaybeElements("Tab").Select(op => new PromotionTab(op)).ToList();
                PromotionID = xml.GetValue<string>("Promo_Idx");

                CodeAndName = xml.Element("CodeAndName").MaybeValue();

                try
                {
                    var vu = new List<PromotionViewingUser>();
                    var cv = xml.Element("CurrentlyViewingUsers");
                    foreach (var u in cv.Elements("Users"))
                    {
                        vu.Add(new PromotionViewingUser() { Name = u.GetValue<string>("User_DisplayName"), LastSeenRaw = u.GetValueOrDefault<DateTime>("LastSeenTime") });
                    }

                    ViewingUsers = vu;
                }
                catch (Exception)
                {


                }

            }
        }

        public string PromotionID { get; set; }
    }

    public class PromotionGetResults
    {
//<Results>
//  <Promotion>
//    <ID>109</ID>
//    <Name>AL-8 Booker Price Down</Name>
//    <URL>http://exceedracom.cloudapp.net:10510/ReportServer/Pages/ReportViewer.aspx?%2fESP_Demo_DEV_FoodDrink_Demo_Reports%2fP-01&amp;rs:Command=Render&amp;rs:ClearSession=true&amp;Promo_Idx=109&amp;AntiCache=0.421921&amp;rc:Parameters=false</URL>
//    <WizardStartScreenName>Review</WizardStartScreenName>
//    <WizardPages>
//      <Tab>
//        <WizardTab_Code>Customer</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//      <Tab>
//        <WizardTab_Code>Dates</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//      <Tab>
//        <WizardTab_Code>Products</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//      <Tab>
//        <WizardTab_Code>Attributes</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//      <Tab>
//        <WizardTab_Code>Volumes</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//      <Tab>
//        <WizardTab_Code>Financials</WizardTab_Code>
//        <IsCompleted>1</IsCompleted>
//      </Tab>
//    </WizardPages>
//    <StatusID>1</StatusID>
//    <PromoIsEditable>1</PromoIsEditable>
//  </Promotion>
//</Results>

        public string PromotionID { get; set; }
        public string Name { get; set; }
        public string CodeAndName { get; set; }
        public string URL { get; set; }
        public string WizardStartScreenName { get; set; }
        public List<PromotionViewingUser> ViewingUsers { get; set; }
        public int StatusID { get; set; }
        public List<PromotionTab> WizardPages { get; set; }

        public bool IsAmendable { get; set; }

        public PromotionGetResults()
        {
        }

        public PromotionGetResults(XElement xml)
        {
            if (xml != null)
            {
                var p = xml.GetElement("Promotion");
                Name = p.GetValue<string>("Name");
                CodeAndName = p.GetValue<string>("CodeAndName");
                URL = p.GetValue<string>("URL");
                IsAmendable = (p.Element("IsAmendable").MaybeValue() == "1");
                StatusID = p.GetValue<int>("StatusID");

                WizardStartScreenName = p.GetValue<string>("WizardStartScreenName");

                WizardPages = new List<PromotionTab>();
                WizardPages = p.Element("WizardPages").MaybeElements("Tab").Select(op => new PromotionTab(op)).ToList();
                PromotionID = p.GetValue<string>("Promo_Idx");
 
                try
                {
                    var vu = new List<PromotionViewingUser>();
                    var cv = p.Element("CurrentlyViewingUsers");
                    foreach (var u in cv.Elements("Users"))
                    {
                        vu.Add(new PromotionViewingUser(){Name = u.GetValue<string>("User_DisplayName"), LastSeenRaw = u.GetValueOrDefault<DateTime>("LastSeenTime")});
                    }

                    ViewingUsers = vu;
                }
                catch (Exception)
                {


                }

            }
        }

    }

    public class PromotionTab
    {
        public string WizardTabCode { get; set; }

        public DateTime? LastSavedDate { get; set; }
        public PromotionTabStatus IsCompleted { get; set; }

        public PromotionTab()
        {
        }

        public PromotionTab(XElement xml)
        {
            if (xml != null)
            {
                WizardTabCode = xml.GetValue<string>("WizardTab_Code");
                ;
                IsCompleted = (PromotionTabStatus) Convert.ToInt32(xml.GetValue<string>("IsCompleted"));


                var din = xml.Element("LastSaveDate").MaybeValue();
                DateTime? dt;

                if (!string.IsNullOrEmpty(din))
                {
                    dt = DateTime.Parse(din);
                }
                else
                {
                    dt = null;
                }

                LastSavedDate = dt;
            }

        }

    }


    public enum PromotionTabStatus
    {
        NoStarted = 0,
        Complete = 1,
        NeedsReview = 2
    }

    public class PromotionViewingUser
    {
        public string Name { get; set; }
        public DateTime LastSeenRaw { get; set; }
        public DateTime LastSeen { get { return LastSeenRaw.ToLocalTime(); } }
    }
 

}
