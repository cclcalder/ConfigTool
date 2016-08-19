using System.Xml.Linq;
using Exceedra.Chart.ViewModels;
using Exceedra.Common;
using Exceedra.Common.Xml;
using Exceedra.Controls.Messages;
using Model.DataAccess.Generic;
using Model.Entity;
using WPF.Navigation;
using WPF.Pages.PromoPowerEditor;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using WPF.ViewModels.PromotionPowerEditor;
using WPF.Wizard;

namespace WPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Model;
    using Model.DataAccess;
    using Exceedra.Common.Mvvm;
    using ViewHelper;
    using global::ViewModels;
    using Model.Enumerators;
    public class PromotionMainViewModel : ViewModelBase
    {
        #region Constructors

        public static PromotionMainViewModel New()
        {
            var instance = new PromotionMainViewModel();

            return instance;
        }

        public static PromotionMainViewModel New(bool reloadCacheList)
        {
            var instance = new PromotionMainViewModel();

            return instance;
        }

        public PromotionPowerEditorViewModel PowerEditorViewModel;

        private PromotionMainViewModel()
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            SetupTabs();
            

            var datesXml = CommonXml.GetBaseArguments("GetPromotionDates");

            var statusXml = CommonXml.GetBaseArguments("GetPromotionStatuses");
            statusXml.AddElement("IncludeTemplateStatuses", ShowTemplates ? "1" : "0");
            statusXml.AddElement("IncludePromotionStatuses", 1);

            

            FiltersVM = new FilterViewModel
            {
                ApplyFilter = TestApply,
                CurrentScreenKey = ScreenKeys.PROMOTION,
                SingleTreeArguments = statusXml,
                StatusTreeProc = StoredProcedure.Promotion.GetPromotionStatuses,
                DateArguments = datesXml,
                DatesProc = StoredProcedure.Promotion.GetPromotionDefaultFilterDates,
                SaveExtraArguments = FilterSaveDefaultsExtraXml
            };

            FiltersVM.Load();
        }

        private FilterViewModel _filtersVM;
        public FilterViewModel FiltersVM
        {
            get
            {
                return _filtersVM;
            }
            set
            {
                _filtersVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }

        private XElement FilterSaveDefaultsExtraXml
        {
            get { return XElement.Parse("<Extra><Graph_Idx>" + (SelectedPromotionChart == null ? "" : SelectedPromotionChart.Idx) + "</Graph_Idx><UsePowerEditor>" + (IsPowerEditor ? "1" : "0") + "</UsePowerEditor></Extra>"); }
        }

        private TabViewModel _tabData;
        public TabViewModel TabData
        {
            get { return _tabData; }
            set
            {
                _tabData = value;
                NotifyPropertyChanged(this, vm => vm.TabData);
            }
        }

        public List<Tab> TabList { get; set; }

        private void SetupTabs()
        {
            Tab fstTab = new Tab
            {
                TabName = "Promotions",
                TabTitle = App.CurrentLang.GetValue("Label_Promotions", "Promotions"),
                TabType = "HorizontalGrid",
                TabMainContentProc = StoredProcedure.Promotion.GetPromotions,
                ApplyRootXml = "GetPromotions"
            };

            if (TabList == null)
            {
                TabList = new List<Tab>();
            }

            TabList = new List<Tab> { fstTab };

            if (ChartingVisibility == Visibility.Visible)
            {
                Tab sndTab = new Tab
                {
                    TabName = "Chart",
                    TabTitle = App.CurrentLang.GetValue("Label_PromotionsChart", "Charts"),
                    TabType = "Chart",
                    TabMainContentProc = StoredProcedure.Promotion.GetGraph,
                    TabChartListProc = StoredProcedure.Promotion.GetGraphList
                };
                TabList.Add(sndTab);
            }

            if (ShowTemplates)
            {
                Tab trdTab = new Tab
                {
                    TabName = "Templates",
                    TabTitle = App.CurrentLang.GetValue("Label_PromotionTemplates", "Templates"),
                    TabType = "HorizontalGrid",
                    TabMainContentProc = StoredProcedure.Template.GetTemplates,
                    ApplyRootXml = "GetTemplates"
                };

                TabList.Add(trdTab);
            }

            if (ShowSchedule)
            {
                Tab scheduleTab = new Tab
                {
                    TabName = "Schedule",
                    TabTitle = "Schedule",
                    TabType = "ScheduleGrid",
                    TabMainContentProc = StoredProcedure.Promotion.GetPromotions,
                    ApplyRootXml = "GetPromotions"
                };

                TabList.Add(scheduleTab);
            }

            var promoStatusXml = XElement.Parse("<ROOT><IncludeTemplateStatuses>0</IncludeTemplateStatuses><IncludePromotionStatuses>1</IncludePromotionStatuses><ReturnAsList>1</ReturnAsList></ROOT>");
            var templateStatusXml = XElement.Parse("<ROOT><IncludeTemplateStatuses>1</IncludeTemplateStatuses><IncludePromotionStatuses>0</IncludePromotionStatuses><ReturnAsList>1</ReturnAsList></ROOT>");


            var dropdowns = new List<DropdownCollection>
            {
                new DropdownCollection {DropdownProc = StoredProcedure.Template.GetFilterStatuses, Name = "PromotionStatusDropdown", AdditionalLoadXml = promoStatusXml, RootNode = "GetPromotionStatuses", Label = App.CurrentLang.GetValue("Label_UpdatePromotionStatuses", "Update Promotion Statuses"), DropdownCommand = UpdateMultipleStatusCommand},
            };

            // JS: I have moved it here because if the templates are disabled it's pointless to call the proc to get the statuses for the update-multiple-templates-statuses dropdown
            if (ShowTemplates)
                dropdowns.Add(new DropdownCollection { DropdownProc = "app.Procast_SP_PROMO_GetFilterStatuses", Name = "TemplateStatusDropdown", AdditionalLoadXml = templateStatusXml, RootNode = "GetPromotionStatuses", Label = App.CurrentLang.GetValue("Label_UpdateTemplateStatuses", "Update Template Statuses"), DropdownCommand = UpdateMultipleTemplateStatusCommand, IsVisible = Visibility.Visible });

            var buttons = new List<ButtonCollection>
            {
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewPromotion", "New Promotion"), ButtonStyle = StyleType.Success, ButtonCommand = NewPromotionCommand, IsVisible = CreatePromotionVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_NewTemplate", "New Template"), ButtonStyle = StyleType.Success, ButtonCommand = NewTemplateCommand, IsVisible = CreateTemplateVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_CreatePromotionsFromTemplate", "Create Promotion"), ButtonStyle = StyleType.Primary, ButtonCommand = CreatePromotionsFromTemplate, IsVisible = CreatePromotionFromTemplateVisibility},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Copy", "Copy"), ButtonStyle = StyleType.Warning, ButtonCommand = CopyPromotionCommand},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Remove", "Remove"), ButtonStyle = StyleType.Danger, ButtonCommand = RemovePromotionCommand},
                new ButtonCollection {Label = App.CurrentLang.GetValue("Button_Phasing", "Phasing"), ButtonStyle = StyleType.Info, ButtonCommand = ApplyPhasingCommand, IsVisible = PhasingVisibility}
            };

            TabData = new TabViewModel(TabList)
            {
                Dropdowns = dropdowns,
                Buttons = buttons,
                GetFilterXml = GetFilterXml
            };

            TabData.LoadContent();
        }

        private void TestApply()
        {          
                TabData.LoadContent();
        }

        public XElement GetFilterXml(string rootNode)
        {
            var root = String.IsNullOrWhiteSpace(rootNode) ? "GetPromotions" : rootNode;
            if (FiltersVM != null && FiltersVM.ApplyFilterCommand.CanExecute(null))
            { 
                return FiltersVM.GetFiltersAsXml(root);
            }
            else
            {
                var x =  CommonXml.GetBaseArguments(root);
                x.AddElement("LoadFromDefaults", "1");

                return x;
            }
           
        }


        //TODO: Implement the chart stuff on the tab control
        //public void ApplyFilters()
        //{
        //    if (PromotionChartList == null || !PromotionChartList.Any())
        //        LoadPromotionChartList();
        //    LoadChart();
        //}


        private void LoadChart()
        {           
            if (SelectedPromotionChart != null)
            {
                var args = FiltersVM.GetFiltersAsXml("GetPromotions");
                args.AddElement("Promotion_Graph_Idx", SelectedPromotionChart.Idx);
                _promotionDataAccess.GetPromotionChartDataAsync(args).ContinueWith(t =>
                {
                    if (t.Result == null)
                    {
                        PromotionChartData = new RecordViewModel(false);
                    }
                    else
                    {
                        PromotionChartData = RecordViewModel.LoadWithData(t.Result);
                        App.AppCache.Upsert("Promotion_ChartData", PromotionChartData, DateTime.Now);
                    }
                });
            }
        }
        
        public void LoadPromotionChartList()
        {
            if (App.Configuration.IsPromotionsChartingActive)
            {
                PromotionChartList =
                    new ObservableCollection<PromotionChart>(_promotionDataAccess.GetPromotionChartList());
                SelectedPromotionChart = (PromotionChartList.Where(s => s.IsDefault)).FirstOrDefault();
            }
        }

        # endregion

        # region Properties
        
        #region PromotionData

        private RecordViewModel _promotionChartData;


        public RecordViewModel PromotionChartData
        {
            get
            {
                return _promotionChartData;
            }
            set
            {
                if (_promotionChartData != value)
                {
                    _promotionChartData = value;
                    NotifyPropertyChanged(this, vm => vm.PromotionChartData);
                }
            }
        }

        public IEnumerable<string> SelectedPromotionIds
        {
            get { return TabData.GetTab("Promotions").GetSelectedItems(); }
        }

        public IEnumerable<string> SelectedTemplateIds
        {
            get { return TabData.GetTab("Templates").GetSelectedItems(); }
        }

        #endregion

        # region Charts


        private ObservableCollection<PromotionChart> _promotionChartList;

        public ObservableCollection<PromotionChart> PromotionChartList
        {
            get { return _promotionChartList; }
            set
            {
                if (_promotionChartList != value)
                {
                    _promotionChartList = value;
                    NotifyPropertyChanged(this, vm => vm.PromotionChartList);
                }
            }
        }

        private PromotionChart _selectedPromotionChart;

        public PromotionChart SelectedPromotionChart
        {
            get { return _selectedPromotionChart; }
            set
            {
                if (_selectedPromotionChart == value) return;
                _selectedPromotionChart = value;
                NotifyPropertyChanged(this, vm => vm.SelectedPromotionChart);

                if (SelectedPromotionChart != null)
                {
                    LoadChart();
                }

                FiltersVM.SaveExtraArguments = FilterSaveDefaultsExtraXml;
            }
        }

        # endregion

        #endregion

        #region Commands

        public ICommand NewTemplateCommand
        {
            get { return new ViewCommand(NewTemplate);}
        }

        public ICommand NewPromotionCommand
        {
            get { return new ViewCommand(NewPromotion); }
        }

        public ICommand UpdateMultipleStatusCommand
        {
            get { return new ViewCommand(CanUpdateStatus, UpdateMultipleStatus); }
        }

        public bool CanUpdateStatus(object obj)
        {
            return (CanCopyCloseDelete(obj) && TabData.SelectedTab.TabName.Equals("Promotions") && TabData.GetDropdown("PromotionStatusDropdown").Items.SelectedItem != null);
        }

        private void UpdateMultipleStatus(object obj)
        {
            if (SelectedPromotionIds == null || !SelectedPromotionIds.Any()) return;

            var item = TabData.GetDropdown("PromotionStatusDropdown").Items.SelectedItem;

            string message = string.Format("Are you sure you want to set selected promotions to {0} status?", item.Name);

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            var res = _promotionDataAccess.UpdateStatusPromotions(SelectedPromotionIds.ToArray(), item.Idx);
            MessageBoxShow(res);
            FiltersVM.ApplyFilter();
        }

        public bool CanUpdateTemplateStatus(object obj)
        {
            return (CanCopyCloseDelete(obj) && TabData.SelectedTab.TabName.Equals("Templates") && TabData.GetDropdown("TemplateStatusDropdown").Items.SelectedItem != null);
        }

        public bool CanCreatePromoFromTemplate(object o)
        {
            return CanCopyCloseDelete(o) && TabData.SelectedTab.TabName.Equals("Templates");
        }

        private void UpdateMultipleTemplateStatus(object obj)
        {
            var item = TabData.GetDropdown("TemplateStatusDropdown").Items.SelectedItem;

            string message = string.Format("Are you sure you want to set selected templates to {0} status?", item.Name);

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            var templateAccess = new PromotionTemplateAccess();
            var res = templateAccess.UpdateStatusPromotions(SelectedTemplateIds.ToArray(), item.Idx);
            MessageBoxShow(res);
            FiltersVM.ApplyFilter();
        }

        public ICommand RemovePromotionCommand
        {
            get { return new ViewCommand(CanCopyCloseDelete, Remove); }
        }

        public ICommand CopyPromotionCommand
        {
            get { return new ViewCommand(CanCopyCloseDelete, Copy); }
        }

        public ICommand CopyTemplateCommand
        {
            get { return new ViewCommand(CanCopyCloseDelete, Copy); }
        }

        public ICommand RemoveTemplateCommand
        {
            get { return new ViewCommand(CanCopyCloseDelete, Remove); }
        }

        public ICommand UpdateMultipleTemplateStatusCommand
        {
            get { return new ViewCommand(CanUpdateTemplateStatus, UpdateMultipleTemplateStatus); }
        }

        public ICommand CreatePromotionsFromTemplate
        {
            get { return new ViewCommand(CanCreatePromoFromTemplate, CreatePromotionsFromSelectedTemplates); }
        }
        
        public ICommand ApplyPhasingCommand
        {
            get { return new ViewCommand(CanApplyPhasing, ApplyPhasing); }
        }

        private void ApplyPhasing(object obj)
        {
            if (!CanCopyCloseDelete(obj)) return;

            var promos = TabData.SelectedTab.TabMainContent as Exceedra.Controls.DynamicGrid.ViewModels.RecordViewModel;
            var ids = TabData.SelectedTab.GetSelectedItems();
            Dictionary<string, string> selectedPromotionIdxs = promos.Records.Where(t => ids.Contains(t.Item_Idx)).ToDictionary(r => r.Item_Idx, y => y.GetProperty("Name").Value);


            var applyPhasingViewModel = new ApplyPhasingViewModel(selectedPromotionIdxs);
            var managePhasingViewModel = new ManagePhasingViewModel();
            var viewModel = new PhasingViewModel(applyPhasingViewModel, managePhasingViewModel);
            App.Navigator.NavigateTo(new Pages.Phasing(viewModel));
        }

        # endregion

        private readonly PromotionAccess _promotionDataAccess = new PromotionAccess();
        
        #region IObserver Members

        public void UpdateState()
        {
            NotifyPropertyChanged(this, vm => vm.SelectedPromotionIds);
        }

        #endregion

        public bool CanOpenWithPowerEditorCheck(object parameter)
        {
            if (IsPowerEditor)
            {

            }

            return true;
        }

        public void NewTemplate(object o)
        {
            RedirectMe.Goto("Template");
        }

        public void NewPromotion(object parameter)
        {
            if (IsPowerEditor)
            {
                App.Navigator.NavigateTo(
                    new Page1(new PromotionPowerEditorViewModel()));
            }
            else
            {
                RedirectMe.Goto("Promotion");
            }
        }
       
        public bool CanCopyCloseDelete(object o)
        {
            return TabData.SelectedTab.AreItemsSelected(null); 
        }

        public bool CanApplyPhasing(object o)
        {
            return TabData.SelectedTab.TabName == "Promotions" && CanCopyCloseDelete(null);
        }        

        public void Copy(object o)
        {
            if(TabData.SelectedTab.TabName.Equals("Promotions"))
                CopyPromotion(o);
            else if(TabData.SelectedTab.TabName.Equals("Templates"))
                CopyTemplate(o);
        }

        public void CopyPromotion(object param)
        {
            if (SelectedPromotionIds == null || !SelectedPromotionIds.Any()) return;

            bool anyPromotions = SelectedPromotionIds.Any();

            string message = anyPromotions
                                       ? "Are you sure you want to copy selected promotions?"
                                       : "Are you sure you want to copy selected payments?";

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                if (anyPromotions)
                    _promotionDataAccess.CopyPromotion(SelectedPromotionIds.ToArray());

                MessageBoxShow("Copied successfully.");

                FiltersVM.ApplyFilter();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        public void CopyTemplate(object param)
        {
            if (SelectedTemplateIds == null || !(SelectedTemplateIds.Any())) return;

            bool anyPromotions = SelectedTemplateIds.Any();

            string message = (anyPromotions ? "Are you sure you want to copy selected templates?" : "");

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                if (anyPromotions)
                {
                    var TemplateAccess = new PromotionTemplateAccess();
                    TemplateAccess.CopyPromotion(SelectedTemplateIds.ToArray());
                }
                MessageBoxShow("Copied successfully.");

                FiltersVM.ApplyFilter();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        public void Remove(object o)
        {
            if (TabData.SelectedTab.TabName.Equals("Promotions"))
                RemovePromotion(o);
            else if (TabData.SelectedTab.TabName.Equals("Templates"))
                RemoveTemplate(o);
        }

        public void RemovePromotion(object param)
        {
            if (SelectedPromotionIds == null || !SelectedPromotionIds.Any()) return;

            bool anyPromotions = SelectedPromotionIds.Any();

            string message = "Are you sure you want to delete selected promotions?";

            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                MessageBoxResult.No)
                return;

            try
            // Delete selected promotions
            {
                string result = "An unexpected error occurred";
                if (anyPromotions)
                {
                    result = _promotionDataAccess.DeletePromotion(SelectedPromotionIds.ToArray());
                }

                MessageBoxShow(result);

                FiltersVM.ApplyFilter();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        public void RemoveTemplate(object param)
        {
            if (SelectedTemplateIds == null || !(SelectedTemplateIds.Any())) return;

            bool anyPromotions = SelectedTemplateIds.Any();

            string message = (anyPromotions ? "Are you sure you want to remove selected templates?" : "");


            if (CustomMessageBox.Show(message, "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            try
            {
                if (anyPromotions)
                {
                    var TemplateAccess = new PromotionTemplateAccess();
                    var result = TemplateAccess.DeleteTemplatePromotion(SelectedTemplateIds.ToArray());
                    MessageBoxShow(result);
                }

                FiltersVM.ApplyFilter();
            }
            catch (ExceedraDataException ex)
            {
                MessageBoxShow(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        #region visibilites

        public bool ShowTemplates
        {
            get { return App.Configuration.IsTemplatingActive; }
        }
        public bool ShowSchedule
        {
            get { return App.Configuration.IsPromoShowSchedule; }
        }
        public Visibility PromoPowerEditorVisibility
        {
            get { return App.Configuration.IsPromoPowerEditorActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CreatePromotionVisibility
        {
            get { return App.Configuration.IsCreatePromotionActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyPromotionVisibility
        {
            get { return App.Configuration.IsCopyPromotionActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeletePromotionVisibility
        {
            get { return App.Configuration.IsDeletePromotionActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CreatePromotionFromTemplateVisibility
        {
            get { return App.Configuration.IsCreatePromotionFromTemplateActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CreateTemplateVisibility
        {
            get { return App.Configuration.IsCreateTemplateActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CopyTemplateVisibility
        {
            get { return App.Configuration.IsCopyTemplateActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteTemplateVisibility
        {
            get { return App.Configuration.IsDeleteTemplateActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility PaymentsVisibility
        {
            get { return App.Configuration.IsPaymentsActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility PhasingVisibility
        {
            get { return App.Configuration.IsPhasingActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility ChartingVisibility
        {
            get { return App.Configuration.IsPromotionsChartingActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        #endregion

        //TODO: Move to the tab control
        private bool _isPowerEditor;
        public bool IsPowerEditor
        {
            get
            {

                return _isPowerEditor;
            }
            set
            {
                _isPowerEditor = value;
                NotifyPropertyChanged(this, vm => vm.IsPowerEditor);
                FiltersVM.SaveExtraArguments = FilterSaveDefaultsExtraXml;
            }
        }

        internal void CreatePromotionsFromSelectedTemplates(object obj)
        {
            var taccess = new PromotionTemplateAccess();
            var results = taccess.ApplyTemplate(SelectedTemplateIds.ToList());

            var test = results.GetValue<string>("Message");
            var id = results.GetValue<string>("ID");

            if (id != null)
            {
                MessageBoxResult messageBoxResult = CustomMessageBox.Show(test + "\n Do you want to edit this promotion now?", "Edit new promotion", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    //  RedirectMe.EntryPoint("promotion", Convert.ToInt32(id));
                    App.Navigator.NavigateTo(new WizardFrame(id));
                }
            }
            else
            {
                Messages.Instance.Put(new InformationMessage(test));    
            } 
           
            FiltersVM.ApplyFilter();
 
        }
    }
}