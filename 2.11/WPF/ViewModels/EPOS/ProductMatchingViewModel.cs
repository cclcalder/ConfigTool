using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.CellsGrid;
using Exceedra.Common.GroupingMenu;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Controls.Messages;
using Model;
using Model.DataAccess;
using Model.DataAccess.Epos;
using Model.DataAccess.Generic;
using Model.Entity.Canvas;
using Model.Entity.Generic;
using Model.Entity.Listings;
using ViewHelper;
using ViewModels;
using WPF.UserControls.Trees.ViewModels;


namespace WPF.ViewModels.EPOS
{
    public class ProductMatchingViewModel : ViewModelBase
    {
        #region properties

        private CellsGridViewModel _cellsGrid;
        public CellsGridViewModel CellsGrid
        {
            get { return _cellsGrid; }
            set
            {
                _cellsGrid = value;
                NotifyPropertyChanged(this, vm=>vm.CellsGrid);
            }
        }

        #endregion

        public ProductMatchingViewModel()
        {
            UnmatchedItems = new TreeViewModel(EposAccess.GetUnmappedProducts().Result);
            UnmatchedItems.SelectionChanged += UnmatchedItems_SelectionChanged;
            SpProducts = new TreeViewModel(EposAccess.GetSpProducts().Result);

            

            LoadMatches();
            LoadInsights();
        }

        private void LoadInsights()
        {
            if (string.IsNullOrEmpty(App.Configuration.DefaultDashboardCanvasId))
            {
                CustomMessageBox.ShowOK("No dashboard found in user config", "Information", "OK");
            }
            else
            {
                CellsGrid = CellsGridViewModel.GetEmptyCellsGrid(User.CurrentUser.ID,
                User.CurrentUser.SalesOrganisationID, App.Configuration.DefaultDashboardCanvasId);
                CellsGrid.LoadControlsData("dashboard", null);
            }
        }


        private GroupingMenu _insights;
        public GroupingMenu Insights
        {
            get { return _insights; }
            set
            {
                if (_insights == value) return;

                _insights = value;
                NotifyPropertyChanged(this, vm => vm.Insights);
            }
        }
 
        private Insight _selectedInsight;
        public Insight SelectedInsight
        {
            get { return _selectedInsight; }
            set
            {
                if (_selectedInsight == value) return;

                _selectedInsight = value;
                NotifyPropertyChanged(this, vm => vm.SelectedInsight);
            
           
            }
        }

        private void LoadMatches()
        {
            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.EPOS.Mapping.GetMappings,  CommonXml.GetBaseArguments("GetData")).ContinueWith(
                    t =>
                    {
                        MatchesVM = new RecordViewModel(t.Result);
                    });
        }

        #region "linking tab"
        //private ComboboxItem _selectedPossibleProduct;
        //public ComboboxItem SelectedPossibleProduct
        //{
        //    get { return _selectedPossibleProduct; }
        //    set
        //    {
        //        _selectedPossibleProduct = value;
        //        NotifyPropertyChanged(this, vm => vm.SelectedPossibleProduct);
        //    }
        //}
   
        private void UnmatchedItems_SelectionChanged()
        {
            var eposProd = UnmatchedItems.GetSingleSelectedNode();

            if (eposProd != null
                && !string.IsNullOrEmpty(eposProd.Idx)
                && _selectedEposIDx != eposProd.Idx)
            {
                _selectedEposIDx = eposProd.Idx;;
                 
                PopulateSuggestedMatchesByIdx(_selectedEposIDx);
            }
        }

        public ICommand LinkedMatchedCommand { get { return new ViewCommand(CanLinkedMatched, LinkedMatched); } }

        public ICommand LoadFeedsDashboardCommand { get { return new ViewCommand(LoadTab); } }

        private void LoadTab(object i)
        {
        
        }

        public int SelectedTabIndex { get; set; }

        public ICommand LinkedPossibleMatchedCommand { get { return new ViewCommand(CanLinkedPossibleMatched, LinkedPossibleMatched); } }

        private void LinkedPossibleMatched(object obj)
        {
            var eposIDx = UnmatchedItems.GetSingleSelectedNode().Idx;
            var spIdx = PossibleMatchedItems.GetSingleSelectedNode().Idx;

            LinkEposSP(eposIDx, spIdx);

        }

        private void LinkedMatched(object obj)
        {
            var eposIDx = UnmatchedItems.GetSingleSelectedNode().Idx;
            var spIdx = SpProducts.GetSingleSelectedNode().Idx;

            LinkEposSP(eposIDx, spIdx);
        }

        private void LinkEposSP(string eposIDx, string spIdx)
        {
            if (EposAccess.CreateMapping(eposIDx, spIdx))
            {
                //refload unmatched
                UnmatchedItems = new TreeViewModel(EposAccess.GetUnmappedProducts().Result);
            }
        }

        public bool CanLinkedMatched(object o)
        {
            return UnmatchedItems != null
               && UnmatchedItems.GetSingleSelectedNode() != null
               && SpProducts != null
               && SpProducts.GetSingleSelectedNode() != null;
        }

        private static string _selectedEposIDx;
        public bool CanLinkedPossibleMatched(object o)
        {
            return UnmatchedItems != null
               && UnmatchedItems.GetSingleSelectedNode() != null
               && PossibleMatchedItems != null
                && PossibleMatchedItems.GetSingleSelectedNode() != null;
        }

        private   void PopulateSuggestedMatchesByIdx(string idx)
        {
            PossibleMatchedItems = new TreeViewModel(EposAccess.GetPossibleMatched(idx).Result);
             
        }

      
        private TreeViewModel _unmatchedItems;
        public TreeViewModel UnmatchedItems
        {
            get { return _unmatchedItems; }
            set
            {
                _unmatchedItems = value;
                NotifyPropertyChanged(this, vm => vm.UnmatchedItems);
            }
        }

        private TreeViewModel _possibleMatchedItems;
        public TreeViewModel PossibleMatchedItems
        {
            get { return _possibleMatchedItems; }
            set
            {
                _possibleMatchedItems = value;
                NotifyPropertyChanged(this, vm => vm.PossibleMatchedItems);
            }
        }

        private TreeViewModel _spProducts;
        public TreeViewModel SpProducts
        {
            get { return _spProducts; }
            set
            {
                _spProducts = value;
                NotifyPropertyChanged(this, vm => vm.SpProducts);
            }
        }
         
        #endregion

        #region "matched tab"


        private RecordViewModel _matchesVM;
        public RecordViewModel MatchesVM
        {
            get { return _matchesVM; }
            set
            {
                _matchesVM = value;
                NotifyPropertyChanged(this, vm => vm.MatchesVM);
            }
        }

        #endregion   
    }
}
