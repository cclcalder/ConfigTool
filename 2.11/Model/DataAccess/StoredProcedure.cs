namespace Model
{
    public static class StoredProcedure
    {
        private const string _aSP = "app.Procast_SP_";
        private const string _pP = _aSP + "PROMO_";

        public const string GetPlanningHierarchies = _aSP + "PLANNING_GetFilterHierarchy";
        public const string CommitPlanningData = "CommitPlanningData";
        public const string UserLogin = _aSP + "LOGIN_User_Login";
        public const string LoginWithSession = _aSP + "LOGIN_User_LoginWithSession";
        public const string SaveUser = _aSP + "UserPref_Save";
        public const string ActiveDirectoryLogin = _aSP + "LOGIN_AD_Login";
        public const string GetClientConfiguration = _aSP + "LOGIN_GetSysConfig";
        public const string AppLoginCheckDependencies = _aSP + "LOGIN_CheckDependencies";        
        public const string GetSalesOrgs = _aSP + "SHARED_GetSalesOrg";
		public const string GetInsightReports = _aSP + "GetInsightReports";
        public const string GetPlanningProducts = _aSP + "PLANNING_GetFilterProducts";
        public const string GetPlanningMeasures = _aSP + "PLANNING_GetFilterMeasures";
        public const string GetPlanningDateIntervals = _aSP + "PLANNING_GetFilterDateIntervals";
        public const string GetPlanningData = _aSP + "PLANNING_GetPlanningData";
        public const string GetPlanningDataCustomers = _aSP + "PLANNING_GetPlanningData_Customers";
        public const string SaveUserPrefsPlanning = _aSP + "SHARED_SaveDefaults";
        public const string GetPlanningTimeRanges = _aSP + "PLANNING_GetFilterTimeRanges";
        public const string SavePlanningData = _aSP + "PLANNING_SavePlanningData";
        public const string SavePlanningCustData = _aSP + "PLANNING_SavePlanningData_Customers";
        public const string GetPlanningScenarios = _aSP + "PLANNING_GetFilterScenarios";
		public const string SavePricingData = _aSP + "SavePricingData";
		public const string SavePricingDataProduct = _aSP + "SavePricingData_Product";
		public const string ValidatePricingData = _aSP + "Validate_Pricing";
		public const string ValidatePricingDataProduct = _aSP + "Validate_Pricing_Product"; 
		public const string GetPhasesDaily = _aSP + "GetPhasesDaily";
		public const string GetPhasesWeekly = _aSP + "GetPhasesWeekly";
		public const string GetPhasingDetail = _aSP + "GetPhasingDetail";
		public const string SavePhasing = _aSP + "SavePhasing";
		public const string DeletePhasing = _aSP + "DeletePhasing";
		public const string ApplyPhasing = _aSP + "SavePromoPhasing";
		public const string RemovePromoPhasing = _aSP + "RemovePromoPhasing";
		public const string GetPromoPhasing = _aSP + "GetPromoPhasing";
		public const string ValidatePromoPhasing = _aSP + "Validate_PromoPhasing";
		public const string GetItemCustomers = _aSP + "GetItemsCustomers";
		public const string GetItems = _aSP + "GetItems";
		public const string GetItemsProduct = _aSP + "GetItems_Product";
		public const string GetItemDetails = _aSP + "GetItemDetails";
		public const string GetItemDetailsProduct = _aSP + "GetItemDetails_Product";
		public const string GetItemScenarios = _aSP + "GetItemScenarios";
		public const string GetItemProducts = _aSP + "GetItemProducts";
	    public const string ForgottenPasswordSavePassword = _aSP + "LOGIN_ForgottenPassword_SavePassword";
        public const string GetPasswordPolicy = _aSP + "LOGIN_GetPasswordPolicy";
        public const string ForgottenPasswordGetDetails = _aSP + "LOGIN_ForgottenPassword_GetDetails";
        public const string GetSSOSettings = _aSP + "PRELOGIN_GetSSOSettings";
	    public const string ChangePassword = _aSP + "Settings_ChangePassword";

        public static class AdminPatternList
        {
            //Side menu
            public const string GetMenuItemList = _aSP + "ADMIN_GetMenuItems";

            //Screen 1
            public const string GetListPattern1 = _aSP + "ADMIN_GetList";
            public const string GetGrid = _aSP + "ADMIN_GetGrid";
            public const string SaveGrid = _aSP + "ADMIN_SaveGrid";
            public const string DeleteGrid = _aSP + "ADMIN_DeleteGrid";
            public const string CopyGrid = _aSP + "ADMIN_CopyGrid";

            //Screen 2
            public const string UserList = _aSP + "ADMIN_GetList";
            public const string ApplySelection = _aSP + "ADMIN_ApplySelectionGetList";
            public const string SaveListMatches = _aSP + "ADMIN_SaveListMatches";
        }

        public static class Analytics
        {
            public const string SaveUserReportLayout = _aSP + "ANALYTICS_SaveUserReportLayout";
            public const string GetReports = _aSP + "ANALYTICS_GetReports";
            //public const string GetUserCubesList = _aSP + "ANALYTICS_GetUserCubeList";
            public const string GetUserList = _aSP + "ANALYTICS_GetUserList";
            //public const string GetUserReportsList = _aSP + "ANALYTICS_GetUserReportsList";
            public const string GetUserReport = _aSP + "ANALYTICS_GetReport";
            public const string DeleteUserReport = _aSP + "ANALYTICS_DeleteUserReport";
        }

        public static class Claims
        {
            public const string GetClaimFilterValues = _aSP + "CLAIM_GetFilterClaimValues";
            public const string ReturnEvents = _aSP + "CLAIM_GetEvents";
            public const string ReturnMatches = _aSP + "CLAIM_ApplyFilters_ReturnMatches";
            public const string GetMatchVisibilityValues = _aSP + "CLAIM_GetMatchVisibilityValues";
            public const string GetClaim = _aSP + "CLAIM_ClaimEditor_GetClaim";
            public const string GetEventList = _aSP + "CLAIM_ClaimEditor_GetEvents";
            public const string GetApportionments = _aSP + "CLAIM_ClaimEditor_GetApportionments";
            public const string GetEventProducts = _aSP + "CLAIM_ClaimEditor_GetProducts";
            public const string GetEvent = _aSP + "CLAIM_EventsScreen_GetEvent";
            public const string SetDefaultFilters = _aSP + "SHARED_SaveDefaults";
            public const string AutomaticMatch = _aSP + "CLAIM_AutomaticMatch";
            public const string SaveMatches = _aSP + "CLAIM_SaveMatches";
            public const string ApprovePayments = _aSP + "CLAIM_ApprovePayments";
            public const string SettleEvents = _aSP + "CLAIM_SettleEvents";
            public const string GetUserDefaults = _aSP + "CLAIM_GetUserDefaults";
            public const string GetAllowedEventStatuses = _aSP + "CLAIM_GetAllowedEventStatuses";
            public const string GetClaimComments = _aSP + "CLAIM_ClaimEditor_GetClaimComments";
            public const string GetEventComments = _aSP + "CLAIM_EventsScreen_GetEventComments";
            public const string AddClaimComments = _aSP + "CLAIM_AddClaimComment";
            public const string AddEventComments = _aSP + "CLAIM_AddEventComment";
            public const string GetSettlementReasonCodes = _aSP + "CLAIM_GetSettlementReasonCodes";
            public const string GetCustomersOnClaimEntry = _aSP + "CLAIM_GetFilterCustomersForClaimAdd";
            public const string GetClaims = _aSP + "CLAIM_GetClaims";
            public const string GetManualClaimsEntry = _aSP + "CLAIM_GetManualEntryGrid";
            public const string SaveManualClaimsEntry = _aSP + "CLAIM_SaveManualEntryGrid";
            public const string ClaimEditorValidate = _aSP + "CLAIM_ClaimEditor_ValidateClaim";
            public const string ClaimEditorSave = _aSP + "CLAIM_ClaimEditor_SaveClaim";
            public const string ClaimEventEditorGetClaims = _aSP + "CLAIM_EventEditor_GetClaims";
            public const string ClaimEventEditorGetProducts = _aSP + "CLAIM_EventEditor_GetProducts";
            public const string EventEditorValidateEvent = _aSP + "CLAIM_EventEditor_ValidateEvent";
            public const string EventEditorSaveEvent = _aSP + "CLAIM_EventEditor_SaveEvent";
            public const string GetFilterStatuses = _aSP + "CLAIM_GetFilterStatuses";
        }

        public static class Conditions
        {
            public const string GetFilterStatuses = _aSP + "COND_GetFilterStatuses";
            public const string GetConditionStatuses = _aSP + "COND_GetConditionStatuses";
            public const string GetFilterDates = _aSP + "COND_GetFilterDates";
            public const string GetConditions = _aSP + "COND_GetConditions";
            public const string CopyConditions = _aSP + "COND_CopyCondition";
            public const string DeleteConditions = _aSP + "COND_DeleteCondition";
            public const string SaveUserPreferences = _aSP + "SaveUserPrefs_Conditions";
            public const string GetCustomerLevels = _aSP + "COND_GetCustomerLevels";
            public const string GetProductLevels = _aSP + "COND_GetProductLevels";
            public const string GetCustomerLevelItems = _aSP + "COND_GetCustomerLevelItems";
            public const string GetProductLevelItems = _aSP + "COND_GetProductLevelItems";
            public const string GetCondition = _aSP + "COND_GetCondition";
            public const string GetConditionComments = _aSP + "COND_GetConditionComments";
            public const string GetConditionTypes = _aSP + "COND_GetConditionTypes";
            public const string GetConditionReasons = _aSP + "COND_GetConditionReasons";
            public const string GetConditionSelection = _aSP + "COND_GetConditionSelection";
            public const string GetConditionScenarios = _aSP + "COND_GetConditionScenarios";
            public const string GetConditionApplyToScenarios = _aSP + "COND_GetConditionApplyToScenarios";
            public const string SaveCondition = _aSP + "COND_SaveCondition";
            public const string GetConditionControlIsEnabled = _aSP + "COND_GetEnabledControls";
            public const string ValidateCondition = _aSP + "COND_ValidateCondition";
            public const string AddComment = _aSP + "COND_AddComment";
            public const string GetConditionMissingPrices = _aSP + "COND_MissingPrices";
        }

        public static class Demand
        {
            public const string GetConfig = _aSP + "DEMAND_GetConfig";
            public const string GetCustSku = _aSP + "DEMAND_GetCustSku";
            public const string GetSeasonalsGrid = _aSP + "DEMAND_GetSeasonalProfilesData";
            public const string GetVolumesTreeGrid = _aSP + "DEMAND_GetVolumesTreeGrid";
            public const string GetSeasonals = _aSP + "DEMAND_GetSeasonal";
            public const string GetDataModel = _aSP + "DEMAND_GetData_Model";
            public const string SaveForecast = _aSP + "DEMAND_SaveForecast";
            public const string SaveSettings = _aSP + "DEMAND_SaveSettings";
            public const string SaveSeasonalProfile = _aSP + "DEMAND_SaveSeasonalProfile";
            public const string GetLibrarySeasonals = _aSP + "DEMAND_GetLibrarySeasonals";
	    }

        public static class Seasonals
        {
            public const string GetListingSeasonals = _aSP + "SEASONALS_GetSeasonals";
            public const string SaveListingSeasonals = _aSP + "SEASONALS_SaveSeasonals";
        }

        public static class DemandMgmt
        {
            /*List*/
            public const string GetTrialForecasts = _aSP + "FCMGMT_GetForecasts";

            /*Editor*/
            public const string GetCustSku = _aSP + "FCMGMT_GetCustSku";
            public const string GetFilterOther = _aSP + "FCMGMT_GetDesignGrid";
            public const string GetFcGrid = _aSP + "FCMGMT_GetHistory";
            public const string GetComments = _aSP + "FCMGMT_GetComments";
            public const string AddComments = _aSP + "FCMGMT_AddComment";
            public const string GetUsers = _aSP + "FCMGMT_GetUsers";
            public const string Copy = _aSP + "FCMGMT_CopyForecast";
            public const string Delete = _aSP + "FCMGMT_DeleteForecast";
            public const string Update = _aSP + "FCMGMT_PublishForecast";
            public const string Save = _aSP + "FCMGMT_SaveForecast";

            /*Some other Elliot's crap*/
            public const string GetScenarios = _aSP + "FCMGMT_GetScenarios";
        }

        public static class Dummy
        {
            public const string GetHorizontalGrid = _aSP + "DUMMY_GetHorizontalGrid";
        }

        public static class Info
        {
            public const string AccountPlanBuildQueue = _aSP + "INFO_AccountPlanBuildQueue";
            public const string AsynchronousTaskQueue = _aSP + "INFO_AsynchronousTaskQueue";
        }

        public static class Insights
        {
            //Side menu
            public const string GetMenuItemList = _aSP + "CANVAS_GetMenuItems";

            //Top menu - filters
            public const string GetFilters = _aSP + "CANVAS_GetFilters";
            public const string SaveDefaults = _aSP + "CANVAS_SaveDefaults";

            //Main Grid - Items
            public const string GetGridItemList = _aSP + "CANVAS_GetGridItems";
        }

        public static class Fund
        {

            //List screen
            public const string GetFunds = _aSP + "FUND_GetFundList";
            public const string GetFilterDates = _aSP + "FUND_GetFilterDates";
            public const string GetFilterStatuses = _aSP + "FUND_GetFilterStatuses";
            public const string DropDownStatuses = _aSP + "FUND_GetMassUpdateStatuses";
            public const string GetParentFunds = _aSP + "FUND_GetFundList";
            public const string GetParentFundsList = _aSP + "FUND_GetParentFundsList";
            public const string MultipleStatusParentFundUpdate = _aSP + "FUND_UpdateMultipleFundStatus";            
            public const string SaveUserPrefs = _aSP + "SaveUserPrefs_Funds";
            public const string Remove = _aSP + "FUND_DeleteFund";
            public const string Copy = _aSP + "FUND_CopyFund";
            public const string MultipleStatusUpdate = _aSP + "FUND_UpdateMultipleFundStatus";
            public const string SaveFundTransfer = _aSP + "FUND_TransferWindow_SaveTransfer";
             
             //Editor
            public const string GetFund = _aSP + "FUND_GetFund";
            public const string GetTypes = _aSP + "FUND_GetTypes";
            public const string GetSubTypes = _aSP + "FUND_GetSubTypes";
            public const string GetCustomerLevels = _aSP + "FUND_GetCustLevels";
            public const string GetCustomerLevelItems = _aSP + "FUND_GetCustSelection";
            public const string GetProductLevels = _aSP + "FUND_GetProdLevels";
            public const string GetProductLevelItems = _aSP + "FUND_GetProdSelection";
            public const string GetWorkflowStatuses = _aSP + "FUND_GetWorkflowStatuses";
            public const string GetComments = _aSP + "FUND_GetComments";
	        public const string GetFundTransferWindowDetails = _aSP + "FUND_TransferWindow_GetParentFund";
            public const string GetFundTransferWindowChildFunds = _aSP + "FUND_TransferWindow_GetChildFunds";

            //looks wrong but the end part is passed in from the app for each grid
            public const string GetDynamicGridData= _aSP + "FUND_";

            public const string AddComment = _aSP + "FUND_AddComment"; 
            public const string Save = _aSP + "FUND_SaveFund";
            public const string GetImpacts = _aSP + "FUND_GetImpacts";
            public const string GetFundList = _aSP + "FUND_GetFundList";
        }

        public static class Language
        {
            public const string SqlMessageSave = _aSP + "SQLMessage_Save";
            public const string LoadAll = _aSP + "SQLMessages_LoadAll";
        }

        public static class Login
        {
            public const string UserLanguage = _aSP + "UserLanguage";
            public const string GetLanguageList = _aSP + "GetLanguageList";
        }

		public static class NPD
		{
            public const string NPDSave = _aSP + "NPD_Save";
            public const string GetNPDs = _aSP + "NPD_GetNPDList";
		    public const string GetDefaultFilterDates = _aSP + "NPD_GetFilterDates";
		    public const string GetFilterStatuses = _aSP + "NPD_GetFilterStatuses";
		    public const string GetUsers = _aSP + "NPD_GetNPDUsers";
            public const string GetCustomers = _aSP + "NPD_GetNPDListings";
            public const string GetProductGrid = _aSP + "NPD_GetNPDProductGrid";
            public const string GetCustomerProductGrid = _aSP + "NPD_GetNPDCustomerProductGrid";
		    public const string GetWorkflowStatuses = _aSP + "NPD_GetWorkflowStatuses";
		    public const string GetDesignGrid = _aSP + "NPD_GetDesignGrid";
		    public const string SaveUserPreferences = _aSP + "SaveUserPrefs_NPD";
            public const string Copy = _aSP + "NPD_CopyNPD";
            public const string Remove = _aSP + "NPD_DeleteNPD";
            public const string Replace = _aSP + "NPD_Replace";
            public const string GetFilterReplacementProducts = _aSP + "NPD_GetFilterReplacementProducts";
		    public const string GetFiltersGridPopulateDropdowns = _aSP + "SHARED_GetFiltersGrid_PopulateDropdowns";
		    public const string GetBomSkus = _aSP + "NPD_GetBOMSkus";

            //Forecasting
            public const string GetForecastFilters = _aSP + "NPD_GetForecastFactors";
		}

        public static class Promotion
        {
           

            public const string GetPowerPromotionSKULevelMeasures = _pP + "GetPowerPromotion_SKULevelMeasures";
            public const string GetPowerPromotionBaseVolume = _pP + "GetPowerPromotion_BaseVolume";
            public const string GetPowerPromotionVolume = _pP + "GetPowerPromotionVolume";
            public const string GetPowerPromotionIncrementalVolume = _pP + "GetPowerPromotionIncrementalVolume";
            public const string GetPowerPromotionTotals = _pP + "GetPowerPromotionTotals";
            public const string GetPandLGrid = _pP + "PowerEditor_GetPandLGrid";

            public const string SavePromotion = _pP + "Wizard_SavePromotion";
            public const string UpdateMultiplePromotionStatus = _pP + "UpdateMultiplePromotionStatus";
            public const string RecalculatePandL = _pP + "Wizard_Review_RecalculatePandL";
            public const string GetProductsList = _pP + "ReferenceData_GetProductsList";
            public const string GetProductsReferenceData = _pP + "ReferenceData_GetProductsReferenceData";
            public const string SaveProductsReferenceData = _pP + "ReferenceData_SaveProductsReferenceData";
            public const string SavePromoPage1 = _pP + "PowerEditor_SavePromoPage1";
            public const string SavePromoPage2 = _pP + "PowerEditor_SavePromoPage2";
            public const string GetCustomersSubLevel = _pP + "Wizard_Customers_GetCustomersSubLevel";
            public const string GetAttributes = _pP + "Wizard_Attributes_GetAttributes";
            public const string GetPromotions = _pP + "GetPromotions";
            public const string GetGraph = _pP + "GetPromotion_Graph";
            public const string GetGraphList = _pP + "GetPromotion_GraphList";
            public const string RemovePromotionViewer = _pP + "Wizard_RemovePromotionViewer";
            public const string GetPanLGridFirst = _pP + "Wizard_Review_GetPandLGrid_First";
            public const string GetPanLGridSecond = _pP + "Wizard_Review_GetPandLGrid_Second";
            public const string GetPanLGridThird  = _pP + "Wizard_Review_GetPandLGrid_Third";
            public const string GetPanLGridFourth = _pP + "Wizard_Review_GetPandLGrid_Fourth";
            public const string GetReportGrid = _pP + "Wizard_Review_GetReportGrid";
            public const string GetDocumentGrid = _pP + "Wizard_Review_GetDocumentGrid";
            

            //Transfered from PromotionProcNameGetter
            public const string GetPromotionsFilterCustomers = _pP + "GetFilterCustomers";
            public const string GetAddPromotionsCustomers = _pP + "Wizard_Customers_GetCustomers";
            public const string GetPromotionStatuses = _pP + "GetFilterStatuses";
            public const string GetPromotionScenarios = _pP + "Wizard_Review_GetPromotionScenarios";
            public const string GetPromotionDateTypes = _pP + "Wizard_Dates_GetDateTypes";
            public const string GetPromotionDatePeriods = _pP + "Wizard_Dates_GetDatePeriods";
            public const string GetPromotionDefaultFilterDates = _pP + "GetFilterDates";
            public const string GetAddPromotionProducts = _pP + "Wizard_Products_GetProducts";
            public const string GetAddPromotionProductPrices = _pP + "Wizard_Products_GetProductPrices";
            public const string GetProductVolumes = _pP + "Wizard_Volumes_GetProductVolumes";
            public const string GetProductStealVolumes = _pP + "Wizard_Volumes_GetStealVolumes";
            public const string GetFinancialProductMeasures = _pP + "Wizard_Financials_GetProductMeasures";
            public const string GetFinancialParentProductMeasues = _pP + "Wizard_Financials_GetParentProductMeasures";
            public const string GetFinancialPromoMeasures = _pP + "Wizard_Financials_GetPromoMeasures";
            public const string GetPromotion = _pP + "Wizard_GetPromotion";
            public const string DeletePromotion = _aSP + "DeletePromotion";
            public const string ValidatePromotionVolumesDaily = _aSP + "Validate_PromotionVolumesDaily";
            public const string CopyPromotion = _aSP + "CopyPromotion";
            public const string GetApprovalLevels = _pP + "Wizard_Review_GetApprovalLevels";
            public const string AddPromotionComment = _pP + "Wizard_AddComment";
            public const string GetPromotionWorkflowStatuses = _pP + "Wizard_Review_GetWorkflowStatuses";
            public const string GetPromotionComments = _pP + "Wizard_GetPromotionComments";
            public const string GetAddPromotionCustomersSubLevel = _pP + "Wizard_Customers_GetCustomersSubLevel";
            public const string GetProductDailyVolumes = _aSP + "GetProductVolumes_Daily";
            public const string SaveProductDailyVolumes = _aSP + "SavePromotionVolume_Daily";
            public const string GetPromotionChartData = _pP + "GetPromotion_Graph";
            public const string GetPromotionChartList = _pP + "GetPromotion_GraphList";
            public const string GetProductDisplaytUnits = _pP + "Wizard_Volumes_GetDisplayVolumes";
        }

        public static class ROB
        {
            public const string GetRobs = _aSP + "ROB_GetRobs";
            public const string GetMaterialRobs = _aSP + "ROB_GetMaterialRobs";
            public const string GetContracts = _aSP + "ROB_GetContracts";
            public const string GetFilterDates = _aSP + "ROB_GetFilterDates";
            public const string Remove = _aSP + "ROB_DeleteROB";
            public const string Save = _aSP + "ROB_SaveRob";
            public const string AddComment = _aSP + "ROB_AddComment";
            public const string Copy = _aSP + "ROB_CopyROB";
            public const string DropDownStatuses = _aSP + "ROB_GetMassUpdateStatuses";
            public const string GetRob = _aSP + "ROB_GetROB";
            public const string GetTypes = _aSP + "ROB_GetTypes";
            public const string GetSubTypes = _aSP + "ROB_GetSubTypes";
            public const string GetFilterStatuses = _aSP + "ROB_GetFilterStatuses";
            public const string GetCustomerLevels = _aSP + "ROB_GetCustLevels";
            public const string GetCustomerLevelItems = _aSP + "ROB_GetCustLevelItems";
            public const string GetProductLevels = _aSP + "ROB_GetProdLevels";
            public const string GetProductLevelItems = _aSP + "ROB_GetProdLevelItems";
            public const string GetImpactItems = _aSP + "ROB_GetImpactItems";
            public const string GetWorkflowStatuses = _aSP + "ROB_GetWorkflowStatuses";
            public const string GetScenarios = _aSP + "ROB_GetScenarios";
            public const string GetCommentTypes = _aSP + "ROB_GetCommentTypes";
            public const string GetFilterCommentTypes = _aSP + "ROB_GetFilterCommentTypes";
            public const string DeleteComment = _aSP + "ROB_DeleteComment";
            public const string GetRobRecipients = _aSP + "ROB_GetRecipients";
            public const string GetComments = _aSP + "ROB_GetComments";
            public const string GetRobSkus = _aSP + "ROB_GetSKUs";
            public const string LoadInformationGrid = _aSP + "ROB_LoadInformationGrid";
            public const string UpdateMultipleRobStatus = _aSP + "ROB_UpdateMultipleROBStatus";
        }

	    public static class RobContracts
	    {
	        public const string GetContractDetails = _aSP + "ROB_GetContractDetails";
            public const string GetTermDetails = _aSP + "ROB_GetTermDetails";
            public const string GetContractTabs = _aSP + "ROB_GetContractTabs";
            public const string GetContractTerms = _aSP + "ROB_GetContractTerms";
	        public const string GetContractTermsDetails = "aap.Procast_SP_ROB_GetContractTermDetails";
            public const string SaveContractTerms = _aSP + "ROB_SaveContractTerms";
            public const string CopyContract = _aSP + "ROBGroup_CopyROBGroup";
            public const string RemoveContract = _aSP + "ROBGroup_DeleteROBGroup";
            public const string UpdateContractStatus = _aSP + "ROBGroup_UpdateMultipleROBGroupStatus";
	    }

	    public static class RobGroup
	    {

            //editor
	        public const string GetWorkflowStatuses =  _aSP + "ROBGroup_GetWorkflowStatuses";
            public const string GetScenarios = _aSP + "ROBGroup_GetScenarios";
	        public const string GetRecipients = _aSP + "ROBGroup_GetRecipients";
            public const string GetROBGroup = _aSP + "ROBGroup_GetROBGroup";
            public const string GetROBGroupDetailsGrid = _aSP + "ROBGroup_GetROBsDetails";
	        public const string GetComments = _aSP + "ROBGroup_GetComments";
            public const string AddComment = _aSP + "ROBGroup_AddComment";
            public const string Save = _aSP + "ROBGroup_SaveROBGroup";
             
            //list
            public const string Remove = _aSP + "ROBGroup_DeleteROBGroup";
            public const string Copy = _aSP + "ROBGroup_CopyROBGroup";
            public const string MultipleStatusUpdate = _aSP + "ROBGroup_UpdateMultipleROBGroupStatus";
	        public const string GetTabLayout = _aSP + "ROBGroup_GetTabLayout";
	        public const string GetCustomers = _aSP + "ROBGroup_GetCustomers";
	        public const string GetTerms = _aSP + "ROB_GetContractTerms";
	    }

	    public static class RobGroupCreator
	    {
            public const string GetProperties = _aSP + "ROB_GetGroupProperties";
	        public const string GetCustomerLevels = _aSP + "ROB_GetCustLevels";
            public const string GetCustomers = _aSP + "ROB_GetCustLevelItems";
	        public const string GetROBGroup = _aSP + "ROB_GetROBGroup";
	        public const string SaveROBGroup = _aSP + "ROB_SaveROBGroup";
	    }

        public static class Scenarios
        {
            public const string GetPromotions = _aSP + "SCEN_GetPromotions";
            public const string GetScenariosShort = _aSP + "SCEN_GetScenarioList";
            public const string GetScenariosLong = _aSP + "SCEN_GetScenarios";
            public const string DeleteScenarios = _aSP + "SCEN_DeleteScenarios";
            public const string CloseScenarios = _aSP + "SCEN_CloseScenarios";
            public const string CopyScenarios = _aSP + "SCEN_CopyScenarios";
            public const string CreateWorkingScenario = _aSP + "SCEN_CreateWorkingScenario";
            public const string GetScenarioTypes = _aSP + "SCEN_GetScenarioTypes";
            public const string GetScenarioStatuses = _aSP + "SCEN_GetScenarioStatus";
            public const string GetScenarioDetails = _aSP + "SCEN_GetScenarioDetails";
            public const string GetCustLevels = _aSP + "SCEN_GetCustLevels";
            public const string GetCustLevelItems = _aSP + "SCEN_GetCustLevelItems";
            public const string GetProdLevels = _aSP + "SCEN_GetProdLevels";
            public const string GetProdLevelItems = _aSP + "SCEN_GetProdLevelItems";
            public const string SaveScenario = _aSP + "SCEN_SaveScenario";
            public const string GetUsers = _aSP + "SCEN_GetUserList";
            public const string SaveUserPrefs = _aSP + "SaveUserPrefs_Scenarios";
            public const string GetFilterDates = _aSP + "SCEN_GetFilterDates";
            public const string GetPromotionStatuses = _aSP + "SCEN_GetPromotionStatuses";
            public const string GetFundingStatuses = _aSP + "SCEN_GetFundingStatuses";
            public const string GetFunding = _aSP + "SCEN_GetFunding";
            public const string GetApplicableROBs = _aSP + "SCEN_GetApplicableROBs";
            public const string SaveActiveBudgets = _aSP + "SCEN_SaveActiveBudgets";

            public const string LastClosedScenarios = _aSP + "SCEN_UpdateLatestClosed";
            public const string GetFilterStatuses = _aSP + "SCEN_GetFilterStatuses";
        }

        public static class Schedule
        {
            //Promo schedule data
            public const string GetPromotionScheduleData = _aSP + "SCHD_GetItems";
            public const string GetSingleItem = _aSP + "SCHD_GetEditorVerticalGrid_NEW_VERSION_TO_BE_IMPLEMENTED_IN_APP";
            public const string SaveSingleItem = _aSP + "SCHD_SaveEditorVerticalGrid_NEW_VERSION_TO_BE_IMPLEMENTED_IN_APP";
            public const string CopySingleItem = _aSP + "SCHD_CopyItem_NEW_VERSION_TO_BE_IMPLEMENTED_IN_APP";
            public const string GetDates = _aSP + "SCHD_GetDates";
            public const string GetStatuses = _aSP + "SCHD_GetStatuses";

            public const string DeleteSingleItem =
                _aSP + "SCHD_DeleteEditorVerticalGrid_NEW_VERSION_TO_BE_IMPLEMENTED_IN_APP";
        }

        public static class Settings
        {
            public const string GetScreenList = _aSP + "Settings_GetScreenList";
            public const string SaveStartScreen = _aSP + "Settings_SaveStartScreen";
            public const string SaveListingsGroup = _aSP + "SETTINGS_SaveListingsGroup";
            public const string DeleteListingsGroup = _aSP + "SETTINGS_DeleteListingsGroup";
            public static string SaveScreenOrder = _aSP + "SETTINGS_SaveScreenOrder";
            public static string GetProcedures = _aSP + "APPINFO_GetProcedures";
        }

        public static class Shared
        {
            public const string GetListingsGroups = _aSP + "SHARED_GetListingsGroupsDropdown"; 
            public const string GetListingsGroup = _aSP + "SHARED_GetListingsGroup";
            public const string GetListings = _aSP + "SHARED_GetListings";
            public const string GetFilterCustomers = _aSP + "SHARED_GetFilterCustomers";
            public const string GetFilterProducts = _aSP + "SHARED_GetFilterProducts";
            public const string SaveDefaults = _aSP + "SHARED_SaveDefaults";
            public const string GetFilterStatusesAndTypes = _aSP + "SHARED_GetFilterStatusesAndTypes";
            public const string GetFiltersGrid = _aSP + "SHARED_GetFiltersGrid";
        }

        public static class Supersession
        {
            public const string GetProductMappings = _aSP + "SUPERSESSION_GetProductMappings";
            public const string GetProductTreeGrid = _aSP + "SUPERSESSION_GetActualsData";
            public const string Save = _aSP + "SUPERSESSION_Save";
            public const string GetDataFeeds = _aSP + "SUPERSESSION_GetDataFeeds";
        }

        public static class Template
        {
            public const string GetTemplates = _aSP + "TEMPLATE_GetTemplates";
            public const string GetCustomers = _aSP + "TEMPLATE_Wizard_Customers_GetCustomers";
            public const string GetCustomersSubLevel = _aSP + "TEMPLATE_Wizard_Customers_GetCustomersSubLevel";
            public const string GetFilterStatuses = _pP + "GetFilterStatuses";
            public const string GetDateTypes = _aSP + "TEMPLATE_Wizard_Dates_GetDateTypes";
            public const string GetProducts = _aSP + "TEMPLATE_Wizard_Products_GetProducts";
            public const string GetProductPrices = _aSP + "TEMPLATE_Wizard_Products_GetProductPrices";
            public const string GetPromoMeasures = _aSP + "TEMPLATE_Wizard_Financials_GetPromoMeasures";
            public const string GetParentProductMeasures = _aSP + "TEMPLATE_Wizard_Financials_GetParentProductMeasures";
            public const string GetProductMeasures = _aSP + "TEMPLATE_Wizard_Financials_GetProductMeasures";
            public const string SaveTemplate = _aSP + "TEMPLATE_SaveTemplate";
            public const string ApplyTemplate = _pP + "ApplyTemplate";
            public const string GetTemplate = _aSP + "TEMPLATE_GetTemplate";
            public const string CopyTemplate = _aSP + "TEMPLATE_CopyTemplate";
            public const string UpdateMultipleTemplateStatus = _aSP + "TEMPLATE_UpdateMultipleTemplateStatus";
            public const string GetTemplateComments = _aSP + "Template_Wizard_GetTemplateComments";
            public const string AddComment = _aSP + "Template_Wizard_AddComment";
            public const string DeleteTemplates = _aSP + "TEMPLATE_DeleteTemplates";
            public const string GetWorkflowStatuses = _aSP + "TEMPLATE_Wizard_Review_GetWorkflowStatuses";
            public const string GetConstraintTypes = _aSP + "TEMPLATE_Wizard_Products_GetConstraintTypes";
            public const string GetConstraints = _aSP + "TEMPLATE_Wizard_Products_GetConstraints";
            public const string SaveConstraints = _aSP + "TEMPLATE_Wizard_Products_SaveConstraints";
            public const string GetAttributes = _aSP + "TEMPLATE_Wizard_Attributes_GetAttributes";
            public const string GetPandLGridFirst = _aSP + "TEMPLATE_Wizard_Review_GetPandLGrid_First";
            public const string GetPandLGridSecond = _aSP + "TEMPLATE_Wizard_Review_GetGrid_Second";
        }

        public static class EPOS
        {
            public static class Mapping
            {
                public const string CreateMapping =                     "epos.SP_MAPPING_CreateMapping";
                public const string DeleteMapping =                     "epos.SP_MAPPING_DeleteMapping";
                public const string GenerateMappingSuggestions =        "epos.SP_MAPPING_GenerateMappingSuggestions";
                public const string GetMappings =                       "epos.SP_MAPPING_GetMappings";
                public const string GetMappingSuggestions =             "epos.SP_MAPPING_GetMappingSuggestions";
                public const string GetProducts =                       "epos.SP_MAPPING_GetProducts";
                public const string GetUnmappedProducts =               "epos.SP_MAPPING_GetUnmappedProducts";                                 
            }           
        }

        public class ListingMgmnt
        {
            public const string GetListings = _aSP + "LISTINGS_GetListings";

            public const string GetListingDetails = _aSP + "LISTINGS_ListingsEditor_GetList";
            public const string GetListingCustomers = _aSP + "LISTINGS_GetCustomers";
            public const string GetListingProducts = _aSP + "LISTINGS_GetProducts";

            public const string DeleteListing = _aSP + "LISTINGS_DeleteListing";

            public const string SaveListing = _aSP + "LISTINGS_SaveListings";
        }

        public class Storage
        {
            public const string GetFiles = _aSP + "SHARED_GetFiles";
            public const string SaveFile = _aSP + "SHARED_SaveFiles";
        }

        public class Scenario
        {
            public const string AddComment = _aSP + "SCEN_AddComment";
            public const string GetComments = _aSP + "SCEN_GetComments";
        }
    }
}
