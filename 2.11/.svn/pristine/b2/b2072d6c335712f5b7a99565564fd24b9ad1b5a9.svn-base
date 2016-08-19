using System.Xml.Linq;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Generic;
using Exceedra.Controls.DynamicGrid.ViewModels;

namespace ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Model;
    using Model.DataAccess;
    using ViewHelper;
    using Exceedra.Common;
    using Exceedra.TreeGrid.Models;
    using AutoMapper;
    using Model.Entity.Listings;
    using Exceedra.TreeGrid.ViewModels;
    using Exceedra.SingleSelectCombo.ViewModel;
    using WPF.UserControls.Filters.ViewModels;
    public class PlanningViewModel : ViewModelBase
    {
        # region Constructor

        public static PlanningViewModel New()
        {
            var instance = new PlanningViewModel();
            instance.Init();

            return instance;
        }

        private void Init()
        {
            FiltersVM = new FilterViewModel
            {
                ApplyFilter = ApplyFilter,
                CurrentScreenKey = ScreenKeys.PLANNING,
                StatusTreeProc = StoredProcedure.GetPlanningMeasures,
                IsUsingPlanningFilters = true
            };

            SkuGridDropDownVM.PropertyChanged += _skuGridDropDownVM_PropertyChanged;

            FiltersVM.Load();
        }


        private FilterViewModel _filtersVM;
        public FilterViewModel FiltersVM
        {
            get { return _filtersVM; }
            set
            {
                _filtersVM = value;
                NotifyPropertyChanged(this, vm => vm.FiltersVM);
            }
        }


        private XElement GetFilterXml()
        {
            return PlanningAccess.GetPlanningDataInputXml(GetFilterArgs());
        }

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                if (SelectedTabIndex == value) return;

                _selectedTabIndex = value;
                NotifyPropertyChanged(this, vm => vm.SelectedTabIndex);
            }
        }

        # endregion

        # region Commands

        private bool _isApplying = false;
        public void ApplyFilter()
        {
            _isApplying = true;

            /* Because the db is not sending back the measure names, we set a static property */
            RecordViewModel.Measures = FiltersVM.StatusTreeVM.GetFlatTree()
                    .GroupBy(m => m.Idx)
                    .ToDictionary(d => d.Key, d => d.First().Name);

            if (FiltersVM.Heirarchies.SelectedItem.Idx == "0")
            {
                ParsePlanningDataIntoTreeGrid();
            }
            else
            {
                var selectedLeafs = FiltersVM.ListingsVM.SelectedProducts.Where(p => !p.HadChildrenInitially).Select(p => new ComboboxItem { Idx = p.Idx, Name = p.Name, IsEnabled = true }).ToList();
                selectedLeafs.First().IsSelected = true;
                SkuGridDropDownVM.SetItems(selectedLeafs);
            }

        }


        private void ParsePlanningDataIntoTreeGrid()
        {
            SelectedTabIndex = 0;
            NewTreeGrid.IsLoading = true;

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.GetPlanningData, GetFilterXml()).ContinueWith(t =>
            {
                var res = t.Result;

                XElement metaData = null;
                if (res.ToString().Contains("(c)"))
                {
                    var currentName = res.Element("Row").Element("Columns").Elements().First(e => e.Attribute("Name").MaybeValue() != null && e.Attribute("Name").MaybeValue().Contains("(c)")).Attribute("Name").Value;
                    var firstName = res.Element("Row").Element("Columns").Elements().First(e => e.Attribute("Name").MaybeValue() != null).Attribute("Name").Value;
                    metaData = new XElement("MetaData");
                    metaData.AddElement("ChartLine", currentName);
                    var markedZone = new XElement("MarkedArea");
                    markedZone.SetAttributeValue("HorizontalFrom", firstName);
                    markedZone.SetAttributeValue("HorizontalTo", currentName);
                    markedZone.SetAttributeValue("Fill", "#43262626");
                    metaData.Add(markedZone);
                }

                var nodes = t.Result.Elements().GroupBy(e => e.Element("Idx").MaybeValue());

                //Turn each node into a treegridnode

                var treeNodes = nodes.Select(n => new TreeGridNode { Idx = n.Key, ParentIdx = n.Key, Data = ConvertPlanningNodeToDynGrid(n.Select(g => g)) });

                var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
                var mapper = config.CreateMapper();

                var skus = ListingsAccess.GetFilterProducts(false, true).Result.FlatTree;

                var skusAsTreeGrid = skus.Select(n => mapper.Map<TreeGridNode>(n));

                var skusHierarchy = TreeGridNode.ConvertListToTree(skusAsTreeGrid.ToList());

                var leafIdxs = skusHierarchy.GetLeafs().Select(l => l.Idx);

                NewTreeGrid = new TreeGridViewModel(treeNodes.Where(n => leafIdxs.Contains(n.Idx)), skusHierarchy, new XElement("Planning", nodes.First().Select(g => g)), metaData);

                /* Temp code until the db can configure the correct nodes itself */
                if (NewTreeGrid.LoadedNodes[0].Data.RemovedRecords.Any() && _isApplying)
                {
                    var message = "The following measures have been removed as their calculations contain unloaded measures: " + Environment.NewLine;
                    NewTreeGrid.LoadedNodes[0].Data.RemovedRecords.Do(r => message += r.Item_Name + Environment.NewLine);
                    MessageBoxShow(message, "Removed Measures");
                }
                _isApplying = false;
            });



        }

        private TreeGridViewModel _newTreeGrid = new TreeGridViewModel();
        public TreeGridViewModel NewTreeGrid
        {
            get { return _newTreeGrid; }
            set
            {
                _newTreeGrid = value;
                NewTreeGrid.SaveCommand = SaveCommand;
                NewTreeGrid.MainColumn = "Products";
                NewTreeGrid.SecondColumn = "Measures";
                NotifyPropertyChanged(this, vm => vm.NewTreeGrid);
            }
        }

        private RecordViewModel ConvertPlanningNodeToDynGrid(IEnumerable<XElement> rows)
        {
            var xml = new XElement("Planning", rows);
            return RecordViewModel.LoadWithNewXml(xml);
        }

        private SingleSelectViewModel _skuGridDropDownVM = new SingleSelectViewModel();
        public SingleSelectViewModel SkuGridDropDownVM
        {
            get { return _skuGridDropDownVM; }
        }

        private void _skuGridDropDownVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                ParsePlanningDataIntoCustomerTreeGrid();
            }
        }

        private TreeGridViewModel _newCustomerTreeGrid = new TreeGridViewModel();
        public TreeGridViewModel NewCustomerTreeGrid
        {
            get { return _newCustomerTreeGrid; }
            set
            {
                _newCustomerTreeGrid = value;
                NewCustomerTreeGrid.SaveCommand = SaveCustomerCommand;
                NewCustomerTreeGrid.MainColumn = "Customers";
                NewCustomerTreeGrid.SecondColumn = "Measures";
                NotifyPropertyChanged(this, vm => vm.NewCustomerTreeGrid);
            }
        }

        private void ParsePlanningDataIntoCustomerTreeGrid()
        {
            SelectedTabIndex = 1;
            NewCustomerTreeGrid.IsLoading = true;

            var args = GetFilterXml();
            args.AddElement("Sku_Idx", SkuGridDropDownVM.SelectedItem.Idx);

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.GetPlanningDataCustomers, args).ContinueWith(t =>
            {
                var res = t.Result;

                XElement metaData = null;
                if (res.ToString().Contains("(c)"))
                {
                    var currentName = res.Element("Row").Element("Columns").Elements().First(e => e.Attribute("Name").MaybeValue() != null && e.Attribute("Name").MaybeValue().Contains("(c)")).Attribute("Name").Value;
                    var firstName = res.Element("Row").Element("Columns").Elements().First(e => e.Attribute("Name").MaybeValue() != null).Attribute("Name").Value;
                    metaData = new XElement("MetaData");
                    metaData.AddElement("ChartLine", currentName);
                    var markedZone = new XElement("MarkedArea");
                    markedZone.SetAttributeValue("HorizontalFrom", firstName);
                    markedZone.SetAttributeValue("HorizontalTo", currentName);
                    markedZone.SetAttributeValue("Fill", "#43262626");
                    metaData.Add(markedZone);
                }

                var nodes = t.Result.Elements().GroupBy(e => e.Element("Idx").MaybeValue());

                //Turn each node into a treegridnode

                var treeNodes = nodes.Select(n => new TreeGridNode { Idx = n.Key, ParentIdx = n.Key, Data = ConvertPlanningNodeToDynGrid(n.Select(g => g)) });

                var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
                var mapper = config.CreateMapper();

                var custs = ListingsAccess.GetFilterCustomers(false, true).Result.FlatTree;

                var custsAsTreeGrid = custs.Select(n => mapper.Map<TreeGridNode>(n));

                var custsHierarchy = TreeGridNode.ConvertListToTree(custsAsTreeGrid.ToList());

                var leafIdxs = custsHierarchy.GetLeafs().Select(l => l.Idx);

                NewCustomerTreeGrid = new TreeGridViewModel(treeNodes.Where(n => leafIdxs.Contains(n.Idx)), custsHierarchy, new XElement("Planning", nodes.First().Select(g => g)), metaData);

                /* Temp code until the db can configure the correct nodes itself */
                if (NewCustomerTreeGrid.LoadedNodes[0].Data.RemovedRecords.Any() && _isApplying)
                {
                    var message = "The following measures have been removed as their calculations contain unloaded measures: " + Environment.NewLine;
                    NewTreeGrid.LoadedNodes[0].Data.RemovedRecords.Do(r => message += r.Item_Name + Environment.NewLine);
                    MessageBoxShow(message, "Removed Measures");
                }
                _isApplying = false;
            });


        }

        private PlanningFilterArgDTO GetFilterArgs()
        {
            var filterArg = new PlanningFilterArgDTO
            {
                ProductIDs = FiltersVM.ListingsVM.ProductIDsList,
                CustomerIDs = FiltersVM.ListingsVM.CustomerIDsList,
                MeasureIDs = FiltersVM.StatusTreeVM.GetSelectedIdxs(),
                StartDate = FiltersVM.DateFrom,
                EndDate = FiltersVM.DateTo,
                IntervalIdx = FiltersVM.Intervals.SelectedItem.Idx,
                SelectedTimeRangeIdx = FiltersVM.PredefinedTimeRanges.SelectedItem.Idx,
                SelectedScenarioIdx = FiltersVM.PlanningScenarioList.SelectedItem.Idx,
                SelectedCharts = new List<Series>()
            };

            if (FiltersVM.UseCustomTimeRange)
                filterArg.SelectedTimeRangeIdx = "";

            return filterArg;
        }


        #endregion

        #region Planning Save

        private ViewCommand _saveCommand;
        private ViewCommand SaveCommand
        {
            get { return _saveCommand != null ? _saveCommand : _saveCommand = new ViewCommand(CanSave, Save); }
        }

        private void Save(object obj)
        {
            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.SavePlanningData, GetPlanningSaveXml(NewTreeGrid)))
                NewTreeGrid.CleanseTree();
                //if (FiltersVM.ApplyFilterCommand.CanExecute(null))
                //    FiltersVM.ApplyFilterCommand.Execute(null);
        }

        private bool CanSave(object obj)
        {
            return NewTreeGrid.HasChanges() || NewTreeGrid.HasCommentChanges();
        }

        private ViewCommand _saveCustomerCommand;
        private ViewCommand SaveCustomerCommand
        {
            get { return _saveCommand != null ? _saveCommand : _saveCommand = new ViewCommand(CanSaveCustomer, SaveCustomer); }
        }

        private void SaveCustomer(object obj)
        {
            var custArgs = GetPlanningSaveXml(NewCustomerTreeGrid);
            custArgs.AddElement("Sku_Idx", SkuGridDropDownVM.SelectedItem.Idx);
            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.SavePlanningCustData, custArgs))
                NewCustomerTreeGrid.CleanseTree();
            //if (FiltersVM.ApplyFilterCommand.CanExecute(null))
            //    FiltersVM.ApplyFilterCommand.Execute(null);
        }

        private bool CanSaveCustomer(object obj)
        {
            return NewCustomerTreeGrid.HasChanges() || NewCustomerTreeGrid.HasCommentChanges();
        }

        public XElement GetPlanningSaveXml(TreeGridViewModel treeGrid)
        {
            var changedValues = treeGrid.GetLowestLevelValueOrCommentChanges();

            var data = CommonXml.GetBaseSaveArguments();
            data.AddElement("TimeInterval_Idx", FiltersVM.Intervals.SelectedItem.Idx);
            data.AddElement("TargetScenario_Idx", FiltersVM.PlanningScenarioList.SelectedItem.Idx);

            changedValues.Do(c =>
            {
                var row = new XElement("Row");
                row.AddElement("Item_Idx", c.Item1);
                row.AddElement("Measure_Idx", c.Item2.Item_Idx);

                var columns = new XElement("Columns");
                c.Item2.Properties.Do(p =>
                {
                    var column = new XElement("Column");
                    column.SetAttributeValue("Idx", p.IDX);
                    if (p.HasChanged)
                    {
                        column.SetAttributeValue("Value_New", RecordViewModel.FixValue(p.Value));
                        column.SetAttributeValue("Value_Original", RecordViewModel.FixValue(p.OriginalValue));
                    }
                    if (p.HasCommentChanged)
                    {
                        var commentXml = new XElement("NewComments");

                        var newComments = p.CommentList.Where(comment => !p.OriginalCommentIdxs.Contains(comment.Idx));

                        newComments.Do(comment => commentXml.AddElement("Comment", comment.Value));

                        column.Add(commentXml);
                    }

                    if (p.HasChanged || p.HasCommentChanged)
                        columns.Add(column);
                });
                row.Add(columns);
                data.Add(row);
            });

            data.Add(InputConverter.ToCustomers(FiltersVM.ListingsVM.CustomerIDsList));

            return data;
        }

        #endregion

    }
}