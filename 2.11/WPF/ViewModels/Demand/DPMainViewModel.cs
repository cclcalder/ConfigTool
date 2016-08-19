using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Demand;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Filters.ViewModels;
using Exceedra.TreeGrid.ViewModels;
using Exceedra.TreeGrid.Models;
using AutoMapper;
using Model.Entity.Listings;
using QFXModels;

namespace WPF.ViewModels.Demand
{
    public class DPMainViewModel : ViewModelBase
    {
        private readonly DemandAccess _demandAccess = new DemandAccess();

        public DPMainViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            LoadFilters();
            LoadNewModelTypes();
        }

        private void LoadFilters()
        {
            FiltersVM = new FilterViewModel
            {
                UseListingsGroups = false,
                ApplyFilter = ApplyFilter,
                NoSingleTree = true,
                CurrentScreenKey = ScreenKeys.DPSQL,
                IsUsingOtherFilters = true,
            };

            FiltersVM.DataLoaded += SetupDropdownChanges;

            FiltersVM.Load();
        }

        private void SetupDropdownChanges()
        {
            TrialForecastDropdown.PropertyChanged += TrialForecastDropdown_PropertyChanged;
            if (TrialForecastDropdown.SelectedItem != null)
                ConfigureVisibleListings(TrialForecastDropdown.SelectedItem.Item_Idx);

            if (FiltersVM.CanApplyFilter(null))
                FiltersVM.ApplyFilter();
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
            FiltersVM.ListingsVM.SetVisibleCustSkus(listings.GetCustomerIdxs(), listings.GetSkuIdxs(), clearSelection);
            if (!clearSelection)
                FiltersVM.ListingsVM.SetSelections(listings);
        }

        private FilterViewModel _filterVM;
        public FilterViewModel FiltersVM
        {
            get { return _filterVM; }
            set
            {
                _filterVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }

        #region Loaders

        public void LoadCurrentModelData(DemandFilterObject inputs)
        {
            CurrentModel.IsLoading = true;
            var args = DemandAccess.DfoToXElement(inputs);
            args.Element("Products").Elements().Remove();
            args.Element("Products").AddElement("Idx", SelectedSku.Idx);
            /* Add in leafs so the db can determine the appropriate seasonal for a parent */
            var leafs = new XElement("LeafNodes");
            SelectedSku.GetLeafs().Do(l => leafs.AddElement("Idx", l.Idx));
            args.Add(leafs);
            DynamicDataAccess.GetGenericItemAsync<RowViewModel>(StoredProcedure.Demand.GetDataModel, args).ContinueWith(t =>
            {
                CurrentModel = t.Result;
            });
        }

        public void LoadNewModelTypes()
        {
            var tmp = _demandAccess.LoadNewModelTypes();
            if (tmp != null)
            {
                ModelTypes = tmp;
            }
        }

        #endregion

        #region Helper Properties

        public RowProperty TrialForecastDropdown
        {
            get
            {
                return FiltersVM.OtherFiltersVM.Records[0].Properties.First(p => p.ColumnCode.ToLower().Contains("forecast"));
            }
        }

        #endregion

        /* Load all the seasonals for the loaded products so when we forecast we have the data ready */
        private void LoadSeasonalsCache()
        {
            SeasonalsCache = new Dictionary<string, IEnumerable<double>>();
            var nodes = BaselineTreeGrid.GetFlatTree();
            var seasonalIdxs = nodes.Select(n => n.AdditionalIdx).Distinct().Where(s => s != null && s != "-1");
            seasonalIdxs.Do(s =>
            {
                DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Demand.GetSeasonals, XElement.Parse("<GetSeasonal><Seasonal_Idx>" + s + "</Seasonal_Idx></GetSeasonal>"), false).ContinueWith(t =>
                {
                    SeasonalsCache.Add(s, t.Result.Element("Pattern").Elements().Select(e => e.Element("Value").Value.AsNumericDouble()));
                });
            });
        }

        private Dictionary<string, IEnumerable<double>> SeasonalsCache;

        #region TreeGrids

        private TreeGridViewModel _actFcTreeGrid = new TreeGridViewModel();
        public TreeGridViewModel ActFcTreeGrid
        {
            get { return _actFcTreeGrid; }
            set
            {
                _actFcTreeGrid = value;

                ActFcTreeGrid.PropertyChanged += ActFcTreeGrid_PropertyChanged;

                ActFcTreeGrid.LoadedNodes[0].GetLeafs().Do(n =>
                {
                    n.CalculatedChanged += Act_ValueChanged;
                });

                ActFcTreeGrid.MainColumn = "Products";
                ActFcTreeGrid.SecondColumn = "Measures";
                ActFcTreeGrid.SaveCommand = SaveForecastCommand;
                SetOutlierChangedListeners(ActFcTreeGrid, true);
                NotifyPropertyChanged(this, vm => vm.ActFcTreeGrid);
            }
        }

        private void ActFcTreeGrid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode" && ((TreeGridViewModel)sender).SelectedNode != null)
            {
                if (((TreeGridViewModel)sender).SelectedNode != null && (SelectedSku == null || SelectedSku.Idx != ((TreeGridViewModel)sender).SelectedNode.Idx))
                    SelectedSku = ((TreeGridViewModel)sender).SelectedNode;

                if (ActFcTreeGrid.LoadedNodes.Any() && (ActFcTreeGrid.SelectedNode == null || ActFcTreeGrid.SelectedNode.Idx != SelectedSku.Idx))
                {
                    ActFcTreeGrid.SelectedNode = ActFcTreeGrid.LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n)).FirstOrDefault(n => n.Idx == SelectedSku.Idx);
                }
                if (BaselineTreeGrid.LoadedNodes.Any() && (BaselineTreeGrid.SelectedNode == null || BaselineTreeGrid.SelectedNode.Idx != SelectedSku.Idx))
                {
                    BaselineTreeGrid.SelectedNode = BaselineTreeGrid.LoadedNodes.SelectMany(n => TreeViewHierarchy.GetFlatTree(n)).FirstOrDefault(n => n.Idx == SelectedSku.Idx);
                }
            }
        }

        private TreeGridNode _selectedSku;
        public TreeGridNode SelectedSku
        {
            get { return _selectedSku; }
            set
            {
                _selectedSku = value;
                LoadCurrentModelData(BaseDFO);
                NotifyPropertyChanged(this, vm => vm.SelectedSku);
            }
        }

        private TreeGridViewModel _baselineTreeGrid = new TreeGridViewModel();
        public TreeGridViewModel BaselineTreeGrid
        {
            get { return _baselineTreeGrid; }
            set
            {
                _baselineTreeGrid = value;

                LoadSeasonalsCache();

                BaselineTreeGrid.PropertyChanged += ActFcTreeGrid_PropertyChanged;

                BaselineTreeGrid.LoadedNodes[0].GetLeafs().Do(n =>
                 {
                     n.CalculatedChanged += Baseline_ValueChanged;
                 });

                BaselineTreeGrid.MainColumn = "Products";
                BaselineTreeGrid.SecondColumn = "Measures";
                BaselineTreeGrid.SaveCommand = SaveForecastCommand;
                SetOutlierChangedListeners(BaselineTreeGrid, false);
                NotifyPropertyChanged(this, vm => vm.BaselineTreeGrid);
            }
        }

        /* recordIdx, columnIdx, treegridnode */
        private void Baseline_ValueChanged(Tuple<Tuple<string, string>, TreeGridNode> changedObject)
        {
            var newValue = changedObject.Item2.Data.GetProperty(changedObject.Item1.Item1, changedObject.Item1.Item2).Value.AsNumericInt();

            var pairedProp = ActFcTreeGrid.GetNode(changedObject.Item2.Idx).Data.GetProperty(changedObject.Item1.Item1, changedObject.Item1.Item2);
            if (!pairedProp.IsEditable || pairedProp.ControlType.ToLower() == "labelwithcheckbox") return;

            pairedProp.LeftRightLocked = true;
            var weekIdx = pairedProp.ColumnCode.Substring(4).ToString().AsNumericInt();
            var seasonalValue = SeasonalsCache[changedObject.Item2.AdditionalIdx].Skip(weekIdx - 1).First();
            pairedProp.Value = (newValue * seasonalValue).ToString();
            pairedProp.LeftRightLocked = false;
        }

        private void Act_ValueChanged(Tuple<Tuple<string, string>, TreeGridNode> changedObject)
        {
            var newValue = changedObject.Item2.Data.GetProperty(changedObject.Item1.Item1, changedObject.Item1.Item2).Value.AsNumericInt();

            var pairedProp = BaselineTreeGrid.GetNode(changedObject.Item2.Idx).Data.GetProperty(changedObject.Item1.Item1, changedObject.Item1.Item2);
            if (!pairedProp.IsEditable || pairedProp.ControlType.ToLower() == "labelwithcheckbox") return;

            pairedProp.LeftRightLocked = true;
            var weekIdx = pairedProp.ColumnCode.Substring(4).AsNumericInt();
            var seasonalValue = SeasonalsCache[changedObject.Item2.AdditionalIdx].Skip(weekIdx - 1).First();
            pairedProp.Value = (seasonalValue == 0 ? 0 : (newValue / seasonalValue)).ToString();
            pairedProp.LeftRightLocked = false;
        }

        public TreeGridNode GetProductsAsTreeGridNodes()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
            var mapper = config.CreateMapper();

            var skus = ListingsAccess.GetFilterProducts(false, true).Result.FlatTree;

            var skusAsTreeGrid = skus.Select(n => mapper.Map<TreeGridNode>(n));

            return TreeGridNode.ConvertListToTree(skusAsTreeGrid.ToList());
        }

        #region Conversions to F# inputs

        private IEnumerable<Actual> GetBaselineActuals()
        {
            return BaselineTreeGrid.GetNode(SelectedSku.Idx).Data.GetRecordByType("DEWEIGHT_ACTUALS").Properties.Where(p => p.Value.IsNumeric() && !(p.ControlType.ToLower() == "treegridcell" && p.IsEditable)).Select(p => new Actual(p.ColumnCode.AsNumericInt(), p.Value.AsNumericDouble(), p.Value2.ToLower() == "true"));
        }

        private IEnumerable<ForecastParameter> GetParamerters()
        {
            return NewModelParameters.Records.First().Properties.Select(p => new ForecastParameter { name = p.ColumnCode, value = p.Value });
        }

        private IEnumerable<ForecastParameter> GetCurrentModel()
        {
            return CurrentModel.Records.First().Properties.Select(p => new ForecastParameter { name = p.ColumnCode, value = p.Value });
        }

        private IEnumerable<double> GetSeasonals(string idx)
        {
            var xml = DynamicDataAccess.GetDynamicData(StoredProcedure.Demand.GetSeasonals, XElement.Parse("<GetSeasonal><Seasonal_Idx>" + idx + "</Seasonal_Idx></GetSeasonal>"), false);
            return xml.Element("Pattern").Elements().Select(e => e.Element("Value").Value.AsNumericDouble());
        }

        private string GetCurrentSeasonalProfileIdx()
        {
            var seasonalIdx = CurrentModel.Records.First().Properties.First(p => p.ColumnCode == "Seasonal_Idx").Value;
            return string.IsNullOrEmpty(seasonalIdx) ? "1" : seasonalIdx;
        }

        #endregion

        #endregion

        #region Dynamic Grid

        /* TreeCode$NodeIdx$RecordIdx$ColumnCode */
        private void SetOutlierChangedListeners(TreeGridViewModel tgvm, bool IsActTree)
        {
            if (tgvm.LoadedNodes != null)
                tgvm.GetFlatTree().Do(n => n.Data.Records.Do(rec => rec.Properties.Where(p => p.Value2 != null).Do(prop =>
                {
                    prop.UpdateToCell = (IsActTree ? "Base" : "Act") + "$" + n.Idx + "$" + rec.Item_Idx + "$" + prop.ColumnCode;
                    prop.PropertyChanged += Outlier_PropertyChanged;
                })));
        }

        private void Outlier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cell = (Property)sender;

            if (e.PropertyName == "Value2")
            {
                UpdateOutliers(cell, cell.Value2);
            }
        }

        private void UpdateOutliers(Property changedCell, string newValue)
        {
            var cellToChange = GetCellToChange(changedCell.UpdateToCell);

            UpdateCell(cellToChange, newValue);
        }

        private Property GetCellToChange(string updateToCell)
        {
            var updateProps = updateToCell.Split('$');
            var treeCode = updateProps[0];
            var nodeCode = updateProps[1];
            var recordCode = updateProps[2];
            var columnCode = updateProps[3];

            var tree = treeCode == "Act" ? ActFcTreeGrid : BaselineTreeGrid;
            var node = tree.GetNode(nodeCode);
            var record = node.Data.GetRecord(recordCode);
            var property = record.GetProperty(columnCode);

            return property;
        }

        private void UpdateCell(Property cellToChange, string newValue)
        {
            if (cellToChange != null)
            {
                if (cellToChange.Value2.ToLower() != newValue.ToLower())
                    cellToChange.Value2 = newValue;
            }
        }

        #endregion

        #region Dynamic Rows

        private RowViewModel _newModelParameters;

        public RowViewModel NewModelParameters
        {
            get { return _newModelParameters; }
            set
            {
                _newModelParameters = value;
                NotifyPropertyChanged(this, vm => vm.NewModelParameters);
            }
        }

        private RowViewModel _currentModel = new RowViewModel();

        public RowViewModel CurrentModel
        {
            get { return _currentModel; }
            set
            {
                _currentModel = value;
                NotifyPropertyChanged(this, vm => vm.CurrentModel);
            }
        }

        #endregion

        #region Comboboxes

        private List<ModelType> _modelTypes;

        public List<ModelType> ModelTypes
        {
            get { return _modelTypes; }
            set
            {
                _modelTypes = value;
                if (ModelTypes != null && ModelTypes.Any())
                    SelectedModelType = ModelTypes.FirstOrDefault(model => model.IsSelected) ?? ModelTypes[0];
                NotifyPropertyChanged(this, vm => vm.ModelTypes);
            }
        }

        private ModelType _selectedModelTypes;

        public ModelType SelectedModelType
        {
            get { return _selectedModelTypes; }
            set
            {
                _selectedModelTypes = value;
                NewModelParameters = new RowViewModel(SelectedModelType.Parameters);

                NotifyPropertyChanged(this, vm => vm.SelectedModelType);
            }
        }

        #endregion

        #region Checkboxes

        private bool _excludeFromBulkForecast;

        public bool ExcludeFromBulkForecast
        {
            get { return _excludeFromBulkForecast; }
            set
            {
                _excludeFromBulkForecast = value;
                NotifyPropertyChanged(this, vm => vm.ExcludeFromBulkForecast);
            }
        }

        private bool _baselineExcludeFromBulkForecast;

        public bool BaselineExcludeFromBulkForecast
        {
            get { return _baselineExcludeFromBulkForecast; }
            set
            {
                _baselineExcludeFromBulkForecast = value;
                NotifyPropertyChanged(this, vm => vm.BaselineExcludeFromBulkForecast);
            }
        }

        #endregion

        #region Filter Commands

        #region Executes

        private void ApplyFilter()
        {
            //Store the applied filters
            BaseDFO = new DemandFilterObject
            {
                CustomerIdxs = FiltersVM.ListingsVM.CustomerIDsList,
                ProductIdxs = FiltersVM.ListingsVM.ProductIDsList,
                DateRange = new FilterDateRange(FiltersVM.StartDate, FiltersVM.EndDate),
                TrialIdx = TrialForecastDropdown.SelectedItem.Item_Idx,
                TabIdx = "ActFc",
            };


            LoadActFcTreeGrid();
            LoadBaselineTreeGrid();
             
        }

        public void LoadActFcTreeGrid()
        {
            ActFcTreeGrid.IsLoading = true;
            var args = DemandAccess.DfoToXElement(BaseDFO);

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Demand.GetVolumesTreeGrid, args).ContinueWith(t =>
            {
                ActFcTreeGrid = new TreeGridViewModel(t.Result, GetProductsAsTreeGridNodes());
            });
        }
        public void LoadBaselineTreeGrid()
        {
            BaselineTreeGrid.IsLoading = true;
            var args = DemandAccess.DfoToXElement(BaseDFO);
            args.Element("Tab_Idx").Value = "Baseline";

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Demand.GetVolumesTreeGrid, args).ContinueWith(t =>
            {
                BaselineTreeGrid = new TreeGridViewModel(t.Result, GetProductsAsTreeGridNodes());
            });
        }


        public DemandFilterObject BaseDFO { get; set; }

        #endregion

        #endregion

        #region Act/Fc+Baseline Commands

        #region Constructors

        public ICommand SaveForecastCommand
        {
            get { return new ViewCommand(SaveForecast); }
        }

        public ICommand SaveSettingsCommand
        {
            get { return new ViewCommand(SaveSettings); }
        }

        public ICommand CalibrateModelCommand
        {
            get { return new ViewCommand(CanCalibrateCalculate, CalibrateModel); }
        }

        public ICommand CalculateForecastCommand
        {
            get { return new ViewCommand(CanCalibrateCalculate, CalculateForecast); }
        }

        #endregion

        #region CanExcutes

        private bool CanCalibrateCalculate(object parameters)
        {
            return SelectedModelType != null
                && SelectedSku != null
                && SelectedSku.AdditionalIdx != null
                && SeasonalsCache != null
                && SeasonalsCache.ContainsKey(SelectedSku.AdditionalIdx);

        }

        #endregion

        #region Excecutes

        private void SaveForecast(object obj)
        {
            var saveXml = GetSaveXml();

            if (_demandAccess.SaveForecast(saveXml))
            {
                ActFcTreeGrid.IsLoading = true;
                BaselineTreeGrid.IsLoading = true;

                BaselineTreeGrid.GetFlatLeafs().Select(l => l.Data).Do(d =>
                {
                    var forecastRow = d.Records.First(r => r.Item_Name == "Saved Forecast");
                    var newForecastRow = d.Records.First(r => r.Item_Name.Contains("Actuals"));
                    newForecastRow.Properties.Where(p => p.IsEditable && p.ControlType.ToLower() == "treegridcell").Do(p => forecastRow.GetProperty(p.ColumnCode).SetValue(p.Value));
                });

                ActFcTreeGrid.GetFlatLeafs().Select(l => l.Data).Do(d =>
                {
                    var forecastRow = d.Records.First(r => r.Item_Name == "Saved Forecast");
                    var newForecastRow = d.Records.First(r => r.Item_Name.Contains("Actuals"));
                    newForecastRow.Properties.Where(p => p.IsEditable && p.ControlType.ToLower() == "treegridcell").Do(p => forecastRow.GetProperty(p.ColumnCode).SetValue(p.Value));
                });

                ActFcTreeGrid.CleanseTree();
                BaselineTreeGrid.CleanseTree();

                ActFcTreeGrid.IsLoading = false;
                BaselineTreeGrid.IsLoading = false;
            }
        }

        private void SaveSettings(object obj)
        {
            _demandAccess.SaveSettings(ExcludeFromBulkForecast);
        }

        private void CalibrateModel(object obj)
        {
            var url = App.Configuration.DemandCalibrateModelUrl;
            var code = url != null ? url.Split('/').Last() : "Calibrate";

            CalibrateCalculate(code);
        }

        private void CalculateForecast(object obj)
        {
            var url = App.Configuration.DemandCalculateForecastUrl;
            var code = url != null ? url.Split('/').Last() : "Forecast";

            CalibrateCalculate(code);
        }

        private void CalibrateCalculate(string code)
        {
            var actuals = GetBaselineActuals();

            if(!actuals.Any())
            {
                MessageBoxShow("The Listing you have selected has no associated Historic Data", code + " Cancelled", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Stop);
                return;
            }

            var tmp = _demandAccess.CalibrateCalculate(GetCurrentModel(), GetParamerters(), SelectedModelType.Idx, code, actuals, GetSeasonals(GetCurrentSeasonalProfileIdx()));
            if (tmp != null)
            {
                var currentNode = BaselineTreeGrid.GetNode(SelectedSku.Idx);
                var props = currentNode.Data.Records.First(r => r.Item_Name.Contains("Actuals")).Properties.Where(p => p.IsEditable && p.ControlType.ToLower() == "treegridcell");
                tmp.NewForecast.Do(f =>
                {                    
                    var matchedProp = props.FirstOrDefault(p => p.ColumnCode == f.Date.ToString());
                    if(matchedProp != null)
                        matchedProp.Value = f.Value.ToString();
                });

                var last = tmp.NewForecast.Last();

                NewModelParameters = new RowViewModel { Records = new ObservableCollection<RowRecord> { new RowRecord { Properties = new ObservableCollection<RowProperty>(tmp.Parameters.Select(p => new RowProperty { ColumnCode = p.name, HeaderText = p.name, Value = p.value, IsDisplayed = true, IsEditable = (p.name != "RSquared"), ControlType = "textbox" })) } } };

                /* If we already have a pairing for this sku, remove it */
                var matchingPair = NewParameterPairing.FirstOrDefault(n => n.Item1 == currentNode.Idx);
                if (matchingPair != null) NewParameterPairing.Remove(matchingPair);
                NewParameterPairing.Add(new Tuple<string, IEnumerable<ForecastParameter>, Tuple<string, string>>(SelectedSku.Idx, tmp.Parameters, new Tuple<string, string>(SelectedModelType.Idx, SelectedModelType.Name)));
            }
        }

        /* SkuIdx, Parameters, Code&Name */
        private List<Tuple<string, IEnumerable<ForecastParameter>, Tuple<string, string>>> NewParameterPairing = new List<Tuple<string, IEnumerable<ForecastParameter>, Tuple<string, string>>>();

        #endregion

        #endregion


        #region Webservice Response Parsing

        private void ParseResponse(XElement response)
        {
            var parameters = response.Element("Parameters");
            var tabs = response.Descendants("Tab");

            if (parameters != null)
                NewModelParameters = new RowViewModel(parameters);

            foreach (var tab in tabs)
            {
                tab.Element("Grid").Elements().First().Elements("RootItem").Where(r => r.Element("Item_Type").MaybeValue() == "System Outlier").Do(r => r.Element("Item_IsDisplayed").Value = "1");
                switch (tab.Element("Tab_Idx").MaybeValue())
                {
                    case "Baseline":
                        var newForecast = new RecordViewModel(tab.Element("Grid")).Records.First(r => r.Item_Type == "New Trial Forecast");
                        BaselineTreeGrid.GetNode(SelectedSku.Idx).Data.Records.First(r => r.Item_Name == "New Forecast").Properties.Skip(1).Do(p => p.Value = newForecast.GetProperty(p.ColumnCode).Value);
                        break;

                }
            }

        }

        #endregion

        private XElement GetListingsXml(string rootTag = "Listings")
        {
            return ListingConverter.ToListingXml(BaseDFO.CustomerIdxs.ToHashSet(), BaseDFO.ProductIdxs.ToHashSet(), ListingsAccess.GetListings(), rootTag);
        }

        public XElement GetSaveXml()
        {
            var changedValues = ActFcTreeGrid.GetLowestLevelValueOrValue2Changes();
            
            var data = CommonXml.GetBaseSaveArguments();
            data.AddElement("Forecast_Idx", BaseDFO.TrialIdx);
            data.Add(GetListingsXml("ForecastListings"));

            var forecastData = new XElement("ForecastData");

            changedValues.Do(c =>
            {
                var newForecastRow = new XElement("Row");
                newForecastRow.SetAttributeValue("Parent_Idx", c.Item1);
                newForecastRow.SetAttributeValue("Code", "New_Forecast");//c.Item2.Item_Type

                var actualsRow = new XElement("Row");
                actualsRow.SetAttributeValue("Parent_Idx", c.Item1);
                actualsRow.SetAttributeValue("Code", "Actuals");

                var newForecastColumns = new XElement("Columns");
                var actualsColumns = new XElement("Columns");
                c.Item2.Properties.Do(p =>
                {
                    var column = new XElement("Column");
                    column.SetAttributeValue("Idx", p.IDX);
                    if (p.HasChanged)
                    {
                        column.SetAttributeValue("Value_New", RecordViewModel.FixValue(p.Value));
                        column.SetAttributeValue("Value_Original", RecordViewModel.FixValue(p.OriginalValue));
                    }
                    if (p.HasValue2Changed)
                    {
                        column.SetAttributeValue("IsOutlier_New", p.Value2.ToLower() == "true" ? "1" : "0");
                        column.SetAttributeValue("IsOutlier_Original", p.OriginalValue2.ToLower() == "true" ? "1" : "0");
                    }

                    if (p.HasChanged)
                        newForecastColumns.Add(column);

                    if (p.HasValue2Changed)
                        actualsColumns.Add(column);
                });
                newForecastRow.Add(newForecastColumns);
                actualsRow.Add(actualsColumns);

                var newModel = NewParameterPairing.FirstOrDefault(n => n.Item1 == c.Item1);

                var model = new XElement("ForecastModel");

                if (newModel != null)
                {
                    model.SetAttributeValue("Code", newModel.Item3.Item1);
                    model.SetAttributeValue("Name", newModel.Item3.Item2);
                    var parameters = new XElement("Parameters");

                    newModel.Item2.Do(p =>
                    {
                        var parameter = new XElement("Parameter");
                        parameter.SetAttributeValue("Name", p.name);
                        parameter.SetAttributeValue("Value", p.value);
                        parameters.Add(parameter);
                    });
                    model.Add(parameters);
                }


                var grid = new XElement("Grid");
                newForecastRow.Add(model);

                grid.Add(newForecastRow);
                grid.Add(actualsRow);
                forecastData.Add(grid);

            });

            data.Add(forecastData);


            return data;
        }
    }

}