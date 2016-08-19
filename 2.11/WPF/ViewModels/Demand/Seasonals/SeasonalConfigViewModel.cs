using AutoMapper;
using Exceedra.Chart.Model;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Demand;
using Model.Entity.Generic;
using Model.Entity.Listings;
using QFXModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Filters.ViewModels;
using ChartViewModel = Exceedra.Chart.ViewModels.RecordViewModel;


namespace WPF.ViewModels.Demand.Seasonals
{
    public class SeasonalConfigViewModel : ViewModelBase
    {
        public SeasonalConfigViewModel()
        {
            LoadFilters();

            SeasonalProfiles = new ObservableCollection<SelectableItem>
            {
                new SelectableItem {ItemDescription = "New Internal Profile", Idx = "Internal", IsSelected = true},
                new SelectableItem {ItemDescription = "New Global Profile", Idx = "Library"}
            };
        }

        #region Filters

        public DemandFilterObject BaseFO { get; set; }

        private void LoadFilters()
        {
            CreationFilters = new FilterViewModel
            {
                UseListingsGroups = false,
                ApplyFilter = ApplyFilter,
                NoSingleTree = true,
                CurrentScreenKey = ScreenKeys.SEASONALS,
                IsUsingOtherFilters = true,
            };

            CreationFilters.DataLoaded += SetupDropdownChanges;

            CreationFilters.Load();
        }

        private void ApplyFilter()
        {
            SeasonalRecordViewModel = new RecordViewModel();
            SeasonalChartData.IsLoading = true;
            ProfilesTree = new List<SeasonalTreeNode>();
            //Implement IsLoading stuff for the second tree

            BaseFO = new DemandFilterObject
            {
                CustomerIdxs = CreationFilters.ListingsVM.CustomerIDsList,
                ProductIdxs = CreationFilters.ListingsVM.ProductIDsList,
                DateRange = new FilterDateRange(CreationFilters.StartDate, CreationFilters.EndDate),
                TrialIdx = TrialForecastDropdown.SelectedItem.Item_Idx,
                TabIdx = "Seasonal",
            };
            var args = DemandAccess.DfoToXElement(BaseFO);

            LoadSeasonalsGrid(args);
            LoadLibrarySeasonals(args);            

            SeasonalProfileName = "";
        }

        private void SetupDropdownChanges()
        {
            TrialForecastDropdown.PropertyChanged += TrialForecastDropdown_PropertyChanged;
            if (TrialForecastDropdown.SelectedItem != null)
                ConfigureVisibleListings(TrialForecastDropdown.SelectedItem.Item_Idx);

            if (CreationFilters.CanApplyFilter(null))
                CreationFilters.ApplyFilter();
            else
                SeasonalRecordViewModel = new RecordViewModel(false);
        }

        private void TrialForecastDropdown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                ConfigureVisibleListings(TrialForecastDropdown.SelectedItem.Item_Idx, true);
            }
        }

        private void ConfigureVisibleListings(string trialForecastIdx, bool clearSelection = false)
        {
            var listings = DemandAccess.GetVisibleListings(trialForecastIdx);
            CreationFilters.ListingsVM.SetVisibleCustSkus(listings.GetCustomerIdxs(), listings.GetSkuIdxs(), clearSelection);
            if (!clearSelection)
                CreationFilters.ListingsVM.SetSelections(listings);
        }

        private FilterViewModel _creationFilters;
        public FilterViewModel CreationFilters
        {
            get { return _creationFilters; }
            set
            {
                _creationFilters = value;
                NotifyPropertyChanged(this, vm => vm.CreationFilters);
            }
        }

        public RowProperty TrialForecastDropdown
        {
            get
            {
                return CreationFilters.OtherFiltersVM.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("forecast"));
            }
        }

        #endregion

        #region Seasonal Creation

        public void LoadSeasonalsGrid(XElement args)
        {
            DynamicDataAccess.GetGenericItemAsync<RecordViewModel>(StoredProcedure.Demand.GetSeasonalsGrid, args).ContinueWith(t =>
            {
                SeasonalRecordViewModel = t.Result;
                SeasonalRecordViewModel.Records.Last().Properties.Do(p => p.PropertyChanged += SeasonalProperties_PropertyChanged);
                var chart = GetBaseSeasonalChart();
                chart.Series = GetSeriesToShow();
                SeasonalChartData = new ChartViewModel(false) { Chart = chart };
            });
        }

        private void SeasonalProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                SeasonalChartData.Chart.Series.Last().Datapoints = GetSeriesToShow().Last().Datapoints;
            }
        }

        public void LoadLibrarySeasonals(XElement args)
        {
            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Demand.GetLibrarySeasonals, args).ContinueWith(t =>
            {
                SeasonalDropdownXml = t.Result;
                SeasonalLibraries.SetItems(SeasonalDropdownXml);

                //If we already have the tree, then set/reset the dropdowns on it.
                if(ProfilesTree != null && ProfilesTree.Any())
                    LoadTreeDropdowns(SeasonalDropdownXml);
                else
                    LoadProfilesTree();
            });
        }

        private static XElement SeasonalDropdownXml;

        private Exceedra.Chart.Model.Chart GetBaseSeasonalChart()
        {
            return new Exceedra.Chart.Model.Chart
            {
                ChartType = "Categorical",
                XAxisTitle = "Periods",
                YAxisTitle = "Seasonal",
                XAxisType = "Categorical",
                YAxisType = "Linear"
            };
        }

        private ObservableCollection<SingleSeries> GetSeriesToShow()
        {
            return new ObservableCollection<SingleSeries>(SeasonalRecordViewModel.Records.Where(r => r.Item_IsDisplayed).Select(i => new SingleSeries { SeriesName = i.Item_Name, SeriesType = "Line", Datapoints = i.ToChartSeries() }));
        }


        private RecordViewModel _seasonalRecordViewModel;

        public RecordViewModel SeasonalRecordViewModel
        {
            get { return _seasonalRecordViewModel; }
            set
            {
                _seasonalRecordViewModel = value;

                NotifyPropertyChanged(this, vm => vm.SeasonalRecordViewModel);
            }
        }

        private ChartViewModel _seasonalChartData = new ChartViewModel(false);

        public ChartViewModel SeasonalChartData
        {
            get { return _seasonalChartData; }
            set
            {
                _seasonalChartData = value;
                NotifyPropertyChanged(this, vm => vm.SeasonalChartData);
            }
        }

        private string _seasonalProfileName;

        public string SeasonalProfileName
        {
            get { return _seasonalProfileName; }
            set
            {
                _seasonalProfileName = value;
                NotifyPropertyChanged(this, vm => vm.SeasonalProfileName);
            }
        }

        private SingleSelectViewModel _seasonalLibraries = new SingleSelectViewModel();
        public SingleSelectViewModel SeasonalLibraries
        {
            get { return _seasonalLibraries; }
            set
            {
                _seasonalLibraries = value;
                NotifyPropertyChanged(this, vm => vm.SeasonalLibraries);
            }
        }

        #region Commands

        public ICommand LoadLibraryCommand
        {
            get { return new ViewCommand(CanLoadLibrary, LoadLibrary); }
        }

        public ICommand SaveSeasonalProfileCommand
        {
            get { return new ViewCommand(CanSaveSeasonalProfile, SaveSeasonalProfile); }
        }
        public ICommand SaveExistingCommand
        {
            get { return new ViewCommand(CanSaveExistingProfile, SaveExistingProfile); }
        }
        public ICommand SetAsCurrentProfileCommand
        {
            get { return new ViewCommand(CanSetAsCurrentProfile, SetAsCurrentProfile); }
        }

        public ICommand CalculateSeasonalCommand
        {
            get { return new ViewCommand(CanCalcSmooNorm, CalculateSeasonal); }
        }

        public ICommand SmoothSeasonalCommand
        {
            get { return new ViewCommand(CanCalcSmooNorm, SmoothSeasonal); }
        }

        public ICommand NormaliseSeasonalCommand
        {
            get { return new ViewCommand(CanCalcSmooNorm, NormaliseSeasonal); }
        }

        private bool CanLoadLibrary(object parameter)
        {
            return SeasonalLibraries.SelectedItem != null;
        }

        private bool CanSaveSeasonalProfile(object parameter)
        {
            return !String.IsNullOrWhiteSpace(SeasonalProfileName) && SelectedSeasonalProfile != null;
        }

        private bool CanSaveExistingProfile(object parameter)
        {
            return SeasonalLibraries.SelectedItem != null && SeasonalRecordViewModel != null &&
                   SeasonalRecordViewModel.Records != null &&
                   SeasonalRecordViewModel.Records.FirstOrDefault(r => r.Item_Type.ToLower().Contains("new_seasonal")) != null;
        }

        private bool CanSetAsCurrentProfile(object parameter)
        {
            return SeasonalLibraries.SelectedItem != null
                && SeasonalRecordViewModel != null
                && SeasonalRecordViewModel.Records != null
                && SeasonalRecordViewModel.Records.Any(r => r.Item_Type.ToLower().Contains("new_seasonal"));
        }

        private bool CanCalcSmooNorm(object parameters)
        {
            return true;
        }

        private void LoadLibrary(object obj)
        {
            var libraryIdx = SeasonalLibraries.SelectedItem.Idx;

            var response = DemandAccess.LoadLibrary(libraryIdx);

            if (response != null)
            {
                var seasonals = response.Element("Pattern").Elements().Select(e => e.Element("Value").Value.AsNumericDouble());
                SetNewSeasonals(seasonals);
            }

        }

        private void SaveSeasonalProfile(object obj)
        {
            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.Demand.SaveSeasonalProfile, GetSeasonalXml(true, SelectedSeasonalProfile.Idx != "Internal", true)))
                LoadLibrarySeasonals(DemandAccess.DfoToXElement(BaseFO));
        }

        /* TODO: This needs to save the values in New Trial Forecast as the values for the currently selected global profile */
        private void SaveExistingProfile(object obj)
        {
            MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.Demand.SaveSeasonalProfile, GetSeasonalXml(true, false, false)));
        }

        private XElement GetSeasonalXml(bool savingSeasonal, bool isNewGlobalProfile, bool isNewProfile, bool applyToListings = false)
        {


            var xml = CommonXml.GetBaseSaveArguments();
            xml.AddElement("Seasonal_Idx", isNewProfile ? null : SeasonalLibraries.SelectedItem.Idx);
            xml.AddElement("Forecast_Idx", TrialForecastDropdown.SelectedItem.Item_Idx);
            xml.AddElement("SaveSeasonal", savingSeasonal ? 1 : 0);//If not saving, then setting
            xml.AddElement("SaveForecast", applyToListings ? 1 : 0);
            xml.AddElement("IsNewProfileGlobal", isNewGlobalProfile ? 1 : 0);
            xml.AddElement("Seasonal_Name", isNewProfile ? SeasonalProfileName : null);
            if (!isNewGlobalProfile)
                xml.Add(GetListingsXml());

            if (!applyToListings)
            {
                var values = GetNewSeasonals();
                var periods = new XElement("Periods", values.Select(v => new XElement("Period", new XAttribute("Number", v.Date), new XAttribute("Value", v.Value))));
                xml.Add(periods);
            }

            return xml;
        }

        private XElement GetListingsXml(string rootTag = "Listings")
        {
            return ListingConverter.ToListingXml(BaseFO.CustomerIdxs.ToHashSet(), BaseFO.ProductIdxs.ToHashSet(), ListingsAccess.GetListings(), rootTag);
        }

        private void SetAsCurrentProfile(object obj)
        {
            MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.Demand.SaveSeasonalProfile, GetSeasonalXml(false, false, false, true)));
        }

        private void CalculateSeasonal(object obj)
        {
            var url = App.Configuration.DemandCalculateSeasonalProfileUrl;
            if (url == null)
            {
                MessageBoxShow("Could not find DemandCalculateSeasonalProfileUrl");
                return;
            }

            CalcSeasonals(url);
        }

        private void SmoothSeasonal(object obj)
        {
            var url = App.Configuration.DemandSmoothSeasonalProfileUrl;
            if (url == null)
            {
                MessageBoxShow("Could not find DemandSmoothSeasonalProfileUrl");
                return;
            }

            SeasonalsSmooNorm(url);
        }

        private void NormaliseSeasonal(object obj)
        {
            var url = App.Configuration.DemandNormaliseSeasonalProfileUrl;
            if (url == null)
            {
                MessageBoxShow("Could not find DemandNormaliseSeasonalProfileUrl");
                return;
            }

            SeasonalsSmooNorm(url);
        }

        private void CalcSeasonals(string url)
        {
            var tmp = DemandAccess.CalcSeasonals(GetSeasonalsGrid(), url);
            if (tmp != null)
            {
                SetNewSeasonals(tmp);

                SeasonalChartData.Chart.Series.Last().Datapoints = GetSeriesToShow().Last().Datapoints;
            }
        }

        private void SeasonalsSmooNorm(string url)
        {
            var tmp = DemandAccess.SeasonalsSmooNorm(GetNewSeasonals(), url);
            if (tmp != null)
            {
                SetNewSeasonals(tmp);

                SeasonalChartData.Chart.Series.Last().Datapoints = GetSeriesToShow().Last().Datapoints;
            }
        }

        private IEnumerable<Actual> GetSeasonalsGrid()
        {
            var yearRows = SeasonalRecordViewModel.Records.Where(r => r.Item_Type.Contains("20"));
            var weekProps = yearRows.SelectMany(r => r.Properties.Skip(1).Where(p => p.IsEditable));

            return weekProps.Select(p => new Actual(p.ColumnCode.AsNumericInt(), p.Value.AsNumericDouble(), p.Value2.ToLower() == "true"));
        }

        private IEnumerable<Actual> GetNewSeasonals()
        {
            return SeasonalRecord.Properties.Where(p => p.IsEditable && p.IsDisplayed).Select(p => new Actual(p.ColumnCode.AsNumericInt(), p.Value.AsNumericDouble()));
        }

        private void SetNewSeasonals(IEnumerable<double?> newSeasonals)
        {
            var seasonalArray = newSeasonals.ToArray();
            SeasonalRecord.Properties.Where(p => p.IsEditable && p.IsDisplayed).Do(p => p.SetValue(seasonalArray[p.ColumnCode.AsNumericInt() - 1].ToString()));

            SeasonalChartData.Chart.Series.Last().Datapoints = GetSeriesToShow().Last().Datapoints;
        }

        private void SetNewSeasonals(IEnumerable<double> newSeasonals)
        {
            var seasonalArray = newSeasonals.ToArray();
            SeasonalRecord.Properties.Where(p => p.IsEditable && p.IsDisplayed).Do(p => p.SetValue(seasonalArray[p.ColumnCode.AsNumericInt() - 1].ToString()));

            SeasonalChartData.Chart.Series.Last().Datapoints = GetSeriesToShow().Last().Datapoints;
        }

        /* Builds the New Seasonal Record incase the DB doesn't send it */
        private Exceedra.Controls.DynamicGrid.Models.Record SeasonalRecord
        {
            get
            {
                var seasonalRecord = SeasonalRecordViewModel.GetRecordByType("NEW_SEASONAL");
                if (seasonalRecord == null)
                {
                    var newSeasonalRecord = new Exceedra.Controls.DynamicGrid.Models.Record();
                    newSeasonalRecord.Item_Idx = "-5";
                    newSeasonalRecord.Item_IsDisplayed = true;
                    newSeasonalRecord.Item_Name = "New Seasonal";
                    newSeasonalRecord.Item_Type = "NEW_SEASONAL";
                    newSeasonalRecord.Properties = new ObservableCollection<Property>();
                    SeasonalRecordViewModel.Records.First(r => r.Item_Type.Contains("20")).Properties.Do(p =>
                    {
                        newSeasonalRecord.Properties.Add(new Property { ColumnCode = p.ColumnCode, Alignment = p.Alignment, ControlType = "Textbox", HeaderText = p.HeaderText, IsEditable = true, IDX = p.IDX, IsDisplayed = p.IsDisplayed, Value = "1", StringFormat = "N4" });
                    });
                    var rowNameProp = newSeasonalRecord.Properties.First(p => p.ColumnCode == "Row_Name");
                    rowNameProp.Value = "New Seasonal";
                    rowNameProp.IsEditable = false;
                    rowNameProp.StringFormat = "";
                    SeasonalRecordViewModel.AddRecord(newSeasonalRecord);

                    var chart = GetBaseSeasonalChart();
                    chart.Series = GetSeriesToShow();
                    SeasonalChartData = new ChartViewModel(false) { Chart = chart };

                    return SeasonalRecordViewModel.GetRecordByType("NEW_SEASONAL");
                }
                else
                {
                    return seasonalRecord;
                }
            }
        }


        private ObservableCollection<SelectableItem> _seasonalProfiles;

        public ObservableCollection<SelectableItem> SeasonalProfiles
        {
            get { return _seasonalProfiles; }
            set
            {
                _seasonalProfiles = value;
                NotifyPropertyChanged(this, vm => vm.SeasonalProfiles);
            }
        }

        public SelectableItem SelectedSeasonalProfile
        {
            get { return SeasonalProfiles.FirstOrDefault(item => item.IsSelected); }
        }

        #endregion



        #endregion

        #region Seasonal Setting

        private List<SeasonalTreeNode> _profilesTree;
        public List<SeasonalTreeNode> ProfilesTree
        {
            get { return _profilesTree; }
            set
            {
                _profilesTree = value;
                NotifyPropertyChanged(this, vm => vm.ProfilesTree);
            }
        }

        private SeasonalTreeNode _selectedProfilesTreeNode;
        public SeasonalTreeNode SelectedProfilesTreeNode
        {
            get { return _selectedProfilesTreeNode; }
            set
            {
                _selectedProfilesTreeNode = value;
                SelectedSeasonalProductName = SelectedProfilesTreeNode != null ? SelectedProfilesTreeNode.Name + ":" : "No Selection";
                NotifyPropertyChanged(this, vm => vm.SelectedProfilesTreeNode);
            }
        }

        private string _selectedSeasonalProductName;
        public string SelectedSeasonalProductName
        {
            get { return _selectedSeasonalProductName; }
            set
            {
                _selectedSeasonalProductName = value;
                NotifyPropertyChanged(this, vm => vm.SelectedSeasonalProductName);
            }
        }

        private SingleSelectViewModel _massApplySeasonals = new SingleSelectViewModel();
        public SingleSelectViewModel MassApplySeasonals
        {
            get
            {
                return _massApplySeasonals;
            }
            set
            {
                _massApplySeasonals = value;
                NotifyPropertyChanged(this, vm => vm.MassApplySeasonals);
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand { get { return _saveCommand == null ? (_saveCommand = new ViewCommand(CanSave, Save)) : _saveCommand; } }


        private ICommand _massApplyCommand;
        public ICommand MassApplyCommand { get { return _massApplyCommand == null ? (_massApplyCommand = new ViewCommand(CanMassApply, MassApply)) : _massApplyCommand; } }

        public bool CanMassApply(object o)
        {
            return SelectedProfilesTreeNode != null
                && MassApplySeasonals != null
                && MassApplySeasonals.SelectedItem != null;
        }

        public void MassApply(object o)
        {
            //Using Flatten to exclude the top node and get the children only
            SelectedProfilesTreeNode.GetLeafs(SelectedProfilesTreeNode).Do(n => n.Seasonals.SelectedItem = n.Seasonals.Items.First(i => i.Idx == MassApplySeasonals.SelectedItem.Idx));
        }

        public bool CanSave(object o)
        {
            return ProfilesTree != null 
                && ProfilesTree.Any()
                && ProfilesTree[0].GetLeafs(ProfilesTree[0]).Where(l => l.HasChanged).Any();
        }

        public void Save(object o)
        {
            var leafSeasonalPairings = ProfilesTree[0].GetLeafs(ProfilesTree[0]).Where(l => l.HasChanged).Select(l => new Tuple<string, string>(l.Idx, l.Seasonals.SelectedItem.Idx)).ToDictionary(l => l.Item1, l => l.Item2);
            var xml = CommonXml.GetBaseSaveArguments();

            var listingsXml = ListingConverter.ToListingXml(CreationFilters.ListingsVM.CustomerIDsList.ToHashSet(), CreationFilters.ListingsVM.ProductIDsList.Where(p => leafSeasonalPairings.ContainsKey(p)).ToHashSet(), ListingsAccess.GetListings());
            listingsXml.Elements().Do(e =>
            {                
                var sku = e.Attribute("Sku_Idx").MaybeValue();
                if (sku != null)
                {
                    e.SetAttributeValue("Seasonal_Idx", leafSeasonalPairings[sku]);
                }
            });

            xml.AddElement("Forecast_Idx", CreationFilters.OtherFiltersVM.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("forecast")).SelectedItem.Item_Idx);
            xml.Add(listingsXml);

            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.Seasonals.SaveListingSeasonals, xml))
                LoadTreeDropdowns(SeasonalDropdownXml);
        }

        private void LoadProfilesTree()
        {
            IsLoading = true;

            ListingsAccess.GetFilterProducts(false, true).ContinueWith(t =>
            {
                var baseProductTree = t.Result;
                var baseleafIdxs = baseProductTree.GetLeafs(baseProductTree).Select(l => l.Idx);

                var selectedSkuIdxs = CreationFilters.ListingsVM.ProductIDsList.ToHashSet();

                var leafsToLoseIdxs = baseleafIdxs.Where(l => !selectedSkuIdxs.Contains(l)).ToHashSet();

                var trimmedBaseTree = TreeViewHierarchy.ConvertListToTree(TreeViewHierarchy.GetFlatTree(baseProductTree).Where(p => !leafsToLoseIdxs.Contains(p.Idx)).ToList());

                TreeViewHierarchy.RemoveChildlessParents(trimmedBaseTree);
                trimmedBaseTree = TreeViewHierarchy.RemoveSingularParents(trimmedBaseTree);

                var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, SeasonalTreeNode>());
                var mapper = config.CreateMapper();

                var finalTree = SeasonalTreeNode.ConvertListToTree(TreeViewHierarchy.GetFlatTree(trimmedBaseTree).Select(n => mapper.Map<SeasonalTreeNode>(n)).ToList());
                finalTree.GetLeafs(finalTree).Do(l => ((SeasonalTreeNode)l).ShowSeasonals = true);

                ProfilesTree = new List<SeasonalTreeNode> { finalTree };

                LoadTreeDropdowns(SeasonalDropdownXml);
            });
        }

        private void LoadTreeDropdowns(XElement dropdownXml)
        {
            IsLoading = true;

            MassApplySeasonals.SetItems(dropdownXml);
            ProfilesTree[0].GetLeafs(ProfilesTree[0]).Do(n => ((SeasonalTreeNode)n).Seasonals.SetItems(dropdownXml));
            ProfilesTree[0].ExpandAll();

            LoadListingsSeasonal();

        }

        private void LoadListingsSeasonal()
        {
            var xml = CommonXml.GetBaseArguments("GetData");
            var listingsXml = ListingConverter.ToListingXml(CreationFilters.ListingsVM.CustomerIDsList.ToHashSet(), CreationFilters.ListingsVM.ProductIDsList.ToHashSet(), ListingsAccess.GetListings());
            xml.AddElement("Forecast_Idx", CreationFilters.OtherFiltersVM.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("forecast")).SelectedItem.Item_Idx);
            xml.Add(listingsXml);

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Seasonals.GetListingSeasonals, xml).ContinueWith(r =>
            {
                var leafs = ProfilesTree[0].GetLeafs(ProfilesTree[0]);
                r.Result.Elements("Product").Do(s =>
                {
                    var skuIdx = s.Attribute("Sku_Idx").MaybeValue();
                    var seasonalIdx = s.Attribute("Seasonal_Idx").MaybeValue();
                    var node = leafs.First(l => l.Idx == skuIdx);
                    if (seasonalIdx == "-1")
                    {
                        node.Seasonals.Items.Add(new ComboboxItem { Idx = "-1", Name = "Multiple", IsEnabled = true });
                        node.Seasonals.SelectedItem = node.Seasonals.Items.Last();
                    }
                    else if (seasonalIdx == "-2")
                    {
                        node.Seasonals.Items.Add(new ComboboxItem { Idx = "-2", Name = "Internal", IsEnabled = true });
                        node.Seasonals.SelectedItem = node.Seasonals.Items.Last();
                    }
                    else if (seasonalIdx != null)
                        leafs.First(l => l.Idx == skuIdx).Seasonals.SetSelection(seasonalIdx);

                    if (seasonalIdx != null)
                        node.OriginalSelectionIdx = node.Seasonals.SelectedItem.Idx;
                });
            });

            IsLoading = false;
        }

        #endregion

        #region Loading Properties 

        public bool NoData
        {
            get
            {
                return ProfilesTree == null || !ProfilesTree.Any();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged(this, vm => vm.IsLoading);
                NotifyPropertyChanged(this, vm => vm.NoDataMessage);
                NotifyPropertyChanged(this, vm => vm.NoData);
            }
        }

        public bool NoDataMessage { get { return NoData && !IsLoading; } }

        #endregion
    }

    public class SeasonalTreeNode : TreeViewHierarchy
    {
        public SeasonalTreeNode()
        {
            Seasonals.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "SelectedItem")
                    OnPropertyChanged("HasChanged");
            };
        }

        public bool ShowSeasonals { get; set; }

        private SingleSelectViewModel _seasonals = new SingleSelectViewModel();
        public SingleSelectViewModel Seasonals
        {
            get { return _seasonals; }
            set
            {
                _seasonals = value;
            }
        }

        private string _originalSelectionIdx;
        public string OriginalSelectionIdx
        {
            get { return _originalSelectionIdx; }
            set
            {
                _originalSelectionIdx = value;
                OnPropertyChanged("HasChanged");
            }
        }

        public bool HasChanged { get { return Seasonals != null && Seasonals.SelectedItem != null && Seasonals.SelectedItem.Idx != OriginalSelectionIdx; } }

        public static SeasonalTreeNode ConvertListToTree(List<SeasonalTreeNode> list)
        {
            var hashedList = list.ToLookup(l => l.ParentIdx);
            list.ForEach(item =>
            {
                item.Children.Clear();
                item.Children.AddRange(hashedList[item.Idx].ToList());
                item.Children.Do(c => c.Parent = item);

            });

            var rootNode = list.First(t => string.IsNullOrEmpty(t.ParentIdx)) ?? list.First(t => t.Parent == null);
            return rootNode;
        }



    }

    /* This is for the seasonal radio buttons */
    public class SelectableItem : INotifyPropertyChanged
    {
        public string ItemDescription { get; set; }

        public bool IsSelected { get; set; }

        public string Idx { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
