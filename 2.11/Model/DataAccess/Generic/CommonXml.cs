using System.Linq;
using System.Xml.Linq;
using Exceedra.Common.Xml;

namespace Model.DataAccess.Generic
{
    public static class CommonXml
    {
        private static XElement UserIdxElement{ get { return new XElement("User_Idx", User.CurrentUser.ID); } }
        private static XElement SalesOrgIdxElement { get { return new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID); } } 

        //NOTE: All new procs should actually use GetData as their root node. However, GetItems is still in use in some old procs so has been left as the default
        public static XElement GetBaseArguments(string tag = "GetItems")
        {
            var args = new XElement(tag ?? "GetItems");
            args.Add(UserIdxElement);
            args.Add(SalesOrgIdxElement);

            return args;
        }

        public static XElement GetBaseAttributeArguments(string tag = "GetItems")
        {
            var args = new XElement(tag ?? "GetItems");
            args.SetAttributeValue(UserIdxElement.Name, UserIdxElement.Value);
            args.SetAttributeValue(SalesOrgIdxElement.Name, SalesOrgIdxElement.Value);

            return args;
        }

        public static XElement GetBaseSaveArguments(string tag = "SaveData")
        {
            return GetBaseArguments(tag);
        }

        public static XElement GetBaseSaveAttributeArguments(string tag = "SaveData")
        {
            return GetBaseAttributeArguments(tag);
        }

        public static XElement GetBaseArguments(string tag, string appTypeIdx, string screenCode = null)
        {
            var args = new XElement(tag ?? "GetItems");
            args.Add(UserIdxElement);
            args.Add(SalesOrgIdxElement);

            if (!string.IsNullOrEmpty(appTypeIdx))
                args.Add(new XElement("AppType_Idx", appTypeIdx));

            if (!string.IsNullOrEmpty(screenCode))
                args.Add(new XElement("Screen_Code", screenCode));

            return args;
        }

        /* This should have been implemented as the above GetBaseArguments(string tag, string appTypeIdx), replacing appType with screen code
         * However, appType is still require by the db. Eventually I hope this will be made redundant and replaced by screen code.
         */
        public static XElement GetBaseScreenArguments(string screenCode, string tag = "GetItems")
        {
            var args = GetBaseArguments(tag);
            args.Add(new XElement(XMLNode.Nodes.Screen_Code.ToString(), screenCode));

            return args;
        }

        /*Only use this to avoid forcing a db change. Never use for new development*/
        public static XElement ConvertToOldStyle(XElement xml)
        {
            xml.Descendants().Do(n => n.Name = n.Name.ToString().Replace("_Idx", "ID").Replace("Idx", "ID"));
            return xml;
        }
    }
}