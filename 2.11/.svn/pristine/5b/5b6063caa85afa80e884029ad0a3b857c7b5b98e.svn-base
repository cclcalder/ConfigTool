using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using AutoMapper;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Exceedra.TreeGrid.Models;
using Exceedra.TreeGrid.ViewModels;
using Model;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity.Generic;
using Model.Entity.Listings;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Trees.ViewModels;
using Model.Entity;

namespace WPF.ViewModels.Demand
{
    public class SupersessionViewModel : ViewModelBase
    {
        public SupersessionViewModel()
        {
            PageTitle = App.Configuration.GetScreens().Single(f => f.Key == ScreenKeys.SUPERSESSION.ToString()).Label;
            BaseSkuTreeViewModel = new TreeViewModel(ListingsAccess.GetFilterProducts(false, true).Result);
            BaseSkuTreeViewModel.Listings.IsExpanded = true;
            SupersessionSkuTreeViewModel = new TreeViewModel(ListingsAccess.GetFilterProducts(false, true).Result);
            SupersessionSkuTreeViewModel.Listings.IsExpanded = true;
            SupersessionTreeGridViewModel = new TreeGridViewModel();

            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Supersession.GetDataFeeds,
                CommonXml.GetBaseArguments("GetData")).ContinueWith(
                    t =>
                    {
                        DataFeeds.SetItems(t.Result);
                        if (DataFeeds.SelectedItem == null)
                            DataFeeds.SelectedItem = DataFeeds.Items.First();
                    });

        }

        #region UI Properties

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                NotifyPropertyChanged(this, vm => vm.PageTitle);
            }
        }


        private SingleSelectViewModel _dataFeeds = new SingleSelectViewModel();

        public SingleSelectViewModel DataFeeds
        {
            get
            {
                return _dataFeeds;
            }
            set
            {
                _dataFeeds = value;
                NotifyPropertyChanged(this, vm => vm.DataFeeds);
            }
        }

        private TreeViewModel _baseSkuTreeViewModel;
        public TreeViewModel BaseSkuTreeViewModel
        {
            get { return _baseSkuTreeViewModel; }
            set
            {
                _baseSkuTreeViewModel = value;
                NotifyPropertyChanged(this, vm => vm.BaseSkuTreeViewModel);
            }
        }

        private TreeViewModel _supersessionSkuTreeViewModel;
        public TreeViewModel SupersessionSkuTreeViewModel
        {
            get { return _supersessionSkuTreeViewModel; }
            set
            {
                _supersessionSkuTreeViewModel = value;

                /* Give leafs the 'Add' template to display a '+' button */
                SupersessionSkuTreeViewModel.GetFlatTree().Where(s => !s.HasChildren).Do(s => s.Template = "Add");
                SupersessionSkuTreeViewModel.AddClick = AddSkuToGrid;
                SupersessionSkuTreeViewModel.DeleteClick = Delete;

                NotifyPropertyChanged(this, vm => vm.SupersessionSkuTreeViewModel);
            }
        }

        private RecordViewModel _supersessionSkuGrid = new RecordViewModel(XElement.Parse("<Results><RowsLimitedAt>500</RowsLimitedAt><RowsAvailable>0</RowsAvailable><RootItem><Item_Idx>-1</Item_Idx><Item_Type>SupersessionGrid</Item_Type><Item_RowSortOrder>-1</Item_RowSortOrder><Attributes><Attribute><ColumnCode>NO_RESULTS</ColumnCode><HeaderText>No Selection</HeaderText><Value>Please select a Sku to view</Value><IsDisplayed>1</IsDisplayed><IsEditable>0</IsEditable><ControlType>Textbox</ControlType></Attribute></Attributes></RootItem></Results>"));
        public RecordViewModel SupersessionSkuGrid
        {
            get { return _supersessionSkuGrid; }
            set
            {
                _supersessionSkuGrid = value;
                NotifyPropertyChanged(this, vm => vm.SupersessionSkuGrid);
            }
        }

        private TreeGridViewModel _supersessionTreeGridViewModel;

        public TreeGridViewModel SupersessionTreeGridViewModel
        {
            get
            {
                return _supersessionTreeGridViewModel;
            }
            set
            {
                _supersessionTreeGridViewModel = value;
                SupersessionTreeGridViewModel.SaveCommand = SaveCommand;
                NotifyPropertyChanged(this, vm => vm.SupersessionTreeGridViewModel);
                SupersessionTreeGridViewModel.MainColumn = "Customers";
                SupersessionTreeGridViewModel.SecondColumn = "Products";
            }
        }

        #endregion

        #region Serialization Properties

        private string CurrentLoadedSkuIdx { get; set; }
        private string CurrentLoadedDataStream { get; set; }



        #endregion

        #region Commands

        /* Bool to identify a freshly summed reload from the db, i.e. not loading the saved dummy actuals */
        public bool FreshReload { get; set; }
        public ICommand LoadSkuSupersessionCommand { get { return new ViewCommand(CanLoadSkuSupersession, LoadSkuSupersessionData); } }
        public ICommand ReloadSupersessionCommand { get { return new ViewCommand(CanReloadSkuSupersession, ReloadSkuSupersessionGrid); } }
        public ICommand SaveCommand { get { return new ViewCommand(CanSave, Save); } }


        /* Loads both the treegrid and grid */
        private void LoadSkuSupersessionData(object o)
        {
            CurrentLoadedSkuIdx = BaseSkuTreeViewModel.GetSingleSelectedNode().Idx;
            CurrentLoadedDataStream = DataFeeds.SelectedItem.Idx;
            FreshReload = false;

            var args = CommonXml.GetBaseArguments("GetData");
            args.AddElement("Sku_Idx", CurrentLoadedSkuIdx);
            args.AddElement("DataFeed_Idx", CurrentLoadedDataStream);

            GetSuppressionGrid(args);

            GetSuppressionTreeGrid(args);

        }

        private void ReloadSkuSupersessionGrid(object o)
        {
            CurrentLoadedDataStream = DataFeeds.SelectedItem.Idx;
            FreshReload = true;

            var args = CommonXml.GetBaseArguments("GetData");
            args.AddElement("Sku_Idx", CurrentLoadedSkuIdx);
            args.Add(GetProductMappings());
            args.AddElement("DataFeed_Idx", CurrentLoadedDataStream);

            GetSuppressionTreeGrid(args);
        }

        private XElement GetProductMappings()
        {
            var mappings = new XElement("Product_Mappings");

            SupersessionSkuGrid.Records.Where(r => r.Item_IsDisplayed).Do(r =>
            {
                var sku = new XElement("Sku");
                sku.SetAttributeValue("Idx", r.Item_Idx);
                sku.SetAttributeValue("Date_Start", InputConverter.ToIsoFormat(r.GetProperty("Date_Start").Value));
                sku.SetAttributeValue("Date_End", InputConverter.ToIsoFormat(r.GetProperty("Date_End").Value));

                mappings.Add(sku);
            });


            return mappings;
        }

        private void GetSuppressionTreeGrid(XElement args)
        {
            SupersessionTreeGridViewModel.IsLoading = true;

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Supersession.GetProductTreeGrid, args).ContinueWith(
                t =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        var data = t.Result;
                        if (!MessageConverter.CheckForError(data) && data.ToString() != "<Result></Result>")
                        {
                            var config = new MapperConfiguration(cfg => cfg.CreateMap<TreeViewHierarchy, TreeGridNode>());
                            var mapper = config.CreateMapper();

                            var customers = ListingsAccess.GetFilterCustomers().Result.FlatTree;

                            var custAsTreeGrid = customers.Select(n => mapper.Map<TreeGridNode>(n));

                            var custHierarchy = TreeGridNode.ConvertListToTree(custAsTreeGrid.ToList());

                            SupersessionTreeGridViewModel = new TreeGridViewModel(data, custHierarchy);
                        }
                        else
                        {
                            SupersessionTreeGridViewModel = new TreeGridViewModel();
                        }
                    }));
                });
        }

        private void GetSuppressionGrid(XElement args)
        {
            DynamicDataAccess.GetGenericItemAsync<RecordViewModel>(StoredProcedure.Supersession.GetProductMappings, args)
                .ContinueWith(t =>
                {
                    SupersessionSkuGrid = t.Result;
                    SetSelectableTreeNodes(SupersessionSkuGrid.Records.Select(r => r.Item_Idx).ToList());
                });
        }

        private bool CanLoadSkuSupersession(object o)
        {
            return BaseSkuTreeViewModel.GetSingleSelectedNode() != null
                && DataFeeds.SelectedItem != null;
        }

        private bool CanReloadSkuSupersession(object o)
        {
            return SupersessionSkuGrid != null
                   && SupersessionSkuGrid.Records != null
                   && SupersessionSkuGrid.Records.Any(r => r.Item_IsDisplayed)
                   && SupersessionSkuGrid.Records.Any(r => r.Properties.Count > 2)
                   && DataFeeds.SelectedItem != null;
        }

        private bool CanSave(object o)
        {
            return FreshReload || SupersessionTreeGridViewModel.HasChanges();
        }

        private void Save(object o)
        {
            var saveXml = CommonXml.GetBaseSaveAttributeArguments();
            saveXml.AddElement("DataFeed_Idx", CurrentLoadedDataStream);
            saveXml.SetAttributeValue("Sku_Idx", CurrentLoadedSkuIdx);

            saveXml.Add(GetProductMappings());

            saveXml.Add(SupersessionTreeGridViewModel.GetSaveXml());

            if (MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(StoredProcedure.Supersession.Save, saveXml)))
                LoadSkuSupersessionData(o);
        }

        public RoutedEventHandler DeleteHandler { get { return Delete; } }

        public void Delete(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Record;
            SupersessionSkuGrid.DeleteRecord(obj);

            var skuIdx = obj.Properties.First(p => p.ColumnCode.Equals("Source_Sku_Idx")).Value;

            var thisNode =
                SupersessionSkuTreeViewModel.GetFlatTree().FirstOrDefault(n => n.Idx.Equals(skuIdx));

            if (thisNode == null)
                return;

            thisNode.IsSelectable = true;
            thisNode.IsParentNode = false;
        }

        public void Delete(object o)
        {
            var skuNode = (TreeViewHierarchy)o;
            SupersessionSkuGrid.DeleteRecord(SupersessionSkuGrid.Records.First(r => r.Item_Idx == skuNode.Idx));

            skuNode.IsSelectable = true;
            skuNode.IsParentNode = false;
        }


        private Record _recordTemplate;
        public void AddSkuToGrid(object o)
        {
            var skuNode = (TreeViewHierarchy)o;

            if (!skuNode.IsSelectable) return;

            //If we don't know the shape of the grid, we need to get it before we can add a record.
            if (_recordTemplate == null)
            {
                //If we still have the default grid then we cannot determine the layout.
                if (SupersessionSkuGrid.Records[0].Properties[0].ColumnCode == "NO_RESULTS")
                {
                    MessageBox.Show("Please Select and Load the Product for which you wish to manage the Supersession", "Warning", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                _recordTemplate = SupersessionSkuGrid.Records[0].GetRecordTemplate();

                //If the first record is not dislayed, then it means the user loaded a sku with not supersession, and it is just a template that we can extract and delete.
                //If it is visible then it is a sku.
                if (!SupersessionSkuGrid.Records[0].Item_IsDisplayed)
                    SupersessionSkuGrid.RemoveRecord(SupersessionSkuGrid.Records[0]);
            }

            var nodeAsRecord = new Record(_recordTemplate);
            nodeAsRecord.Item_Idx = skuNode.Idx;
            nodeAsRecord.Item_IsDisplayed = true;
            nodeAsRecord.GetProperty("Source_Sku_Idx").Value = skuNode.Idx;
            nodeAsRecord.GetProperty("Source_Sku_Name").Value = skuNode.Name;
            nodeAsRecord.GetProperty("Author_Name").Value = User.CurrentUser.DisplayName;
            nodeAsRecord.GetProperty("Date_Created").Value = DateTime.Now.ToShortDateString();
            nodeAsRecord.GetProperty("Date_Start").Value = DateTime.Now.AddYears(-1).ToShortDateString();
            nodeAsRecord.GetProperty("Date_End").Value = DateTime.Now.ToShortDateString();


            SupersessionSkuGrid.AddRecord(nodeAsRecord);

            skuNode.IsSelectedBool = false;
            skuNode.IsSelectable = false;
            skuNode.IsParentNode = true;
        }

        public void SetSelectableTreeNodes(List<string> selectedSkuIdxs)
        {
            SupersessionSkuTreeViewModel.GetFlatTree().Where(n => n.IsParentNode).Do(n =>
            {
                n.IsSelectable = true;
                n.IsParentNode = false;
            });

            SupersessionSkuTreeViewModel.GetFlatTree().Where(n => selectedSkuIdxs.Contains(n.Idx)).Do(n =>
            {
                n.IsSelectable = false;
                n.IsParentNode = true;
            });
        }

        #endregion
    }
}
