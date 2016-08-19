using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Admin
{
    public class Pattern1Profile
    {
        private string m_profileID = "Profile_Idx";
        public string ProfileID { get; set; }

        private string m_profileName = "Profile_Name";
        public string ProfileName { get; set; }

        public Pattern1Profile(XElement element)
        {
            ProfileID = element.GetValue<string>(m_profileID);
            ProfileName = element.GetValue<string>(m_profileName);
        }
             
    }
}
