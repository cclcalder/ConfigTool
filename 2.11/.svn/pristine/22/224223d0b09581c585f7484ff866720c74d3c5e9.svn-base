using System.Globalization;
using Exceedra.Common;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity.ROBs;
using ROBComment = Model.Entity.ROBs.Comment;
using Status = Model.Entity.ROBs.Status;

namespace Model.DataAccess
{
    using Entity.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class RobAccess
    {
        public XElement AppTypeElement;

        public RobAccess(string appTypeIdx)
        {
            AppTypeElement = new XElement("AppType_Idx", appTypeIdx);
        }

        public XElement CreateArgs(string name)
        {
            var args = CommonXml.GetBaseArguments(name);
            args.Add(AppTypeElement);
            return args;
        }

        #region Filter Procs

        public string GetFilterStatusProc()
        {
            return StoredProcedure.ROB.GetFilterStatuses;
        }

        public string GetFilterDatesProc()
        {
            return StoredProcedure.ROB.GetFilterDates;
        }

        //public string GetSaveAsDefaultsProc()
        //{
        //    return StoredProcedure.ROB.SaveUserPrefs;
        //}

        #endregion

        #region Tab Procs

        public string GetSkuDetailsProc()
        {
            return StoredProcedure.ROB.GetRobSkus;
        }

        public string GetRobsProc()
        {
            return StoredProcedure.ROB.GetRobs;
        }

        public string GetMaterialRobsProc()
        {
            return StoredProcedure.ROB.GetMaterialRobs;
        }

        public string DropdownStatusProc()
        {
            return StoredProcedure.ROB.DropDownStatuses;
        }

        public string GetContractsProc()
        {
            return StoredProcedure.ROB.GetContracts;
        }

        #endregion

        public Task<Rob> GetRob(string appTypeID, string robID)
        {
            var args = CreateArgs("GetROB");
            args.Add(new XElement("ROB_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetRob, args)
                .ContinueWith(t => GetRobContinuation(robID, t));
        }

        private Rob GetRobContinuation(string robID, Task<XElement> task)
        {
            if (task.IsCanceled) return null;

            // Just use the Result, if faulted, the exception will be rethrown which is the preferred behaviour here.
            var rob = Rob.FromGetRobXml(task.Result.Element("ROB"));
            rob.ID = robID;
            return rob;
        }

        public XElement GetCustomersArgs(string customerLevelID, string robID)
        {
            var args = CreateArgs("DataSourceInput");
            args.Add(new XElement("CustLevel_Idx", customerLevelID));
            args.Add(new XElement("ROB_Idx", robID));
            return args;
        }

        public XElement GetProductArgs(string productLevelID, string robID, bool filterByListings = false, string customerLevelID = "", IEnumerable<string> customerIds = null)
        {
            var args = CreateArgs("DataSourceInput");
            args.Add(new XElement("ProdLevel_Idx", productLevelID));
            args.Add(new XElement("ROB_Idx", robID));

            args.Add(new XElement("IsFilteredByListings", filterByListings ? "1" : "0"));
            if (filterByListings)
            {
                args.Add(new XElement("CustLevelID", customerLevelID));

                XElement xSelectedCustomers = new XElement("Customers");
                foreach (var customerId in customerIds)
                    xSelectedCustomers.Add(new XElement("CustomerId", customerId));
                args.Add(xSelectedCustomers);
            }

            return args;
        }

        public Task<IList<Impact>> GetImpacts(string subTypeID, string robID)
        {
            var args = CreateArgs("GetImpactItems");
            args.Add(new XElement("SubType_Idx", subTypeID));
            args.Add(new XElement("ROB_Idx", robID));

            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetImpactItems, args, DisplayErrors.No)
                .ContinueWith(t => GetImpactsContinuation(t));
        }

        private IList<Impact> GetImpactsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return new List<Impact>();
            return task.Result.Elements("Impact").Select(Impact.FromXml).ToList();
        }

        public XElement GetScenariosArgs(string robID)
        {
            var args = CreateArgs("GetScenarios");
            args.Add(new XElement("ROB_Idx", robID));
            return args;
        }

        public XElement SaveRob(Rob rob, IList<ROBComment> comments)
        {
            var args = CreateRobXml(rob, comments, "SaveROB");

            return WebServiceProxy.Call(StoredProcedure.ROB.Save, args, DisplayErrors.Yes);
        }

        private XElement CreateRobXml(Rob rob, IList<ROBComment> comments, string rootElementName)
        {
            var args = CreateArgs(rootElementName);
            args.Add(new XElement("Name", rob.Name));
            args.Add(new XElement("ROB_Idx", rob.ID));
            args.Add(new XElement("CustLevel", new XElement("ID", rob.CustomerLevelIdx)));
            args.Add(new XElement("Customers", rob.Customers.Select(c => new XElement("ID", c.Idx))));
            args.Add(InputConverter.ToList("Products", "ID", rob.SelectedProductIdxs));
            args.Add(InputConverter.ToList("Scenarios", "ID", rob.SelectedScenarioIdxs));
            args.Add(new XElement("ProdLevel", new XElement("ID", rob.ProductLevelIdx)));
            args.Add(new XElement("SubType", new XElement("ID", rob.ItemType)));
            var dates = new XElement("Dates");
            dates.Add(new XElement("Start", rob.Start.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture)));
            dates.Add(new XElement("End", rob.End.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture)));
            args.Add(dates);
            args.Add(new XElement("Status", new XElement("ID", rob.StatusIdx)));

            var options =
                rob.ImpactOptions.Select(
                    io => new XElement("Option", new XElement("ID", io.ID), new XElement("Value", io.Value)))
                   .Cast<object>()
                   .ToArray();
            args.Add(new XElement("ImpactOptions", options));

            if (comments.Count > 0)
            {
                args.Add(new XElement("Comments", comments.Select(c => new XElement("Comment", new XElement("Value", c.Value), new XElement("Type", c.CommentType)))));
            }
            if (rob.Recipients != null && rob.Recipients.Any())
            {
                XElement recipients = new XElement("Recipients");
                foreach (var recipient in rob.Recipients)
                {
                    recipients.Add(new XElement("Idx", recipient));
                }
                args.Add(recipients);
            }

            if (rob.FileLocation != null)
                args.Add(new XElement("File_Location", rob.FileLocation));

            return args;
        }

        public Task<IList<ROBComment>> GetComments(string robID)
        {
            var args = CreateArgs("GetComments");
            args.Add(new XElement("Rob_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetComments, args, DisplayErrors.No)
                .ContinueWith(t => GetCommentsContinuation(t));
        }

        public Task AddCommentAsync(string robID, ROBComment comment)
        {
            var args = CreateArgs("AddComment");
            args.Add(new XElement("Rob_Idx", robID));
            args.Add(new XElement("Comment", comment.Value));
            args.Add(new XElement("ROBCommentTypeIdx", comment.CommentType));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.AddComment, args, DisplayErrors.No);
        }

        public Task<IList<CommentType>> GetCommentTypes()
        {
            var args = CreateArgs("GetROBCommentTypes");
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetCommentTypes, args, DisplayErrors.No)
                .ContinueWith(t => GetCommentTypesContinuation(t));
        }

        public Task<IEnumerable<CommentComboboxItem>> GetFilterCommentTypes()
        {
            var args = CreateArgs("ROBCommentFilterType");
            return DynamicDataAccess.GetGenericEnumerableAsync<CommentComboboxItem>(StoredProcedure.ROB.GetFilterCommentTypes, args);

        }

        public Task DeleteCommentAsync(string robId, string commentId)
        {
            var args = new XElement("DeleteComment");
            args.Add(new XElement("UserID", User.CurrentUser.ID));
            args.Add(new XElement("RobID", robId));
            args.Add(new XElement("CommentID", commentId));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.DeleteComment, args, DisplayErrors.No);
        }

        public string DeleteComment(string robId, string commentId)
        {
            var args = new XElement("DeleteComment");
            args.Add(new XElement("UserID", User.CurrentUser.ID));
            args.Add(new XElement("RobID", robId));
            args.Add(new XElement("CommentID", commentId));
            return WebServiceProxy.Call(StoredProcedure.ROB.DeleteComment, args, DisplayErrors.No).ToString();

        }

        public XElement GetRobRecipientArgs(string idx, string customerLevelIdx, IList<string> customers)
        {
            var args = CreateArgs("GetROBRecipients");
            args.AddElement("ROB_Idx", idx);
            args.Add(new XElement("CustLevelID", customerLevelIdx));
            args.Add(InputConverter.ToList("Customers", "ID", customers));

            return args;
        }

        private IList<CommentType> GetCommentTypesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<CommentType>();
            return task.Result.Elements("ROBCommentType").Select(CommentType.FromXml).ToList();
        }

        private IList<ROBComment> GetCommentsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<ROBComment>();
            return task.Result.Elements("Comment").Select(ROBComment.FromXml).ToList();
        }

        public bool RemoveRob(List<string> robs)
        {
            var args = CreateArgs("DeleteROB");
            args.Add(InputConverter.ToIdxList("ROBs", robs));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.ROB.Remove, args));
        }

        public bool CopyRob(string appTypeId, List<string> robs)
        {
            var args = CommonXml.GetBaseArguments("CopyROB");
            args.AddElement("AppType_Idx", appTypeId);
            args.Add(InputConverter.ToIdxList("ROBs", robs));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.ROB.Copy, args));
        }

        public bool UpdateStatusRobs(List<string> idxs, string statusIdx, string appTypeIdx)
        {
            var args = CommonXml.GetBaseArguments("UpdateROBStatus");
            args.Add(new XElement("Target_Status_Idx", statusIdx));
            args.Add(new XElement("AppType_Idx", appTypeIdx));
            args.Add(InputConverter.ToList("ROBs", "ROB_Idx", idxs));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.ROB.UpdateMultipleRobStatus, args));            
        }

        #region contract details



        public Task<XElement> GetContractDetailsVerticalGrid(string robID)
        {
            var args = CreateArgs("GetData");
            args.Add(new XElement("ROBGroup_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.RobContracts.GetContractDetails, args, DisplayErrors.No);
        }

        public Task<XElement> GetAddNewTermVerticalGrid(string robID)
        {
            var args = CreateArgs("GetData");
            args.Add(new XElement("ROBGroup_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.RobContracts.GetTermDetails, args, DisplayErrors.No);
        }

        public Task<XElement> GetTermsTabControl(string robID)
        {
            var args = CreateArgs("GetData");
            args.Add(new XElement("ROBGroup_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.RobContracts.GetContractTabs, args, DisplayErrors.No);
        }

        public Task<XElement> AddTerm(string robID, XDocument contractDetails, XDocument addTerms,
            IEnumerable<string> customersIds, IEnumerable<string> parentCustomersIds, IEnumerable<string> productsIds, IEnumerable<string> parentProductsIds, string lowestRobId)
        {
            var pci = parentCustomersIds.ToArray();
            var pdi = parentProductsIds.ToArray();

            var args = CreateArgs("DataSourceInput");
            args.Add(new XElement("ROBGroup_Idx", robID));
            args.Add(new XElement("MinROB_Idx", lowestRobId));

            XElement xContractDetails = new XElement("ContractDetails", contractDetails.Root);
            args.Add(xContractDetails);

            XElement xTermDetails = new XElement("TermDetails", addTerms.Root);
            args.Add(xTermDetails);

            XElement xCustomers = new XElement("Customers");
            foreach (var customerId in customersIds)
            {
                XElement xCustomer = new XElement("Customer");
                xCustomer.Add(new XElement("Idx", customerId));
                var isParent = pci.Contains(customerId);
                xCustomer.Add(new XElement("IsParentNode", isParent ? "1" : "0"));
                xCustomers.Add(xCustomer);
            }
            args.Add(xCustomers);

            XElement xProducts = new XElement("Products");
            foreach (var productId in productsIds)
            {
                XElement xProduct = new XElement("Product");
                xProduct.Add(new XElement("Idx", productId));
                var isParent = pdi.Contains(productId);
                xProduct.Add(new XElement("IsParentNode", isParent ? "1" : "0"));
                xProducts.Add(xProduct);
            }
            args.Add(xProducts);


            return WebServiceProxy.CallAsync(StoredProcedure.RobContracts.GetContractTerms, args);
        }

        public Task<XElement> SaveContract(string appTypeID, string robID, XDocument contractDetails,
            IEnumerable<string> customersIds, IEnumerable<string> parentCustomersIds, XDocument terms, IEnumerable<string> termsToDeleteIds, string statusId, List<string> recipientIds, IList<string> scenariosIds)
        {
            var pci = parentCustomersIds.ToArray();

            var args = CreateArgs("SaveContract");

            args.Add(new XElement("ROBGroup_Idx", robID));

            XElement xStatus = new XElement("Status_Idx", statusId);
            args.Add(xStatus);

            if (recipientIds != null && recipientIds.Any())
            {
                XElement xRecipient = new XElement("Recipients");
                foreach (var recipient in recipientIds)
                {
                    xRecipient.Add(new XElement("Idx", recipient));
                }
                args.Add(xRecipient);
            }

            XElement xScenarios = new XElement("Scenarios");
            foreach (var scenarioId in scenariosIds)
                xScenarios.Add(new XElement("Idx", scenarioId));
            args.Add(xScenarios);

            XElement xTermsToDelete = new XElement("DeletedROBs");
            foreach (var termToDeleteId in termsToDeleteIds)
                xTermsToDelete.Add(new XElement("Idx", termToDeleteId));
            args.Add(xTermsToDelete);

            XElement xContractDetails = new XElement("ContractDetails", contractDetails.Root);
            args.Add(xContractDetails);

            XElement xCustomers = new XElement("Customers");
            foreach (var customerId in customersIds)
            {
                XElement xCustomer = new XElement("Customer");
                xCustomer.Add(new XElement("Idx", customerId));
                var isParent = pci.Contains(customerId);
                xCustomer.Add(new XElement("IsParentNode", isParent ? "1" : "0"));
                xCustomers.Add(xCustomer);
            }
            args.Add(xCustomers);

            XElement xTerms = new XElement("ROBGroup", terms.Element("Results").Elements("RootItem"));
            args.Add(xTerms);

            return WebServiceProxy.CallAsync(StoredProcedure.RobContracts.SaveContractTerms, args);
        }

        public bool CopyContract(IEnumerable<string> contactIdxs)
        {
            var args = CreateArgs("CopyROBGroup");
            args.Add(InputConverter.ToIdxList("ROBGroup", contactIdxs));

            return MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.RobContracts.CopyContract, args));
        }

        public bool DeleteContract(IEnumerable<string> contactIdxs)
        {
            var args = CreateArgs("DeleteROBGroup");
            args.Add(InputConverter.ToIdxList("ROBGroup", contactIdxs));

            return MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.RobContracts.RemoveContract, args));
        }

        public bool UpdateContractStatuses(IEnumerable<string> contactIdxs, string targetIdx)
        {
            var args = CreateArgs("UpdateROBGroupStatus");
            args.AddElement("Target_Status_Idx", targetIdx);
            args.Add(InputConverter.ToIdxList("ROBGroups", contactIdxs));

            return MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.RobContracts.UpdateContractStatus, args));
        }

        #endregion


        public Task<XElement> LoadInformationGrid(string robID)
        {
            var args = CreateArgs("LoadInformation");
            args.Add(new XElement("Rob_Idx", robID));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.LoadInformationGrid, args, DisplayErrors.No)
                .ContinueWith(t => GetInformationGridContinuation(t)); ;
        }

        private XElement GetInformationGridContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled) return null;

            //return XElement.Parse(@" 
            //<Results>
            //  <RowsLimitedAt>500</RowsLimitedAt>
            //  <RowsAvailable>5</RowsAvailable>
            //  <RootItem>
            //    <Item_Idx>17</Item_Idx>
            //    <Item_Type>ROBGrid</Item_Type>
            //    <Item_RowSortOrder>1</Item_RowSortOrder>
            //    <Attributes>
            //      <Attribute>
            //        <ColumnCode>IsSelected</ColumnCode>
            //        <HeaderText />
            //        <Value>false</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>1</IsEditable>
            //        <ControlType>Checkbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>-1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Idx</ColumnCode>
            //        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
            //        <Value>17</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>0</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Name</ColumnCode>
            //        <HeaderText>Name</HeaderText>
            //        <Value>(EA-218) Marketing Uplift Opportunity 2016 - Poundland</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Hyperlink</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>SubType</ColumnCode>
            //        <HeaderText>SubType</HeaderText>
            //        <Value>Marketing Uplift</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Customer_Name</ColumnCode>
            //        <HeaderText>Customer</HeaderText>
            //        <Value> Poundland</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>2</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Status_Name</ColumnCode>
            //        <HeaderText>Status</HeaderText>
            //        <Value>Draft</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour>#ffffff</BorderColour>
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>3</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>StartDate</ColumnCode>
            //        <HeaderText>Start</HeaderText>
            //        <Value>2016-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>4</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>EndDate</ColumnCode>
            //        <HeaderText>End</HeaderText>
            //        <Value>2017-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>5</ColumnSortOrder>
            //      </Attribute>
            //    </Attributes>
            //  </RootItem>
            //  <RootItem>
            //    <Item_Idx>18</Item_Idx>
            //    <Item_Type>ROBGrid</Item_Type>
            //    <Item_RowSortOrder>2</Item_RowSortOrder>
            //    <Attributes>
            //      <Attribute>
            //        <ColumnCode>IsSelected</ColumnCode>
            //        <HeaderText />
            //        <Value>false</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>1</IsEditable>
            //        <ControlType>Checkbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>-1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Idx</ColumnCode>
            //        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
            //        <Value>18</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>0</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Name</ColumnCode>
            //        <HeaderText>Name</HeaderText>
            //        <Value>(EA-219) Marketing Uplift Opportunity 2016 - Tesco</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Hyperlink</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>SubType</ColumnCode>
            //        <HeaderText>SubType</HeaderText>
            //        <Value>Marketing Uplift</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Customer_Name</ColumnCode>
            //        <HeaderText>Customer</HeaderText>
            //        <Value> Tesco</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>2</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Status_Name</ColumnCode>
            //        <HeaderText>Status</HeaderText>
            //        <Value>Draft</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour>#ffffff</BorderColour>
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>3</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>StartDate</ColumnCode>
            //        <HeaderText>Start</HeaderText>
            //        <Value>2016-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>4</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>EndDate</ColumnCode>
            //        <HeaderText>End</HeaderText>
            //        <Value>2017-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>5</ColumnSortOrder>
            //      </Attribute>
            //    </Attributes>
            //  </RootItem>
            //  <RootItem>
            //    <Item_Idx>20</Item_Idx>
            //    <Item_Type>ROBGrid</Item_Type>
            //    <Item_RowSortOrder>3</Item_RowSortOrder>
            //    <Attributes>
            //      <Attribute>
            //        <ColumnCode>IsSelected</ColumnCode>
            //        <HeaderText />
            //        <Value>false</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>1</IsEditable>
            //        <ControlType>Checkbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>-1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Idx</ColumnCode>
            //        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
            //        <Value>20</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>0</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Name</ColumnCode>
            //        <HeaderText>Name</HeaderText>
            //        <Value>(EA-221) Marketing Uplift Opportunity 2016 - Morrisons</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Hyperlink</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>SubType</ColumnCode>
            //        <HeaderText>SubType</HeaderText>
            //        <Value>Marketing Uplift</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Customer_Name</ColumnCode>
            //        <HeaderText>Customer</HeaderText>
            //        <Value> Morrisons</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>2</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Status_Name</ColumnCode>
            //        <HeaderText>Status</HeaderText>
            //        <Value>Draft</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour>#ffffff</BorderColour>
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>3</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>StartDate</ColumnCode>
            //        <HeaderText>Start</HeaderText>
            //        <Value>2016-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>4</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>EndDate</ColumnCode>
            //        <HeaderText>End</HeaderText>
            //        <Value>2017-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>5</ColumnSortOrder>
            //      </Attribute>
            //    </Attributes>
            //  </RootItem>
            //  <RootItem>
            //    <Item_Idx>23</Item_Idx>
            //    <Item_Type>ROBGrid</Item_Type>
            //    <Item_RowSortOrder>4</Item_RowSortOrder>
            //    <Attributes>
            //      <Attribute>
            //        <ColumnCode>IsSelected</ColumnCode>
            //        <HeaderText />
            //        <Value>false</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>1</IsEditable>
            //        <ControlType>Checkbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>-1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Idx</ColumnCode>
            //        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
            //        <Value>23</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>0</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Name</ColumnCode>
            //        <HeaderText>Name</HeaderText>
            //        <Value>(EA-224) Marketing Uplift Opportunity 2016 - Booker</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Hyperlink</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>SubType</ColumnCode>
            //        <HeaderText>SubType</HeaderText>
            //        <Value>Marketing Uplift</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Customer_Name</ColumnCode>
            //        <HeaderText>Customer</HeaderText>
            //        <Value> Booker</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>2</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Status_Name</ColumnCode>
            //        <HeaderText>Status</HeaderText>
            //        <Value>Draft</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour>#ffffff</BorderColour>
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>3</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>StartDate</ColumnCode>
            //        <HeaderText>Start</HeaderText>
            //        <Value>2016-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>4</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>EndDate</ColumnCode>
            //        <HeaderText>End</HeaderText>
            //        <Value>2017-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>5</ColumnSortOrder>
            //      </Attribute>
            //    </Attributes>
            //  </RootItem>
            //  <RootItem>
            //    <Item_Idx>30</Item_Idx>
            //    <Item_Type>ROBGrid</Item_Type>
            //    <Item_RowSortOrder>5</Item_RowSortOrder>
            //    <Attributes>
            //      <Attribute>
            //        <ColumnCode>IsSelected</ColumnCode>
            //        <HeaderText />
            //        <Value>false</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>1</IsEditable>
            //        <ControlType>Checkbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>-1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Idx</ColumnCode>
            //        <HeaderText>ROB_Idx Not Displayed To User</HeaderText>
            //        <Value>30</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>0</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>ROB_Name</ColumnCode>
            //        <HeaderText>Name</HeaderText>
            //        <Value>(EA-231) Marketing Uplift Opportunity 2016 - England Cricket Board</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Hyperlink</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>0</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>SubType</ColumnCode>
            //        <HeaderText>SubType</HeaderText>
            //        <Value>Marketing Uplift</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>1</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Customer_Name</ColumnCode>
            //        <HeaderText>Customer</HeaderText>
            //        <Value> England Cricket Board</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>2</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>Status_Name</ColumnCode>
            //        <HeaderText>Status</HeaderText>
            //        <Value>Draft</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour>#ffffff</BorderColour>
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>3</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>StartDate</ColumnCode>
            //        <HeaderText>Start</HeaderText>
            //        <Value>2016-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>4</ColumnSortOrder>
            //      </Attribute>
            //      <Attribute>
            //        <ColumnCode>EndDate</ColumnCode>
            //        <HeaderText>End</HeaderText>
            //        <Value>2017-01-01</Value>
            //        <Format />
            //        <ForeColour />
            //        <BorderColour />
            //        <IsDisplayed>1</IsDisplayed>
            //        <IsEditable>0</IsEditable>
            //        <ControlType>Textbox</ControlType>
            //        <TotalsAggregationMethod>NONE</TotalsAggregationMethod>
            //        <ColumnSortOrder>5</ColumnSortOrder>
            //      </Attribute>
            //    </Attributes>
            //  </RootItem>
            //</Results>
            //");

            return task.Result;
        }


        public Task<IEnumerable<Note>> GetNotes(string rOBGroup_Idx)
        {
            XElement argument = new XElement("GetComments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("ROBGroup_Idx", rOBGroup_Idx));

            return WebServiceProxy.CallAsync(StoredProcedure.RobGroup.GetComments, argument, DisplayErrors.No).ContinueWith(t => GetNotesContinuation(t)); ;
        }

        private IEnumerable<Note> GetNotesContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<Note>();

            return task.Result.Elements("Comment").Select(Note.FromXml).ToList();
        }

        public string AddNote(string rOBGroup_Idx, string comment)
        {
            XElement argument = new XElement("AddComment");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("ROBGroup_Idx", rOBGroup_Idx));
            argument.Add(new XElement("Comment", comment));

            var node = WebServiceProxy.Call(StoredProcedure.RobGroup.AddComment, argument).Elements().FirstOrDefault();

            return node.Value;
        }
    }
}
