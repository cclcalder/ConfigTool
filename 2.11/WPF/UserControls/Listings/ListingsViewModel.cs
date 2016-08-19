using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ViewModels;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess.Generic;
using Model.Entity;
using Model.DataAccess.Listings;
using Model.Entity.Generic;
using Model.Entity.Listings;
using Telerik.Windows.Diagrams.Core;
using WPF.UserControls.Trees.ViewModels;

namespace WPF.UserControls.Listings
{
    public class ListingsViewModel : BaseViewModel
    {
        public string ScreenCode { get; set; }
        public bool UseGroups { get; set; }


        private bool _isReadOnly;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged(this, vm => vm.IsReadOnly);
            }
        }

        private bool _removeDelistedProducts;

        public bool RemoveDelistedProducts
        {
            get { return _removeDelistedProducts; }
            set
            {
                _removeDelistedProducts = value;
                NotifyPropertyChanged(this, vm => vm.RemoveDelistedProducts);
            }
        }

        //This is set from the codebehind of the tree
        public bool IsCustomersMultiSelection = true;

        public bool IsCustomerSingleSelection
        {
            get { return !IsCustomersMultiSelection; }
        }


        private readonly Func<Task<TreeViewHierarchy>> _getCustomersSingleItem;
        private readonly Func<Task<TreeViewHierarchy>> _getProductsSingleItem;

        private readonly Func<string, XElement, Task<TreeViewHierarchy>> _getCustomersWithParam;
        private readonly Func<string, XElement, Task<TreeViewHierarchy>> _getProductsWithParam;
        private readonly string _customersParam;
        private readonly string _productsParam;
        private readonly XElement _customersParam2;
        private readonly XElement _productsParam2;

        public static Listing ThisListing;

        private static bool _settingsEditor;

        private SingleSelectViewModel _listingsGroups = new SingleSelectViewModel();

        public SingleSelectViewModel ListingGroups
        {
            get
            {
                return _listingsGroups;               
            }
            set
            {
                _listingsGroups = value;
                NotifyPropertyChanged(this, vm => vm.ListingGroups);
            }
        }

        public List<string> ProductIDsList
        {
            get
            {
                return VisibleProducts.GetSelectedIdxs();
            }
        }


        public List<string> CustomerIDsList
        {
            get
            {
                if (IsCustomerSingleSelection)
                {
                    return new List<string> {Customers.GetSingleSelectedNode().Idx};
                }

                return Customers.GetSelectedIdxs();
            }
        }

        private TreeViewModel _customers = new TreeViewModel { IsTreeLoading  = true };

        public TreeViewModel Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                _customers = value;
                Customers.SelectionChanged += SetProductsFromListings;
                NotifyPropertyChanged(this, vm => vm.Customers);
            }
        }

        private TreeViewHierarchy _singleSelectionCustomer;

        public TreeViewHierarchy SingleSelectionCustomer
        {
            get { return _singleSelectionCustomer; }
            set
            {
                _singleSelectionCustomer = value;
                NotifyPropertyChanged(this, vm => vm.SingleSelectionCustomer);
            }
        }

        public ObservableCollection<TreeViewHierarchy> SelectedCustomers
        {
            get { return new ObservableCollection<TreeViewHierarchy>(Customers.GetSelectedNodes()); }
        }

        /* Flat trees:
         * These are essential as they provide a very quick and easy way to reset a tree from an existing list of nodes
         * Each time nodes in the trees need to be removed/added, we start from a freshly reset tree using these flat trees
         * A reset doesn't reset the selections, but resets the hierarchy.
         */
        public List<TreeViewHierarchy> FlatProductList;

        public List<TreeViewHierarchy> FlatCustomerList;

        public TreeViewHierarchy FullCustomerTree { get { return TreeViewHierarchy.ConvertListToTree(FlatCustomerList); } }
        public TreeViewHierarchy FullProductTree { get { return TreeViewHierarchy.ConvertListToTree(FlatProductList); } }

        private List<string> _allLeafIdxs;

        private TreeViewModel _visibleProducts = new TreeViewModel {IsTreeLoading = true};

        public TreeViewModel VisibleProducts
        {
            get { return _visibleProducts; }
            set
            {
                _visibleProducts = value;
                NotifyPropertyChanged(this, vm => vm.VisibleProducts);
            }
        }

        public ObservableCollection<TreeViewHierarchy> SelectedProducts
        {
            get { return new ObservableCollection<TreeViewHierarchy>(VisibleProducts.GetSelectedNodes()); }

        }

        private DateTime? _dateTimeFromParent;
        public DateTime? DateTimeFromParent
        {
            get { return _dateTimeFromParent; }
            set
            {
                _dateTimeFromParent = value;
                NotifyPropertyChanged(this, vm => vm.DateTimeFromParent);
                SetProductsFromListings();
            }
        }

        public DateTime? GetCorrectDate()
        {
            if (App.Configuration.RetentionOfDelistedProducts != null && DateTimeFromParent != null && DateTimeFromParent.Value.Year != 1)
            {
                var thisDateTime = DateTimeFromParent;

                thisDateTime =
                    thisDateTime.Value.AddDays(
                    double.Parse(App.Configuration.RetentionOfDelistedProducts.ToString()) * -1);

                return thisDateTime;
            }
            return DateTimeFromParent;
        }

        public ListingsViewModel()
        {
            IsReadOnly = false;
        }

        public ListingsViewModel(Func<Task<TreeViewHierarchy>> getCustomersMethod, Func<Task<TreeViewHierarchy>> getProductsMethod, bool isMultiSelect = true, bool isTesting = true)
        {
            IsCustomersMultiSelection = isMultiSelect;
            ListingGroups.PropertyChanged += ListingGroupsOnPropertyChanged;

            IsReadOnly = false;

            _getCustomersSingleItem = getCustomersMethod;
            _getProductsSingleItem = getProductsMethod;
            TryLoad();

        }

        public ListingsViewModel(TreeViewHierarchy customers, TreeViewHierarchy products, string screenCode = null, bool useGroups = true, bool settingsEditor = false)
        {
            ScreenCode = screenCode;
            UseGroups = useGroups;
            IsCustomersMultiSelection = true;
            ListingGroups.PropertyChanged += ListingGroupsOnPropertyChanged;
            _settingsEditor = settingsEditor;

            IsReadOnly = false;

            SetCustomers(customers);

            FlatProductList = products.FlatTree.ToList();

            _allLeafIdxs = FlatProductList.Where(t => !t.HasChildren).Select(leaf => leaf.Idx).ToList();
            
            ThisListing = ListingsAccess.GetListings();

            if (FlatCustomerList.Any() && FlatProductList.Any() && ThisListing != null && ThisListing.TheseListings != null)
            {
                Init();
            }

        }

        public ListingsViewModel(Func<string, XElement, Task<TreeViewHierarchy>> getCustomersMethod, Func<string, XElement, Task<TreeViewHierarchy>> getProductsMethod, string custParam1, string prodParam1, bool isMultiSelect = true, XElement custParam2 = null, XElement prodParam2 = null, string currentScreen = "")
        {
            IsCustomersMultiSelection = isMultiSelect;
            IsReadOnly = false;

            ScreenCode = currentScreen;
            ListingGroups.PropertyChanged += ListingGroupsOnPropertyChanged;

            _getCustomersWithParam = getCustomersMethod;
            _getProductsWithParam = getProductsMethod;
            _customersParam = custParam1;
            _customersParam2 = custParam2;
            _productsParam = prodParam1;
            _productsParam2 = prodParam2;
            TryLoad();
        }

        private void ListingGroupsOnPropertyChanged(object o, PropertyChangedEventArgs p)
        {
            if (p.PropertyName == "SelectedItem")
            {
                var vm = (SingleSelectViewModel)o;

                if (vm.SelectedItem == null) return;

                LoadListingsData(vm.SelectedItem.Idx);
            }
        }

        public void LoadListingsData(string groupIdx)
        {
            var args = CommonXml.GetBaseScreenArguments(ScreenCode, "GetData");
            args.AddElement("ListingsGroup_Idx", ListingGroups.SelectedItem.Idx);

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Shared.GetListingsGroup, args).ContinueWith(t =>
            {
                var grouping = new UserSelectedDefaults(t.Result);

                if(_settingsEditor == false)
                SetVisibleCustSkus(grouping.GetCustomerIdxs(), grouping.GetSkuIdxs());

                SetSelections(grouping);
                Customers.Listings.ClearSearch();
                VisibleProducts.Listings.ClearSearch();
            });
        }
        
        public void TryLoad()
        {
            LoadStaticCustomers(LoadCustomers());
            
            var allProducts = LoadProducts();
            if (allProducts != null)
            {
                FlatProductList = allProducts.First().FlatTree.ToList();
                _allLeafIdxs = FlatProductList.Where(t => !t.HasChildren).Select(leaf => leaf.Idx).ToList();
            }

            ThisListing = ListingsAccess.GetListings();

            if (FlatCustomerList.Any() && FlatProductList.Any() && ThisListing != null && ThisListing.TheseListings != null)
            {
                Init();
            }


        }

        public void LoadListingsGroups()
        {
            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Shared.GetListingsGroups,
                CommonXml.GetBaseScreenArguments(ScreenCode, "GetData")).ContinueWith(t =>
                {
                    ListingGroups.SetItems(t.Result);
                });
        }


        private IEnumerable<TreeViewHierarchy> LoadProducts()
        {
            if (_getProductsWithParam != null)
                return new List<TreeViewHierarchy> { _getProductsWithParam(_productsParam, _productsParam2).Result };

            return new List<TreeViewHierarchy> { _getProductsSingleItem().Result };
        }

        private IEnumerable<TreeViewHierarchy> LoadCustomers()
        {
            if (_getCustomersWithParam != null)
                return new List<TreeViewHierarchy> { _getCustomersWithParam(_customersParam, _customersParam2).Result };

            return new List<TreeViewHierarchy> { _getCustomersSingleItem().Result };
        }

        public void Init()
        {
            Customers = new TreeViewModel(FullCustomerTree);
            VisibleProducts = new TreeViewModel(FullProductTree);

            SetProducts();
        }

        public void SetProducts(bool useGroups = true)
        {
            if (!string.IsNullOrWhiteSpace(ScreenCode) && UseGroups)
                LoadListingsGroups();
            else
                SetProductsFromListings();
        }

        public bool FilterCheckAndUpdate(object obj = null)
        {
            if (ThisListing == null || ThisListing.TheseListings == null || !ThisListing.TheseListings.Any())
            {
                if ((_getCustomersSingleItem != null || _getCustomersWithParam != null) && (_getProductsSingleItem != null || _getProductsWithParam != null))
                    TryLoad();
            }
            else
            {
                if (SelectedCustomers != null && SelectedProducts != null)
                {
                    if (CustomerIDsList.Any() && ProductIDsList.Any())
                        return true;
                }
            }

            return false;
        }
    

        #region Customer methods

        private void LoadStaticCustomers(IEnumerable<TreeViewHierarchy> cachedCustomers)
        {
            if(cachedCustomers == null) return;            

            if (IsCustomersMultiSelection == false)
            {
                singleSelectionModeCustomerValidation(new ObservableCollection<TreeViewHierarchy>(
                    cachedCustomers.Where(p => p.Parent == null)
                        .Select(p => new TreeViewHierarchy(p))
                    ).FirstOrDefault());

                if (!foundOne)
                {
                    SelectTheFirstChild(tempStaticList);
                }
            }
            else
            {
                FlatCustomerList = FlattenCustomers(cachedCustomers).ToList();
            }
        }

        private void SetCustomers(TreeViewHierarchy customers)
        {
            if (customers == null) return;

            if (IsCustomersMultiSelection == false)
            {
                singleSelectionModeCustomerValidation(customers);

                if (!foundOne)
                {
                    SelectTheFirstChild(tempStaticList);
                }
            }
            else
            {
                FlatCustomerList = customers.FlatTree.ToList();
            }
        }

        private List<TreeViewHierarchy> tempStaticList = new List<TreeViewHierarchy>();
        private bool foundOne { get; set; }
        private void singleSelectionModeCustomerValidation(TreeViewHierarchy input)
        {
            foreach (var p in input.Children)
            {
                if (p.HasChildren)
                {
                    if (p.IsSelectedBool == true)
                    {
                        p.IsSelectedBool = false;
                    }

                    tempStaticList.Add(p);
                    singleSelectionModeCustomerValidation(p);
                }
                else
                {
                    if (foundOne == false)
                    {
                        if (p.IsSelectedBool == true)
                            foundOne = true;
                    }
                    else
                    {
                        p.IsSelectedBool = false;
                    }

                    tempStaticList.Add(p);
                }
            }
        }

        private void SelectTheFirstChild(List<TreeViewHierarchy> input)
        {
            input.First(node => !node.HasChildren).IsSelectedBool = true;
        }


        #endregion

        #region Products methods

        public void SetProductsFromListings()
        {
            VisibleProducts.IsTreeLoading = true;
            SetProductsFromListings(SelectedCustomers);
            VisibleProducts.IsTreeLoading = false;
        }

        public void SetProductsFromListings(IEnumerable<TreeViewHierarchy> treeView)
        {
            HashSet<string> listingList = new HashSet<string>();
            List<string> copyOfProdIds = new List<string>();

            if (ThisListing == null) return;

            var totalWatch = Stopwatch.StartNew();

            var s = new Stopwatch();
            s.Start();

            if (VisibleProducts != null)
                copyOfProdIds = new List<string>(VisibleProducts.GetFlatTree().Select(prod => prod.Idx));

            var selectedCustomers = treeView.Where(a => a.IsSelectedBool == true && !a.HasChildren).Distinct();
            var selectedCustomerIDs = selectedCustomers.Select(a => a.Idx).ToHashSet();

            var hashedCustList = ThisListing.TheseListings.ToLookup(l => l.Key.Split('@')[0]);

            /* Remove non-selected custs */
            var hashedSelectedCustList = hashedCustList.Where(c => selectedCustomerIDs.Contains(c.Key)).SelectMany(k => hashedCustList[k.Key]).ToLookup(l => l.Key.Split('@')[0]);

            /* Get the new set of listings that only contain the selected customers */
            var applicableListings = hashedSelectedCustList.Select(c => c.Key).SelectMany(k => hashedSelectedCustList[k]).ToList();

            var hashedSkuList = applicableListings.ToLookup(l => l.Key.Split('@')[1]);

            var correctDate = GetCorrectDate();

            /* Look up to quickly get the TreeViewHierarchy for a skuIdx */
            var skuLookup = FlatProductList.ToLookup(t => t.Idx);

            try
            {
                /* Get the list of skus that are listed against the selected custs. */
                listingList = applicableListings.Select(l => l.Value.ProductID).Distinct().ToHashSet();

                /* foreach listed sku, determine is delisting state */
                foreach (var prodIdx in listingList)
                {
                    if (correctDate != null)
                    {
                        var custsListedTo = hashedSkuList[prodIdx].ToHashSet();

                        /* If all listedToCusts say the sku is delisted, then true */
                        if (custsListedTo.All(a => a.Value.DelistingsDate.HasValue && correctDate > a.Value.DelistingsDate.Value))
                            skuLookup[prodIdx].Do(t => t.IsDelisted = true);
                        /* If atleat one listedToCusts say the sku is delisted, then null */
                        else if (custsListedTo.Any(a => a.Value.DelistingsDate.HasValue && correctDate > a.Value.DelistingsDate.Value))
                            skuLookup[prodIdx].Do(t => t.IsDelisted = null);
                        /* If no listedToCusts say the sku is delisted, then false */
                        else
                            skuLookup[prodIdx].Do(t => t.IsDelisted = false);
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError("SetProductsFromListings " + ex.Message + " @" + DateTime.Now);
            }

            s.Stop();

            Stopwatch s2 = new Stopwatch();
            s2.Start();

            /* Not production code, but useful during debug to test data */
            //TestForCircularDependencies(FlatProductList);

            if (listingList.Any())
            {
                var leafsToLoseIdxs = _allLeafIdxs.Where(t => !listingList.Contains(t)).ToHashSet(); //These are what we want to lose

                leafsToLoseIdxs.AddRange(FlatProductList.Where(a => a.IsDelisted == true).Select(a => a.Idx)); //Add delisted skus to the list of what we don't want

                if (VisibleSkus != null)
                {
                    var listleafsFromListingsIdxs = _allLeafIdxs.Except(leafsToLoseIdxs); //These are what we want based on just listings

                    var extraLeafsToLose = listleafsFromListingsIdxs.Except(VisibleSkus);

                    leafsToLoseIdxs.AddRange(extraLeafsToLose);
                }

                //Deselect the ones we are going to hide:
                FlatProductList.Where(p => leafsToLoseIdxs.Contains(p.Idx)).Do(l => l.IsSelectedBool = false);

                var products = TreeViewHierarchy.ConvertListToTree(FlatProductList.Where(p => !leafsToLoseIdxs.Contains(p.Idx)).ToList());

                TreeViewHierarchy.RemoveChildlessParents(products);

                VisibleProducts = new TreeViewModel(products);

                if (IsProductSelectedByDefault)
                    VisibleProducts.GetFlatTree()
                        .Where(prod => !copyOfProdIds.Contains(prod.Idx))
                        .Do(p => p.IsSelectedBool = true);

                //Since prods have been removed/added, reassert parent nodes states.
                TreeViewHierarchy.CheckAllStates(VisibleProducts.Listings);

                NotifyPropertyChanged(this, vm => vm.VisibleProducts);
            }
            else
            {
                FlatProductList.Where(p => p.IsSelectedBool != false).Do(p => p.IsSelectedBool = false);

                VisibleProducts = new TreeViewModel();
            }

            s2.Stop();

            /* The time to beat is 0.45seconds when selecting ALL Ach listings 
             * listingList ~ 2400 (aka # of sku leafs)
             * listings ~ 19000
             * Delisted Skus = 0
             * 
             * 0.06 on DEV for all sales org
             */
            totalWatch.Stop();

        }

        /* Do not remove. Useful for debugging */
        private void TestForCircularDependencies(ObservableCollection<TreeViewHierarchy> flatProductList)
        {
            Dictionary<string, string> testDictionary = new Dictionary<string, string>();

            foreach (var prod in flatProductList)
            {
                testDictionary.Add(prod.Idx, prod.ParentIdx);
            }

            foreach (var element in testDictionary)
            {
                string nextParentId = element.Value;
                int counter = 0;
                while (nextParentId != null)
                {
                    nextParentId = testDictionary[nextParentId];
                    counter++;

                    if (counter > 50)
                    {
                        MessageBoxShow("Products have circular dependency", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        private bool IsProductSelectedByDefault
        {
            get { return App.Configuration.IsProductSelectedByDefault; }
        }

        private void UpdateProductHierarchy(TreeViewHierarchy current, string targetID, bool loading = false)
        {
            if (current != null)
                UpdateProductHierarchy(current.Children, current, targetID, loading);

        }

        private void UpdateProductHierarchy(IEnumerable<TreeViewHierarchy> ph, TreeViewHierarchy currentParent, string targetID, bool loading = false)
        {
            var t = new List<TreeViewHierarchy>();
            if (ph != null)
            {
                if (currentParent.Idx == targetID)
                {
                    if (currentParent.Children.Any(a => a.IsSelectedBool == true) &&
                        currentParent.Children.Any(a => a.IsSelectedBool != true))
                    {
                        currentParent.IsSelectedBool = null;
                    }

                    if (currentParent.Children.All(a => a.IsSelectedBool == true))
                        currentParent.IsSelectedBool = true;

                    if (currentParent.Children.All(a => a.IsSelectedBool == false))
                        currentParent.IsSelectedBool = false;
                }
                else
                {
                    foreach (TreeViewHierarchy currentChild in ph)
                    {
                        //Set all child nodes to the same as their parent

                        if (currentChild.Children != null)
                        {
                            if (currentChild.Idx == targetID)
                            {
                                if (currentChild.Children.Any(a => a.IsSelectedBool == true) &&
                                    currentChild.Children.Any(a => a.IsSelectedBool != true))
                                {
                                    currentChild.IsSelectedBool = null;
                                }
                                if (currentChild.Children.All(a => a.IsSelectedBool == true))
                                    currentChild.IsSelectedBool = true;

                                if (currentChild.Children.All(a => a.IsSelectedBool == false))
                                    currentChild.IsSelectedBool = false;

                                UpdateProductHierarchy(VisibleProducts.Listings, currentChild.ParentIdx);


                            }
                            else
                            {
                                UpdateProductHierarchy(currentChild.Children, currentParent, targetID);
                            }
                        }
                    }
                }

            }

        }

        public IEnumerable<TreeViewHierarchy> FlattenCustomers(IEnumerable<TreeViewHierarchy> customers)
        {
            if(customers == null) return new List<TreeViewHierarchy>();

            var list = customers.ToList();
            var l = list.Concat(list.SelectMany(GetCustomers));

            return l;
        }

        public IEnumerable<TreeViewHierarchy> GetCustomers(TreeViewHierarchy parent)
        {
            yield return parent;

            if (parent.Children != null)
            {
                foreach (var relative in parent.Children.SelectMany(GetCustomers))
                    yield return relative;
            }
        }

        #endregion

        public void SetSelections(UserSelectedDefaults defaults)
        {
            if (defaults != null)
            {
                if (defaults.Customers != null) SetSelectedCustomers(defaults.GetSelectedCustomerIdxs());
                if (defaults.Products != null) SetSelectedProducts(defaults.GetSelectedSkuIdxs());

                FilterCheckAndUpdate();
            }
        }

        internal void SetSelectedCustomers(HashSet<string> selectedCustomerIDs)
        {
            Customers.DeselectAll();
            Customers.GetFlatTree().Do(c => c.IsSelectedBool = selectedCustomerIDs.Contains(c.Idx));

            TreeViewHierarchy.CheckAllStates(Customers.Listings);

            SetProductsFromListings();
        }

        internal void SetSelectedProducts(HashSet<string> selectedProductIDs)
        {
            VisibleProducts.DeselectAll();
            VisibleProducts.GetFlatTree().Do(c => c.IsSelectedBool = selectedProductIDs.Contains(c.Idx));

            TreeViewHierarchy.CheckAllStates(VisibleProducts.Listings);
        }

        /* These are additionaly restrictions that act above the listings restrictions */
        private HashSet<string> VisibleCusts { get; set; }
        private HashSet<string> VisibleSkus { get; set; }

        public void SetVisibleCustSkus(HashSet<string> visibleCusts, HashSet<string> visibleSkus, bool clearSelection = false)
        {
            if (clearSelection)
            {
                FlatCustomerList.Do(c => c.IsSelectedBool = false);
                FlatProductList.Do(c => c.IsSelectedBool = false);
            }

            VisibleCusts = visibleCusts;
            ConfigureVisibleCusts();

            VisibleSkus = visibleSkus;

            if (!clearSelection)
            {
                SetSelectedCustomers(VisibleCusts);
                SetSelectedProducts(VisibleSkus);
            }
            else
            {
                SetProductsFromListings();
            }
        }


        private void ConfigureVisibleCusts()
        {
            var newTreeList = TreeViewHierarchy.ConvertListToTree(FlatCustomerList.Where(c => c.HadChildrenInitially || VisibleCusts.Contains(c.Idx)).ToList());
            TreeViewHierarchy.RemoveChildlessParents(newTreeList);
            Customers = new TreeViewModel(newTreeList);
        }

      

    }
}
