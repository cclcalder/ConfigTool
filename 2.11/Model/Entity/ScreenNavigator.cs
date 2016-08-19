using System.Collections.Generic;

namespace Model.Entity
{
    public enum ScreenKeys
    {
        SETTINGS,
        CONDITION,
        INSIGHTS,
        REPORTS,
        CANVAS,
        ANALYTICS,
        PLANNING,
        SCHEDULE,
        PRICING,
        PROMOTION,
        SCENARIO,
        FUND,
        NPD,
        LISTINGSMGMT,
        CLAIM,
        ADMIN,
        ROB_TERMS = 300,
        TERMSv2,
        ROB_RISK_OPS = 400,
        ROB_MARKETING = 500,
        ROB_MANAGEMENTADJUST = 600,
        ROB_TARGET = 700,
        DPSQL,
        SEASONALS,
        FCMGMT,
        DASHBOARD,
        SUPERSESSION,
        EPOS,
        DEMANDCONTROL,
        PHASINGPROFILES,
        PROMOTIONPHASING,
        CONTROLSTEST,
        PARENTSCREEN
    }

    public static class ScreenNavigator
    {
        private static readonly Dictionary<string, string> ScreenKeyToUri = new Dictionary<string, string>
        {
            {ScreenKeys.SETTINGS.ToString(), "/Pages/UserSettings/Default.xaml"},
            {ScreenKeys.INSIGHTS.ToString(), "/Pages/Insights.xaml"},
            {ScreenKeys.REPORTS.ToString(), "/Pages/Combi/ReportWrapper.xaml"},
            {ScreenKeys.CANVAS.ToString(), "/Pages/Canvas/Canvas.xaml"}, 
            {ScreenKeys.ANALYTICS.ToString(), "/Pages/AnalyticsV2.xaml"},
            {ScreenKeys.PLANNING.ToString(), "/Pages/Planning.xaml"}, 
            {ScreenKeys.SCHEDULE.ToString(), "/Pages/SchedulePageNewFilters.xaml"},
            {ScreenKeys.PRICING.ToString(), "/Pages/Pricing.xaml"},
            {ScreenKeys.PROMOTION.ToString(), "/Pages/Promotions.xaml"},
            {ScreenKeys.SCENARIO.ToString(), "/Pages/ScenariosList.xaml"},
            {ScreenKeys.FUND.ToString(), "/Pages/Funds/FundsList.xaml"},
            {ScreenKeys.CONDITION.ToString(), "/Pages/Conditions.xaml"},
            {ScreenKeys.NPD.ToString(), "/Pages/NPD/NPDList.xaml"},
             {ScreenKeys.LISTINGSMGMT.ToString(), "/Pages/LISTINGSMGMT/EditList.xaml"},
            {ScreenKeys.CLAIM.ToString(), "/Pages/Claims.xaml"},
            {ScreenKeys.ADMIN.ToString(), "/Pages/Admin/SideMenu.xaml"}, 
            {ScreenKeys.ROB_TERMS.ToString(), "RobScreen_"},
            {ScreenKeys.ROB_RISK_OPS.ToString(), "RobScreen_"},
            {ScreenKeys.ROB_MARKETING.ToString(), "RobScreen_"},
            {ScreenKeys.ROB_MANAGEMENTADJUST.ToString(), "RobScreen_"},
            {ScreenKeys.ROB_TARGET.ToString(), "RobScreen_"},
            {ScreenKeys.FCMGMT.ToString(), "/Pages/Demand/DPFcMgmt/DPFcMgmtList.xaml"},
            {ScreenKeys.DPSQL.ToString(), "/Pages/Demand/DPMain.xaml"},
            {ScreenKeys.DEMANDCONTROL.ToString(), "/Pages/Demand/DemandControl.xaml"},
            {ScreenKeys.PHASINGPROFILES.ToString(), "/Pages/Phasings/PhasingProfile.xaml"},
            {ScreenKeys.PROMOTIONPHASING.ToString(), "/Pages/Phasings/PromotionPhasing.xaml"},
            {ScreenKeys.TERMSv2.ToString(), "/Pages/RobGroups/GroupsList.xaml"},
            {ScreenKeys.DASHBOARD.ToString(), "/Pages/MyActionsDashboard/MyActionsDashboard.xaml"},
            {ScreenKeys.SUPERSESSION.ToString(), "/Pages/Supersession/Supersession.xaml"},
            {ScreenKeys.EPOS.ToString(), "/Pages/EPOS/Home.xaml"},
            {ScreenKeys.SEASONALS.ToString(), "/Pages/Demand/Seasonals/SeasonalsConfiguration.xaml"},
            {ScreenKeys.CONTROLSTEST.ToString(), "/Pages/Dev/ControlsTest.xaml"},
            {ScreenKeys.PARENTSCREEN.ToString(), "/Pages/NoWhere.xaml"}
        };

        public static string GetUri(string screenKey, bool showGroup = false, string robTypeID ="" )
        {
            var temp = string.Empty;
            //get raw path
            ScreenKeyToUri.TryGetValue(screenKey, out temp);

            if (!string.IsNullOrEmpty(robTypeID))
            {
                if (showGroup)
                {
                   temp = "/Pages/RobGroups/GroupsList.xaml";
                }
                else
                {
                    temp += robTypeID;
                }
            }

            return temp;

        }
    }
}
