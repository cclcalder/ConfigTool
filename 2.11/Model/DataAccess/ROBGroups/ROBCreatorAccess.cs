using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.Generic;
using Model.Entity.ROBs;

namespace Model.DataAccess.ROBGroups
{
    public class ROBCreatorAccess
    {
        private readonly XElement _userIdxElement = new XElement("User_Idx", User.CurrentUser.ID);
        private readonly XElement _salesOrgIdxElement = new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID);
        private readonly XElement _appTypeIdxElement;
        private string AppTypeIdx { get; set; }

        public ROBCreatorAccess(string appTypeIdx)
        {
            AppTypeIdx = appTypeIdx;
            _appTypeIdxElement = new XElement("AppType_Idx", AppTypeIdx);
        }

        public Task<XElement> GetProperties(string robGroupIdx = null)
        {
            var args = GetBaseArguments();

            if (!string.IsNullOrEmpty(robGroupIdx))
                args.Add(new XElement("ROBGroup_Idx", robGroupIdx));

            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.RobGroupCreator.GetProperties, args);
        }

        public Task<List<ComboboxItem>> GetCustomerLevels(string robGroupIdx)
        {
            var args = GetBaseArguments("DataSourceInput");

            if (!string.IsNullOrEmpty(robGroupIdx))
                args.Add(new XElement("ROBGroup_Idx", robGroupIdx));

            return ComboBoxAccess.GetComboboxItems(StoredProcedure.RobGroupCreator.GetCustomerLevels, args);
        }

        public Task<List<ComboboxItem>> GetCustomers(string customerLevelIdx, string robGroupIdx)
        {
            XElement args = GetBaseArguments("DataSourceInput");
            args.AddElement("CustLevel_Idx", customerLevelIdx);

            if (!string.IsNullOrEmpty(robGroupIdx))
                args.Add(new XElement("ROBGroup_Idx", robGroupIdx));

            return ComboBoxAccess.GetComboboxItems(StoredProcedure.RobGroupCreator.GetCustomers, args);
        }

        public List<Impact> GetImpacts(string subTypeIdx)
        {
            var arguments = GetBaseArguments("GetImpactItems");
            arguments.AddElement("SubType_Idx", subTypeIdx);
            arguments.AddElement("IsGroupCreator", 1);

            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.ROB.GetImpactItems, arguments).Result.Elements("Impact").Select(Impact.FromXml).ToList();
        }


        public Task<XElement> GetROBGroup(XElement properties, XElement impactOptions, List<string> customerIdx, Dictionary<string, string> productIdxs)
        {
            XElement args = GetBaseArguments();
            args.Add(InputConverter.ToCustomers(customerIdx));
            args.Add(InputConverter.ToProducts(productIdxs));
            args.Add(properties);
            args.Add(impactOptions);

            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.RobGroupCreator.GetROBGroup, args);
        }

        public Task<XElement> GetROBGroup(XElement xRobGroupIdx)
        {
            XElement args = GetBaseArguments();
            args.Add(xRobGroupIdx);

            return DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.RobGroupCreator.GetROBGroup, args);
        }

        private XElement GetBaseArguments(string tag = "GetItems")
        {
            var args = new XElement(tag);
            args.Add(_userIdxElement);
            args.Add(_salesOrgIdxElement);
            args.Add(_appTypeIdxElement);

            return args;
        }

        public bool SaveROBGroup(XElement gridXml)
        {
            var arguments = GetBaseArguments("SaveROBs");
            arguments.Add(gridXml);

            var result = WebServiceProxy.Call(StoredProcedure.RobGroupCreator.SaveROBGroup, arguments);

            MessageConverter.DisplayMessage(result);

            return result.ToString().ToLower().Contains("success");
        }

        public XElement SaveROBGroup(string robGroupIdx, params XElement[] args)
        {
            var arguments = GetBaseArguments("SaveROBs");
            arguments.Add(new XElement("ROBGroup_Idx", robGroupIdx));
            arguments.Add(args);

            var result = WebServiceProxy.Call(StoredProcedure.RobGroupCreator.SaveROBGroup, arguments);

            MessageConverter.DisplayMessage(result);

            return result;
        }
    }
}