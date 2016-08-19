using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
 

namespace Model.DataAccess
{
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using System.Threading.Tasks;


    //SaveDefaults    - Procast_SP_SaveUserPrefs_Schedule
    //GetStatuses     - Procast_SP_SCHD_GetStatuses
    //GetCustomers    - Procast_SP_SCHD_GetCustomers
    //GetDates        - Procast_SP_SCHD_GetDates
    //GetScheduleData - Procast_SP_SCHD_GetItems

    public class PromotionStatuses
    {
        public IEnumerable<ScheduleStatuses> GetScheduleStatuses()
        {
            // if (_PromotionData == null)

            #region  Sample input XMl

              // Expected input XML:
              //  <GetPromotionStatuses>
              //    <User_Idx>100</User_Idx>
              //    <SalesOrg_Idx>100</SalesOrg_Idx>
              //  </GetPromotionStatuses>

              //Expected output XML:          
              //  <Results>
              //    <PromotionStatuses>
              //      <Status_Idx>1</Status_Idx>
              //      <Status_Name>Draft</Status_Name>
              //      <Status_Colour>#ffffff</Status_Colour>
              //    </PromotionStatuses>
              //    <PromotionStatuses>
              //      <Status_Idx>8</Status_Idx>
              //      <Status_Name>Planned</Status_Name>
              //      <Status_Colour>#8B668B</Status_Colour>
              //    </PromotionStatuses>
              //  </Results>


            #endregion


            string arguments = "<GetPromotionStatuses><UserID>{0}</UserID><SalesOrg_Idx></SalesOrg_Idx></GetPromotionStatuses>".FormatWith(User.CurrentUser.ID);
            return WebServiceProxy.Call(StoredProcedure.Schedule.GetStatuses, XElement.Parse(arguments))
                .Elements("Type")
                .Select(xml => new ScheduleStatuses(xml));

           
        }

    }
    
}
