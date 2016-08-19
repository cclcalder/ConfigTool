using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Entity.Generic;
using Model.Entity.GroupEditor;
using Model.Entity.ROBs;
using Status = Model.Entity.Generic.Status;

namespace Model.DataAccess
{

    public class GroupListAccess
    {

        private static XElement CreateArgs(string name, string appTypeID)
        {
            var args = new XElement(name);
            args.Add(new XElement("User_Idx", User.CurrentUser.ID));
            args.Add(new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID));
            args.Add(new XElement("AppType_Idx", appTypeID));
            return args;
        }


        #region Filters
 
        public string UpdateStatusGroups(string[] Ids, string StatusID, string appTypeID)
        {
            XElement argument = new XElement("UpdateROBStatus");
            argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
            argument.Add(new XElement("Target_Status_Idx", StatusID));
            argument.Add(new XElement("AppType_Idx", appTypeID));
            argument.Add(new XElement("ROBs"));

            foreach (var id in Ids)
            {
                argument.Element("ROBs").Add(new XElement("ROB_Idx", id));
            }

            var node = WebServiceProxy.Call(StoredProcedure.RobGroup.MultipleStatusUpdate, argument).Elements().FirstOrDefault();

            return node.Value;
        }
    #endregion
        

        #region Grid
         

        public Task<XElement> GetGroups(string appTypeID, IEnumerable<string> statusIDs, List<string> inputCustomers, List<string> inputProducts, DateTime? start, DateTime? end)
        {
            var args = CreateArgs("GetRobs", appTypeID);
            var statuses = new XElement("Statuses");
            foreach (var statusID in statusIDs)
            {
                statuses.Add(new XElement("ID", statusID));
            }
            args.Add(statuses);

            var customers = new XElement("Customers");
            foreach (var customer in inputCustomers.Distinct())
            {
                customers.Add(new XElement("ID", customer));
            }
            args.Add(customers);

            var products = new XElement("Products");
            foreach (var product in inputProducts.Distinct())
            {
                products.Add(new XElement("ID", product));
            }
            args.Add(products);

            if (!start.HasValue)
            {
                start = new DateTime(1970, 1, 1);
            }
            if (!end.HasValue)
            {
                end = new DateTime(2999, 12, 31);
            }
            args.Add(new XElement("Start", start.ToString("yyyy-MM-dd")));
            args.Add(new XElement("End", end.ToString("yyyy-MM-dd")));
            return WebServiceProxy.CallAsync(StoredProcedure.ROB.GetRobs, args, DisplayErrors.Yes)
                .ContinueWith(t => GetGroupsContinuation(t));
        }

        private XElement GetGroupsContinuation(Task<XElement> task)
        {
            if (task.IsFaulted || task.IsCanceled || task.Result == null) return null;
            var robs = task.Result;
            return robs;
        }


        #endregion


#region Buttons
         
        public bool RemoveGroup(string appTypeId, List<string> robs)
        {
            var args = CommonXml.GetBaseArguments("DeleteROBGroup");
            args.AddElement("AppType_Idx", appTypeId);
            args.Add(InputConverter.ToList("ROBGroup", "Idx", robs));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.RobGroup.Remove, args));
        }

        private XElement RemoveGroupContinuation(Task<XElement> task)
        {
            return task.Result;
        }

        public bool CopyGroup(string appTypeId, List<string> groups)
        {
            var args = CommonXml.GetBaseArguments("CopyROBGroup");
            args.AddElement("AppType_Idx", appTypeId);
            args.Add(InputConverter.ToList("ROBGroup", "Idx", groups));

            return MessageConverter.DisplayMessage(WebServiceProxy.Call(StoredProcedure.RobGroup.Copy, args));
        }
         
#endregion
    }

   public class GroupEditorAccess
   {
       // Procast_SP_ROBGroup_GetROBGroup

       // Procast_SP_ROBGroup_GetROBsDetails


       // Procast_SP_ROBGroup_SaveROBGroup

       #region Recipients

       public XElement GetRobRecipient(string appTypeID, string robGroupID, IList<string> customersIDs)
       {
           var args = CreateArgs("GetROBRecipients", appTypeID);

           args.Add(new XElement("ROBGroup_Idx", robGroupID));

           if (customersIDs.Count > 0)
               args.Add(new XElement("Customers", customersIDs.Select(c => new XElement("ID", c))));

           return args;
       }

       private IList<RobRecipient> GetRobRecipientContinuation(Task<XElement> task)
       {
           if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<RobRecipient>();
           return task.Result.Elements("ROBRecipient").Select(RobRecipient.FromXml).ToList();
       }

       #endregion

       #region Statuses

       // Procast_SP_ROBGroup_GetWorkflowStatuses
       public Task<IList<Status>> GetWorkflowStatuses(string appTypeID, string robGroupID)
       {
      //<GetWorkflowStatuses>
      //  <UserID>1</UserID>
      //  <AppTypeID>100</AppTypeID>
      //  <ROBGroupID>1</ROBGroupID>
      //</GetWorkflowStatuses>

           var args = CreateArgs("GetWorkflowStatuses", appTypeID);
           args.Add(new XElement("ROBGroup_Idx", robGroupID));
           return WebServiceProxy.CallAsync(StoredProcedure.RobGroup.GetWorkflowStatuses, args, DisplayErrors.No)
               .ContinueWith(t => GetStatusesContinuation(t));
       }

       private IList<Status> GetStatusesContinuation(Task<XElement> task)
       {
           if (task.IsFaulted || task.IsCanceled) return new List<Status>();
           return task.Result.Elements("Status").Select(Status.FromXml).ToList();
       }

        #endregion

       #region Scenarios

       // Procast_SP_ROBGroup_GetScenarios
       public Task<IEnumerable<ComboboxItem>> GetScenarios(string appTypeID, string robGroupID)
       {
            //<GetScenarios>
            //  <UserID>1</UserID>
            //  <ROBGroup_Idx>1</ROBGroup_Idx>
            //</GetScenarios>

           var args = CreateArgs("GetScenarios", appTypeID);
           args.Add(new XElement("ROBGroup_Idx", robGroupID));

           return DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.RobGroup.GetScenarios, args);
       }

        #endregion

       #region Detail

       // Procast_SP_ROBGroup_GetWorkflowStatuses
       public Task<GroupBase> GetDetail(string appTypeID, string robGroupID)
       {
           //<GetWorkflowStatuses>
           //  <UserID>1</UserID>
           //  <AppTypeID>100</AppTypeID>
           //  <ROBGroupID>1</ROBGroupID>
           //</GetWorkflowStatuses>

           var args = CreateArgs("GetROBGroup", appTypeID);
           args.Add(new XElement("ROBGroup_Idx", robGroupID));
           return WebServiceProxy.CallAsync(StoredProcedure.RobGroup.GetROBGroup, args, DisplayErrors.No)
               .ContinueWith(t => GetDetailContinuation(t));
       }

       private GroupBase GetDetailContinuation(Task<XElement> task)
       {
           if (task.IsFaulted || task.IsCanceled) return new GroupBase();
           return task.Result.Elements("ROBDetails").Select(GroupBase.FromXml).FirstOrDefault();
       }

       #endregion

     
       private static XElement CreateArgs(string name, string appTypeID)
       {
           var args = new XElement(name);
           args.Add(new XElement("User_Idx", User.CurrentUser.ID));
           args.Add(new XElement("SalesOrg_Idx", User.CurrentUser.SalesOrganisationID));
           args.Add(new XElement("AppType_Idx", appTypeID));
           return args;
       }

       #region Save
       public XElement Save(string argument)
       { 
           var results = WebServiceProxy.Call(StoredProcedure.RobGroup.Save, argument);  
           return results;
       }

       #endregion

       #region DynamciGridWrapper

       public Task<XElement> LoadDynamicGrid(string appTypeID, string robGroupID)
       {
           var args = CreateArgs("GetROBsDetails", appTypeID);
           args.Add(new XElement("ROBGroup_Idx", robGroupID));

           return WebServiceProxy.CallAsync(StoredProcedure.RobGroup.GetROBGroupDetailsGrid, args, DisplayErrors.No)
               .ContinueWith(t => LoadDynamicGridContinuation(t));
       }

       private XElement LoadDynamicGridContinuation(Task<XElement> task)
       {
           if (task.IsFaulted || task.IsCanceled) return null;
           return task.Result;
       }

       #endregion

       #region Comments


       /// <summary>
       /// Returns list of added comments for a given fund
       /// </summary>
       /// <param name="fundID"></param>
       /// <returns></returns>
       public Task<IEnumerable<Note>> GetNotes(string robID)
       {

           XElement argument = new XElement("GetComments");
           argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
           argument.Add(new XElement("ROBGroup_Idx", robID));

           return WebServiceProxy.CallAsync(StoredProcedure.RobGroup.GetComments, argument, DisplayErrors.No).ContinueWith(t => GetNotesContinuation(t)); ;

       }

       private IEnumerable<Note> GetNotesContinuation(Task<XElement> task)
       {
           if (task.IsFaulted || task.IsCanceled || task.Result == null) return new List<Note>();

           return task.Result.Elements("Comment").Select(Note.FromXml).ToList();
       }


       public string AddNote(string robID, string comment)
       {
           XElement argument = new XElement("AddComment");
           argument.Add(new XElement("User_Idx", User.CurrentUser.ID));
           argument.Add(new XElement("ROBGroup_Idx", robID));
           argument.Add(new XElement("Comment", comment));

           var node = WebServiceProxy.Call(StoredProcedure.RobGroup.AddComment, argument).Elements().FirstOrDefault();

           return node.Value;
       }


       #endregion

    
   }
}
