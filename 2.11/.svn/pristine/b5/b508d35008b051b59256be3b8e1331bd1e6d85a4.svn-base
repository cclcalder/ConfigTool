using System.Xml.Linq;
using Exceedra.Common;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.Utilities;
using WPF.UserControls.Filters.ViewModels;
using WPF.UserControls.Tabs.Models;

namespace WPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Model;
    using global::ViewModels;
     
    public class EditListViewModel : ViewModelBase
    {
        #region Constructors

        public static EditListViewModel New()
        {
            var instance = new EditListViewModel();

            return instance;
        }

        public static EditListViewModel New(bool reloadCacheList)
        {
            var instance = new EditListViewModel();

            return instance;
        }

        private EditListViewModel()
        {
            LoadFilters();
        }

        private void LoadFilters()
        {
            var listingsXmlIn = CommonXml.GetBaseScreenArguments(ScreenKeys.LISTINGSMGMT.ToString());
      
            FiltersVM = new FilterViewModel
            {
                ApplyFilter = TestApply,
                CurrentScreenKey = ScreenKeys.LISTINGSMGMT,
                SingleTreeArguments = listingsXmlIn,
                StatusTreeProc = StoredProcedure.Shared.GetFilterStatusesAndTypes, 
                SaveExtraArguments = null,
                OtherFiltersProc = StoredProcedure.Shared.GetFiltersGrid, 
                OtherArguments = listingsXmlIn
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

        public event WebNavigatedEventHandler FiltersApplied;
        protected virtual void OnFiltersApplied(string controller, string action, Dictionary<string, string> args)
        {
            if (FiltersApplied != null)
                FiltersApplied.Invoke(controller, action, args);
        }

        private void TestApply()
        {
            var filtersXml = GetFilterXml("GetListings");
            var filtersString = filtersXml.ToString();

            OnFiltersApplied("ListingsMgmt", "GetList", new Dictionary<string, string>
            {
                { "xFilters", filtersString }
            });
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
         
        # endregion
    }
}