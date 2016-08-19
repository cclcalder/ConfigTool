using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.Entity.Admin
{
    public class Pattern1Permission
    {
        private string m_permissionId = "Permission_Idx";
        private string m_permissionName = "Permission_Name";

        public string PermissionId { get; set; }
        public string PermissionName { get; set; }

        public  Pattern1Permission(XElement element)
        {
            PermissionId = element.GetValue<string>(m_permissionId);
            PermissionName = element.GetValue<string>(m_permissionName);
        }

    }
}
