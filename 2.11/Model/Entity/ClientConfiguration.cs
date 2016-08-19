using System;
using System.Windows;
using Exceedra.Common;
using Model.Entity.Diagnostics;


namespace Model.Entity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class ClientConfiguration
    {
        public static readonly ClientConfiguration Empty = new ClientConfiguration(Enumerable.Empty<Feature>(),
                                                                                   Enumerable.Empty<ROBScreen>(),
                                                                                   Enumerable.Empty<Screen>());
        private static HashSet<Feature> _activeFeatures;
        // private readonly List<ROBScreen> _robScreens;
        private   List<Screen> _screens;
        private readonly string _startScreen;
        private Dictionary<string, string> _analytics;
        private Dictionary<string, string> _configuration;
        public ClientConfiguration(IEnumerable<Feature> activeFeatures, IEnumerable<ROBScreen> robScreens, IEnumerable<Screen> screens, string startScreen = null, Dictionary<string, string> analytics = null, Dictionary<string, string> configuration = null)
        {
            _activeFeatures = new HashSet<Feature>(activeFeatures);
            // _robScreens = new List<ROBScreen>(robScreens);
            _screens = new List<Screen>(screens);

            var defualtScreen = GetScreens().FirstOrDefault(screen => screen.IsDefault);
            if (defualtScreen != null)
                _startScreen = defualtScreen.Uri;

            _analytics = analytics;
            _configuration = configuration;

            //const string fundingAppTypeTitle = "Funding";
            //var fundingScreen = _robScreens.FirstOrDefault(rs => rs.Title == fundingAppTypeTitle);
            //if (fundingScreen != null)
            //    FundingAppTypeId = fundingScreen.AppTypeID;
        }

        public void Logout()
        {
            _activeFeatures = null;
            _screens = null;
            _analytics = null;
            _configuration = null;
        }

        public string StartScreen
        {
            get { return _startScreen; }
        }

        public static ClientConfiguration Everything
        {
            get { return new ClientConfiguration(Feature.None(), Enumerable.Empty<ROBScreen>(), Enumerable.Empty<Screen>()); }
        }

        public Dictionary<string, string> Analytics
        {
            get { return _analytics; }

        }

        public Dictionary<string, string> Configuration
        {
            get { return _configuration; }

        }

        public static Visibility IsFeatureVisible(string featureName)
        {
            Feature feature;

            try
            {
                feature = Feature.Parse(featureName);
            }
            catch
            {
                return Visibility.Collapsed;
            }

            if (feature == null || _activeFeatures == null) return Visibility.Collapsed;
            if (_activeFeatures.Contains(feature)) return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public bool IsPromotionsChartingActive
        {
            get
            {
                return _activeFeatures.Contains(Feature.PromotionsCharting);
            }
        }

        public bool IsPromotionDatePeriodsActive
        {
            get { return _activeFeatures.Contains(Feature.PromotionDatePeriods); }
        }

        public bool IsCannibalisationActive
        {
            get { return _activeFeatures.Contains(Feature.Cannibalisation); }
        }

        //Can be removed?
        public bool IsPostPromoActive
        {
            get { return _activeFeatures.Contains(Feature.PostPromo); }
        }

        public bool IsPromotionsSubCustomerActive
        {
            get { return true; }//_activeFeatures.Contains(Feature.PromotionsSubCustomer)
        }

        public bool IsPaymentsSubCustomerActive
        {
            get { return _activeFeatures.Contains(Feature.PaymentsSubCustomer); }
        }

        public bool IsDiagnosticsAccountPlanQueues
        {
            get { return _activeFeatures.Contains(Feature.DiagnosticsAccountPlanQueues); }
        }

        public bool IsDiagnosticsTab
        {
            get { return _activeFeatures.Contains(Feature.DiagnosticsTab); }
        }

        public bool IsPaymentsActive
        {
            get { return _activeFeatures.Contains(Feature.Payments); }
        }

        public bool IsPhasingActive
        {
            get { return _activeFeatures.Contains(Feature.Phasing); }
        }

        public List<Screen> ROBScreens
        {
            get { return GetScreens().Where(p => p.RobAppType != null).ToList(); }
        }

        public List<Screen> Screens
        {
            get { return _screens; }
            set
            {
                _screens = value; 
            }
        }

        public Screen GetScreen(ScreenKeys screenKey)
        {
            if (Screens == null) return null;

            Screen screen = GetScreens().FirstOrDefault(s => s.Key == screenKey.ToString());
            if (screen == null) return null;

            return screen;
        }

        public IEnumerable<Screen> GetScreens()
        {
            var screens = Screens.Flatten(s => s.Children);
            return screens;
        } 

        public bool IsPhasingDailyActive
        {
            get { return _activeFeatures.Contains(Feature.PhasingDaily); }
        }

        public bool IsPhasingPostActive
        {
            get { return _activeFeatures.Contains(Feature.PhasingPost); }
        }

        public bool IsDailyVolumePhasingActive
        {
            get { return _activeFeatures.Contains(Feature.DailyVolume); }
        }

        public bool IsWeeklyVolumePhasingActive
        {
            get { return _activeFeatures.Contains(Feature.WeeklyVolume); }
        }

        public bool IsCreateClaimsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateClaims); }
        }


        // target

        public bool IsCreateTargetActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTarget); }
        }

        public bool IsCopyTargetActive
        {
            get { return _activeFeatures.Contains(Feature.CopyTarget); }
        }

        public bool IsDeleteTargetActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteTarget); }
        }

        public bool IsTargetContractsActive
        {
            get { return _activeFeatures.Contains(Feature.TargetContracts); }
        }

        public bool IsCreateTargetContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTargetContracts); }
        }

        public bool IsCopyTargetContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyTargetContracts); }
        }

        public bool IsRemoveTargetContractsActive
        {
            get { return _activeFeatures.Contains(Feature.RemoveTargetContracts); }
        }

        public bool IsSkuDetailTargetActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailTarget); }
        }

        public bool IsScheduleTargetActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleTarget); }
        }

        public static string TargetDefaultTab { get; set; }

        public bool IsGroupCreatorCheckedInTarget
        {
            get { return _activeFeatures.Contains(Feature.TargetDefaultGroupCreator); }
        }

        public bool IsGroupCreatorVisibileInTarget
        {
            get { return _activeFeatures.Contains(Feature.TargetShowGroupCreator); }
        }

        public bool TargetEditorFolderSelector
        {
            get { return _activeFeatures.Contains(Feature.TargetEditorFileSelection); }
        }


        // management adjust

        public bool IsCreateManagementAdjustActive
        {
            get { return _activeFeatures.Contains(Feature.CreateManagementAdjust); }
        }

        public bool IsCopyManagementAdjustActive
        {
            get { return _activeFeatures.Contains(Feature.CopyManagementAdjust); }
        }

        public bool IsDeleteManagementAdjustActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteManagementAdjust); }
        }

        public bool IsManagementAdjustContractsActive
        {
            get { return _activeFeatures.Contains(Feature.ManagementAdjustContracts); }
        }

        public bool IsCreateManagementAdjustContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateManagementAdjustContracts); }
        }

        public bool IsCopyManagementAdjustContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyManagementAdjustContracts); }
        }

        public bool IsRemoveManagementAdjustContractsActive
        {
            get { return _activeFeatures.Contains(Feature.RemoveManagementAdjustContracts); }
        }

        public bool IsSkuDetailManagementAdjustActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailManagementAdjust); }
        }

        public bool IsScheduleManagementAdjustActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleManagementAdjust); }
        }

        public static string ManagementAdjustDefaultTab { get; set; }

        public bool IsGroupCreatorCheckedInManagementAdjust
        {
            get { return _activeFeatures.Contains(Feature.ManagementAdjustDefaultGroupCreator); }
        }

        public bool IsGroupCreatorVisibileInManagementAdjut
        {
            get { return _activeFeatures.Contains(Feature.ManagementAdjustShowGroupCreator); }
        }

        public bool ManagementAdjustEditorFileSelector
        {
            get { return _activeFeatures.Contains(Feature.ManagementAdjustEditorFileSelection); }
        }


        // marketing

        public bool IsCreateMarketingActive
        {
            get { return _activeFeatures.Contains(Feature.CreateMarketing); }
        }

        public bool IsCopyMarketingActive
        {
            get { return _activeFeatures.Contains(Feature.CopyMarketing); }
        }

        public bool IsDeleteMarketingActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteMarketing); }
        }

        public bool IsMarketingContractsActive
        {
            get { return _activeFeatures.Contains(Feature.MarketingContracts); }
        }

        public bool IsCreateMarketingContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateMarketingContracts); }
        }

        public bool IsCopyMarketingContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyMarketingContracts); }
        }

        public bool IsRemoveMarketingContractsActive
        {
            get { return _activeFeatures.Contains(Feature.RemoveMarketingContracts); }
        }

        public bool IsSkuDetailMarketingActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailMarketing); }
        }

        public bool IsScheduleMarketingActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleMarketing); }
        }

        public static string MarketingDefaultTab { get; set; }

        public bool IsGroupCreatorCheckedInMarketing
        {
            get { return _activeFeatures.Contains(Feature.MarketingDefaultGroupCreator); }
        }

        public bool IsGroupCreatorVisibleInMarketing
        {
            get { return _activeFeatures.Contains(Feature.MarketingShowGroupCreator); }
        }

        public bool MarketingEditorFileSelector
        {
            get { return _activeFeatures.Contains(Feature.MarketingEditorFileSelection); }
        }

        public bool IsGroupCreatorVisibileInMarketing
        {
            get { return _activeFeatures.Contains(Feature.MarketingShowGroupCreator); }
        }


        // risk & ops

        public bool IsCreateRiskOpsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateRiskOps); }
        }

        public bool IsCopyRiskOpsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyRiskOps); }
        }

        public bool IsDeleteRiskOpsActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteRiskOps); }
        }

        public bool IsRiskOpsContractsActive
        {
            get { return _activeFeatures.Contains(Feature.RiskOpsContracts); }
        }

        public bool IsCreateRiskOpsContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateRiskOpsContracts); }
        }

        public bool IsCopyRiskOpsContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyRiskOpsContracts); }
        }

        public bool IsRemoveRiskOpsContractsActive
        {
            get { return _activeFeatures.Contains(Feature.RemoveRiskOpsContracts); }
        }

        public bool IsSkuDetailRiskOpsActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailRiskOps); }
        }

        public bool IsScheduleRiskOpsActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleRiskOps); }
        }

        public static string RiskOpsDefaultTab { get; set; }

        public bool IsGroupCreatorCheckedInRiskOps
        {
            get { return _activeFeatures.Contains(Feature.RiskOpsDefaultGroupCreator); }
        }

        public bool IsGroupCreatorVisibleInRiskOps
        {
            get { return _activeFeatures.Contains(Feature.RiskOpsShowGroupCreator); }
        }

        public bool RiskOpsEditorFolderSelector
        {
            get { return _activeFeatures.Contains(Feature.RiskOpsEditorFileSelection); }
        }

        // terms

        public bool IsCreateTermsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTerms); }
        }

        public bool IsCopyTermsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyTerms); }
        }

        public bool IsDeleteTermsActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteTerms); }
        }

        public bool IsCreateTermContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTermContracts); }
        }

        public bool IsCopyTermContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTermContracts); }
        }

        public bool IsRemoveTermContractsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTermContracts); }
        }

        public bool IsSkuDetailTermsActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailTerms); }
        }

        public bool IsScheduleTermsActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleTerms); }
        }

        private static bool _isParentFundsActive;
        public bool IsParentFundsActive
        {
            get { return _isParentFundsActive; }
            set { _isParentFundsActive = value; }
        }

        public static string TermsDefaultTab { get; set; }

        public bool TermsEditorFolderSelector
        {
            get { return _activeFeatures.Contains(Feature.TermsEditorFileSelection); }
        }

        public bool IsGroupCreatorCheckedInTerms
        {
            get { return _activeFeatures.Contains(Feature.TermsDefaultGroupCreator); }
        }

        public bool IsGroupCreatorVisibleInTerms
        {
            get { return _activeFeatures.Contains(Feature.TermsShowGroupCreator); }
        }

        public bool IsCreatePromotionActive
        {
            get { return _activeFeatures.Contains(Feature.CreatePromotion); }
        }

        public bool IsCopyPromotionActive
        {
            get { return _activeFeatures.Contains(Feature.CopyPromotion); }
        }

        public bool IsDeletePromotionActive
        {
            get { return _activeFeatures.Contains(Feature.DeletePromotion); }
        }

        public bool IsCreatePromotionFromTemplateActive
        {
            get { return _activeFeatures.Contains(Feature.CreatePromotionFromTemplate); }
        }

        public bool IsPromoPowerEditorActive
        {
            get { return _activeFeatures.Contains(Feature.PromoPowerEditor); }
        }

        public bool IsCreateTemplateActive
        {
            get { return _activeFeatures.Contains(Feature.CreateTemplate); }
        }

        public bool IsCopyTemplateActive
        {
            get { return _activeFeatures.Contains(Feature.CopyTemplate); }
        }

        public bool IsDeleteTemplateActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteTemplate); }
        }

        public bool IsCreateNpdActive
        {
            get { return _activeFeatures.Contains(Feature.CreateNpd); }
        }

        public bool IsCopyNpdActive
        {
            get { return _activeFeatures.Contains(Feature.CopyNpd); }
        }

        public bool IsDeleteNpdActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteNpd); }
        }

        public bool IsCreateFundsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateFunds); }
        }

        public bool IsCopyFundsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyFunds); }
        }

        public bool IsDeleteFundsActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteFunds); }
        }

        public bool IsCreateParentFundsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateParentFunds); }
        }

        public bool IsCopyParentFundsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyParentFunds); }
        }

        public bool IsDeleteParentFundsActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteParentFunds); }
        }

        public bool IsTermContractsActive
        {
            get { return _activeFeatures.Contains(Feature.TermContracts); }
        }

        public bool IsSkuDetailFundsActive
        {
            get { return _activeFeatures.Contains(Feature.SkuDetailFunds); }
        }

        public bool IsScheduleFundsActive
        {
            get { return _activeFeatures.Contains(Feature.ScheduleFunds); }
        }

        public static string FundsDefaultTab { get; set; }

        public bool IsFundsTransferActive
        {
            get { return _activeFeatures.Contains(Feature.FundsTransfer); }
        }

        public bool IsConditionScheduleActive
        {
            get { return _activeFeatures.Contains(Feature.CondtionSchedule); }
        }

        public bool IsScenarioScheduleActive
        {
            get { return _activeFeatures.Contains(Feature.ScenarioSchedule); }
        }

        public bool IsCreateScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.CreateScenario); }
        }

        public bool IsCopyScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.CopyScenario); }
        }

        public bool IsDeleteScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteScenario); }
        }

        public bool IsCloseScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.CloseScenario); }
        }

        public bool IsUpdateBudgetScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.UpdateBudgetScenario); }
        }

        public bool IsEditLastClosedScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.EditLastClosedScenario); }
        }

        public bool IsExportScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.ExportScenario); }
        }

        public bool IsPromotionScenarioActive
        {
            get { return _activeFeatures.Contains(Feature.IsPromotionScenarioActive); }
        }

        public bool IsCreateConditionsActive
        {
            get { return _activeFeatures.Contains(Feature.CreateConditions); }
        }

        public bool IsCopyConditionsActive
        {
            get { return _activeFeatures.Contains(Feature.CopyConditions); }
        }

        public bool IsDeleteConditionsActive
        {
            get { return _activeFeatures.Contains(Feature.DeleteConditions); }
        }

        public bool IsCreatePricingActive
        {
            get { return _activeFeatures.Contains(Feature.CreatePricing); }
        }

        //public bool IsPopupWindowsActive
        //{
        //    get { return _activeFeatures.Contains(Feature.PopupWindows); }
        //}

        public static bool IsFilterClosedByDefault
        {
            get { return _activeFeatures.Contains(Feature.FiltersClosed); }
        }

        public bool IsCommentsActive
        {
            get
            {
                return _activeFeatures.Contains(Feature.FeedbackActive); 
            }
        }

        public bool IsJobsActive
        {
            get { return _activeFeatures.Contains(Feature.Jobs); }
        }

        /* Used to turn off have the sku as the parent to the customers tree in Planning as the DB is too lazy to save it */
        public bool IsPlanningSkuParentActive
        {
            get { return false; }
        }

        public bool IsSearchKeystroke
        {
            get
            {

                try
                {
                    return _activeFeatures.Contains(Feature.FilterOnKeystroke);
                }
                catch
                {
                    return true;
                }

            }
        }

        public bool IsPromotionProductsRefDataEnabled
        {
            get { return _activeFeatures.Contains(Feature.EnablePromotionProductsReferenceData); }
        }

        public bool IsTemplateRestrainstEnabled
        {
            get { return _activeFeatures.Contains(Feature.EnableTemplateConstraints); }
        }

        public bool IsProductSelectedByDefault
        {
            get { return _activeFeatures.Contains(Feature.ListingsProductSelectedByDefault); }
        }

        public bool IsPromoCanvasReportTabVisible
        {
            get { return _configuration.ContainsKey("IsPromoCanvasReportTabVisible"); }
        }

        public bool IsPromoReviewDetailTabEnabled
        {
            get { return _configuration.ContainsKey("IsPromoDetailTabVisible"); }
        }

        public bool IsPromoDocumentTabVisible
        {
            get { return _configuration.ContainsKey("IsPromoDocumentTabVisible"); }
        }

        public string DefaultPromoCanvasReportId
        {
            get
            {
                string a;

                _configuration.TryGetValue("DefaultPromo_CanvasReportId", out a);

                return a;
            }
        }

        public bool IsPromoReportTabVisible
        {
            get { return _configuration.ContainsKey("IsPromoReportTabVisible"); }
        }

        public bool IsRetainFilterSelectionEnabled
        {
            get { return _activeFeatures.Contains(Feature.RetainFilters); }
        }

        public bool IsNpdForecastEnabled
        {
            get { return true || _activeFeatures.Contains(Feature.NPD_ShowForecast); }
        }

        public bool IsDeleteDelistingsActive
        {
            get
            {
                return _activeFeatures.Contains(Feature.DeleteDelistedProducts);
            }
        }

        public bool CanLoginWithTwitter
        {
            get { return _activeFeatures.Contains(Feature.CanSignInWithTwitter); }
        }

        #region Demand Flags

        public bool IsNewFcVisible
        {
            get { return _activeFeatures.Contains(Feature.NewFc); }
        }

        public bool IsCopyFcVisible
        {
            get { return _activeFeatures.Contains(Feature.CopyFc); }
        }

        public bool IsDeleteFcVisible
        {
            get { return _activeFeatures.Contains(Feature.DeleteFc); }
        }

        public bool IsBulkFcVisible
        {
            get { return _activeFeatures.Contains(Feature.BulkFc); }
        }

        #endregion

        #region Settings

        public bool IsSettingsAppDetailsVisible
        {
            get { return _activeFeatures.Contains(Feature.IsSettingsAppDetailsVisible); }
        }

        #endregion

        public string FundingAppTypeId { get; private set; }

        internal static ClientConfiguration FromXml(XElement clientConfigXml)
        {
            var activeFeatures = GetActiveFeatures(clientConfigXml);
            //var startScreen = GetStartScreen(clientConfigXml);
            // var robScreens = GetROBScreens(clientConfigXml);
            var screens = GetScreens(clientConfigXml);

            var xElement = clientConfigXml.Element("SysConfig");
            if (xElement != null)
            {
                User.CurrentUser.DisplayName = xElement.Element("DisplayName").MaybeValue();
                User.CurrentUser.LanguageCode = xElement.Element("LanguageCode").MaybeValue();
                User.CurrentUser.SalesOrganisationID = Convert.ToInt32(xElement.Element("DefaultSalesOrg").MaybeValue());
            }

            Dictionary<string, string> analytics = GetURLs(clientConfigXml, "AnalyticsURL");
            Dictionary<string, string> configuration = GetConfiguation(clientConfigXml, "Configuration");
            return new ClientConfiguration(activeFeatures, null, screens, null, analytics, configuration);
        }

        public static string LanguageCode { get; set; }

        public static string DisplayName { get; set; }

        private static Dictionary<string, string> GetURLs(XElement clientConfigXml, string p)
        {
            var res = clientConfigXml.Elements("SysConfig").Elements("Config").Elements("ConfigItem");

            var q = (from element in res
                     where element.Element("Section").MaybeValue() == "Analytics"
                     select new
                     {
                         Name = element.Element("Key").MaybeValue(),
                         Value = element.Element("Value").MaybeValue()
                     }).ToDictionary(o => o.Name, o => o.Value);

            return q;
        }

        private static Dictionary<string, string> GetConfiguation(XElement clientConfigXml, string p)
        {
            var res = clientConfigXml.Elements("SysConfig").Elements("Config").Elements("ConfigItem");

            var q = (from element in res
                     where element.Element("Section").MaybeValue() == p
                     select new
                     {
                         Name = element.Element("Key").MaybeValue(),
                         Value = element.Element("Value").MaybeValue()
                     }).ToDictionary(o => o.Name, o => o.Value);

            // todo: ### CONSTRAINTS TO REFACTOR
            foreach (var xTab in clientConfigXml.Elements("SysConfig").Elements("Screens").Elements("Screen").Elements("Tabs").Elements("Tab"))
            {
                var xKey = xTab.Element("Key");
                if (xKey != null)
                    switch (xKey.Value)
                    {
                        case "TEMPLATE":
                            if (!q.ContainsKey("IsTemplatingActive"))
                                q.Add("IsTemplatingActive", "1"); break;
                    }
            }
            // /### CONSTRAINTS TO REFACTOR

            return q;
        }

        private static IEnumerable<Feature> GetActiveFeatures(XElement clientConfigXml)
        {

            var res = clientConfigXml.Elements("SysConfig").Elements("Config").Elements("ConfigItem");

            var q = (
                    from element in res
                    where element.Element("Section").MaybeValue() == "ActiveFeatures"
                        && element.Element("Value").MaybeValue() == "1"
                    select element.Element("Key").MaybeValue()
                    ).ToList();

            // todo: ### CONSTRAINTS TO REFACTOR
            //foreach (var xTab in clientConfigXml.Elements("SysConfig").Elements("Screens").Elements("Screen").Elements("Tabs").Elements("Tab"))

            foreach (var xScreen in clientConfigXml.Elements("SysConfig").Elements("Screens").Elements("Screen"))
            {
                XElement xNewButton;
                XElement xCopyButton;
                XElement xDeleteButton;
                XElement xIsGroupCreatorChecked;
                XElement FileSelectorVisibile;
                XElement ShowGroupCreator;

                var xKey = xScreen.Element("Key");
                if (xKey != null)
                    switch (xKey.Value.Replace("ROB_",""))
                    {
                        case "DIAGNOSTICS":
                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                switch (tab.Element("Key").Value)
                                {
                                    case "XML":
                                        q.Add(Feature.DiagnosticsTab.Name); break;
                                    case "ACCOUNTPLANQUEUES":
                                        q.Add(Feature.DiagnosticsAccountPlanQueues.Name); break;
                                }
                            }
                            break;


                        case "PROMOTION":
                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "PROMOTION":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreatePromotion.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyPromotion.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeletePromotion.Name);

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabPromotion.Name);

                                        break;
                                    case "CHART":
                                        q.Add(Feature.PromotionsCharting.Name); break;
                                    case "TEMPLATE":
                                        var xCreateButton = tab.Element("CreatePromotionFromTemplate");
                                        if (xCreateButton.MaybeValue() == "1") q.Add(Feature.CreatePromotionFromTemplate.Name);
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateTemplate.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyTemplate.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteTemplate.Name);

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabTemplate.Name);

                                        break;
                                }
                            }
                            break;

                        case "NPD":

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "NPD":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateNpd.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyNpd.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteNpd.Name);
                                        break;
                                }
                            }
                            break;

                        case "FUND":
                            _isParentFundsActive = false;

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "FUND":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateFunds.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyFunds.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteFunds.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            FundsDefaultTab = "RadTabItem";

                                        if(tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabFund.Name);

                                        break;
                                    case "PARENTFUND":

                                        var CreateParentFund = tab.Element("CreateParentFund");

                                        _isParentFundsActive = true;

                                        if (CreateParentFund.MaybeValue() == "1") q.Add(Feature.CreateParentFunds.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyParentFunds.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteParentFunds.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            FundsDefaultTab = "ParentTab";
                                        break;
                                }
                            }
                            break;

                        case "SCENARIO":

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");
                                var xCloseButton = tab.Element("CloseButton");
                                var xExportButton = tab.Element("ExportButton");
                                var xUpdateBudgetsButton = tab.Element("UpdateBudgetsButton");
                                var xUpdateLatestClosedButton = tab.Element("UpdateLatestClosedButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "SCENARIO":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateScenario.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyScenario.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteScenario.Name);
                                        if (xCloseButton.MaybeValue() == "1") q.Add(Feature.CloseScenario.Name);
                                        if (xExportButton.MaybeValue() == "1") q.Add(Feature.ExportScenario.Name);
                                        if (xUpdateBudgetsButton.MaybeValue() == "1") q.Add(Feature.UpdateBudgetScenario.Name);
                                        if (xUpdateLatestClosedButton.MaybeValue() == "1") q.Add(Feature.EditLastClosedScenario.Name);

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabScenario.Name);

                                        break;
                                    case "SCHEDULE":
                                        q.Add(Feature.ScenarioSchedule.Name); break;
                                }
                            }

                            break;

                        case "PRICING":

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "PRICING":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreatePricing.Name);
                                        break;
                                }
                            }
                            break;

                        case "CONDITION":

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "CONDITION":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateConditions.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyConditions.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteConditions.Name);

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabCondition.Name);

                                        break;
                                    case "SCHEDULE":
                                        q.Add(Feature.CondtionSchedule.Name); break;
                                }
                            }

                            break;

                        case "CLAIM":

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                switch (tab.Element("Key").Value)
                                {
                                    case "CLAIM":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateClaims.Name);

                                        if (tab.Element("UploadFilesTabClaims").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabClaims.Name);

                                        if (tab.Element("UploadFilesTabEvents").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabClaimsEvents.Name);

                                        break;
                                }
                            }

                            break;

                        case "TERMS":

                            xIsGroupCreatorChecked = xScreen.Element("IsGroupCreatorChecked");
                            if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.TermsDefaultGroupCreator.Name);

                            ShowGroupCreator = xScreen.Element("ShowGroupCreator");
                            if (ShowGroupCreator.MaybeValue() == "1") q.Add(Feature.TermsShowGroupCreator.Name);

                            FileSelectorVisibile = xScreen.MaybeElement("FileSelector");
                            if (FileSelectorVisibile.MaybeValue() == "1") q.Add(Feature.TermsEditorFileSelection.Name);

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "TERMS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateTerms.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyTerms.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteTerms.Name);
                                        if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.TermsDefaultGroupCreator.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TermsDefaultTab = "RadTabItem";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabTerm.Name);

                                        break;

                                    case "CONTRACTS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateTermContracts.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyTermContracts.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.RemoveTermContracts.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TermsDefaultTab = "ContractsTab";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabTermContracts.Name);

                                        q.Add(Feature.TermContracts.Name); break;

                                    case "SKU_LIST":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TermsDefaultTab = "SkuDetailTab";

                                        q.Add(Feature.SkuDetailTerms.Name); break;

                                    case "SCHEDULE":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TermsDefaultTab = "ScheduleTab";

                                        q.Add(Feature.ScheduleTerms.Name); break;
                                }
                            }

                            break;

                        case "RISK_OPS":
                            xIsGroupCreatorChecked = xScreen.Element("IsGroupCreatorChecked");
                            if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.RiskOpsDefaultGroupCreator.Name);

                            ShowGroupCreator = xScreen.Element("ShowGroupCreator");
                            if (ShowGroupCreator.MaybeValue() == "1") q.Add(Feature.RiskOpsShowGroupCreator.Name);

                            FileSelectorVisibile = xScreen.MaybeElement("FileSelector");
                            if (FileSelectorVisibile.MaybeValue() == "1") q.Add(Feature.RiskOpsEditorFileSelection.Name);

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "RISK_OPS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateRiskOps.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyRiskOps.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteRiskOps.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            RiskOpsDefaultTab = "RadTabItem";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabRiskOps.Name);

                                        break;

                                    case "CONTRACTS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateRiskOpsContracts.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyRiskOpsContracts.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.RemoveRiskOpsContracts.Name);


                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            RiskOpsDefaultTab = "ContractsTab";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabRiskOpsContracts.Name);

                                        q.Add(Feature.RiskOpsContracts.Name); break;

                                    case "SKU_LIST":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            RiskOpsDefaultTab = "SkuDetailTab";

                                        q.Add(Feature.SkuDetailRiskOps.Name); break;

                                    case "SCHEDULE":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            RiskOpsDefaultTab = "ScheduleTab";

                                        q.Add(Feature.ScheduleRiskOps.Name); break;
                                }
                            }

                            break;

                        case "MARKETING":

                            xIsGroupCreatorChecked = xScreen.Element("IsGroupCreatorChecked");
                            if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.MarketingDefaultGroupCreator.Name);

                            ShowGroupCreator = xScreen.Element("ShowGroupCreator");
                            if (ShowGroupCreator.MaybeValue() == "1") q.Add(Feature.MarketingShowGroupCreator.Name);

                            FileSelectorVisibile = xScreen.MaybeElement("FileSelector");
                            if (FileSelectorVisibile.MaybeValue() == "1") q.Add(Feature.MarketingEditorFileSelection.Name);

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "MARKETING":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateMarketing.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyMarketing.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteMarketing.Name);
                                        if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.MarketingDefaultGroupCreator.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            MarketingDefaultTab = "RadTabItem";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabMarketing.Name);

                                        break;

                                    case "CONTRACTS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateMarketingContracts.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyMarketingContracts.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.RemoveMarketingContracts.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            MarketingDefaultTab = "ContractsTab";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabMarketingContracts.Name);

                                        q.Add(Feature.MarketingContracts.Name); break;

                                    case "SKU_LIST":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            MarketingDefaultTab = "SkuDetailTab";

                                        q.Add(Feature.SkuDetailMarketing.Name); break;

                                    case "SCHEDULE":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            MarketingDefaultTab = "ScheduleTab";

                                        q.Add(Feature.ScheduleMarketing.Name); break;
                                }
                            }

                            break;

                        case "MANAGEMENTADJUST":

                            xIsGroupCreatorChecked = xScreen.Element("IsGroupCreatorChecked");
                            if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.ManagementAdjustDefaultGroupCreator.Name);

                            ShowGroupCreator = xScreen.Element("ShowGroupCreator");
                            if (ShowGroupCreator.MaybeValue() == "1") q.Add(Feature.ManagementAdjustShowGroupCreator.Name);

                            FileSelectorVisibile = xScreen.MaybeElement("FileSelector");
                            if (FileSelectorVisibile.MaybeValue() == "1") q.Add(Feature.ManagementAdjustEditorFileSelection.Name);

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "MANAGEMENTADJUST":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateManagementAdjust.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyManagementAdjust.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteManagementAdjust.Name);
                                        if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.ManagementAdjustDefaultGroupCreator.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            ManagementAdjustDefaultTab = "RadTabItem";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabManagementAdjust.Name);

                                        break;

                                    case "CONTRACTS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateManagementAdjustContracts.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyManagementAdjustContracts.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.RemoveManagementAdjustContracts.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            ManagementAdjustDefaultTab = "ContractsTab";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabManagementAdjustContracts.Name);

                                        q.Add(Feature.ManagementAdjustContracts.Name); break;

                                    case "SKU_LIST":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            ManagementAdjustDefaultTab = "SkuDetailTab";

                                        q.Add(Feature.SkuDetailManagementAdjust.Name); break;

                                    case "SCHEDULE":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            ManagementAdjustDefaultTab = "ScheduleTab";

                                        q.Add(Feature.ScheduleManagementAdjust.Name); break;
                                }
                            }

                            break;

                        case "TARGET":

                            xIsGroupCreatorChecked = xScreen.Element("IsGroupCreatorChecked");
                            if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.TargetDefaultGroupCreator.Name);

                            ShowGroupCreator = xScreen.Element("ShowGroupCreator");
                            if (ShowGroupCreator.MaybeValue() == "1") q.Add(Feature.TargetShowGroupCreator.Name);

                            FileSelectorVisibile = xScreen.MaybeElement("FileSelector");
                            if (FileSelectorVisibile.MaybeValue() == "1") q.Add(Feature.TargetEditorFileSelection.Name);

                            foreach (var tab in xScreen.Elements("Tabs").Elements("Tab"))
                            {
                                xNewButton = tab.Element("NewButton");
                                xCopyButton = tab.Element("CopyButton");
                                xDeleteButton = tab.Element("DeleteButton");

                                switch (tab.Element("Key").Value)
                                {
                                    case "TARGET":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateTarget.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyTarget.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.DeleteTarget.Name);
                                        if (xIsGroupCreatorChecked.MaybeValue() == "1") q.Add(Feature.TargetDefaultGroupCreator.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TargetDefaultTab = "RadTabItem";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabTarget.Name);

                                        break;

                                    case "CONTRACTS":
                                        if (xNewButton.MaybeValue() == "1") q.Add(Feature.CreateTargetContracts.Name);
                                        if (xCopyButton.MaybeValue() == "1") q.Add(Feature.CopyTargetContracts.Name);
                                        if (xDeleteButton.MaybeValue() == "1") q.Add(Feature.RemoveTargetContracts.Name);

                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TargetDefaultTab = "ContractsTab";

                                        if (tab.Element("UploadFilesTab").MaybeValue() == "1")
                                            q.Add(Feature.UploadFilesTabTargetContracts.Name);

                                        q.Add(Feature.TargetContracts.Name); break;

                                    case "SKU_LIST":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TargetDefaultTab = "SkuDetailTab";

                                        q.Add(Feature.SkuDetailTarget.Name); break;

                                    case "SCHEDULE":
                                        if (tab.Element("IsDefault").MaybeValue() == "1")
                                            TargetDefaultTab = "ScheduleTab";

                                        q.Add(Feature.ScheduleTarget.Name); break;
                                }
                            }

                            break;                                                  
                            
                    }
            }
            // /### CONSTRAINTS TO REFACTOR

            IEnumerable<Feature> activeFeatures = q.Where(Feature.IsDefined).Select(Feature.Parse);
            return activeFeatures;
        }


        private static IEnumerable<ROBScreen> GetROBScreens(XElement clientConfigXml)
        {

            var res = clientConfigXml.Elements("SysConfig").Elements("Config").Elements("ConfigItem");

            var q = (from element in res
                     where element.Element("Section").MaybeValue() == "ActiveFeatures"
                         && (element.Element("Key").MaybeValue() ?? string.Empty).StartsWith("ROB:")
                     select
                         new ROBScreen
                         {
                             AppTypeID = element.Element("Value").MaybeValue(),
                             Title = element.Element("Key").MaybeValue().Substring(4),
                             Key = element.Element("Key").MaybeValue()
                         }).ToList();

            q.ToList().ForEach(x => x.Create =
                (from element in res
                 where element.Element("Section").MaybeValue() == "ActiveFeatures"
                     && (element.Element("Key").MaybeValue() ?? string.Empty).StartsWith("ROBCreate:" + x.Title)
                 select element.Element("Value").MaybeValue()).FirstOrDefault() == "1" ? true : false);

            q.ToList().ForEach(x => x.Recipient =
                (from element in res
                 where element.Element("Section").MaybeValue() == "ActiveFeatures"
                     && (element.Element("Key").MaybeValue() ?? string.Empty).StartsWith("ROBRecipient:" + x.Title)
                 select element.Element("Value").MaybeValue()).FirstOrDefault() == "1" ? true : false);

            return q;
        }

        private static List<Screen> GetScreens(XElement clientConfigXml)
        {
            List<Screen> screens = new List<Screen>();

            //MENU GROUPS!
            //screens.Add(new Screen { IsDefault = false, LabelKey = "Reports", Label = "Reports", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "LineChart", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            //screens.Add(new Screen { IsDefault = false, LabelKey = "Demand Planning", Label = "Demand Planning", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "BarChartOutline", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            //screens.Add(new Screen { IsDefault = false, LabelKey = "Sales Planning", Label = "Sales Planning", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "Globe", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            //screens.Add(new Screen { IsDefault = false, LabelKey = "Finance", Label = "Finance", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "BriefCase", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            //screens.Add(new Screen { IsDefault = false, LabelKey = "S & OP", Label = "S & OP", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "Users", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            //screens.Add(new Screen { IsDefault = false, LabelKey = "Admin", Label = "Admin", Key = ScreenKeys.PARENTSCREEN.ToString(), Icon = "Unlock", SortOrder = 0, Uri = ScreenNavigator.GetUri(ScreenKeys.PARENTSCREEN.ToString()) });
            
            GetScreenFromXml(clientConfigXml, screens);

            return screens;
        }

        private static void GetScreenFromXml(XElement clientConfigXml, List<Screen> screens)
        {
            //Store all the screens here, parse then into their parent screens and add any remainders to the final screen set
            var tempScreens = new List<Screen>();

            foreach (var xScreen in clientConfigXml.Elements("SysConfig").Elements("Screens").Elements("Screen"))
            {
                var screen = CreateScreenDetail(xScreen);

                if (screen.Key != "DIAGNOSTICS")
                    tempScreens.Add(screen);
            }

            foreach (var parentXml in clientConfigXml.Elements("SysConfig").Elements("ParentScreens").Elements("ParentScreen"))
            {
                var parentScreen = CreateParentScreen(parentXml);

                var children = tempScreens.Where(s => s.ParentScreen_Key == parentScreen.Key).ToList();

                if (children.Any())
                {
                    parentScreen.Children.AddRange(children);
                    tempScreens.Remove(children);

                    tempScreens.Add(parentScreen);
                }
            }

            screens.AddRange(tempScreens);
        }

        private static Screen CreateScreenDetail(XElement xScreen)
        {
            Screen screen = new Screen
            {
                // ReSharper disable PossibleNullReferenceException
                Key = xScreen.Element("Key") != null ? xScreen.Element("Key").Value : null,
                ParentScreen_Key = xScreen.Element("ParentScreen_Key").MaybeValue(),
                Icon = xScreen.Element("Icon").MaybeValue() ?? "ExclamationCircle", //Exclamation to indicate no symbol has been provided
                LabelKey = xScreen.Element("LabelKey") != null ? xScreen.Element("LabelKey").Value : null,
                UseKeyToLoadData = xScreen.Element("UseKeyToLoadData") != null && xScreen.Element("UseKeyToLoadData").Value == "1",
                IsDefault = xScreen.Element("IsDefault") != null && xScreen.Element("IsDefault").Value == "1",
                RobAppType = xScreen.Element("ROBAppType").MaybeValue(),
                ShowAsROBGroup = xScreen.Element("ShowAsROBGroup") != null && xScreen.Element("ShowAsROBGroup").Value == "1",
                RobAppRecipient = xScreen.Element("ROBRecipient") != null && xScreen.Element("ROBRecipient").Value == "1",
                IsFilteredByListings = xScreen.Element("IsFilteredByListings") != null && xScreen.Element("IsFilteredByListings").Value == "1",
                SortOrder = xScreen.GetValue<int>("SortOrder")
                // Tabs = GetTabs(xScreen.Element("Tabs"))                 
            };

            screen.Uri = !string.IsNullOrEmpty(screen.RobAppType) ? ScreenNavigator.GetUri(screen.Key) + screen.RobAppType : ScreenNavigator.GetUri(screen.Key);

            return screen;
        }

        private static Screen CreateParentScreen(XElement xml)
        {
            Screen screen = new Screen
            {
                Key = xml.Attribute("Key").MaybeValue(),
                Icon = xml.Attribute("Icon").MaybeValue() ?? "ExclamationCircle", //Exclamation to indicate no symbol has been provided
                LabelKey = xml.Attribute("LabelKey").MaybeValue(),
                SortOrder = xml.Attribute("SortOrder").MaybeValue().AsNumericInt()
            };

            return screen;
        }

        private static string GetStartScreen(XElement clientConfigXml)
        {
            var res = clientConfigXml.Elements("SysConfig").Elements("Config").Elements("ConfigItem");

            var q = from element in res
                    where element.Element("Section").MaybeValue() == "StartScreen"
                    select element.Element("Value").MaybeValue();

            return q.FirstOrDefault();
        }

        public bool ScheduleLink
        {
            get
            {
                return _configuration.ContainsKey("ScheduleView_ScheduleLink");

            }
        }

        public bool LockChildren
        {
            get
            {
                string b;
                _configuration.TryGetValue("ROBCreator_LockChildren", out b);

                return b == "1";
            }
        }

        public bool StopPromotionDatePeriodsDropdownReset
        {
            get
            {
                var b = "0";
                _configuration.TryGetValue("StopPromotionDatePeriodsDropdownReset", out b);

                return b == "1";

            }
        }

        public bool CanEnterNegativeClaimsAppotionment
        {
            get
            {
                return _configuration.ContainsKey("Claims_CanEnterNegativeClaimsApportionment");

            }
        }

        public bool ForcePromoProductsExpand
        {
            get
            {
                var a = "0";

                _configuration.TryGetValue("ForcePromoProductsTreeExpand", out a);
                if (a != null)
                    return a == "1";
                else
                    return false;
            }
        }

        public bool IsTemplatingActive
        {
            get
            {
                var a = "0";

                _configuration.TryGetValue("IsTemplatingActive", out a);
                if (a != null)
                    return a == "1";
                else
                    return false;
            }
        }

        public bool IsPromoShowSchedule
        {
            get
            {
                var a = "0";

                _configuration.TryGetValue("IsPromoShowSchedule", out a);
                if (a != null)
                    return a == "1";
                else
                    return false;
                //return true;
            }
        }


        public bool UseUpliftForecasting
        {

            get
            {
                var a = "0";

                _configuration.TryGetValue("UseUpliftForecasting", out a);
                if (a != null)
                    return a == "1";
                else
                    return false;
                //return true;
            }

        }

        public string UpliftForecastingURL
        {
            get
            {
                var a = "";

                _configuration.TryGetValue("UpliftForecastingURL", out a);

                return a;
            }
        }



        public string DefaultDashboardCanvasId
        {
            get
            {
                var a = "";

                _configuration.TryGetValue("DefaultDashboard_CanvasId", out a);

                return a;
            }
        }


        #region Demand WebService

        public string DemandCalibrateModelUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandCalibrateModelUrl", out a);

                return a;
            }
        }

        public string DemandCalculateForecastUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandCalculateForecastUrl", out a);

                return a;
            }
        }

        public string DemandCalculateSeasonalProfileUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandCalculateSeasonalProfileUrl", out a);

                return a;
            }
        }

        public string DemandSmoothSeasonalProfileUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandSmoothSeasonalProfileUrl", out a);

                return a;
            }
        }

        public string DemandNormaliseSeasonalProfileUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandNormaliseSeasonalProfileUrl", out a);

                return a;
            }
        }

        public string DemandNewModelTypesUrl
        {
            get
            {
                string a;

                _configuration.TryGetValue("DemandNewModelTypesUrl", out a);

                return a;
            }
        }


        #endregion
        /// <summary>
        /// Always returns true unless the config is explicitly false
        /// </summary>
        public bool IsVerBoseLogging
        {
            get
            {
                // return true;

                if (_configuration == null) return true;

                var a = "";

                _configuration.TryGetValue("IsVerboseLogging", out a);
                if (a != null)
                {
                    return a != "0";
                }
                else
                {
                    return true;
                }

            }
        }

        //public bool IsPasswordEncrypted
        //{
        //    get
        //    {
        //        var a = "0";

        //        _configuration.TryGetValue("PasswordEncryption", out a);
        //        if (a != null)
        //            return a == "1";
                
        //            return false;
        //    }
        //}

        public int? RetentionOfDelistedProducts
        {
            get
            {
                int a;
                string configString;
                _configuration.TryGetValue("RetentionOfDelistedProducts", out configString);

                return int.TryParse(configString, out a) ? (int?)a : null;
            }
        }

        public bool ClaimsCanAutoMatch
        {
            get
            {
                return _configuration.ContainsKey("Claims_CanAutoMatch");
            }
        }

        public string AnalyticsRole
        {
            get
            {
                var a = "";
                _configuration.TryGetValue("AnalyticsRole", out a);
                return a ?? (a = "");
            }
        }

        public object AnalyticsLogin
        {
            get
            {
                var a = "";
                _configuration.TryGetValue("AnalyticsLogin", out a);
                return a ?? (a = "");
            }
        }

        public bool ShowMenuIcon
        {
            get
            {
                string  a;

                _configuration.TryGetValue("ShowMenuIcon", out a);
                if (a != null)
                    return a == "0";

                return true;
            }
        }

        public bool UseAzureStorage
        {
            get
            {
                string a;

                _configuration.TryGetValue("UseAzureStorage", out a);
                if (a != null)
                    return a == "0";

                return true;
            }
        }

        private static StorageData _storageDetails;

        public StorageData StorageDetails
        {
            get
            {
                if (UseAzureStorage)
                {
                    if (_storageDetails == null)
                    {
                        _storageDetails = new StorageData();
                    }
                    return _storageDetails;
                }

                return null;
            }            
        }


    }

    public class ROBScreen
    {
        public string AppTypeID { get; set; }
        public string Title { get; set; }
        public bool Create { get; set; }
        public string Key { get; set; }
        public bool Recipient { get; set; }
    }
}