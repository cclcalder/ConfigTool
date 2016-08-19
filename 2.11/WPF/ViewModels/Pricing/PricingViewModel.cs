using System.Windows;

namespace WPF.ViewModels.Pricing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Coder.WPF.UI;
    using Model;
    using Model.DataAccess;
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using ViewHelper;
    using global::ViewModels;
    using Exceedra.Common;
    public class PricingViewModel : PropertyChangedBase, IDisposable
    {
        private readonly ActionCommand _applySelection;
        private readonly ICommand _clearSelectedProducts;
        private readonly ISearchableTreeViewNodeEventsConsumer _eventsConsumer;
        private readonly IPricingAccess _pricingAccess;
        private readonly ActionCommand _save;
        private decimal _adjustAllValues;
        private IEnumerable<ItemCustomer> _customers;
        private IList<PricingProduct> _flattenedProducts = new List<PricingProduct>();
        private bool _isAdjustAllValuesSelected;
        private bool _isSetAllValuesSelected;
        private IEnumerable<ItemDetailViewModel> _itemDetails;
        private IEnumerable<Item> _items;
        private IEnumerable<PricingProduct> _products = Enumerable.Empty<PricingProduct>();
        private IEnumerable<PricingProductViewModel> _rootProducts;
        private IEnumerable<Scenario> _scenarios;
        private ItemCustomer _selectedCustomer;
        private Item _selectedItem;
        private Scenario _selectedScenario;
        private decimal _setAllValues;
        private DateTime _setFromDate = DateTime.Today;
        private DateTime _setToDate = DateTime.Today;
        private bool _customerMode = true;
        private string _adjustAllValuesDisplay;
        private string _setAllValuesDisplay;

        private PricingViewModel(ISearchableTreeViewNodeEventsConsumer eventsConsumer, IPricingAccess pricingAccess)
        {
            _pricingAccess = pricingAccess;
            _eventsConsumer = eventsConsumer;
            _applySelection = new ActionCommand(ApplySelectionImpl, CanApplySelectionImpl,
                                                pceh => PropertyChanged += pceh);
            _save = new ActionCommand(SaveImpl, CanSaveImpl, pceh => PropertyChanged += pceh);
            _clearSelectedProducts = new ActionCommand(ClearSelectedProductsImpl, CanClearSelectedProductsImpl,
                                                       pceh => PropertyChanged += pceh);
            PricingProductViewModel.IsSelectedChanged += PricingProductViewModelIsSelectedChanged;
        }

        public Visibility CreatePricingVisibility
        {
            get { return App.Configuration.IsCreatePricingActive ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ICommand ClearSelectedProducts
        {
            get { return _clearSelectedProducts; }
        }

        public IEnumerable<Item> Items
        {
            get { return _items; }
            private set { Set(ref _items, value, () => Items); }
        }

        public Item SelectedItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value, () => SelectedItem); }
        }

        public bool CustomerMode
        {
            get { return _customerMode; }
            set
            {
                if (Set(ref _customerMode, value, () => CustomerMode))
                {
                    if (!_customerMode)
                    {
                        Products = _pricingAccess.GetProducts(string.Empty).ToList();
                        Items = _pricingAccess.GetItemsProduct().OrderBy(i => i.Name).ToList();
                    }
                    else
                    {
                        if (SelectedCustomer == null)
                        {
                            Products = Enumerable.Empty<PricingProduct>();
                        }
                        else
                        {
                            Products = _pricingAccess.GetProducts(SelectedCustomer).ToList();
                        }

                        Items = _pricingAccess.GetItems().OrderBy(i => i.Name).ToList();
                    }
                }
            }
        }


        public ItemCustomer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                if (Set(ref _selectedCustomer, value, () => SelectedCustomer))
                {
                    Products = SelectedCustomer == null
                                   ? Enumerable.Empty<PricingProduct>()
                                   : _pricingAccess.GetProducts(SelectedCustomer).ToList();
                }
            }
        }

        public IEnumerable<ItemCustomer> Customers
        {
            get { return _customers; }
            private set { Set(ref _customers, value, () => Customers); }
        }

        public IEnumerable<Scenario> Scenarios
        {
            get { return _scenarios; }
            private set { Set(ref _scenarios, value, () => Scenarios); }
        }

        public Scenario SelectedScenario
        {
            get { return _selectedScenario; }
            set { Set(ref _selectedScenario, value, () => SelectedScenario); }
        }

        public IEnumerable<PricingProduct> Products
        {
            get { return _products; }
            set
            {
                if (Set(ref _products, value.ToList(), () => Products))
                {
                    _flattenedProducts = _products.Flatten(p => p.Children).ToList();
                    RootProducts = CreateRootProductsList();
                }
            }
        }

        public IEnumerable<PricingProductViewModel> RootProducts
        {
            get { return _rootProducts; }
            private set { Set(ref _rootProducts, value.ToList(), () => RootProducts); }
        }

        public IEnumerable<ItemDetailViewModel> ItemDetails
        {
            get { return _itemDetails; }
            private set { Set(ref _itemDetails, value, () => ItemDetails); }
        }

        public DateTime SetFromDate
        {
            get { return _setFromDate; }
            set
            {
                if (Set(ref _setFromDate, value, () => SetFromDate))
                {
                    foreach (ItemDetailViewModel itemDetail in ItemDetails)
                    {
                        itemDetail.FromDate = SetFromDate;
                    }
                    _save.RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime SetToDate
        {
            get { return _setToDate; }
            set
            {
                if (Set(ref _setToDate, value, () => SetToDate))
                {
                    foreach (ItemDetailViewModel itemDetail in ItemDetails)
                    {
                        itemDetail.ToDate = SetToDate;
                    }
                    _save.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsSetAllValuesSelected
        {
            get { return _isSetAllValuesSelected; }
            set
            {
                if (Set(ref _isSetAllValuesSelected, value, () => IsSetAllValuesSelected))
                {
                    IsAdjustAllValuesSelected = !IsSetAllValuesSelected;
                    if (!IsSetAllValuesSelected)
                    {
                        SetAllValuesDisplay = string.Empty;
                    }
                }
            }
        }

        public bool IsAdjustAllValuesSelected
        {
            get { return _isAdjustAllValuesSelected; }
            set
            {
                if (Set(ref _isAdjustAllValuesSelected, value, () => IsAdjustAllValuesSelected))
                {
                    IsSetAllValuesSelected = !IsAdjustAllValuesSelected;
                    if (!IsAdjustAllValuesSelected)
                    {
                        AdjustAllValuesDisplay = string.Empty;
                    }
                }
            }
        }

        public string SetAllValuesDisplay
        {
            get { return _setAllValuesDisplay; }
            set
            {
                if (Set(ref _setAllValuesDisplay, value, () => SetAllValuesDisplay))
                {
                    if (string.IsNullOrEmpty(SetAllValuesDisplay))
                    {
                        return;
                        //SetAllValues = 0;
                    }
                    else
                    {
                        decimal setAllValues;
                        decimal.TryParse(SetAllValuesDisplay, out setAllValues);
                        SetAllValues = setAllValues;
                    }

                    IsSetAllValuesSelected = true;
                    if (ItemDetails != null)
                    {
                        foreach (ItemDetailViewModel itemDetail in ItemDetails)
                        {
                            itemDetail.ModifiedPrice = SetAllValues;
                        }
                        _save.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public decimal SetAllValues
        {
            get { return _setAllValues; }
            set
            {
                if (Set(ref _setAllValues, value, () => SetAllValues))
                {
                }
            }
        }

        public string AdjustAllValuesDisplay
        {
            get
            {
                return _adjustAllValuesDisplay;
            }

            set
            {
                if (Set(ref _adjustAllValuesDisplay, value, () => AdjustAllValuesDisplay))
                {
                    if (string.IsNullOrEmpty(AdjustAllValuesDisplay))
                    {
                        AdjustAllValues = 0;
                    }
                    else
                    {
                        decimal adjustAllValues;
                        decimal.TryParse(AdjustAllValuesDisplay, out adjustAllValues);
                        AdjustAllValues = adjustAllValues;
                    }

                    IsAdjustAllValuesSelected = true;
                    foreach (ItemDetailViewModel itemDetail in ItemDetails)
                    {
                        itemDetail.ModifiedPrice = itemDetail.OriginalPrice * ((100 + AdjustAllValues) / 100);
                    }
                    _save.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal AdjustAllValues
        {
            get { return _adjustAllValues; }
            set
            {
                if (Set(ref _adjustAllValues, value, () => AdjustAllValues))
                {

                }
            }
        }

        public ICommand ApplySelection
        {
            get { return _applySelection; }
        }

        public ICommand Save
        {
            get { return _save; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            PricingProductViewModel.IsSelectedChanged -= PricingProductViewModelIsSelectedChanged;
        }

        #endregion

        private bool CanClearSelectedProductsImpl()
        {
            return _flattenedProducts.Any(p => p.IsSelected);
        }

        private void ClearSelectedProductsImpl()
        {
            foreach (PricingProduct product in _flattenedProducts)
            {
                product.IsSelected = false;
            }
            ClearAllSelected(RootProducts);
        }

        private void ClearAllSelected(IEnumerable<SearchableNode> nodes)
        {
            foreach (SearchableNode node in nodes)
            {
                node.IsSelected = false;
                ClearAllSelected(node.Children);
            }
        }

        private bool CanSaveImpl()
        {
            decimal adjustAllValues, setAllValues;
            bool isValidAdjustAllValues = string.IsNullOrEmpty(AdjustAllValuesDisplay) ? true : decimal.TryParse(AdjustAllValuesDisplay, out adjustAllValues);
            bool isValidSetAllValues = string.IsNullOrEmpty(SetAllValuesDisplay) ? false : decimal.TryParse(SetAllValuesDisplay, out setAllValues);
            if (IsSetAllValuesSelected && isValidSetAllValues == false)
            {
                return false;
            }

            return (IsAdjustAllValuesSelected || IsSetAllValuesSelected) && (isValidAdjustAllValues || isValidSetAllValues || (ItemDetails != null && ItemDetails.Any(id => id.HasChanges)));
        }

        private bool ValidateImpl()
        {

            foreach (var item in ItemDetails)
            {
                item.FromDate = SetFromDate;
                item.ToDate = SetToDate;
                item.ModifiedPrice = IsAdjustAllValuesSelected ? item.ModifiedPrice : SetAllValues;
            }

            WebServiceResult result = _pricingAccess.ValidateItemDetails(ItemDetails.Select(id => id.ItemDetail),
                                                                    SelectedCustomer, SelectedScenario, SelectedItem);
            var successResult = result as WebServiceSuccess;
            if (successResult != null)
            {
                return true;
            }
            var errorResult = result as WebServiceError;
            if (errorResult != null)
            {
                Messages.Instance.Put(new ErrorMessage(errorResult.Message));
                return false;
            }
            return false;
        }

        private void SaveImpl()
        {
            if (ValidateImpl())
            {
                foreach (var item in ItemDetails)
                {
                    item.FromDate = SetFromDate;
                    item.ToDate = SetToDate;
                    item.ModifiedPrice = IsAdjustAllValuesSelected ? item.ModifiedPrice : SetAllValues;
                }

                WebServiceResult result = _pricingAccess.SaveItemDetails(ItemDetails.Select(id => id.ItemDetail),
                                                                         SelectedCustomer, SelectedScenario,
                                                                         SelectedItem);
                var successResult = result as WebServiceSuccess;
                if (successResult != null)
                {
                    ApplySelectionImpl();
                    Messages.Instance.Put(new InformationMessage(successResult.Message));
                    IsSetAllValuesSelected = IsAdjustAllValuesSelected = false;
                    SetAllValuesDisplay = string.Empty;
                    AdjustAllValuesDisplay = string.Empty;
                    return;
                }

                var errorResult = result as WebServiceError;
                if (errorResult != null)
                {
                    Messages.Instance.Put(new ErrorMessage(errorResult.Message));
                }
            }
        }

        private IEnumerable<PricingProductViewModel> CreateRootProductsList()
        {
            return _products
                .Where(p => p.ParentId == null)
                .Select(p => new PricingProductViewModel(_eventsConsumer, null, p));
        }

        private void PricingProductViewModelIsSelectedChanged(object sender, EventArgs e)
        {
            _applySelection.RaiseCanExecuteChanged();
        }

        private bool CanApplySelectionImpl()
        {
            return (SelectedCustomer != null || !CustomerMode)
                   && SelectedScenario != null
                   && SelectedItem != null
                   && _flattenedProducts.Count > 1
                   && _flattenedProducts.Any(p => p.IsSelected);
        }

        private void ApplySelectionImpl()
        {
            List<string> selectedProducts =
                _flattenedProducts.Where(p => p.IsSelected && (!p.Children.Any())).Select(p => p.ID).ToList();
            ItemDetails =
                _pricingAccess.GetItemDetails(SelectedCustomer, SelectedScenario, SelectedItem, selectedProducts)
                    .Select(itemDetail => new ItemDetailViewModel(itemDetail))
                    .ToList();

            //SetFromDate = ItemDetails.Select(idvm => idvm.FromDate).Min();
            //SetToDate = ItemDetails.Select(idvm => idvm.ToDate).Max();
        }

        public static PricingViewModel New(IPricingAccess pricingAccess)
        {
            return New(new StubEventsConsumer(), pricingAccess);
        }

        public static PricingViewModel New(ISearchableTreeViewNodeEventsConsumer eventsConsumer,
                                           IPricingAccess pricingAccess)
        {
            var pricingViewModel = new PricingViewModel(eventsConsumer, pricingAccess);
            pricingViewModel.Load();
            return pricingViewModel;
        }

        private void Load()
        {
            Customers = _pricingAccess.GetCustomers().OrderBy(c => c.Name).ToList();
            Items = _pricingAccess.GetItems().OrderBy(i => i.Name).ToList();
            Scenarios = _pricingAccess.GetScenarios();
        }
    }

    internal class StubEventsConsumer : ISearchableTreeViewNodeEventsConsumer
    {
        #region ISearchableTreeViewNodeEventsConsumer Members

        public void NotifySelectedNodeChanged()
        {
        }

        #endregion
    }
}