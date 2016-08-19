using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;
using Model.DataAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Model.Entity;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Exceedra.Controls.Messages;
using Exceedra.DynamicGrid.Models;
using Model;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity.Generic;
using Model.Entity.Listings;
using ViewHelper;
using Model.Entity.NPD;
using WPF.Navigation;
using WPF.UserControls.Listings;
using WPF.UserControls.Trees.ViewModels;
using CommonExtensions = Telerik.Windows.Diagrams.Core.CommonExtensions;
using Property = Exceedra.Controls.DynamicGrid.Models.Property;

namespace WPF.ViewModels.NPD
{
    public class NPDViewModel : ViewModelBase
    {
        private readonly NDPAccess _ndpAccess;

        //Require when loading an existing NPD so we load the sku and custSku grids using the Idx on first load.
        private bool _firstLoad = true;


        public NPDViewModel(string npdIdx = null)
        {
            _ndpAccess = new NDPAccess(npdIdx);
            _ndpAccess.NPDIdx = npdIdx;
            CancelCommand = new ViewCommand(Cancel);
            ReloadCommand = new ViewCommand(Reload);

            InitData();
        }

        private void InitData(bool loadTrees = true)
        {
            if (loadTrees)
            {
                LoadCustomerTree();
                LoadUserTree();
                LoadComponentTree();
            }
            LoadDesignGrid();

            LoadWorkflowStatuses();
            LoadComponentGrid();
        }

        private void LoadWorkflowStatuses()
        {
            WorkflowStatuses = _ndpAccess.GetWorkflowStatuses().Result.ToList();
        }

        #region Dynamic Grids

        private Task LoadDesignGrid()
        {
            return _ndpAccess.GetNPDDesignGrid().ContinueWith(t => LoadDesignGridContinuation(t.Result));
        }

        private void LoadDesignGridContinuation(XElement result)
        {
            CustomerTree.IsReadOnly = UserTree.IsReadOnly = ComponentTree.IsReadOnly = result.Element("Amendable").MaybeValue() == "0";

            NPDDesignRVM.Init(result);// = new RowViewModel(result);
            SetPropertyChangedListeners(NPDDesignRVM);

            foreach (var record in NPDDesignRVM.Records)
            {
                foreach (var property in record.Properties.Where(r => r.ControlType.Contains("down")).ToList())
                {
                    property.DataSourceInput = property.DataSourceInput.Replace("&gt;", ">").Replace("&lt;", "<");
                    record.InitialDropdownLoad(property);
                }
            }

            _firstLoad = false;
        }

        private void LoadProductGrid()
        {
            NPDProductRowViewModel = new RowViewModel { IsLoading = true };
            _ndpAccess.GetNPDProductGrid(_firstLoad ? null : SelectedMasterProductIdx)
                .ContinueWith(t => NPDProductRowViewModel.Init(t.Result), App.Scheduler);
        }

        private void LoadCustomerProductGrid(string selectedIdx)
        {
            NPDCustomerProductRowViewModel = new RowViewModel { IsLoading = true };
            _ndpAccess.GetNPDCustomerProductGrid(_firstLoad ? null : SelectedMasterProductIdx, selectedIdx)
                .ContinueWith(t => NPDCustomerProductRowViewModel.Init(t.Result), App.Scheduler);
        }

        private void LoadComponentGrid()
        {
            DynamicDataAccess.GetGenericItemAsync<RecordViewModel>(StoredProcedure.NPD.GetBomSkus, _ndpAccess.GetBaseNPDArguments("GetData")).ContinueWith(
                t =>
                {
                    ComponentsRVM = t.Result;
                    DisableTreeNodes();
                });
        }

        private RowViewModel _npdDesignRVM = new RowViewModel{IsLoading = true};
        public RowViewModel NPDDesignRVM
        {
            get { return _npdDesignRVM; }
            set
            {
                _npdDesignRVM = value;
                NotifyPropertyChanged(this, vm => vm.NPDDesignRVM);
            }
        }

        private RowViewModel _npdProductRowViewModel = new RowViewModel();
        public RowViewModel NPDProductRowViewModel
        {
            get { return _npdProductRowViewModel; }
            set
            {
                _npdProductRowViewModel = value;
                NotifyPropertyChanged(this, vm => vm.NPDProductRowViewModel);
            }
        }

        private RowViewModel _npdCustomerProductRowViewModel = new RowViewModel();
        public RowViewModel NPDCustomerProductRowViewModel
        {
            get { return _npdCustomerProductRowViewModel; }
            set
            {
                _npdCustomerProductRowViewModel = value;
                NotifyPropertyChanged(this, vm => vm.NPDCustomerProductRowViewModel);
            }
        }

        //private RowViewModel _forecastDesign;
        //public RowViewModel ForecastDesign
        //{
        //    get { return _forecastDesign;}
        //    set
        //    {
        //        _forecastDesign = value;
        //        NotifyPropertyChanged(this, vm => vm.ForecastDesign);
        //    }
        //}

        private RecordViewModel _componentsRVM;
        public RecordViewModel ComponentsRVM
        {
            get { return _componentsRVM; }
            set
            {
                _componentsRVM = value;
                if (ComponentsRVM.Records != null && ComponentsRVM.Records.Any())
                    SetComponentGridFormat();

                NotifyPropertyChanged(this, vm => vm.ComponentsRVM);
            }
        }

        private static RecordViewModel EmptyForecast
        {
            get
            {
                return new RecordViewModel(XElement.Parse("<Results><Grid_Title>Initial Forecast</Grid_Title><RootItem><Item_Idx>76</Item_Idx><Item_Type>Forecast</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>ValueName</ColumnCode><HeaderText></HeaderText><Value>Value</Value><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute></Attributes></RootItem></Results>"));
            }
        }
        private RecordViewModel _forecastRVM = new RecordViewModel(XElement.Parse(
"<Results><Grid_Title>Initial Forecast</Grid_Title><RootItem><Item_Idx>-1</Item_Idx><Item_Type>ROBGrid</Item_Type><Item_RowSortOrder>1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText/><Value>Value</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Nov 2015</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Dec 2015</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Jan 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Feb 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Mar 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Apr 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>May 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Jun 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Jul 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Aug 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Sep 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute><Attribute><ColumnCode>Component_Idx</ColumnCode><HeaderText>Oct 2016</HeaderText><Value>0</Value><Format/><ForeColour/><BorderColour/><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod><ColumnSortOrder>0</ColumnSortOrder></Attribute></Attributes></RootItem></Results>"));
        public RecordViewModel ForecastRVM
        {
            get { return _forecastRVM; }
            set
            {
                _forecastRVM = value;
                NotifyPropertyChanged(this, vm => vm.ForecastRVM);
            }
        }

        private Record _componentGridFormat;
        private void SetComponentGridFormat()
        {
            var properties = ComponentsRVM.Records[0].Properties.Select(p => new Property
            {
                ColumnCode = p.ColumnCode,
                IsDisplayed = p.IsDisplayed,
                ControlType = p.ControlType,
                ForeColour = "Red",
                StringFormat = p.StringFormat,
                IsLoaded = true
            }).ToList();

            _componentGridFormat = new Record { Properties = new ObservableCollection<Property>(properties) };
        }


        //private Property _forecastGridFormat;
        //private void SetForecastGridFormat()
        //{
        //    _forecastGridFormat =
        //        ForecastRVM.xmlToProperty(
        //            XElement.Parse(
        //                "<Attribute><ColumnCode>Period</ColumnCode><HeaderText>Period</HeaderText><Value>0</Value><IsDisplayed>1</IsDisplayed><IsEditable>1</IsEditable><ControlType>Textbox</ControlType><TotalsAggregationMethod>NONE</TotalsAggregationMethod></Attribute>"));
        //    _forecastGridFormat.IDX = ForecastRVM.Records[0].Item_Idx;
        //    _forecastGridFormat.Type = ForecastRVM.Records[0].Item_Type;
        //}

        #endregion

        #region Treeviews

        private Task LoadUserTree()
        {
            return DynamicDataAccess.GetGenericItemAsync<TreeViewModel>(_ndpAccess.GetUsers(), _ndpAccess.GetBaseNPDArguments("GetNPDDetails")).ContinueWith(
                t =>
                {
                    UserTree.Listings = t.Result.Listings;
                    UserTree.IsTreeLoading = false;
                });
        }

        private Task LoadCustomerTree()
        {
            return DynamicDataAccess.GetGenericItemAsync<TreeViewModel>(_ndpAccess.GetCustomers(), _ndpAccess.GetBaseNPDArguments("GetNPDDetails")).ContinueWith(
                t =>
                {
                    CustomerTree.Listings = t.Result.Listings;
                    CustomerTree.IsTreeLoading = false;
                });
        }

        private Task LoadComponentTree()
        {
            return DynamicDataAccess.GetGenericItemAsync<TreeViewModel>(StoredProcedure.Shared.GetFilterProducts, _ndpAccess.GetBaseNPDArguments("Products")).ContinueWith(t =>
            {
                ComponentTree.Listings = t.Result.Listings;
                DisableTreeNodes();
                ComponentTree.IsTreeLoading = false;
            });
        }

        private TreeViewModel _userTree = new TreeViewModel { IsTreeLoading = true, IsReadOnly = true };
        public TreeViewModel UserTree
        {
            get { return _userTree; }
            set
            {
                _userTree = value;
                NotifyPropertyChanged(this, vm => vm.UserTree);
            }
        }

        private TreeViewModel _customerTree = new TreeViewModel { IsTreeLoading = true, IsReadOnly = true};
        public TreeViewModel CustomerTree
        {
            get { return _customerTree; }
            set
            {
                _customerTree = value;
                NotifyPropertyChanged(this, vm => vm.CustomerTree);
            }
        }

        private TreeViewModel _componentTree = new TreeViewModel { IsTreeLoading = true, IsReadOnly = true };
        public TreeViewModel ComponentTree
        {
            get { return _componentTree; }
            set
            {
                _componentTree = value;
                NotifyPropertyChanged(this, vm => vm.ComponentTree);
            }
        }

        #endregion

        #region Statuses

        private List<ComboboxItem> _workflowStatuses = new List<ComboboxItem>();
        public List<ComboboxItem> WorkflowStatuses
        {
            get { return _workflowStatuses; }
            set
            {
                _workflowStatuses = value;
                SelectedWorkflowStatus = WorkflowStatuses.FirstOrDefault(status => status.IsSelected);
                NotifyPropertyChanged(this, vm => vm.WorkflowStatuses);
            }
        }

        private ComboboxItem _selectedWorkflowStatus;
        public ComboboxItem SelectedWorkflowStatus
        {
            get { return _selectedWorkflowStatus; }
            set
            {
                _selectedWorkflowStatus = value;
                NotifyPropertyChanged(this, vm => vm.SelectedWorkflowStatus);
            }
        }

        #endregion

        #region Other Properties

        private string _quantity = "";
        public string Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                NotifyPropertyChanged(this, vm => vm.Quantity);
            }
        }

        private GridLength _bomWidth = new GridLength(0);
        public GridLength BomWidth
        {
            get { return _bomWidth; }
            set
            {
                _bomWidth = value;
                NotifyPropertyChanged(this, vm => vm.BomWidth);
            }
        }

        private bool _isBom;
        public bool IsBom
        {
            get { return _isBom; }
            set
            {
                _isBom = value;
                BomWidth = IsBom ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
                NotifyPropertyChanged(this, vm => vm.IsBom);
            }
        }

        private string _periods;
        public string Periods
        {
            get
            {
                return _periods;
            }
            set
            {
                _periods = value;
                NotifyPropertyChanged(this, vm => vm.Periods);
            }
        }

        private string _factor;
        public string Factor
        {
            get { return _factor; }
            set
            {
                _factor = value;
                NotifyPropertyChanged(this, vm => vm.Factor);
            }
        }

        #endregion

        #region Commands
        public ICommand ReloadCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand SaveCommand
        {
            get { return new ViewCommand(CanSave, Save); }
        }
        public ICommand SaveCloseCommand
        {
            get { return new ViewCommand(CanSave, SaveClose); }
        }

        public ICommand AddCommand
        {
            get { return new ViewCommand(CanAdd, Add); }
        }


        #endregion

        #region Save

        public bool CanSave(object o)
        {
            if ((UserTree == null || !UserTree.GetSelectedIdxs().Any())
                || (CustomerTree == null || !CustomerTree.GetSelectedIdxs().Any())
                || SelectedWorkflowStatus == null
                || NPDDesignRVM == null
                || NPDProductRowViewModel == null
                || !NPDDesignRVM.AreRecordsFulfilled()
                || NPDCustomerProductRowViewModel == null)
                return false;

            return true;
        }

        private void Save(object o)
        {
            Save(false);
        }

        private void SaveClose(object o)
        {
            Save(true);
        }

        private void Save(bool close)
        {
            NpdProduct n = new NpdProduct();
            n.SelectedUsers = UserTree.GetSelectedIdxs();
            n.SelectedCustomers = CustomerTree.GetSelectedIdxs();
            n.DesignGrid = NPDDesignRVM.ToCoreXml();
            n.ProductSkuGrid = NPDProductRowViewModel.ToCoreXml();
            n.ProductSkuCustGrid = NPDCustomerProductRowViewModel.ToCoreXml();
            n.Status = SelectedWorkflowStatus.Idx;
            n.ComponentsGrid = IsBom ? ComponentsRVM.ToXml() : null;

            var res = _ndpAccess.Save(n.ToXml("SaveNPD"));
            SetIdx(res);

            //Reload all the listings data so the NPD is included in the list
            ListingsAccess access = new ListingsAccess();
            access.ResetListingsData();

            if (!string.IsNullOrEmpty(res))
            {
                if (close)
                    RedirectMe.ListScreen("NPD");
                else
                    Reload(null);
            }
        }

        public bool CanAdd(object o)
        {
            return (!String.IsNullOrWhiteSpace(Quantity) && ComponentTree.GetSingleSelectedNode() != null);
        }

        private void Add(object o)
        {
            var properties = _componentGridFormat.Properties.Select(p => new Property
            {
                ColumnCode = p.ColumnCode,
                IsDisplayed = p.IsDisplayed,
                ControlType = p.ControlType,
                ForeColour = "Red",
                StringFormat = p.StringFormat,
                IsLoaded = true
            }).ToList();

            var treeNode = ComponentTree.GetSingleSelectedNode();
            var newItemIdx = treeNode.Idx;

            foreach (var p in properties)
            {
                switch (p.ColumnCode)
                {
                    case "Component":
                        p.Value = treeNode.Name;
                        break;
                    case "Component_Idx":
                        p.Value = newItemIdx;
                        break;
                    case "Quantity":
                        p.Value = Quantity;
                        p.IsEditable = true;
                        break;
                    case "Delete":
                        p.Value = "Delete";
                        break;
                }
            }

            treeNode.IsSelectable = false;
            treeNode.IsParentNode = true;
            treeNode.SingleSelectedItem = false;

            if (ComponentsRVM.Records[0].Item_Idx.Equals("-1"))
                ComponentsRVM.Records.RemoveAt(0);

            ComponentsRVM.AddRecord(properties, "Components", ComponentsRVM.Records.Count, newItemIdx);
        }

        /*Save proc sends back the Npd Idx since if it is a new NPD we
         * need to send it next time we save so the db knows which
         * NPD to update. 
         */
        public void SetIdx(string newIdx)
        {
            if (!string.IsNullOrEmpty(newIdx) && (_ndpAccess.NPDIdx == null || _ndpAccess.NPDIdx == ""))
            {
                _ndpAccess.NPDIdx = newIdx;
            }
        }

        #endregion

        #region Cancel

        private void Cancel(object o)
        {
            string message = "Are you sure you want to cancel the curent operation?";

            if (CustomMessageBox.Show(message, "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.No)
                return;

            RedirectMe.ListScreen("NPD");
        }

        private void Reload(object o)
        {
            RedirectMe.Goto("NPD", _ndpAccess.NPDIdx);
        }

        #endregion

        #region Delete

        public void Delete(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Record;
            ComponentsRVM.DeleteRecord(obj);

            var thisNode =
                ComponentTree.GetFlatTree()
                    .FirstOrDefault(n => n.Idx.Equals(obj.Properties.First(p => p.ColumnCode.Equals("Component_Idx")).Value));

            if (thisNode == null)
                return;

            thisNode.IsSelectable = true;
            thisNode.IsParentNode = false;
        }

        #endregion


        private void SetPropertyChangedListeners(RowViewModel rvm)
        {
            if (rvm != null)
            {
                rvm.Records.Do(
                    rec =>
                        rec.Properties.Where(
                            p =>
                                !String.IsNullOrWhiteSpace(p.UpdateToColumn) &&
                                p.ControlType.ToLowerInvariant().Equals("dropdown"))
                            .Do(prop => prop.PropertyChanged += Dropdown_PropertyChanged));
                rvm.Records.Do(
                                    rec =>
                                        rec.Properties.Where(
                                            p =>
                                                !String.IsNullOrWhiteSpace(p.UpdateToColumn) &&
                                                p.UpdateToColumn.Equals("BOMSku") &&
                                                p.ControlType.ToLowerInvariant().Equals("checkbox"))
                                            .Do(prop =>
                                            {
                                                prop.PropertyChanged += Checkbox_PropertyChanged;
                                                Checkbox_PropertyChanged(prop, null);
                                            }));
            }
        }

        private void Dropdown_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var cell = (RowProperty)sender;

            if (e.PropertyName == "SelectedItem")
            {
                switch (cell.UpdateToColumn)
                {
                    case "NPDSkuData":
                        LoadProductGrid();
                        break;
                    case "NPDCustSkuData":
                        if (cell.SelectedItem != null && SelectedMasterProductIdx != null)
                            LoadCustomerProductGrid(cell.SelectedItem.Item_Idx);
                        break;
                }
            }

        }

        private void Checkbox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsBom = ((RowProperty)sender).Value == "1";
        }

        private string SelectedMasterProductIdx
        {
            get
            {
                var firstOrDefault = NPDDesignRVM.Records[0].Properties.FirstOrDefault(prop => prop.ColumnCode.Equals("NPD_BasedOnProduct"));
                if (firstOrDefault != null && firstOrDefault.SelectedItem != null) return firstOrDefault.SelectedItem.Item_Idx;

                return null;
            }
        }

        private void DisableTreeNodes()
        {
            if (ComponentsRVM != null && ComponentTree != null)
                foreach (var idx in ComponentsRVM.Records.Select(r => r.Item_Idx))
                {
                    var thisNode = ComponentTree.GetFlatTree().FirstOrDefault(n => n.Idx.Equals(idx));
                    if (thisNode != null)
                    {
                        thisNode.IsSelectable = false;
                        thisNode.IsParentNode = true;
                    }
                }

        }
    }

    static class TaskHelper
    {
        public static Task<T> Completed<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }
    }
}