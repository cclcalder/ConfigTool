using Model.DTOs;
using Model.Entity;
using Model.Entity.ROBs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Model.DataAccess.Generic;

namespace Model.DataAccess
{
    public class ClaimsAccess
    {
        public Task<XElement> GetFilterClaimValues()
        {
            var arguments = CommonXml.GetBaseArguments("GetFilterClaimValueRanges");

            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetClaimFilterValues, arguments)
                .ContinueWith(t => GetFilterClaimValuesContinuation(t));
        }

        private XElement GetFilterClaimValuesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;

            return task.Result;
        }        

        private XElement GetApplyEventArgument(ReturnEventsDTO applyEventDto)
        {
            var argument = new XElement("GetEvents");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            //var customers = new XElement("Customers");
            //foreach (string custId in applyEventDto.CustomerIds)
            //{
            //        var cust = new XElement("Cust");
            //        cust.Add(new XElement("ID", custId));
            //        customers.Add(cust);  
            //}
            //argument.Add(customers);

            //var products = new XElement("Products");
            //foreach (var pId in applyEventDto.ProductIds)
            //{
            //    var prod = new XElement("Prod");
            //    prod.Add(new XElement("ID", pId));
            //    products.Add(prod);
            //}  
            //argument.Add(products);

            var customers = new XElement("Customers");
            argument.Add(customers);
            var products = new XElement("Products");
            argument.Add(products);

            foreach (var c in applyEventDto.CustomerIds.Distinct())
            {
                customers.Add(new XElement("Idx", c));
            }

            foreach (var p in applyEventDto.ProductIds.Distinct())
            {
                products.Add(new XElement("Idx", p));
            }

            var eventDates = new XElement("EventDates");
            eventDates.Add(new XElement("StartDate", applyEventDto.StartDateInputValue));
            eventDates.Add(new XElement("EndDate", applyEventDto.EndDateInputValue));
            argument.Add(eventDates);

            var eventTypes = new XElement("EventTypes");
            foreach (string eventTypeId in applyEventDto.EventTypeIds)
            {
                eventTypes.Add(new XElement("Event_Type_Idx", eventTypeId));
            }
            argument.Add(eventTypes);

            var eventStatus = new XElement("Statuses");
            foreach (string eventStatusId in applyEventDto.EventStatusIds)
            {
                XElement thisElement = new XElement("Status");
                string[] results = eventStatusId.Split('$');

                if(results.Count() > 1)
                    thisElement.Add(new XElement("ID", results[1]));

                thisElement.Add(new XElement("Item_Type", results[0]));

                eventStatus.Add(thisElement);

            }
            argument.Add(eventStatus);

            return argument;
        }

        public Task<XElement> ReturnEvents(XElement defaultLoadXml)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.ReturnEvents, defaultLoadXml)
                .ContinueWith(t => ApplyEventsContinuation(t));
        }

        public Task<XElement> ReturnEvents(ReturnEventsDTO applyEventsDto)
        {
            var arguments = GetApplyEventArgument(applyEventsDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.ReturnEvents, arguments)
                .ContinueWith(t => ApplyEventsContinuation(t));
        }

        private XElement ApplyEventsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return XElement.Parse("<Results></Results>");
            return task.Result;
        }

        private XElement GetApplyMatchesArgument(ReturnedMatchDTO returnedMatchDto)
        {
            var argument = new XElement("ClaimManagementApply_ReturnMatches");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            var claims = new XElement("Claim_Idxs");
            if (returnedMatchDto.ClaimIds != null)
            {
                returnedMatchDto.ClaimIds.ToList().ForEach(claim => claims.Add(new XElement("Claim_Idx", claim)));
            }
            argument.Add(claims);
            var events = new XElement("Event_Idxs");
            if (returnedMatchDto.EventIds != null)
            {
                returnedMatchDto.EventIds.ToList().ForEach(eve => events.Add(new XElement("Event_Idx", eve)));
            }
            argument.Add(events);
            var matches = new XElement("Matches");
            if (returnedMatchDto.Matches != null)
            {
                returnedMatchDto.Matches.ToList().ForEach(match => matches.Add(new XElement("Match", new XElement("Claim_Idx", match.ClaimId), new XElement("Event_Idx", match.EventId))));
            }
            argument.Add(matches);

            return argument;
        }

        public Task<IList<ReturnedMatches>> ReturnMatches(ReturnedMatchDTO returnedMatchDto)
        {
            var arguments = GetApplyMatchesArgument(returnedMatchDto);

                return WebServiceProxy.CallAsync(StoredProcedure.Claims.ReturnMatches, arguments, DisplayErrors.No)
                    .ContinueWith(t => ApplyMatchesContinuation(t));
        }

        private IList<ReturnedMatches> ApplyMatchesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<ReturnedMatches>();
            return task.Result.Elements().Select(ReturnedMatches.FromXml).ToList();
            //var mockReturnedMatchesResults = "<Results>  <Apply_ReturnMatches>    <Event>      <Event_Idx>1</Event_Idx>      <Event_Name>apple</Event_Name>      <Event_Type>Promotion</Event_Type>      <Event_Sub_Type>Growth Incentive</Event_Sub_Type>      <Event_Status>Request Event Adjustment</Event_Status>      <Total_Accrual>5.50</Total_Accrual>      <Settled>0.00</Settled>      <Available_Accrual>5.50</Available_Accrual>      <Total_Outstanding_Claims>3500.00</Total_Outstanding_Claims>    </Event>    <Event>      <Event_Idx>3</Event_Idx>      <Event_Name>Pear</Event_Name>      <Event_Type>Funding</Event_Type>      <Event_Sub_Type>Growth Incentive</Event_Sub_Type>      <Event_Status>Request Event Settlement</Event_Status>      <Total_Accrual>5.50</Total_Accrual>      <Settled>0.00</Settled>      <Available_Accrual>5.50</Available_Accrual>      <Total_Outstanding_Claims>3500.00</Total_Outstanding_Claims>    </Event>    <Claim>      <Claim_Idx>1</Claim_Idx>      <Cust_Idx>20007</Cust_Idx>      <Customer_Name>TESCO - UK</Customer_Name>      <Claim_Date>2014-01-16</Claim_Date>      <Entered_Date>2014-01-17</Entered_Date>      <Claim_Reference>EJ Test</Claim_Reference>      <Claim_Line_Detail>Test Line Detail</Claim_Line_Detail>      <Claim_Value>1000.00</Claim_Value>    </Claim>    <Claim>      <Claim_Idx>2</Claim_Idx>      <Cust_Idx>20007</Cust_Idx>      <Customer_Name>TESCO - UK</Customer_Name>      <Claim_Date>2014-01-17</Claim_Date>      <Entered_Date>2013-12-17</Entered_Date>      <Claim_Reference>EJ Test 2</Claim_Reference>      <Claim_Line_Detail>More Line Detail</Claim_Line_Detail>      <Claim_Value>2500.00</Claim_Value>    </Claim>    <Matches>      <Event_Idx>1</Event_Idx>      <Claim_Idx>1</Claim_Idx>      <Claim_Apportionment>        <Value>0.50</Value>        <Type>p</Type>      </Claim_Apportionment>      <Apportioned_Amount>500.0000</Apportioned_Amount>    </Matches>    <Matches>      <Event_Idx>1</Event_Idx>      <Claim_Idx>2</Claim_Idx>    </Matches>    <Matches>      <Event_Idx>3</Event_Idx>      <Claim_Idx>1</Claim_Idx>    </Matches>    <Matches>      <Event_Idx>3</Event_Idx>      <Claim_Idx>2</Claim_Idx>      <Claim_Apportionment>        <Value>1.00</Value>        <Type>p</Type>      </Claim_Apportionment>      <Apportioned_Amount>2500.0000</Apportioned_Amount>    </Matches>  </Apply_ReturnMatches></Results>";
            //return (XDocument.Parse(mockReturnedMatchesResults).GetElement("Results")).Elements().Select(ReturnedMatches.FromXml).ToList();
        }

        private static readonly object CustomerSyncObject = new object();


        private XElement GetCustomersOnclaimEntryArgument(GetCustomersOnClaimEntryDTO getCustomersOnClaimEntryDTO)
        {
            var argument = new XElement("GetFilterClaimCustomersForClaimAdd");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("SalesOrg_Idx", getCustomersOnClaimEntryDTO.SalesOrganizationId));
            argument.Add(new XElement("Cust_Level_Idx", getCustomersOnClaimEntryDTO.CustomerLevelId));
            return argument;
        }

        public Task<IList<Customer>> GetCustomersOnClaimEntry(GetCustomersOnClaimEntryDTO getEventsDto)
        {
            var arguments = GetCustomersOnclaimEntryArgument(getEventsDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetCustomersOnClaimEntry, arguments)
                .ContinueWith(t => GetCustomersOnClaimEntryContinuation(t));
        }

        private IList<Customer> GetCustomersOnClaimEntryContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<Customer>();
            return task.Result.Elements().Select(Customer.FromXmlSingleReturn).ToList();
        }

        private IList<MatchVisibility> GetMatchVisibilityValuesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<MatchVisibility>();
            return task.Result.Elements().Select(MatchVisibility.FromXml).ToList();
        }

        private XElement GetEventListArgument(string claimId)
        {
            var argument = new XElement("GetEvents");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Claim_Idx", claimId));
            return argument;
        }

        public Task<IList<EventItem>> GetEventList(string claimId)
        {
            var arguments = GetEventListArgument(claimId);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetEventList, arguments)
                .ContinueWith(t => GetEventListContinuation(t));
        }

        private IList<EventItem> GetEventListContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<EventItem>();
            return task.Result.Descendants("Event").Select(EventItem.FromXml).ToList();
        }

        private XElement GetClaimApportionmentsArgument(GetClaimApportionmentsDTO getApportionmentsDto)
        {
            var argument = new XElement("GetApportionments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Claim_Idx", getApportionmentsDto.ClaimId));
            var eventIds = new XElement("Events");
            foreach (string eventId in getApportionmentsDto.EventIds)
            {
                eventIds.Add(new XElement("Event_Idx", eventId));
            }

            argument.Add(eventIds);

            return argument;
        }

        public Task<IList<Apportionment>> GetClaimApportionments(GetClaimApportionmentsDTO getApportionmentsDto)
        {
            var arguments = GetClaimApportionmentsArgument(getApportionmentsDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetApportionments, arguments)
                .ContinueWith(t => GetClaimApportionmentsContinuation(t));
        }

        private IList<Apportionment> GetClaimApportionmentsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<Apportionment>();
            return task.Result.Descendants("Apportionment").Select(Apportionment.FromXml).ToList();
        }

        private XElement GetEventProductsArgument(GetEventProductsDTO getEventProductDto)
        {
            var argument = new XElement("GetEventProducts");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Event_Idx", getEventProductDto.EventId));
            argument.Add(new XElement("Claim_Idx", getEventProductDto.ClaimId));


            return argument;
        }

        public Task<IList<EventProduct>> GetEventProducts(GetEventProductsDTO getEventProductDto)
        {
            var arguments = GetEventProductsArgument(getEventProductDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetEventProducts, arguments)
                .ContinueWith(t => GetEventProductsContinuation(t));
        }

        private IList<EventProduct> GetEventProductsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<EventProduct>();
            return task.Result.Elements().Select(EventProduct.FromXml).ToList();
        }


        private XElement GetEventArgument(string eventId)
        {
            var argument = new XElement("GetEvent");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Event_Idx", eventId));
            return argument;
        }

        public Task<EventDetail> GetEvent(string eventId)
        {
            var arguments = GetEventArgument(eventId);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetEvent, arguments)
                .ContinueWith(t => GetEventContinuation(t));
        }

        private EventDetail GetEventContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new EventDetail();
            return EventDetail.FromXml(task.Result.Element("Event"));
        }

        private XElement GetSetDefaultFiltersArgument(SetDefaultFiltersDTO setDefaultFiltersDto)
        {
            var argument = new XElement("SaveDefaults");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement(XMLNode.Nodes.Screen_Code.ToString(), "Claim"));
            argument.Add(new XElement("Date_Search_Preference", setDefaultFiltersDto.DateSearchPreference));
            argument.AddElement("ListingsGroup_Idx", setDefaultFiltersDto.ListingsGroupIdx);

            //var salesOrgs = new XElement("SalesOrgs");
            //foreach (string salesOrgId in setDefaultFiltersDto.SalesOrgId)
            //{
            //    salesOrgs.Add(new XElement("SalesOrg_Idx", salesOrgId));
            //}
            //argument.Add(salesOrgs);
            argument.Add(new XElement("SalesOrg_Idx", setDefaultFiltersDto.SalesOrgId));

            var matchingStatuses = new XElement("MatchingStatuses");
            foreach (string matchingStatusesId in setDefaultFiltersDto.ClaimMatchingStatusIds)
            {
                matchingStatuses.Add(new XElement("Claim_Matching_Status_Idx", matchingStatusesId));
            }
            argument.Add(matchingStatuses);

            //var claimStatuses = new XElement("ClaimStatuses");
            //foreach (string claimStatusId in setDefaultFiltersDto.Statuses)
            //{
            //    claimStatuses.Add(new XElement("Claim_Status_Idx", claimStatusId));
            //}
            //argument.Add(claimStatuses);

            var claimStatuses = new XElement("Statuses");
            foreach (string eventStatusId in setDefaultFiltersDto.Statuses)
            {
                XElement thisElement = new XElement("Status");
                string[] results = eventStatusId.Split('$');

                if (results.Count() > 1)
                    thisElement.Add(new XElement("ID", results[1]));

                thisElement.Add(new XElement("Item_Type", results[0]));

                claimStatuses.Add(thisElement);

            }
            argument.Add(claimStatuses);

            //var customers = new XElement("Customers");
            //foreach (string customerId in setDefaultFiltersDto.CustomerIds)
            //{
            //    customers.Add(new XElement("Cust_Idx", customerId));
            //}
            //argument.Add(customers);

            var customers = new XElement("Customers");
            argument.Add(customers);
            var products = new XElement("Products");
            argument.Add(products);

            foreach (var c in setDefaultFiltersDto.CustomerIds.Distinct())
            {
                customers.Add(new XElement("Idx", c));
            }

            foreach (var p in setDefaultFiltersDto.ProductIds.Distinct())
            {
                products.Add(new XElement("Idx", p));
            }


            //var customers = new XElement("Customers");
            //foreach (string custId in setDefaultFiltersDto.CustomerIds)
            //{
            //    var cust = new XElement("Cust");
            //    cust.Add(new XElement("ID", custId));
            //    customers.Add(cust);
            //}
            //argument.Add(customers);

            //var products = new XElement("Products");
            //foreach (var pId in setDefaultFiltersDto.ProductIds)
            //{
            //    var prod = new XElement("Prod");
            //    prod.Add(new XElement("ID", pId));
            //    products.Add(prod);
            //}
            //argument.Add(products);

            //var claimValueRanges = new XElement("ClaimValueRanges");
            //foreach (string claimValueRangeId in setDefaultFiltersDto.ClaimValueRangeIds)
            //{
            //    claimValueRanges.Add(new XElement("Claim_Value_Idx", claimValueRangeId));
            //}


            //argument.Add(claimValueRanges);

            var claimValues = new XElement("Claim_Values");
            claimValues.Add(new XElement("Min_Value", setDefaultFiltersDto.ClaimFilterMin));
            claimValues.Add(new XElement("Max_Value", setDefaultFiltersDto.ClaimFilterMax));

            argument.Add(claimValues);

            var claimDates = new XElement("ClaimDates");
            claimDates.Add(new XElement("StartDate", setDefaultFiltersDto.ClaimStartDateInputValue));
            claimDates.Add(new XElement("EndDate", setDefaultFiltersDto.ClaimEndDateInputValue));
            argument.Add(claimDates);

            ////May not be necessary on production server but test data lacked any EventStatusId 
            //if (setDefaultFiltersDto.EventStatusId != null)
            //{
            //    var eventStatuses = new XElement("EventStatuses");
            //    foreach (string eventStatusId in setDefaultFiltersDto.EventStatusId)
            //    {
            //        eventStatuses.Add(new XElement("Event_Status_Idx", eventStatusId));
            //    }
            //    argument.Add(eventStatuses);
            //}

            var eventDates = new XElement("EventDates");
            eventDates.Add(new XElement("StartDate", setDefaultFiltersDto.EventStartDateInputValue));
            eventDates.Add(new XElement("EndDate", setDefaultFiltersDto.EventEndDateInputValue));
            argument.Add(eventDates);

            var eventTypes = new XElement("EventTypes");
            foreach (string eventTypeId in setDefaultFiltersDto.EventTypeIds)
            {
                eventTypes.Add(new XElement("EventType_Idx", eventTypeId));
            }

            argument.Add(eventTypes);



            return argument;
        }

        public Task<SprocResult> SetDefaultFilters(SetDefaultFiltersDTO setDefaultFiltersDto)
        {
            var arguments = GetSetDefaultFiltersArgument(setDefaultFiltersDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.SetDefaultFilters, arguments)
                .ContinueWith(t => SetDefaultFiltersContinuation(t));
        }

        private SprocResult SetDefaultFiltersContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to set default values failed.";
            }
            else if (task.Result.Element("SuccessMessage") != null)
            {
                result.Success = true;// string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("SuccessMessage");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to set default values failed.";
            }

            return result;
        }

        private XElement GetAutomaticMatchesArgument(AutomaticMatchDTO automaticMatchDTO)
        {
            var argument = new XElement("Claim_Event_Automatic_Match");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            var claimIds = new XElement("Claim_Idxs");
            foreach (string eventId in automaticMatchDTO.ClaimIds)
            {
                claimIds.Add(new XElement("Claim_Idx", eventId));
            }

            argument.Add(claimIds);

            var eventIds = new XElement("Event_Idxs");
            foreach (string eventId in automaticMatchDTO.EventIds)
            {
                eventIds.Add(new XElement("Event_Idx", eventId));
            }

            argument.Add(eventIds);
            return argument;
        }

        //public Task<IList<AutomaticMatch>> GetAutomaticMatches(AutomaticMatchDTO automaticMatchDTO)
        //{
        //    var arguments = GetAutomaticMatchesArgument(automaticMatchDTO);
        //    return WebServiceProxy.CallAsync(StoredProcedure.Claims.AutomaticMatch, arguments)
        //        .ContinueWith(t => GetAutomaticMatchesContinuation(t));
        //}

        //public IList<AutomaticMatch> GetAutomaticMatchesContinuation(Task<XElement> task)
        //{
        //    if (task.IsCanceled || task.IsFaulted || task.Result == null)
        //        return new List<AutomaticMatch>();
        //    return task.Result.Descendants("Match_Idxs").Select(AutomaticMatch.FromXml).ToList();
        //}

        public Task<SprocResult> GetAutomaticMatches(AutomaticMatchDTO automaticMatchDTO)
        {
            var arguments = GetAutomaticMatchesArgument(automaticMatchDTO);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.AutomaticMatch, arguments)
                .ContinueWith(t => GetAutomaticMatchesContinuation(t));
        }

        public SprocResult GetAutomaticMatchesContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to match automatically failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = true;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to match automatically failed.";
            }

            return result;
        }

        private XElement GetSaveMatchesArgument(SaveMatchesDTO saveMatchesDTO)
        {
            var argument = new XElement("SaveMatches");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            var claimIds = new XElement("Claim_Idxs");
            foreach (string claimId in saveMatchesDTO.ClaimIds)
            {
                claimIds.Add(new XElement("Claim_Idx", claimId));
            }

            argument.Add(claimIds);

            var eventIds = new XElement("Event_Idxs");
            foreach (string eventId in saveMatchesDTO.EventIds)
            {
                eventIds.Add(new XElement("Event_Idx", eventId));
            }

            argument.Add(eventIds);

            var matches = new XElement("Claim_Event_Matches");
            foreach (var matchItem in saveMatchesDTO.Matches)
            {
                var match = new XElement("Match_Idxs");
                match.Add(new XElement("Claim_Idx", matchItem.ClaimId));
                match.Add(new XElement("Event_Idx", matchItem.EventId));
                matches.Add(match);
            }

            argument.Add(matches);
            return argument;
        }

        public Task<SaveMatchesResult> SaveMatches(SaveMatchesDTO saveMatchesDTO)
        {
            var arguments = GetSaveMatchesArgument(saveMatchesDTO);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.SaveMatches, arguments)
                .ContinueWith(t => SaveMatchesContinuation(t));
        }

        public SaveMatchesResult SaveMatchesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return null;
            return SaveMatchesResult.FromXml(task.Result.Element("Outcome"));
        }

        private XElement GetApprovePaymentsArgument(ApprovePaymentsDTO approvePaymentsDTO)
        {
            var argument = new XElement("ApprovePayments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            var claimStatuses = new XElement("Claim_Idxs");
            foreach (string claimStatusId in approvePaymentsDTO.ClaimIds)
            {
                claimStatuses.Add(new XElement("Claim_Idx", claimStatusId));
            }
            argument.Add(claimStatuses);

            return argument;
        }

        public Task<SprocResult> ApprovePayments(ApprovePaymentsDTO approvePaymentsDTO)
        {
            var arguments = GetApprovePaymentsArgument(approvePaymentsDTO);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.ApprovePayments, arguments)
                .ContinueWith(t => ApprovePaymentsContinuation(t));
        }

        private SprocResult ApprovePaymentsContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to approve payments failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to approve payments failed.";
            }

            return result;
        }

        private XElement GetSettleEventsArgument(SettleEventsDTO settleEventsDTO)
        {
            var argument = new XElement("SettleEvents");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));

            var eventIds = new XElement("Event_Idxs");
            foreach (string eventId in settleEventsDTO.EventIds)
            {
                eventIds.Add(new XElement("Event_Idx", eventId));
            }
            argument.Add(eventIds);

            return argument;
        }

        public Task<SprocResult> SettleEvents(SettleEventsDTO settleEventsDTO)
        {
            var arguments = GetSettleEventsArgument(settleEventsDTO);

            return WebServiceProxy.CallAsync(StoredProcedure.Claims.SettleEvents, arguments)
                .ContinueWith(t => SettleEventsContinuation(t));
        }

        private SprocResult SettleEventsContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to settle events failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to settle events failed.";
            }

            return result;
        }

      

        public Task<ClaimUserDefaults> GetUserDefaults()
        {
            var arguments = CommonXml.GetBaseArguments("GetUserDefaults");
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetUserDefaults, arguments)
                .ContinueWith(t => GetUserDefaultsContinuation(t));
        }

        private ClaimUserDefaults GetUserDefaultsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            { return new ClaimUserDefaults(); }

            return ClaimUserDefaults.FromXml(task.Result.Element("UserDefault"));
        }

        private XElement GetAllowedEventStatusesArgument(string eventStatusId)
        {
            var argument = new XElement("GetAllowedEventStatus");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Status_Idx", eventStatusId));

            return argument;
        }

        public Task<IList<AllowedEventStatuses>> GetAllowedEventStatuses(string eventStatusId)
        {
            var arguments = GetAllowedEventStatusesArgument(eventStatusId);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetAllowedEventStatuses, arguments)
                .ContinueWith(t => GetAllowedEventStatusesContinuation(t));
        }

        private IList<AllowedEventStatuses> GetAllowedEventStatusesContinuation(Task<XElement> task)
        {

            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<AllowedEventStatuses>();
            return task.Result.Elements().Select(AllowedEventStatuses.FromXml).ToList();
        }

        private XElement GetEventCommentsArgument(string eventId)
        {
            var argument = new XElement("GetEventComments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Event_Idx", eventId));

            return argument;
        }

        public Task<IList<Comment>> GetEventComments(string eventId)
        {
            var arguments = GetEventCommentsArgument(eventId);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetEventComments, arguments)
                .ContinueWith(t => GetEventCommentsContinuation(t));
        }

        private IList<Comment> GetEventCommentsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<Comment>();
            if (task.Result.Elements("Comment").FirstOrDefault().HasElements == false)
            {
                return new List<Comment>();
            }
            return task.Result.Elements().Select(Comment.FromXml).ToList();
        }

        private XElement GetClaimCommentsArgument(string claimId)
        {
            var argument = new XElement("GetClaimComments");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Claim_Idx", claimId));

            return argument;
        }

        public Task<IList<Comment>> GetClaimComments(string claimId)
        {
            var arguments = GetClaimCommentsArgument(claimId);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetClaimComments, arguments)
                .ContinueWith(t => GetClaimCommentsContinuation(t));
        }

        private IList<Comment> GetClaimCommentsContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
                return new List<Comment>();
            if (task.Result.Elements("Comment").FirstOrDefault().HasElements == false)
            {
                return new List<Comment>();
            }
            return task.Result.Elements().Select(Comment.FromXml).ToList();
        }

        private XElement GetAddClaimCommentsArgument(AddClaimCommentDTO addClaimCommentDto)
        {
            var argument = new XElement("SaveClaimComment");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Claim_Idx", addClaimCommentDto.ClaimId));
            argument.Add(new XElement("Comment", addClaimCommentDto.Comment));
            return argument;
        }

        public Task<SprocResult> AddClaimComments(AddClaimCommentDTO addClaimCommentDto)
        {
            var arguments = GetAddClaimCommentsArgument(addClaimCommentDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.AddClaimComments, arguments)
                .ContinueWith(t => AddClaimCommentsContinuation(t));
        }

        private SprocResult AddClaimCommentsContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to add claim comments failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to add claim comments failed.";
            }

            return result;
        }

        private XElement GetAddEventCommentsArgument(AddEventCommentDTO addEventCommentDto)
        {
            var argument = new XElement("SaveEventComment");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Event_Idx", addEventCommentDto.EventId));
            argument.Add(new XElement("Comment", addEventCommentDto.Comment));
            return argument;
        }

        public Task<SprocResult> AddEventComments(AddEventCommentDTO addEventCommentDto)
        {
            var arguments = GetAddEventCommentsArgument(addEventCommentDto);
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.AddEventComments, arguments)
                .ContinueWith(t => AddEventCommentsContinuation(t));
        }

        private SprocResult AddEventCommentsContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to add claim comments failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to add claim comments failed.";
            }

            return result;
        }
        public Task<IList<SettlementReasonCode>> GetSettlementReasonCodes()
        {
            var arguments = CommonXml.GetBaseArguments("GetFilterSettlementReasonCodes");
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetSettlementReasonCodes, arguments)
                .ContinueWith(t => GetSettlementReasonCodesContinuation(t));
        }

        private IList<SettlementReasonCode> GetSettlementReasonCodesContinuation(Task<XElement> task)
        {
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            { return new List<SettlementReasonCode>(); }

            return task.Result.Elements().Select(SettlementReasonCode.FromXml).ToList();
        }

        public Task<XElement> GetClaims(XElement loadDefaultXml)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetClaims, loadDefaultXml, DisplayErrors.No).ContinueWith(t => GetClaimsEditorContinuation(t));
        }

        public Task<XElement> GetClaims(ReturnClaimsDTO applyClaimsDto)
        {
            var arguments = new XElement("GetClaims");
            arguments.Add(new XElement("User_Idx", User.CurrentUser.ID));
            arguments.Add(new XElement("Date_Search_Preference", applyClaimsDto.DateSearchPreference));

            var matchingStatuses = new XElement("MatchingStatuses");
            foreach (var matchingStatus in applyClaimsDto.ClaimMatchingStatusIds)
            {
                matchingStatuses.Add(new XElement("Claim_Matching_Status_Idx", matchingStatus));
            }

            arguments.Add(matchingStatuses);

            //var claimsStatuses = new XElement("ClaimStatuses");
            //foreach (var claimsStatus in applyClaimsDto.Statuses)
            //{
            //    claimsStatuses.Add(new XElement("Claim_Status_Idx", claimsStatus));
            //}
            //arguments.Add(claimsStatuses);

            var claimStatuses = new XElement("Statuses");
            foreach (string eventStatusId in applyClaimsDto.Statuses)
            {
                XElement thisElement = new XElement("Status");
                string[] results = eventStatusId.Split('$');

                if (results.Count() > 1)
                    thisElement.Add(new XElement("ID", results[1]));

                thisElement.Add(new XElement("Item_Type", results[0]));

                claimStatuses.Add(thisElement);

            }
            arguments.Add(claimStatuses);



            var customers = new XElement("Customers");
            foreach (var customer in applyClaimsDto.CustomerIds)
            {
                customers.Add(new XElement("Cust_Idx", customer));
            }

            arguments.Add(customers);

            var products = new XElement("Products");
            arguments.Add(products);

            foreach (var p in applyClaimsDto.ProductIds.Distinct())
            {
                products.Add(new XElement("Idx", p));
            }

            var claimValues = new XElement("Claim_Values");
            claimValues.Add(new XElement("Min_Value", applyClaimsDto.ClaimFilterMin));
            claimValues.Add(new XElement("Max_Value", applyClaimsDto.ClaimFilterMax));

            arguments.Add(claimValues);

            var claimDates = new XElement("ClaimDates");
            claimDates.Add(new XElement("StartDate", applyClaimsDto.StartDateInputValue));
            claimDates.Add(new XElement("EndDate", applyClaimsDto.EndDateInputValue));

            arguments.Add(claimDates);

            return WebServiceProxy.CallAsync(StoredProcedure.Claims.GetClaims, arguments, DisplayErrors.No).ContinueWith(t => GetClaimsEditorContinuation(t));
        }

        private XElement GetClaimsEditorContinuation(Task<XElement> task)
        {
            return task.Result;
        }

        public XElement AddDynamicClaims(string salesOrgID)
        {
            var arguments = new XElement("GetManualEntryGrid");
            arguments.Add(new XElement("User_Idx", User.CurrentUser.ID));
            arguments.Add(new XElement("SalesOrg_Idx", salesOrgID));

            return WebServiceProxy.Call(StoredProcedure.Claims.GetManualClaimsEntry, arguments, DisplayErrors.Yes, true);
        }

        public XElement ClaimEditorGetClaim(string claimID)
        {
            var arguments = CommonXml.GetBaseArguments("GetClaim");
            arguments.Add(new XElement("Claim_Idx", claimID));

            return WebServiceProxy.Call(StoredProcedure.Claims.GetClaim, arguments, DisplayErrors.Yes);
        }

        public XElement ClaimEditorGetEvents(string claimID)
        {
            var arguments = CommonXml.GetBaseArguments("GetEvents");
            arguments.Add(new XElement("Claim_Idx", claimID));

            return WebServiceProxy.Call(StoredProcedure.Claims.GetEventList, arguments, DisplayErrors.Yes);
        }

        public XElement ClaimsEditorProducts(string claimID, string eventID)
        {
            var arguments = CommonXml.GetBaseArguments("GetProducts");
            arguments.Add(new XElement("Event_Idx", eventID));
            arguments.Add(new XElement("Claim_Idx", claimID));

            return WebServiceProxy.Call(StoredProcedure.Claims.GetEventProducts, arguments, DisplayErrors.Yes);
        }

        public Task<SprocResult> ValidateClaimsEditor(XElement claimEditor)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.ClaimEditorValidate, claimEditor).ContinueWith(t => ValidateClaimsEditorContinuation(t));
        }

        private SprocResult ValidateClaimsEditorContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }
            else if (task.Result.Element("Validation") != null)
            {
                var validationElement = task.Result.Element("Validation");
                result.Success = string.Compare("1", validationElement.GetValue<string>("Outcome"), true) == 0;
                result.Message = validationElement.GetValue<string>("Message");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }

            if (result.Message != null)
                if (!result.Success)
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);


            return result;
        }

        public Task<SprocResult> SaveClaimsEditor(XElement claimEditor)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.ClaimEditorSave, claimEditor).ContinueWith(t => SaveClaimsEditorContinuation(t));
        }

        private SprocResult SaveClaimsEditorContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }

            if (result.Message != null)
            {
                if (result.Message.ToLower().Contains("fail"))
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else
                {
                    if (result.Message.Contains("Succes"))
                        MessageBox.Show(result.Message, "Success", MessageBoxButton.OK,
                            MessageBoxImage.Information);

                }


            }


            return result;
        }
        public XElement ClaimEventScreenGetClaims(string eventID)
        {
            var arguments = new XElement("GetClaims");
            arguments.Add(new XElement("User_Idx", User.CurrentUser.ID));
            arguments.Add(new XElement("Event_Idx", eventID));

            return WebServiceProxy.Call(StoredProcedure.Claims.ClaimEventEditorGetClaims, arguments, DisplayErrors.Yes);
        }

        public XElement ClaimEventScreenGetProducts(string eventID, string selectedClaimID, string ShowZero)
        {
            var arguments = new XElement("GetProducts");
            arguments.Add(new XElement("User_Idx", User.CurrentUser.ID));
            arguments.Add(new XElement("Event_Idx", eventID));
            arguments.Add(new XElement("Claim_Idx", selectedClaimID));
            arguments.Add(new XElement("ShowZeroes", ShowZero));

            return WebServiceProxy.Call(StoredProcedure.Claims.ClaimEventEditorGetProducts, arguments, DisplayErrors.Yes);
        }

        public Task<SprocResult> ValidateEventsEditor(XElement claimEditor)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.EventEditorValidateEvent, claimEditor).ContinueWith(t => ValidateClaimEventsEditorContinuation(t));
        }

        private SprocResult ValidateClaimEventsEditorContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                var res = task.Result.Element("Outcome");

                result.Success = res.Value == "1";
                result.Message = task.Result.Element("Message").Value;
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }

            return result;
        }

        public Task<SprocResult> SaveEventsEditor(XElement claimEditor)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.EventEditorSaveEvent, claimEditor).ContinueWith(t => SaveEventsEditorContinuation(t));
        }

        private SprocResult SaveEventsEditorContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to save event failed.";
            }

            return result;
        }

        public Task<SprocResult> ManuallyAddClaim(XElement claim)
        {
            return WebServiceProxy.CallAsync(StoredProcedure.Claims.SaveManualClaimsEntry, claim).ContinueWith(t => ManuallyAddClaimContinuation(t));
        }

        public List<string> ManuallyAddClaim(List<XElement> claims)
        {
            var countToSave = claims.Count();
            
            var successful = new List<string>();
            foreach (var claim in claims)
            {
              var res=   WebServiceProxy.Call(StoredProcedure.Claims.SaveManualClaimsEntry, claim);
                if (string.Compare("Success", res.GetValue<string>("Outcome"), true) == 0)
                { 
                    successful.Add(claim.Descendants("Item_Idx").First().Value);
                }
            }

            return successful;
        }

        private SprocResult ManuallyAddClaimContinuation(Task<XElement> task)
        {
            var result = new SprocResult();
            if (task.IsCanceled || task.IsFaulted || task.Result == null)
            {
                result.Success = false;
                result.Message = "Attempt to add claim failed.";
            }
            else if (task.Result.Element("Outcome") != null)
            {
                result.Success = string.Compare("Success", task.Result.GetValue<string>("Outcome"), true) == 0;
                result.Message = task.Result.GetValue<string>("Outcome");
            }
            else if (task.Result.Element("Error") != null)
            {
                result.Success = false;
                result.Message = task.Result.GetValue<string>("Error");
            }
            else
            {
                result.Success = false;
                result.Message = "Attempt to add claim failed.";
            }

            return result;
        }


    }
}
