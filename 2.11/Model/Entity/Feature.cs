namespace Model.Entity
{
    using System;
    using System.Collections.Generic;

    public class Feature : IEquatable<Feature>
    {
        private static readonly Dictionary<string, Feature> Lookup = new Dictionary<string, Feature>(StringComparer.OrdinalIgnoreCase);

        public static readonly Feature Insights = new Feature("Insights");
        public static readonly Feature Analytics = new Feature("Analytics");
        public static readonly Feature Planning = new Feature("Planning");
        public static readonly Feature PlanningSchedule = new Feature("PlanningSchedule");
        public static readonly Feature PlanningScheduleV2 = new Feature("PlanningScheduleV2");
        public static readonly Feature ScheduleView_ScheduleLink = new Feature("ScheduleView_ScheduleLink");
        public static readonly Feature Pricing = new Feature("Pricing");
        public static readonly Feature Promotions = new Feature("Promotions");
        public static readonly Feature PromoPowerEditor = new Feature("PromoPowerEditor");
        public static readonly Feature PromotionsCharting = new Feature("PromotionsCharting");
        public static readonly Feature PromotionDatePeriods = new Feature("PromotionDatePeriods");
        public static readonly Feature Cannibalisation = new Feature("Cannibalisation");
        public static readonly Feature PromotionsSubCustomer = new Feature("PromotionsSubCustomer");
        public static readonly Feature PaymentsSubCustomer = new Feature("PaymentsSubCustomer");
        public static readonly Feature PostPromo = new Feature("PostPromo");
        public static readonly Feature Payments = new Feature("Payments");
        public static readonly Feature Phasing = new Feature("Phasing");
        public static readonly Feature PhasingDaily = new Feature("Phasing.Daily");
        public static readonly Feature PhasingPost = new Feature("Phasing.Post");
        public static readonly Feature DailyVolume = new Feature("InPromoPhasing.Daily");
        public static readonly Feature WeeklyVolume = new Feature("InPromoPhasing.Weekly");
        public static readonly Feature CreateClaims = new Feature("CreateClaims");

        public static readonly Feature CreatePromotion = new Feature("CreatePromotion");
        public static readonly Feature CopyPromotion = new Feature("CopyPromotion");
        public static readonly Feature DeletePromotion = new Feature("DeletePromotion");
        public static readonly Feature CreatePromotionFromTemplate = new Feature("CreatePromotionFromTemplate");
        public static readonly Feature UploadFilesTabPromotion = new Feature("UploadFilesTabPromotion");

        public static readonly Feature CreateTemplate = new Feature("CreateTemplate");
        public static readonly Feature CopyTemplate = new Feature("CopyTemplate");
        public static readonly Feature DeleteTemplate = new Feature("DeleteTemplate");
        public static readonly Feature UploadFilesTabTemplate = new Feature("UploadFilesTabTemplate");

        public static readonly Feature CreateConditions = new Feature("CreateConditions");
        public static readonly Feature CopyConditions = new Feature("CopyConditions");
        public static readonly Feature DeleteConditions = new Feature("DeleteConditions");
        public static readonly Feature UploadFilesTabCondition = new Feature("UploadFilesTabCondition");

        public static readonly Feature CreateNpd = new Feature("CreateNpd");
        public static readonly Feature CopyNpd = new Feature("CopyNpd");
        public static readonly Feature DeleteNpd = new Feature("DeleteNpd");
        public static readonly Feature NPD_ShowForecast = new Feature("NPD_ShowForecast");

        public static readonly Feature CreateFunds = new Feature("CreateFunds");
        public static readonly Feature CopyFunds = new Feature("CopyFunds");
        public static readonly Feature DeleteFunds = new Feature("DeleteFunds");
        public static readonly Feature CreateParentFunds = new Feature("CreateParentFunds");
        public static readonly Feature CopyParentFunds = new Feature("CopyParentFunds");
        public static readonly Feature DeleteParentFunds = new Feature("DeleteParentFunds");
        public static readonly Feature SkuDetailFunds = new Feature("SkuDetailFunds");
        public static readonly Feature ScheduleFunds = new Feature("ScheduleFunds");
        public static readonly Feature FundsTransfer = new Feature("FundsTranfer");
        public static readonly Feature UploadFilesTabFund = new Feature("UploadFilesTabFund");

        public static readonly Feature CreateTerms = new Feature("CreateTerms");
        public static readonly Feature CopyTerms = new Feature("CopyTerms");
        public static readonly Feature DeleteTerms = new Feature("DeleteTerms");
        public static readonly Feature CreateTermContracts = new Feature("CreateTermContracts");
        public static readonly Feature CopyTermContracts = new Feature("CopyTermContracts");
        public static readonly Feature RemoveTermContracts = new Feature("RemoveTermContracts");
        public static readonly Feature TermContracts = new Feature("TermContracts");
        public static readonly Feature SkuDetailTerms = new Feature("SkuDetailTerms");
        public static readonly Feature ScheduleTerms = new Feature("ScheduleTerms");
        public static readonly Feature TermsDefaultTab = new Feature("TermsDefaultTab");
        public static readonly Feature TermsDefaultGroupCreator = new Feature("TermsDefaultGroupCreator");
        public static readonly Feature TermsEditorFileSelection = new Feature("TermsEditorFileSelection");
        public static readonly Feature TermsShowGroupCreator = new Feature("TermsShowGroupCreator");
        public static readonly Feature UploadFilesTabTerm = new Feature("UploadFilesTab_ROB_TERMS");
        public static readonly Feature UploadFilesTabTermContracts = new Feature("UploadFilesTab_ROB_TERMS_Contracts");


        public static readonly Feature CreateTarget = new Feature("CreateTarget");
        public static readonly Feature CopyTarget = new Feature("CopyTarget");
        public static readonly Feature DeleteTarget = new Feature("DeleteTarget");
        public static readonly Feature CreateTargetContracts = new Feature("CreateTargetContracts");
        public static readonly Feature CopyTargetContracts = new Feature("CopyTargetContracts");
        public static readonly Feature RemoveTargetContracts = new Feature("RemoveTargetContracts");
        public static readonly Feature TargetContracts = new Feature("TargetContracts");
        public static readonly Feature SkuDetailTarget = new Feature("SkuDetailTarget");
        public static readonly Feature ScheduleTarget = new Feature("ScheduleTarget");
        public static readonly Feature TargetDefaultGroupCreator = new Feature("TargetDefaultGroupCreator");
        public static readonly Feature TargetEditorFileSelection = new Feature("TargetEditorFileSelection");
        public static readonly Feature TargetShowGroupCreator = new Feature("TargetShowGroupCreator");
        public static readonly Feature UploadFilesTabTarget = new Feature("UploadFilesTab_ROB_TARGET");
        public static readonly Feature UploadFilesTabTargetContracts = new Feature("UploadFilesTab_ROB_TARGET_Contracts");


        public static readonly Feature CreateManagementAdjust = new Feature("CreateManagementAdjust");
        public static readonly Feature CopyManagementAdjust = new Feature("CopyManagementAdjust");
        public static readonly Feature DeleteManagementAdjust = new Feature("DeleteManagementAdjust");
        public static readonly Feature CreateManagementAdjustContracts = new Feature("CreateManagementAdjustContracts");
        public static readonly Feature ManagementAdjustContracts = new Feature("ManagementAdjustContracts");
        public static readonly Feature CopyManagementAdjustContracts = new Feature("CopyManagementAdjustContracts");
        public static readonly Feature RemoveManagementAdjustContracts = new Feature("RemoveManagementAdjustContracts");
        public static readonly Feature SkuDetailManagementAdjust = new Feature("SkuDetailManagementAdjust");
        public static readonly Feature ScheduleManagementAdjust = new Feature("ScheduleManagementAdjust");
        public static readonly Feature ManagementAdjustDefaultGroupCreator = new Feature("ManagementAdjustDefaultGroupCreator");
        public static readonly Feature ManagementAdjustEditorFileSelection = new Feature("ManagementAdjustEditorFileSelection");
        public static readonly Feature ManagementAdjustShowGroupCreator = new Feature("ManagementAdjustShowGroupCreator");
        public static readonly Feature UploadFilesTabManagementAdjust = new Feature("UploadFilesTab_ROB_MANAGEMENTADJUST");
        public static readonly Feature UploadFilesTabManagementAdjustContracts = new Feature("UploadFilesTab_ROB_MANAGEMENTADJUST_Contracts");

        public static readonly Feature CreateMarketing = new Feature("CreateMarketing");
        public static readonly Feature CopyMarketing = new Feature("CopyMarketing");
        public static readonly Feature DeleteMarketing = new Feature("DeleteMarketing");
        public static readonly Feature CreateMarketingContracts = new Feature("CreateMarketingContracts");
        public static readonly Feature MarketingContracts = new Feature("MarketingContracts");
        public static readonly Feature CopyMarketingContracts = new Feature("CopyMarketingContracts");
        public static readonly Feature RemoveMarketingContracts = new Feature("RemoveMarketingContracts");
        public static readonly Feature SkuDetailMarketing = new Feature("SkuDetailMarketing");
        public static readonly Feature ScheduleMarketing = new Feature("ScheduleMarketing");
        public static readonly Feature MarketingDefaultGroupCreator = new Feature("MarketingDefaultGroupCreator");
        public static readonly Feature MarketingEditorFileSelection = new Feature("MarketingEditorFileSelection");
        public static readonly Feature MarketingShowGroupCreator = new Feature("MarketingShowGroupCreator");
        public static readonly Feature UploadFilesTabMarketing = new Feature("UploadFilesTab_ROB_MARKETING");
        public static readonly Feature UploadFilesTabMarketingContracts = new Feature("UploadFilesTab_ROB_MARKETING_Contracts");

        public static readonly Feature CreateRiskOps = new Feature("CreateRiskOps");
        public static readonly Feature CopyRiskOps = new Feature("CopyRiskOps");
        public static readonly Feature DeleteRiskOps = new Feature("DeleteRiskOps");
        public static readonly Feature CreateRiskOpsContracts = new Feature("CreateRiskOpsContracts");
        public static readonly Feature RiskOpsContracts = new Feature("RiskOpsContracts");
        public static readonly Feature CopyRiskOpsContracts = new Feature("CopyRiskOpsContracts");
        public static readonly Feature RemoveRiskOpsContracts = new Feature("RemoveRiskOpsContracts");
        public static readonly Feature SkuDetailRiskOps = new Feature("SkuDetailRiskOps");
        public static readonly Feature ScheduleRiskOps = new Feature("ScheduleRiskOps");
        public static readonly Feature RiskOpsDefaultGroupCreator = new Feature("RiskOpsDefaultGroupCreator");
        public static readonly Feature RiskOpsEditorFileSelection = new Feature("RiskOpsEditorFileSelection");
        public static readonly Feature RiskOpsShowGroupCreator = new Feature("RiskOpsShowGroupCreator");
        public static readonly Feature UploadFilesTabRiskOps = new Feature("UploadFilesTab_ROB_RISK_OPS");
        public static readonly Feature UploadFilesTabRiskOpsContracts = new Feature("UploadFilesTab_ROB_RISK_OPS_Contracts");

        public static readonly Feature CondtionSchedule = new Feature("CondtionSchedule");

        public static readonly Feature ScenarioSchedule = new Feature("ScenarioSchedule");
        public static readonly Feature CreateScenario = new Feature("CreateScenario");
        public static readonly Feature CopyScenario = new Feature("CopyScenario");
        public static readonly Feature DeleteScenario = new Feature("DeleteScenario");
        public static readonly Feature CloseScenario = new Feature("CloseScenario");
        public static readonly Feature UpdateBudgetScenario = new Feature("UpdateBudgetScenario");
        public static readonly Feature EditLastClosedScenario = new Feature("EditLastClosedScenario");
        public static readonly Feature ExportScenario = new Feature("ExportScenario");
        public static readonly Feature CreatePricing = new Feature("CreatePricing");
        public static readonly Feature IsPromotionScenarioActive = new Feature("PromotionScenario");
        public static readonly Feature ScenarioManagement = new Feature("ScenarioManagement");
        public static readonly Feature UploadFilesTabScenario = new Feature("UploadFilesTabScenario");

        public static readonly Feature Conditions = new Feature("Conditions");
        public static readonly Feature NPD = new Feature("NPD");

        public static readonly Feature Claims = new Feature("Claims");
        public static readonly Feature UploadFilesTabClaims = new Feature("UploadFilesTabClaims");
        public static readonly Feature UploadFilesTabClaimsEvents = new Feature("UploadFilesTabClaimsEvents");

        public static readonly Feature Admin = new Feature("AdminTab");
        public static readonly Feature FeedbackActive = new Feature("FeedbackActive");
        public static readonly Feature Jobs = new Feature("JobsTab");
        public static readonly Feature FilterOnKeystroke = new Feature("FilterOnKeystroke");
        public static readonly Feature EnablePromotionProductsReferenceData = new Feature("EnablePromotionProductsReferenceData");
        public static readonly Feature EnableTemplateConstraints = new Feature("EnableTemplateConstraints");

        public static readonly Feature DiagnosticsAccountPlanQueues = new Feature("DiagnosticsAccountPlanQueues");
        public static readonly Feature DiagnosticsTab = new Feature("DiagnosticsTab");

        public static readonly Feature NoFeatures = new Feature("NoFeatures");
        public static readonly Feature ListingsProductSelectedByDefault = new Feature("ListingsProductSelectedByDefault");

        public static readonly Feature RetainFilters = new Feature("RetainFilters");

        public static readonly Feature DeleteDelistedProducts = new Feature("HideDelistedProducts");

        public static readonly Feature PopupWindows = new Feature("PopupWindows");
        public static readonly Feature FiltersClosed = new Feature("FiltersClosed");

        public static readonly Feature IsSettingsAppDetailsVisible = new Feature("IsSettingsAppDetailsVisible");

        public static readonly Feature CanSignInWithTwitter = new Feature("CanSignInWithTwitter");


        #region Demand Features

        public static readonly Feature NewFc = new Feature("NewFc");
        public static readonly Feature CopyFc = new Feature("CopyFc");
        public static readonly Feature DeleteFc = new Feature("DeleteFc");
        public static readonly Feature BulkFc = new Feature("BulkFc");

        
        #endregion

        private readonly string _name;

        private Feature(string name)
        {
            _name = name;
            Lookup.Add(name, this);
        }

        public string Name
        {
            get { return _name; }
        }


        public static bool IsDefined(string featureName)
        {
            return Lookup.ContainsKey(featureName);
            //return typeof (Feature).GetField(featureName, BindingFlags.Static | BindingFlags.Public) != null;
        }

        public static Feature Parse(string featureName)
        {
            if (!Lookup.ContainsKey(featureName)) throw new OverflowException();
            return Lookup[featureName];
            //var fieldInfo = typeof (Feature).GetField(featureName, BindingFlags.Static | BindingFlags.Public);
            //if (fieldInfo == null) throw new OverflowException();
            //return (Feature)fieldInfo.GetValue(null);
        }

        public bool Equals(Feature other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Feature)) return false;
            return Equals((Feature)obj);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public static bool operator ==(Feature left, Feature right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Feature left, Feature right)
        {
            return !Equals(left, right);
        }

        public static IEnumerable<Feature> All()
        {
            yield return Insights;
            yield return Planning;
            yield return Pricing;
            yield return Promotions;
        }

        public static IEnumerable<Feature> None()
        {
            yield return NoFeatures;
        }


    }
}