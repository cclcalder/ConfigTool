using System.ComponentModel;
using Telerik.Windows;
using Telerik.Windows.Controls.TreeView;
using Model.Language;

namespace WPF.PromoTemplates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Model;
    using ViewModels;
    using System.Windows.Media;
    using System.Windows.Input;
    using Telerik.Windows.Controls;
    using Exceedra.Controls;

    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class TemplateProducts : Page
    {
        private readonly int _baseColumnCount;

        public TemplateProducts()
            : this(null)
        {
        }

        //private void ShowModalContent(object sender, RoutedEventArgs e)
        //{      
        //    ProductDataModalPresenter.IsModal = true;
        //}

        static bool showDisplay { get; set; }
        static bool showFOC { get; set; }
        private PromotionTemplateViewModelBase _viewModel;
        public TemplateProducts(PromotionTemplateViewModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            InitializeComponent();
            radTreeView2.SetValue(TreeViewPanel.VirtualizationModeProperty, Telerik.Windows.Controls.TreeView.VirtualizationMode.Hierarchical);
            _baseColumnCount = dgProducts.Columns.Count;
            DataContext = _viewModel = viewModel;
            showDisplay = viewModel.ShowDisplay;
            showFOC = viewModel.ShowFOC;
            Loaded += Products_Loaded;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            this.radTreeView2.AddHandler(RadMenuItem.ClickEvent, new RoutedEventHandler(OnContextMenuClick));
        }

        private void OnContextMenuClick(object sender, RoutedEventArgs args)
        {
            // Get the clicked context menu item
            RadMenuItem menuItem = ((RadRoutedEventArgs)args).OriginalSource as RadMenuItem;

            var c = clickedElement.Item as PromotionHierarchy;
            if (c.Children != null ||c.Children.Any())
            {
                ItemsControl parentItemsControl = (ItemsControl) clickedElement.ParentItem ??
                                                  clickedElement.ParentTreeView;
                string header = menuItem.Header as string;
                c.IsSelected2 = !c.IsSelected2;
                _viewModel.GetIsParentNode(c);

                if (c.IsSelectedBool == false)
                {
                    c.IsSelectedBool = true;
                    _viewModel.GetSelected(c);
                }

            }

        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            
        }

        private void Products_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateGridColumns();
        }

        private void GenerateGridColumns()
        {
            IEnumerable items = dgProducts.ItemsSource;

            if (items == null) return;

            PromotionProductPrice prodPrice = (items as ObservableCollection<PromotionProductPrice>).FirstOrDefault();

            if (prodPrice != null && prodPrice.Measures != null && prodPrice.Measures.Any())
            {
                List<PromotionMeasure> measures = prodPrice.Measures.ToList();

                if (dgProducts.Columns.Count < measures.Count() + _baseColumnCount)
                    for (int i = 0; i < measures.Count; i++)
                    {
                        var column = new DataGridTextColumn
                            {
                                // set index to be after Customer and Status Column.
                                DisplayIndex = i + 1,
                                Header = measures[i].Name,
                                Binding = new Binding(string.Format("Measures[{0}].Value", i)),
                                MinWidth = 90,
                                IsReadOnly = true,
                            };

                        dgProducts.Columns.Add(column);
                    }
            }
        }

        private void dgProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            IEnumerable source = dgProducts.ItemsSource;
            if (source == null)
                return;

            // comparing if the grid columns are not generated currently
            var items = (source as ObservableCollection<PromotionProductPrice>);
            if (items != null && items.Any())
            {
                if (items.First().Measures != null)
                {
                    List<PromotionMeasure> cols = items.First().Measures;
                    if (dgProducts.Columns.Count < cols.Count() + 1)
                        GenerateGridColumns();
                }
            }
        }

        private void dgProducts_Loaded(object sender, RoutedEventArgs e)
        {
            var cols = dgProducts.Columns.Count();

            dgProducts.Columns[1].Visibility = (showFOC == false ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
            dgProducts.Columns[2].Visibility = (showDisplay == false ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
             
        }

        //
        // SINGLE CLICK EDITING
        //
        //private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var cell = sender as DataGridCell;

        //    // Perform only for Checkbox Column types
        //    if (cell.Column is DataGridCheckBoxColumn)
        //    {
        //        if (cell != null &&
        //            !cell.IsEditing)
        //        {
        //            if (!cell.IsFocused)
        //            {
        //                cell.Focus();
        //            }
        //            var dataGrid = FindVisualParent<DataGrid>(cell);
        //            if (dataGrid != null)
        //            {
        //                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
        //                {
        //                    if (!cell.IsSelected)
        //                        cell.IsSelected = true;
        //                }
        //                else
        //                {
        //                    var row = FindVisualParent<DataGridRow>(cell);
        //                    if (row != null && !row.IsSelected)
        //                    {
        //                        row.IsSelected = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //private static T FindVisualParent<T>(UIElement element) where T : UIElement
        //{
        //    UIElement parent = element;
        //    while (parent != null)
        //    {
        //        var correctlyTyped = parent as T;
        //        if (correctlyTyped != null)
        //        {
        //            return correctlyTyped;
        //        }

        //        parent = VisualTreeHelper.GetParent(parent) as UIElement;
        //    }
        //    return null;
        //}


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Search();
        }

 


        private bool _isFiltered;
        public bool isFiltered { get { return _isFiltered; } set { _isFiltered = value; } }

        private string m_filterText;
        public string filterText { get { return m_filterText; } set { m_filterText = value; } }

          private void Search()
        {
            var nodes = this.radTreeView2.Items;

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                 
                PromotionHierarchy n = (PromotionHierarchy)nodes[0];
                n.PerformSearch(SearchTextBox.Text.Split(' '));
                
            }
            else
            {
                PromotionHierarchy n = (PromotionHierarchy)nodes[0];
                n.ClearSearch();
            }

            isFiltered = false;
    }
      

        //private static void GetNodesValue(PromotionHierarchy n, string searchText)
        //{

        //    foreach (PromotionHierarchy child in n.Children)
        //    {
        //        child.PerformSearch(searchText.Split(' '));
        //        //if (child.UserName.ToLower().Contains(searchText))
        //        //{
        //        //    child.StringBackground = "#FFFF96";
                     
        //        //}
        //        //else
        //        //{
        //        //    child.StringBackground = "#ffffff";
                   
        //        //}

        //        //if (child.Children != null)
        //        //{
        //        //    GetNodesValue(child, searchText);
        //        //}
        //    }

        //}

        //private static void ClearNodesFilter(PromotionHierarchy n)
        //{

        //    foreach (PromotionHierarchy child in n.Children)
        //    { 
        //        child.StringBackground = "#ffffff";
        //        if (child.Children != null)
        //        {
        //            ClearNodesFilter(child);
        //        }
        //    }

        //}

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();

            }
        }


        //private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        //{
        //    var current = e.OriginalSource as RadTreeViewItem;
        //    var x = (RadTreeView)sender;
        //    if (x.Items.Count != 0)
        //    {
        //        //current.IsSelected = true;
        //        PromotionHierarchy c = current.Item as PromotionHierarchy;

        //        _viewModel.GetIsParentNode(c);
        //    }

        //}


        private void RadTreeView2_OnChecked(object sender, RadRoutedEventArgs e)
        {
            var current = e.OriginalSource as RadTreeViewItem;

            var x = (RadTreeView)sender;

            bool isInitiallyChecked = (e as RadTreeViewCheckEventArgs).IsUserInitiated;
            if (isInitiallyChecked || (x.Items.Count == 0))
            {
                //current.IsSelected = true;
                PromotionHierarchy c = current.Item as PromotionHierarchy;

                _viewModel.GetSelected(c);
                if (c.IsSelectedBool != true)
                {
                    c.IsSelected2 = false;
                }
                c.PerformExpand();
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchTextBox.Text = "";
            Search();
        }


        RadTreeViewItem clickedElement;
        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            // Find the tree item that is associated with the clicked context menu item
            clickedElement = (sender as RadContextMenu).GetClickedElement<RadTreeViewItem>();
        }
        private void RadTreeView2_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var current = e.OriginalSource as RadTreeViewItem;
            //var x = (RadTreeView)sender;
            //if (x.Items.Count != 0)
            //{
            //    //current.IsSelected = true;
            //    PromotionHierarchy c = current.Item as PromotionHierarchy;

            //    _viewModel.GetIsParentNode(c);
            //}
        }

        private void ModalListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var constraintType = ModalListBox.SelectedItem as ConstraintType;
            var saveButtonLabel = App.LanguageCache.FirstOrDefault().Value.AppLabels.FirstOrDefault(x => x.Code == "Button_Save");

            if (saveButtonLabel != null && constraintType != null)
                SaveConstraintsButton.Content = saveButtonLabel.Name + " " + constraintType.Name.ToLower();
        }
    }
}