using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Exceedra.Controls.DynamicGrid.Models;
using Exceedra.Controls.DynamicGrid.ViewModels;

using Exceedra.DynamicGrid.Models;
using Exceedra.DynamicTab.Converters;
using Model.Entity.ROBs;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using BoolToVisibilityConverter = Exceedra.Converters.BoolToVisibilityConverter;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using Clipboard = System.Windows.Clipboard;
using ComboBox = System.Windows.Controls.ComboBox;
using DataFormats = System.Windows.DataFormats;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
using DataObject = System.Windows.DataObject;
using FlowDirection = System.Windows.FlowDirection;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using Exceedra.Common.Utilities;
using Telerik.Windows.Controls;
using Exceedra.Common;
using Exceedra.Controls.Messages;
using Property = Exceedra.Controls.DynamicGrid.Models.Property;

namespace Exceedra.Controls.DynamicGrid.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DynamicGridControl : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Record> _records = new ObservableCollection<Record>();

        public ObservableCollection<Record> Records
        {
            get { return ItemDataSource == null ? null : ItemDataSource.Records; }
            //set
            //{
            //    _records = value;
            //    PropertyChanged.Raise(this, "Records");
            //    if(Records != null)
            //    GenerateColumns();
            //    dynGrid.CanUserAddRows = CanAddRow;

            //    PropertyChanged.Raise(this, "VisibleRecords");
            //}
        }

        private ObservableCollection<Record> _visibleRecords;

        public ObservableCollection<Record> VisibleRecords
        {
            get
            {
                GenerateColumns();


                if (ItemDataSource != null && ItemDataSource.Records != null)
                {
                    // JS: assigning a new collection every time when reading VisibleRecords causes the dyngrid to rerender (which doesn't seem to be a good practice; we also lose sorting...)
                    //_visibleRecords = ItemDataSource.Records.Where(rec => rec.Item_IsDisplayed).ToList();

                    // JS: that's why instead of reassigning the collection we would like to keep its reference and just manipulate its elements
                    if (_visibleRecords == null)
                        _visibleRecords = new ObservableCollection<Record>();

                    _visibleRecords.Clear();

                    foreach (var record in ItemDataSource.Records)
                        _visibleRecords.Add(record);

                    MenuOptions.ItemsSource =
                        new List<ContextMenuItem>(
                            _visibleRecords.Select(
                                r =>
                                    new ContextMenuItem(r.Item_Idx,
                                        string.IsNullOrWhiteSpace(r.Item_Name) ? r.Item_Type : r.Item_Name, false)));

                    if (_visibleRecords.Any()) IsEmpty = false;
                    else IsEmpty = true;

                    return _visibleRecords;
                }

                return null;

            }
        }

        private void OnCellLostFocus(object sender, RoutedEventArgs e)
        {
            // Check if the cell that has lost focus is editable - if not, do nothing
            var gridCell = sender as DataGridCell;
            if (gridCell == null) return;

            var lostFocusRecord = gridCell.DataContext as Record;
            if (lostFocusRecord == null) return;

            if (string.IsNullOrEmpty(gridCell.Column.SortMemberPath))
                return;

            var lostFocusColumnIndex = int.Parse(gridCell.Column.SortMemberPath.Substring("Properties[", "].ValueToSort", false));
            var lostFocusCell = lostFocusRecord.Properties[lostFocusColumnIndex];
            if (!lostFocusCell.IsEditable)
                return;

            // Check if dyngrid is sorted by the column containing the cell - if not, do nothing
            ICollectionView dynGridView = CollectionViewSource.GetDefaultView(dynGrid.ItemsSource);
            if (dynGridView == null) return;

            var isSortedByLostFocusColumn = dynGridView.SortDescriptions.Any(sortDesc => int.Parse(sortDesc.PropertyName.Substring("Properties[", "].ValueToSort", false)) == lostFocusColumnIndex);
            if (!isSortedByLostFocusColumn) return;

            // Scroll to the place where the edited record is after resorting
            dynGrid.ScrollIntoView(lostFocusRecord);
        }

        public ObservableCollection<Record> Results
        {
            get { return ItemDataSource == null ? null : ItemDataSource.Results; }
            //set
            //{
            //    _results = value;
            //    PropertyChanged.Raise(this, "Results");

            //    LoadingPanel.Message = (Records != null && Records.Any() ? "Complete" : "No data");
            //    LoadingPanel.SubMessage = (Records != null && Records.Any() ? Records.Count + " rows loaded" : "");
            //    LoadingPanel.Visibility = (Records != null && Records.Any() ? Visibility.Collapsed : Visibility.Visible);
            //    LoadingPanel.Complete = true;

            //    if (Results != null)
            //    GenerateTotalsColumns();
            //}
        }

        public ObservableCollection<HeaderRow> Headers
        {
            get { return ItemDataSource == null ? null : ItemDataSource.Headers; }
        }
        
        private ObservableCollection<string> _selectedItems = new ObservableCollection<string>();

        public ObservableCollection<string> selectedItems
        {
            get { return _selectedItems; }
            set
            {
                _selectedItems = value;
                PropertyChanged.Raise(this, "selectedItems");
            }
        }

        //private RecordViewModel _vm;

        public RecordViewModel ItemDataSource
        {
            get { return (RecordViewModel)GetValue(ItemDataSourceProperty); }
            set
            {
                SetValue(ItemDataSourceProperty, value);
               
            }
        }

        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(
            "IsEmpty", typeof(bool), typeof(DynamicGridControl), new PropertyMetadata(default(bool)));

        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }

        public static readonly DependencyProperty IsEmptyMessageProperty = DependencyProperty.Register(
            "IsEmptyMessage", typeof(string), typeof(DynamicGridControl), new PropertyMetadata(default(string)));

        public string IsEmptyMessage
        {
            get { return (string)GetValue(IsEmptyMessageProperty); }
            set { SetValue(IsEmptyMessageProperty, value); }
        }



        private void DynGridOnRowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs dataGridRowDetailsEventArgs)
        {
            //MessageBox.Show("yo");
        }


        public bool CanShowDetails
        {
            get
            {
                return (bool)GetValue(CanShowDetailsProperty);
            }
            set
            {
                SetValue(CanShowDetailsProperty, value);
                //GenerateColumns();
            }
        }

        public Visibility CanShow
        {
            get { return (Visibility)GetValue(CanShowProperty); }
            set { SetValue(CanShowProperty, value); }
        }

        public Visibility CanShowResults
        {
            get { return Results != null && Results[0].Properties.Any() ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyModeProperty); }
            set { SetValue(IsReadOnlyModeProperty, value); }
        }
        
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        public bool CanAddRow
        {
            get { return (bool)GetValue(CanAddProperty); }
            set { SetValue(CanAddProperty, value); }
        }

        public bool CanSort
        {
            get { return (bool)GetValue(CanSortProperty); }
            set
            {
                SetValue(CanSortProperty, value);
                SwitchSorting(value);
            }
        }

        private void SwitchSorting(bool value)
        {

            if (Records != null && Records.Any())
            {
                dynGrid.Columns.Where(r=>r.Visibility == Visibility.Visible).Do(t=>t.CanUserSort=value);
            }



        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }


        /* True: The grid should be able to scroll horizontally off the page when showing large data sets. 
         * False: The grid can only fill the designated horizontal area. 
         */

        public bool CanGridExtendArea
        {
            get { return (bool)GetValue(CanGridExtendAreaProperty); }
            set { SetValue(CanGridExtendAreaProperty, value); }
        }

        public DataGridHeadersVisibility ShowHeaders
        {
            get { return (DataGridHeadersVisibility)GetValue(ShowHeadersProperty); }
            set { SetValue(ShowHeadersProperty, value); }
        }


        public bool CanSelectRow
        {
            get { return (bool)GetValue(CanSelectRowProperty); }
            set { SetValue(CanSelectRowProperty, value); }
        }
        

        public RoutedEventHandler HyperLinkHandler
        {
            get { return (RoutedEventHandler)GetValue(HyperLinkHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(HyperLinkHandlerProperty, value);
                }
            }
        }

        public RoutedEventHandler LostFocusHandler
        {
            get { return (RoutedEventHandler)GetValue(LostFocusHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(LostFocusHandlerProperty, value);
                }
            }
        }

        public SelectionChangedEventHandler DropDownChangedHandler
        {
            get { return (SelectionChangedEventHandler)GetValue(DropDownChangedHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(DropDownChangedHandlerProperty, value);
                }
            }
        }

        public RoutedEventHandler DetailsViewHandler
        {
            get { return (RoutedEventHandler)GetValue(DetailsViewHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(DetailsViewHandlerProperty, value);
                }
            }
        }

        public RoutedEventHandler DeleteHandler
        {
            get { return (RoutedEventHandler)GetValue(DeleteHandlerProperty); }
            set
            {
                if (value != null)
                {
                    SetValue(DeleteHandlerProperty, value);
                }
            }
        }

        #region DependencyProperties

        public static readonly DependencyProperty ItemDataSourceProperty =
            DependencyProperty.Register("ItemDataSource", typeof(RecordViewModel),
                typeof(DynamicGridControl),
                new FrameworkPropertyMetadata()
                {
                    PropertyChangedCallback = OnDataChanged,
                    BindsTwoWayByDefault = true
                }
                );        

        public static readonly DependencyProperty LostFocusHandlerProperty =
            DependencyProperty.Register("LostFocusHandler", typeof(RoutedEventHandler),
                typeof(DynamicGridControl),
                null);

        public static readonly DependencyProperty HyperLinkHandlerProperty =
            DependencyProperty.Register("HyperLinkHandler", typeof(RoutedEventHandler),
                typeof(DynamicGridControl),
                null);

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string),
                typeof(DynamicGridControl),
                null
                );

        public static readonly DependencyProperty CanGridExtendAreaProperty =
            DependencyProperty.Register("CanGridExtendArea", typeof(bool),
                typeof(DynamicGridControl),
                null
                );

        /* Determines the visibility of the main grids headers */
        public static readonly DependencyProperty ShowHeadersProperty =
            DependencyProperty.Register("ShowHeaders", typeof(DataGridHeadersVisibility),
                typeof(DynamicGridControl),
                new PropertyMetadata(DataGridHeadersVisibility.Column)
                );

        public static readonly DependencyProperty CanSelectRowProperty =
            DependencyProperty.Register("CanSelectRow", typeof(bool),
                typeof(DynamicGridControl), 
                null                 
                );

        public static readonly DependencyProperty CanShowProperty =
            DependencyProperty.Register("CanShow", typeof(Visibility),
                typeof(DynamicGridControl),
                null);


        public static readonly DependencyProperty CanShowDetailsProperty =
            DependencyProperty.Register("CanShowDetails", typeof(bool),
                typeof(DynamicGridControl),
                new UIPropertyMetadata(MyPropertyChangedHandler));
 

        private static void MyPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get instance of current control from sender
            // and property value from e.NewValue

            // Set public property on TaregtCatalogControl, e.g.
            ((DynamicGridControl)d).CanShowDetails = (bool)e.NewValue;
        }


        public static readonly DependencyProperty CanShowResultsProperty =
            DependencyProperty.Register("CanShowResults", typeof(Visibility),
                typeof(DynamicGridControl),
                null);


        public static readonly DependencyProperty IsReadOnlyModeProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool),
                typeof(DynamicGridControl),
                null);
        
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double),
                typeof(DynamicGridControl),
                new UIPropertyMetadata(25.00));

        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register("CanAdd", typeof(bool),
                typeof(DynamicGridControl),
                null);

        public static readonly DependencyProperty CanSortProperty =
            DependencyProperty.Register("CanSort", typeof(bool),
                typeof(DynamicGridControl),
                 new FrameworkPropertyMetadata(
                 true,
                  // tell the binding system that this property affects how the control gets rendered
                  FrameworkPropertyMetadataOptions.AffectsRender,
                  // run this callback when the property changes
                  OnSortChanged
                  )
            );

        private static void OnSortChanged(DependencyObject control, DependencyPropertyChangedEventArgs eventArgs)
        {
            var c = (DynamicGridControl)control;
            c.CanSort = (bool)eventArgs.NewValue;
        }

        public static readonly DependencyProperty DropDownChangedHandlerProperty =
            DependencyProperty.Register("DropDownChangedHandler", typeof(SelectionChangedEventHandler),
                typeof(DynamicGridControl), null);


        public static readonly DependencyProperty DetailsViewHandlerProperty =
            DependencyProperty.Register("DetailsViewHandler", typeof(RoutedEventHandler),
                typeof(DynamicGridControl), null);


        public static readonly DependencyProperty DeleteHandlerProperty =
            DependencyProperty.Register("DeleteHandler", typeof(RoutedEventHandler),
                typeof(DynamicGridControl), null);


        public static readonly DependencyProperty DateTimeChangedHandlerProperty =
            DependencyProperty.Register("DateTimeChangedHandler", typeof(SelectionChangedEventHandler),
                typeof(DynamicGridControl), null);

        #endregion

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicGridControl)d).OnTrackerInstanceChanged(e);
        }


        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemDataSource != null)
            {
                /* Remove all columns when changing the DataContext. Allows the grid to get new templates */
                if (dynGrid != null && dynGrid.Columns != null) dynGrid.Columns.Clear();

                ItemDataSource.PropertyChanged += ViewModelPropertyChanged;             

                // The ItemDataSource is initialized (and the headers are generated in the view model) before the DynamicGridControl is - so before it subscribes to the PropertyChanged event of the view model.
                // That means the DynamicGridControl doesn't know that the headers were generated (PropertyChanged.Raise on Headers in the view model) so they were not rendered on the screen.
                // I added the code below simply to pick the header up.
                if (ItemDataSource.Headers != null)
                {
                    GenerateHeaderColumns();
                    PropertyChanged.Raise(this, "Headers");
                }

                if (_records != ItemDataSource.Records)
                {
                    GenerateTotalsColumns();
                    GenerateHeaderColumns();
                    GenerateColumns();
                    PropertyChanged.Raise(this, "VisibleRecords");
                    PropertyChanged.Raise(this, "Results");

                }
            }
            else
            {
                //Records = null;
                //Results = null;
            }
        }

        public DynamicGridControl()
        {
            /* Default to true */
            CanShowDetails = true;
            

            InitializeComponent();

            //Override the default ctrl+c command
            dynGrid.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyCtrlC));

            // A try to implement minimum widths of columns in the grid.
            // dynGrid.Loaded += (sender, args) => SetMinWidthToColumns();
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName.Equals("Records"))
            {
                //PropertyChanged.Raise(this, "Records");
                PropertyChanged.Raise(this, "VisibleRecords");
            }

            if (e.PropertyName.Equals("Results"))
            {
                GenerateTotalsColumns();
                PropertyChanged.Raise(this, "Results");
            }

            if (e.PropertyName.Equals("Headers"))
            {
                GenerateHeaderColumns();
                PropertyChanged.Raise(this, "Headers");
            }

            //if (e.PropertyName.Equals("PanelMainMessage"))
            //{
            //    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            //        LoadingPanel.Message = ItemDataSource.PanelMainMessage
            //        ));
            //}
            if (e.PropertyName.Equals("Filter"))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(FilterVisibleRows
                    ));
            }
            //if (e.PropertyName.Equals("GridTitle") && ItemDataSource != null)
            //{
            //    Header = ItemDataSource.GridTitle;
            //}
        }


        private void GenerateColumns()
        {
            if (Records != null && Records.Any())
            {
                var bgConvert = new IntToColourConverter();
                var bind = new Binding("HasChanges") { Converter = bgConvert };

                var baseRecord = Records.FirstOrDefault(r => r.Item_IsDisplayed);
                //If we don't have a visible record, try and build the columns using the first hidden record
                if (baseRecord == null)
                    baseRecord = Records.First();

                var columns = baseRecord.Properties;

                var propertyIndex = 0;


                /* If we've got equal column count, don't bother rebuilding all the columns. */
                /* A data context change will clear the columns so then they will be rebuilt */
                if (columns.Count() == dynGrid.Columns.Count) return;

                ContextMenuVisibleColumns.Items.Clear();
                dynGrid.Columns.Clear();
                dynGrid.Visibility = Visibility.Collapsed;

                // Even if the "CanShowDetails" property is enabled,
                // we won't show the column with "expand the inner grid" buttons
                // if none of the records has the inner grid
                if (CanShowDetails && Records.Any(rec => rec.HasInnerGrid))
                {
                    //add details open/close column

                    var objectIsNotNullToBool = new BoolToVisibilityConverter();
                    var expanderButtonVisibilityBinding = new Binding("HasInnerGrid") { Converter = objectIsNotNullToBool };
                    var isExpandedBinding = new Binding("IsDetailsViewModelVisible");

                    var d = GridColumns.GetDetailsViewButton(bind, expanderButtonVisibilityBinding, isExpandedBinding, GetDetailsHandler());

                    d.Width = new DataGridLength(25, DataGridLengthUnitType.Pixel);
                    d.CanUserSort = false;
 

                    dynGrid.Columns.Add(d);
                }
                else if (CanSelectRow)
                {
                    var d = GridColumns.GetSelectRowButton(SelectRowCommand());

                    d.Width = new DataGridLength(12, DataGridLengthUnitType.Pixel);
                    d.MaxWidth = 12;
                    d.CanUserSort = false;

                    dynGrid.Columns.Add(d);
                }

                /* ContextMenus do not reside in the visual tree so binding their visibility to a dep. prop. does not work
                 * Hence why I am controlling it here instead.
                 */
                ContextMenuSetValueTo.Visibility = CanSelectRow ? Visibility.Visible : Visibility.Collapsed;
                AddCommentMenuItem.Visibility = ItemDataSource.CanAddComments ? Visibility.Visible : Visibility.Collapsed;

                foreach (var column in columns)
                {
                    // JS: Commented out as it's breaking some events in dyngrid (i.e. lostfocus sends content as "disconnected item" instead of a record that has lost focus)
                    //if (!column.IsDisplayed)
                    //{
                    //    propertyIndex++;
                    //    continue;
                    //}

                    var align = "Right";
                    

                    /* If any column uses the star property, resizing columns will shrink the columns with this property and the grid width will remain constant.
                    * If no column uses the star property, resizing columns will expand the overall grid width.
                    */
                    var columnWidth = CanGridExtendArea
                        ? DataGridLength.Auto
                        : new DataGridLength(1, DataGridLengthUnitType.Star);

                    if (column.IsDisplayed && (column.FitWidth == "Content"))
                    {
                        columnWidth = new DataGridLength(1.2, DataGridLengthUnitType.SizeToCells);
                    }

                    if (column.IsDisplayed && (column.FitWidth == "Header"))
                    {
                        columnWidth = new DataGridLength(1.2, DataGridLengthUnitType.SizeToHeader);
                    }

                    if (column.IsDisplayed && !String.IsNullOrWhiteSpace(column.Width))
                    {
                        try
                        {
                            columnWidth = new DataGridLength(Convert.ToDouble(column.Width), DataGridLengthUnitType.Pixel);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    //if (column.IsDisplayed && (column.FitWidth == "Content"))
                    //{
                    //    string[] headerTextWords = column.HeaderText.Split(' ');
                    //    var longestHeaderTextLength = headerTextWords.OrderByDescending(a => a.Length).FirstOrDefault();

                    //    var pixelLengthOfString = MeasureTextSize(longestHeaderTextLength, this.FontFamily,
                    //        this.FontStyle, this.FontWeight, this.FontStretch, this.FontSize);

                    //    if (pixelLengthOfString.Width > columnWidth.Value)
                    //    {
                    //        columnWidth = new DataGridLength(1.2, DataGridLengthUnitType.SizeToHeader);
                    //    }
                    //}

                    column.HeaderText = column.HeaderText.Trim();
                    var allCheckboxes = column.ControlType.ToLower() == "checkbox" && column.HeaderText == "";// Records.All(rec => rec.Properties[propertyIndex].ControlType.ToLower().Equals("checkbox"));
                    var allDelete = column.ControlType.ToLower() == "delete" && column.HeaderText == "";
                    //   allCheckboxes ? new DataGridLength(30, DataGridLengthUnitType.Pixel) : columnWidth      
                    //Let's try only using a template....
                    DataGridTemplateColumn templateColumn = new DataGridTemplateColumn
                    {
                        Header = column.HeaderText,
                        Visibility = (column.IsDisplayed == false ? Visibility.Collapsed : Visibility.Visible),
                        IsReadOnly = !column.IsEditable,                        
                        //CanUserSort = CanSort,
                        Width = allCheckboxes ? new DataGridLength(20, DataGridLengthUnitType.Pixel) : allDelete ? new DataGridLength(25, DataGridLengthUnitType.Pixel) : columnWidth,
                        CellTemplateSelector = new GridCellSelector(propertyIndex)
                        {
                            CellFormat = column.StringFormat,
                            ColumnCode = column.ColumnCode,
                            HyperlinkHandler = GetHttpLinkHandler(),
                            NavigationHandler = GetNavigationLinkHandler(),
                            DropdownHandler = GetDropDownDependency(),
                            CheckBoxHandler = CheckboxHandler,
                            TextChangedHandler = TextChangedHandler,
                            LostFocusHandler = GenericLostFocusHandler,
                            TextFocusHandler = TextFocusHandler,
                            DeleteHandler = GetDeleteHandler()
                     
                        }

                    };
                    

                    if (CanSort && column.IsDisplayed)
                    {
                        templateColumn.SortMemberPath =string.Format("Properties[{0}].ValueToSort", propertyIndex);
                        templateColumn.CanUserSort = CanSort;
                    }

                    //string[] headerTextWords = column.HeaderText.Split(' ');
                    //var longestHeaderTextLength = headerTextWords.OrderByDescending(a => a.Length).FirstOrDefault();

                    //templateColumn.MinWidth = GetWidth(templateColumn, longestHeaderTextLength);

                    dynGrid.Columns.Add(templateColumn);


                    var menuItem = new MenuItem();
                    menuItem.Header = column.HeaderText;
                    menuItem.Tag = column.ColumnCode;
                    menuItem.Icon = new FontAwesome.WPF.FontAwesome {
                        Icon = FontAwesome.WPF.FontAwesomeIcon.Check,
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontSize = 10,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center                        
                    }; 
                    menuItem.Click += MenuItem_Click;
                    menuItem.StaysOpenOnClick = true;

                    if(!allCheckboxes && !allDelete && !string.IsNullOrWhiteSpace(column.HeaderText))
                        ContextMenuVisibleColumns.Items.Add(menuItem);

                    propertyIndex++;
                }

                //dynGrid.ItemsSource = VisibleRecords;
                dynGrid.IsReadOnly = IsReadOnly;
                dynGrid.Visibility = CanShow;
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;

            var column = dynGrid.Columns.First(c => c.Header == item.Header);
            var toShow = !(column.Visibility == Visibility.Visible);

            ((FontAwesome.WPF.FontAwesome)item.Icon).Visibility = toShow ? Visibility.Visible : Visibility.Collapsed;
            column.Visibility = toShow ? Visibility.Visible : Visibility.Collapsed;
            ItemDataSource.Records.Select(r => r.GetProperty((string)item.Tag)).Do(p => p.IsDisplayed = toShow);
        }

        /// <summary>
        /// The whole idea is to render the grid with the column widths set to auto (set in the view but commented out).
        /// Every column then takes only as much space as it needs to fully show its content (regarding cells and headers width).
        /// We would like to grab the (actual) width of the column in this moment and assign it as its minimum width.
        /// Then we would like to apply the width mechanism contained in the "finding the column width" region.
        /// Good luck.
        /// </summary>
        private void SetMinWidthToColumns()
        {
            if (Records == null || !Records.Any() || dynGrid.Columns == null || !dynGrid.Columns.Any())
                return;

            var columns = Records.First().Properties;

            //bool isEveryColumnRendered = columns.Count == dynGrid.Columns.Count;
            //if (!isEveryColumnRendered) return;

            foreach (var column in columns)
            {
                #region finding the column width

                var columnWidth = CanGridExtendArea
                    ? DataGridLength.Auto
                    : new DataGridLength(1, DataGridLengthUnitType.Star);

                if (column.IsDisplayed && (column.FitWidth == "Content"))
                    columnWidth = new DataGridLength(1.2, DataGridLengthUnitType.SizeToCells);

                if (column.IsDisplayed && (column.FitWidth == "Header"))
                    columnWidth = new DataGridLength(1.2, DataGridLengthUnitType.SizeToHeader);

                if (column.IsDisplayed && (column.Width != ""))
                {
                    try
                    {
                        columnWidth = new DataGridLength(Convert.ToDouble(column.Width), DataGridLengthUnitType.Pixel);
                    }
                    catch (Exception)
                    {

                    }
                }

                var allCheckboxes = column.ControlType.ToLower() == "checkbox" && column.HeaderText == "";// Records.All(rec => rec.Properties[propertyIndex].ControlType.ToLower().Equals("checkbox"));
                var allDelete = column.ControlType.ToLower() == "delete" && column.HeaderText == "";

                columnWidth = allCheckboxes
                    ? new DataGridLength(20, DataGridLengthUnitType.Pixel)
                    : allDelete
                        ? new DataGridLength(25, DataGridLengthUnitType.Pixel)
                        : columnWidth;

                #endregion

                // might be not the most agile piece of code you've ever seen
                // I'm sorry but I really had enough that day
                var equivalentColumnInDynGrid =
                    dynGrid.Columns.FirstOrDefault(
                        col =>
                            ((col as DataGridTemplateColumn).CellTemplateSelector as GridCellSelector).ColumnCode ==
                            column.ColumnCode);

                if (equivalentColumnInDynGrid == null)
                    continue;

                equivalentColumnInDynGrid.MinWidth = equivalentColumnInDynGrid.ActualWidth;
                equivalentColumnInDynGrid.Width = columnWidth;
            }
        }

        public double GetWidth(DataGridColumn column, string textToMeasure)
        {

            var pixelLengthOfString = MeasureTextSize(textToMeasure, this.FontFamily,
                    this.FontStyle, this.FontWeight, this.FontStretch, this.FontSize);

            if (pixelLengthOfString.Width > column.MinWidth)
            {
                return pixelLengthOfString.Width;
            }

            return column.MinWidth;
        }

        public Size MeasureTextSize(string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(fontFamily, fontStyle, FontWeights.Bold, fontStretch),
                                                 fontSize,
                                                 Brushes.Black);
            return new Size(ft.Width, ft.Height);
        }

        private void GenerateTotalsColumns()
        {
            reultsGrid.Columns.Clear();
            if (Results != null)
            {
                var columns = Results.FirstOrDefault();

                if (columns != null)
                {
                    //If we are showing the expander button to display inner grids, then we need to offset the totals by one column also.                    
                    if (CanShowDetails && Records.Any(rec => rec.HasInnerGrid))
                    {
                        reultsGrid.Columns.Add(new DataGridTextColumn() { IsReadOnly = true, Visibility = Visibility.Visible });
                    }

                        var i = 0;
                    foreach (var column in columns.Properties)
                    {
                        if (IsReadOnly)
                            column.IsEditable = false;

                        var binding = new Binding(string.Format("Properties[{0}].Value", i));
                        binding.StringFormat = column.StringFormat;
                        Style style = new Style(typeof(DataGridCell));
                        if (column.BorderColour != null)
                        {
                            style.Setters.Add(new Setter(BorderBrushProperty,
                                (SolidColorBrush)new BrushConverter().ConvertFrom(column.BorderColour)));
                            style.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(8, 1, 1, 1)));
                        }

                        if (column.BackgroundColour != null && column.BackgroundColour != "transparent")
                        {
                            style.Setters.Add(new Setter(BorderBrushProperty,
                                (SolidColorBrush)new BrushConverter().ConvertFrom(column.BorderColour)));
                            style.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(8, 1, 1, 1)));
                        }

                        var col = new DataGridTextColumn()
                        {
                            Binding = binding,
                            Visibility = (column.IsDisplayed == false ? Visibility.Collapsed : Visibility.Visible),
                            IsReadOnly = true
                        };
                        if (CanSort)
                        {
                            col.SortMemberPath = column.HeaderText;
                            col.CanUserSort = CanSort;
                        }
                        col.CellStyle = style;
                        // col.Width = new DataGridLength(100, DataGridLengthUnitType.Star);
                        reultsGrid.Columns.Add(col);

                        i += 1;
                    }
                }
            }
        }

        private void GenerateHeaderColumns()
        {
            headersGrid.Columns.Clear();
            headersGrid.Visibility = Visibility.Collapsed;

            if (Headers == null || !Headers.Any() || Headers[0].Operations == null || Records == null || !Records.Any()) return;

            var everyColumnWidth = new DataGridLength(100, DataGridLengthUnitType.Star);

            for (int i = 0; i < Headers[0].Operations.Count; i++)
            {
                var currentOperations = Headers.Select(header => header.Operations[i]);
                var currentOperation = currentOperations.FirstOrDefault(operation => !string.IsNullOrEmpty(operation.Type));

                if (currentOperation != null)
                {
                    DataGridTemplateColumn newColumn;

                    switch (currentOperation.Type.ToLower())
                    {
                        case "calculation":
                        case "constant":
                        case "prorate":

                            newColumn = GridColumns.GetHeaderOperationColumn(
                                new Binding(string.Format("Operations[{0}].Value", i))
                                {
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    BindsDirectlyToSource = true
                                },
                                Headers[0].Operations[i].OperationLabel,
                                TextChangedHandler2,
                                new Binding(string.Format("Operations[{0}].Key", i)),
                                new Binding(string.Format("Operations[{0}].Visibility", i)));

                            break;

                        case "combofilter":

                            // getting distinct values from the column of the ColumnCode property equal to 'ParentColumnCode'
                            currentOperation.HelperCollection =
                                new ObservableCollection<string>(
                                    Records.SelectMany(r => r.Properties)
                                        .Where(pr => pr.ColumnCode == currentOperation.ParentColumnCode && pr.SelectedItems != null)
                                        .SelectMany(p => p.Values)
                                        .Select(si => si.Item_Name)
                                        .Distinct()
                                    );

                            newColumn = GridColumns.GetComboColumn(
                                new Binding(string.Format("Operations[{0}].Value", i))
                                {
                                    Mode = BindingMode.TwoWay,
                                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                    BindsDirectlyToSource = true
                                },                                                                          // <- selected item
                                new Binding(string.Format("Operations[{0}].HelperCollection", i)),          // <- collection of items to select from
                                currentOperation.OperationLabel,                                            // <- name
                                FilterDropdownSelectionChangedHandler,                                      // <- selected item changed handler
                                currentOperation.ParentColumnCode,                                          // <- tag; used to identify column where the combo filter is
                                null,
                                new Binding(string.Format("Operations[{0}].Visibility", i))                 // <- visibility
                                );

                            break;

                        default: newColumn = new DataGridTemplateColumn(); break;
                    }

                    newColumn.Header = string.Empty;
                    newColumn.Width = everyColumnWidth;
                    newColumn.IsReadOnly = IsReadOnly;
                    newColumn.Visibility = Records.FirstOrDefault().Properties[i].IsDisplayed ? Visibility.Visible : Visibility.Collapsed;
                    headersGrid.Columns.Add(newColumn);
                }
                else if (i == 0)
                {
                    var operationColumn = new DataGridTextColumn
                    {
                        Header = "Operation",
                        Binding = new Binding(string.Format("Operations[{0}].OperationLabel", 0)),
                        CanUserSort = CanSort,
                        CellStyle = new Style(typeof(DataGridCell)),
                        Width = everyColumnWidth,
                        IsReadOnly = IsReadOnly
                    };
                    operationColumn.Visibility = Records.FirstOrDefault().Properties[i].IsDisplayed ? Visibility.Visible : Visibility.Collapsed;
                    headersGrid.Columns.Add(operationColumn);
                }
                else
                {
                    var blankColumn = GridColumns.GetBlankColumn();
                    blankColumn.Width = everyColumnWidth;
                    blankColumn.Visibility = Records.FirstOrDefault().Properties[i].IsDisplayed ? Visibility.Visible : Visibility.Collapsed;
                    blankColumn.IsReadOnly = IsReadOnly;

                    headersGrid.Columns.Add(blankColumn);
                }
            }

            headersGrid.ItemsSource = Headers;
            headersGrid.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            headersGrid.IsReadOnly = true;
        }

        private RoutedEventHandler GetDetailsHandler()
        {
            return DetailsViewHandler ?? Details_Click;
            ;
        }

        private RoutedEventHandler SelectRowCommand()
        {
            return SelectRowHandler;
            ;
        }

        private RoutedEventHandler GetDeleteHandler()
        {
            return DeleteHandler ?? Delete_Click;
            ;
        }

        private SelectionChangedEventHandler GetDropDownDependency()
        {
            return DropDownChangedHandler ?? IsSelectedHandler;
        }

        private RoutedEventHandler GetHttpLinkHandler()
        {
            //if (controltype == "ExternalHyperlink")
            //{
            //    return  GenericExternalLinkHandler;
            //}
           
            return (HyperLinkHandler ?? GenericLinkHandler);
        }

        private RoutedEventHandler GetNavigationLinkHandler()
        {
            return NavigationLinkHandler;
        }

        private RoutedEventHandler GetLostFocusHandler()
        {
            return (LostFocusHandler ?? GenericLostFocusHandler);
        }

        private void SelectRowHandler(object sender, RoutedEventArgs e)
        {
            int rowIdx = ItemDataSource.Records.Where(r => r.Item_IsDisplayed).IndexOf(((FrameworkElement) sender).DataContext as Record);

            var row = dynGrid.GetRow(rowIdx);

            dynGrid.SelectedCells.Clear();

            for (int i = 0; i < dynGrid.Columns.Count; i++)
            {
                var cell = dynGrid.GetCell(row, i);

                if(cell != null)
                dynGrid.SelectedCells.Add(new DataGridCellInfo(cell));
            }
        }

        private void GenericLostFocusHandler(object sender, RoutedEventArgs e)
        {
            if(ActOnLostFocus == false) return;

            Record obj = ((FrameworkElement)sender).DataContext as Record;

            if (obj == null) return;

            ItemDataSource.CalulateRecordColumns(obj);
            ItemDataSource.CalulateRecordColumnTotal(obj);
            //PropertyChanged.Raise(this, "VisibleRecords");
            PropertyChanged.Raise(this, "Results");
        }

        /* This is bool to set if we ever want editable textboxes to invoke an action when we lose focus.
         * In grids with large records (i.e. wide rows) not using this can cause a very noticable lag
         * Set this to false (in xaml) if you know your grid is editable and there are NO intercell connections
         */
        public bool? ActOnLostFocus { get; set; }

        /* We can't update the Value on every change as we only want to do formatting when we've finished typing 
         * So the only check we do on change is if it is valid and change foreground based on this.
         */
        private void TextChangedHandler(object sender, RoutedEventArgs e)
        {
            Record record = ((FrameworkElement)sender).DataContext as Record;
            var textBox = sender as TextBox;
            if (textBox != null && record != null)
            {
                BindingExpression bindingExpression = textBox.GetBindingExpression(ForegroundProperty);
                if (bindingExpression != null)
                {
                    var columnIdx = Convert.ToInt32(textBox.Tag);

                    if(columnIdx >= record.Properties.Count) return;

                    var thisCell = record.Properties[columnIdx];

                    thisCell.CheckValidEntry(textBox.Text);
                    bindingExpression.UpdateTarget();
                }
            }
        }

        private void TextChangedHandler2(object sender, RoutedEventArgs e)
        {
            HeaderOperationControl cb = (HeaderOperationControl)sender;
            HeaderRow obj = ((FrameworkElement)sender).DataContext as HeaderRow;

            ItemDataSource.UpdateColumn(cb.Text, cb.TagData);

            // need to call _vm.CalulateRecordColumnTotal(obj); to update results....
            obj = ItemDataSource.ExecuteOperations(obj);
            PropertyChanged.Raise(this, "VisibleRecords");
            PropertyChanged.Raise(this, "Results");

            //Results = ItemDataSource.Results;
        }

        private void FilterDropdownSelectionChangedHandler(object sender, RoutedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (cb != null)
            {
                string value = cb.SelectedItem != null ? cb.SelectedItem.ToString() : string.Empty;
                string column = cb.Tag != null ? cb.Tag.ToString() : string.Empty;

                ItemDataSource.FilterByColumn(column, value);
            }
        }

        private void TextFocusHandler(object sender, RoutedEventArgs e)
        {
            TextBox cb = (TextBox)sender;
            cb.SelectAll();
        }

        private void GenericSelectedHandler(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            var path = obj.Item_Type;
            var pathID = obj.Item_Idx;

            //  MessageBox.Show(pathID + "  " + cb.IsChecked.ToString() );
        }

        private void IsSelectedHandler(object sender, RoutedEventArgs e)
        {
            selectedItems = new ObservableCollection<string>(from p in _records select p.SelectedItems);
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            if (obj == null) return;

            if (sender.GetType() == typeof(CheckBox))
                obj.HasChanges = 1;

            if (sender.GetType() == typeof(ComboBox) && !String.IsNullOrWhiteSpace(((ComboBox)sender).Text))
                obj.HasChanges = 1;
        }

        // For checkboxes within grids to fire off recalculations
        /* Done in a rush and it looks like the dropdown and checkboxes are sharing the selected items list which may cause problems.
         */
        private void CheckboxHandler(object sender, RoutedEventArgs e)
        {
            IsSelectedHandler(sender, e);

            Record obj = ((FrameworkElement)sender).DataContext as Record;
            if (obj == null) return;

            var columnIdx = Convert.ToInt32(((CheckBox)sender).Tag);

            var thisCell = obj.Properties[columnIdx];

            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                if (thisCell.Value == checkBox.IsChecked.ToString())
                {
                    e.Handled = true;
                    return;
                }


                BindingExpression bindingExpression = checkBox.GetBindingExpression(CheckBox.IsCheckedProperty);
                bindingExpression.UpdateSource();

            }

            if(ItemDataSource != null)
            ItemDataSource.CalulateRecordColumns(obj);
        }

      //  private List<Property> SelectedCells { get; set; }


        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Record;

            if (obj != null && obj.DetailsViewModel == null)
                obj.LoadExpandedGrid();

            // the original source is what was clicked.  For example 
            // a button.
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            var btn = (Button)e.OriginalSource;

            // iteratively traverse the visual tree upwards looking for
            // the clicked row.
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            // if we found the clicked row
            if (dep is DataGridRow)
            {
                DataGridRow row = (DataGridRow)dep;
                Record rec = (Record)row.Item;

                if (row.DetailsVisibility == Visibility.Collapsed)
                {
                    row.DetailsVisibility = Visibility.Visible;
                    rec.IsDetailsViewModelVisible = true;
                    //btn.Content = "-";
                }
                else
                {
                    row.DetailsVisibility = Visibility.Collapsed;
                    rec.IsDetailsViewModelVisible = false;
                    //btn.Content = "+";
                }

                PropertyChanged.Raise(this, string.Format("DetailsVisibility,{0}", ((Record)row.Item).Item_Idx));

            }
        }

        //public void ExpandDetailView(string inputIdx)
        //{
        //    var record = Records.FirstOrDefault(a => a.Item_Idx == inputIdx);

        //    //var buttons = dynGrid.ChildrenOfType<Button>();

        //    //ButtonAutomationPeer = new ButtonAutomationPeer();
        //}

        //private void Details_Click(object sender, RoutedEventArgs e)
        //{
        //    var obj = ((FrameworkElement)sender).DataContext as Record;

        //    if (obj == null) return;
        //    if (obj.DetailsViewModel == null) obj.LoadExpandedGrid();

        //    obj.IsDetailsViewModelVisible = !obj.IsDetailsViewModelVisible;

        //    var btn = (Button)e.OriginalSource;
        //    // the original source is what was clicked. For example a button.

        //    if (obj.IsDetailsViewModelVisible) btn.Content = "-";
        //    else btn.Content = "+";
        //}

        //public void ExpandDetailView(string inputIdx)
        //{
        //    var record = Records.FirstOrDefault(a => a.Item_Idx == inputIdx);

        //    //var buttons = dynGrid.ChildrenOfType<Button>();

        //    //ButtonAutomationPeer = new ButtonAutomationPeer();
        //}



        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((sender as ComboBox).SelectedItem) as ImpactOption;

            // _viewModel.UpdateImapctFormat(selectedItem);
        }



        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Record;
            var current = obj.HasChanges;

            if (current == 2) obj.HasChanges = 0;
            else obj.HasChanges = 2;

            // just to raise the property changed event on records
            // so the view will know that it has to mark the row as "to delete"
            PropertyChanged.Raise(this, "VisibleRecords");
            //ItemDataSource.NotifyRecordsChanged();
        }

        private void GenericLinkHandler(object sender, RoutedEventArgs e)
        {
            Record selectedRow = ((FrameworkElement) sender).DataContext as Record;
            if (selectedRow == null) return;

            string selectedColumnCode = (e.OriginalSource as Button)?.Tag.ToString();
            Property selectedColumn = selectedRow.GetProperty(selectedColumnCode);

            var itemId = selectedRow.GetId();
            var itemValue = ((Button)sender).Content;
            bool isNavigatingUsingPopup = (Keyboard.Modifiers & ModifierKeys.Control) > 0;

            string methodName;
            object[] methodParameters;

            if (selectedColumn?.ColumnCode != null && selectedColumn.ColumnCode.ToLower().EndsWith("location"))
            {
                methodName = "OpenScanLocation";
                methodParameters = new object[] {selectedColumn.External_Data};
            }
            else
            {
                methodName = "HyperlinkClicked";
                methodParameters = new [] { selectedRow.Item_Type, itemId, itemValue, "", isNavigatingUsingPopup };

                var parentPage = ReflectionExtensions.GetParentPage(this);

                //Special case: ROBs
                if (((Page)parentPage).GetType().ToString().ToLower().Contains("eventspage"))
                {
                    var appTypeIdx = ((Page)parentPage).GetType().GetProperty("AppTypeID").GetValue(parentPage, null);
                    methodParameters[3] = appTypeIdx;
                }
            }

            ReflectionExtensions.InvokeMethodOnParentPage(this, methodName, methodParameters);
        }

        private void GenericExternalLinkHandler(object sender, RoutedEventArgs e)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            var path = obj.Item_Type;

            var idxColumn = obj.Properties.FirstOrDefault(t=>t.ControlType.ToLower() == "externalhyperlink");

            if (idxColumn != null && !idxColumn.External_Data.IsEmpty() )
            {
                Process.Start(idxColumn.External_Data);
            }
            else
            {
                CustomMessageBox.ShowOK("No file found in data","Information","Ok");
            }
        }

        private void NavigationLinkHandler(object sender, RoutedEventArgs e)
        {
            Record obj = ((FrameworkElement)sender).DataContext as Record;

            var path = obj.Item_Type;

            var extData = ((Button)sender).Tag;
             
            bool PopOutNavigation = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
        
            var parentPage = VisualTreeHelper.GetParent(this);
            while (!(parentPage is Page))
            {
                parentPage = VisualTreeHelper.GetParent(parentPage);
            }

            //Get the linkHandler method used by the parent ROB List page
            var method = parentPage.GetType().GetMethod("NavigationlinkClicked");
            object[] param = { path, extData, PopOutNavigation};
            if (method != null)
                method.Invoke(parentPage, param);
               
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;

            // Perform only for Checkbox Column types
            if (cell.Column is DataGridCheckBoxColumn)
            {
                if (cell != null &&
                    !cell.IsEditing)
                {
                    if (!cell.IsFocused)
                    {
                        cell.Focus();
                    }
                    var dataGrid = FindVisualParent<DataGrid>(cell);
                    if (dataGrid != null)
                    {
                        if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                        {
                            if (!cell.IsSelected)
                                cell.IsSelected = true;
                        }
                        else
                        {
                            var row = FindVisualParent<DataGridRow>(cell);
                            if (row != null && !row.IsSelected)
                            {
                                row.IsSelected = true;
                            }
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                var correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        private void dynGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (dynGrid.Columns.Count > 0 && reultsGrid.Columns.Count > 0)
            {
                for (int i = 0; i < dynGrid.Columns.Where(c => c.Visibility == Visibility.Visible).Count() && i < reultsGrid.Columns.Where(c => c.Visibility == Visibility.Visible).Count(); ++i)
                {
                    reultsGrid.Columns.Where(c => c.Visibility == Visibility.Visible).ElementAt(i).Width = dynGrid.Columns.Where(c => c.Visibility == Visibility.Visible).ElementAt(i).ActualWidth;
                }
            }

            if (dynGrid.Columns.Count > 0 && headersGrid.Columns.Count > 0)
            {
                for (int i = 0; i < dynGrid.Columns.Where(c => c.Visibility == Visibility.Visible).Count() && i < headersGrid.Columns.Where(c => c.Visibility == Visibility.Visible).Count(); ++i)
                {
                    headersGrid.Columns.Where(c => c.Visibility == Visibility.Visible).ElementAt(i).Width = dynGrid.Columns.Where(c => c.Visibility == Visibility.Visible).ElementAt(i).ActualWidth;
                }
            }
        }

        private void FilterVisibleRows()
        {
            if (ItemDataSource != null && ItemDataSource.Records != null)
            {
                SetRowVisibilityByFilterText();
            }
        }

        private void SetRowVisibilityByFilterText()
        {
            if (ItemDataSource.Records.Count == 0) return;

            GetVisibleRows();
            //GetVisibleRows(dynGrid)
            //    .ToList()
            //    .ForEach(
            //        x =>
            //        {
            //            if (x == null) return;

            //            x.Visibility =
            //                DataMatchesFilterText(x.Item as Record) ? Visibility.Visible : Visibility.Collapsed;
            //        });

        }

        private const string FilterWatermark = "Filter...";

        private bool DataMatchesFilterText(Record data)
        {
            if (ItemDataSource.Filter == FilterWatermark || string.IsNullOrWhiteSpace(ItemDataSource.Filter)) return true;

            return data != null && data.Properties.Any(p => p.Value.Contains(ItemDataSource.Filter, false));
        }

        public void GetVisibleRows()
        {
            if (VisibleRecords == null) return;

            foreach (var visibleRecord in VisibleRecords)
            {
                visibleRecord.Item_IsDisplayed = DataMatchesFilterText(visibleRecord);
            }
        }

        private bool _isScrolling;
        private void DynGridScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isScrolling) return;
            _isScrolling = true;

            // Due to the grid virtualisation the grid row is generated only when it'd be visible on the screen.
            // Therefore, everytime when a user scrolls the grid it generates another rows
            // and that's why it's needed to apply the filter again
            //FilterVisibleRows();

            _isScrolling = false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility =
                        row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #region Copy/Export

        private void ExportCsv(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            bool withHeaders = menuItem != null && (string)menuItem.Tag == "Headers";

            var selectedCellsAsCsv = ConvertCellsToCsvString(dynGrid, withHeaders);

            CopiedCellsToCsvFile(selectedCellsAsCsv);
        }
       

        private string ConvertCellsToCsvString(DataGrid grid, bool withHeaders)
        {
            List<string> columns = new List<string>();
            List<int> rowIdxs = new List<int>();
            List<List<string>> rows = new List<List<string>>();

            //TODO: only give user 'Copy' menu option if they have selected some cells

            foreach (var cell in grid.SelectedCells)
            {
                if (cell.IsValid && cell.Column.Visibility == Visibility.Visible)
                {
                    var templateSelector = (cell.Column as DataGridTemplateColumn).CellTemplateSelector;
                    if (templateSelector == null) continue;

                    var columnIndex = (templateSelector as GridCellSelector).ColumnIdx;
                    var rowIndex = Convert.ToInt32(((Record)cell.Item).Item_RowSortOrder);

                    //If unique column, add it!
                    if (grid.Columns[columnIndex].Header != null && !columns.Contains(grid.Columns[columnIndex].Header.ToString()))
                        columns.Add(grid.Columns[columnIndex].Header.ToString());

                    //Get cell value                    
                    var content = ((Record)cell.Item).Properties[columnIndex].Value;
                    if (content.Contains(",")) content = "\"" + content + "\"";

                    //If we don't have a cell from this row yet, add the row.
                    if (!rowIdxs.Contains(rowIndex))
                    {
                        rows.Add(new List<string>());
                        rowIdxs.Add(rowIndex);
                    }

                    rows[rowIdxs.IndexOf(rowIndex)].Add(content);
                }
            }

            int rowLength = rows.Any() ? rows[0].Count : 0;
            if (rows.Any(row => row.Count != rowLength))
            {
                MessageBox.Show("Please make symmetrical selection", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return "";
            }

            var outputString = "";

            if (!String.IsNullOrWhiteSpace(Header))
                outputString = Header + "\n";

            outputString += withHeaders ? columns.ToString(",") + "\n" : "";
            outputString = rows.Aggregate(outputString, (current, row) => current + (row.ToString(",") + "\n"));
            outputString += " \n";
            return outputString;
        }

        private void CopyCsv(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            bool withHeaders = menuItem != null && (string)menuItem.Tag == "Headers";

            var selectedCellsAsCsv = ConvertCellsToCsvString(dynGrid, withHeaders);

            CopiedCellsToClipboard(selectedCellsAsCsv);
        }

        private void CopiedCellsToCsvFile(string input)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV Comma Delimited |*.csv";

            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                string[] rows = input.Split('\n');

                var last = rows.Last();
                foreach (var r in rows)
                {
                    var newline = r.Equals(last) ? r : string.Format("{0}{1}", r, Environment.NewLine);
                    File.AppendAllText(fileName, newline, Encoding.GetEncoding(1252));
                }
            }
        }

        private void CopiedCellsToClipboard(string input)
        {
            var dataObject = new DataObject();

            var bytes = Encoding.GetEncoding(1252).GetBytes(input);
            var stream = new MemoryStream(bytes);
            dataObject.SetData(DataFormats.CommaSeparatedValue, stream);

            Clipboard.SetDataObject(dataObject, true);
        }

        private void CopyCtrlC(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedCellsAsCsv = ConvertCellsToCsvString(dynGrid, false);

            CopiedCellsToClipboard(selectedCellsAsCsv);
        }



        #endregion

        /* Used to switch between the 'set value' or 'use row' in the context menu */
        public void ShowHideMeasures_Click(object sender, EventArgs e)
        {
            var clickedItem = sender as MenuItem;

            if (clickedItem.Name == "rdnMeasure")
            {
                if (rdnSetValue.IsChecked)
                    rdnSetValue.IsChecked = false;
            }
            else if (rdnMeasure.IsChecked)
            {
                rdnMeasure.IsChecked = false;
            }

            MenuOptions.IsEnabled = rdnMeasure.IsChecked;
        }

        //private ICommand _contextMenuApplyCommand;
        //public ICommand ContextMenuApplyCommand
        //{
        //    get { return _contextMenuApplyCommand ?? (_contextMenuApplyCommand = new CommandHandler(CanApplyContextMenu, ContextMenuApply)); }
        //}

        public bool CanApplyContextMenu(object o)
        {
            /* If they are using an existing rows values */
            if (rdnMeasure.IsChecked)
            {
                /* Get the record we want to grab values from */
                var contextMenuItem = (ContextMenuItem)MenuOptions.SelectionBoxItem;
                return contextMenuItem != null && txtMassAmendValue.Text.Replace("%", "").IsNumeric();
            }
            else if (rdnSetValue.IsChecked)
            {
                return txtMassAmendValue.Text.IsNumeric();
            }

            return false;
        }

        public void ContextMenuApply(object sender, RoutedEventArgs e)
        {
            /* If they are using an existing rows values */
            if (rdnMeasure.IsChecked)
            {
                var pairings = GetSelectedEditableCellsAndRecordsPairings();

                /* Get the record we want to grab values from */
                var name = ((ContextMenuItem) MenuOptions.SelectionBoxItem).DisplayName;
                var valuesFromRecord = ItemDataSource.Records.First(r => r.Item_Type == name || r.Item_Name == name);

                var percChange = txtMassAmendValue.Text.Replace("%", "");

                if (percChange.IsNumeric())
                foreach (var t in pairings)
                {
                    if(valuesFromRecord.Properties[t.Item1].Value.IsNumeric())
                        t.Item2.Properties[t.Item1].Value = (valuesFromRecord.Properties[t.Item1].Value.AsNumeric() * (percChange.AsNumeric() + 100) / 100).ToString();
                }
            }
            else if (rdnSetValue.IsChecked)
            {
                var pairings = GetSelectedEditableCellsAndRecordsPairings();

                foreach (var t in pairings)
                {
                    t.Item2.Properties[t.Item1].Value = txtMassAmendValue.Text;
                }

            }
        }

        /* For all the selected cells:
         * - get the ones generated by the GridCellSelector  
         * - Pair them by ColumnIdx and Record (row)
         * - Filter out the non-editable ones
         */
        private IEnumerable<Tuple<int, Record>> GetSelectedEditableCellsAndRecordsPairings()
        {
            var test =
                dynGrid.SelectedCells.Where(c => c.Column.GetType() == typeof(DataGridTemplateColumn)
                && ((DataGridTemplateColumn)c.Column).CellTemplateSelector != null
                && ((DataGridTemplateColumn)c.Column).CellTemplateSelector.GetType() == typeof(GridCellSelector))
                .Select(
                    c => new Tuple<int, Record>(((GridCellSelector)((DataGridTemplateColumn)c.Column).CellTemplateSelector).ColumnIdx, (Record)c.Item));

            test = test.Where(c => c.Item2.Properties[c.Item1].IsEditable);

            return test;
        }


        #region Filter Textboxes
 
        private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;
            if (filterTextBox.Text.Equals(FilterWatermark))
            {
                filterTextBox.Clear();
                filterTextBox.Foreground = Brushes.Black;
            }
            else
            {
                filterTextBox.SelectAll();
            }
        }

        private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (string.IsNullOrWhiteSpace(filterTextBox.Text) && ItemDataSource != null)
            {
                ItemDataSource.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        private void FilterTextBox_TextChanged(object sender, KeyEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                ItemDataSource.Filter = filterTextBox.Text;
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        #endregion

        /* Some dodgy code for tabbing into a EditableTextBlock and going straight into it's edit mode. */
        private void DynGrid_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Tab && e.Key != Key.Enter) return;

            dynGrid.UnselectAllCells();

            //DependencyObject dep = (DependencyObject)e.OriginalSource;

            //while ((dep != null) && !(dep is DataGridCell))
            //{
            //    dep = VisualTreeHelper.GetParent(dep);
            //}

            //if (dep == null)
            //    return;

            //if (dep is DataGridCell)
            //{
            //    //cancel if datagrid in edit mode
            //    dynGrid.CancelEdit();
            //    //get current cell
            //    DataGridCell cell = dep as DataGridCell;
            //    //deselect current cell
            //    cell.IsSelected = false;
            //    //find next right cell
            //    var nextCell = cell.PredictFocus(FocusNavigationDirection.Right);
            //    //if next right cell null go for find next ro first cell
            //    if (nextCell == null)
            //    {
            //        return;

            //        //DependencyObject nextRowCell;
            //        //nextRowCell = cell.PredictFocus(FocusNavigationDirection.Down);
            //        ////if next row is null so we have no more row Return;
            //        //if (nextRowCell == null) return;
            //        ////we do this because we cant use FocusNavigationDirection.Next for function PredictFocus
            //        ////so we have to find it this way
            //        //while ((nextRowCell as DataGridCell).PredictFocus(FocusNavigationDirection.Left) != null)
            //        //    nextRowCell = (nextRowCell as DataGridCell).PredictFocus(FocusNavigationDirection.Left);
            //        ////set new cell as next cell
            //        //nextCell = nextRowCell;
            //    }

            //    while ((nextCell != null) && !(nextCell is DataGridCell))
            //    {
            //        nextCell = VisualTreeHelper.GetParent(nextCell);
            //    }

            //    var desiredChild = nextCell.ChildrenOfType<TextBox>().FirstOrDefault();

            //    if (desiredChild == null) return;

            //    //change current cell
            //    dynGrid.CurrentCell = new DataGridCellInfo(nextCell as DataGridCell);
            //    //change selected cell
            //    (nextCell as DataGridCell).IsSelected = true;
            //    (nextCell as DataGridCell).IsEditing = true;
            //    //if (desiredChild != null)
            //    //    desiredChild.ToEditMode(null);

            //    // start edit mode
            //    dynGrid.BeginEdit();
            //}
        }
        
        private void btnAddCommentApply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtComment.Text)) return;

            var pairings = GetSelectedEditableCellsAndRecordsPairings();

            foreach (var t in pairings)
            {
                t.Item2.Properties[t.Item1].AddComment(TxtComment.Text);                
            }

            TxtComment.Text = "";
        }

        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hit = VisualTreeHelper.HitTest((Visual)sender, e.GetPosition((IInputElement)sender));
            DependencyObject cell = VisualTreeHelper.GetParent(hit.VisualHit);
            while (cell != null && !(cell is DataGridCell)) cell = VisualTreeHelper.GetParent(cell);
            DataGridCell targetCell = cell as DataGridCell;

            if (targetCell == null || targetCell.IsSelected) return;

            dynGrid.UnselectAllCells();
            targetCell.IsSelected = true;
        }

        /* This is to enable up/down arrow key navigation when inside a cells editable element e.g. textbox, datepicker etc.
         * Left/right navigation should not be allowed as that is useful for editing the internal data.
         */
        private void dynGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                int rowIdx = ItemDataSource.Records.Where(r => r.Item_IsDisplayed).IndexOf(((DataGrid)sender).CurrentCell.Item);

                var cell = GetSelectedCell(dynGrid, rowIdx);
                //If null, then we are editing a template item, e.g. textbox, datepicker... and we need to invoke the arrow key press on the containing cell.l
                if (cell == null)
                {
                    var column = dynGrid.CurrentColumn.DisplayIndex;
                    cell = dynGrid.GetCell(dynGrid.GetRow(rowIdx), column);
                    if (cell != null)
                    {
                        cell.RaiseEvent(
                            new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(cell), 0, e.Key)
                            {
                                RoutedEvent = Keyboard.KeyDownEvent
                            });
                        e.Handled = true;
                    }
                }
            }

        }

        private void dynGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (ItemDataSource == null) return;

            int rowIdx = ItemDataSource.Records.Where(r => r.Item_IsDisplayed).IndexOf(((DataGrid)sender).CurrentCell.Item);

            var cell = GetSelectedCell(dynGrid, rowIdx);
            if (cell != null)
            {
                cell.Focus();
                TextBox textbox = GetVisualChild<TextBox>(cell);
                if (textbox != null)
                {   //TextBox has benn found
                    if ((textbox as TextBox).IsFocused == false)
                    {
                        (textbox as TextBox).SelectAll();
                        (textbox as TextBox).Focus();
                    }
                }
            }

        }

        public static DataGridCell GetSelectedCell(DataGrid grid, int rowIdx)
        {
            var row = grid.GetRow(rowIdx);
            if (row != null)
            {
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    var cell = grid.GetCell(row, i);
                    if (cell != null && cell.IsFocused)
                        return cell;
                }
            }
            return null;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }

    public class GridExportObject
    {
        public List<string> Columns = new List<string>();
        public List<List<string>> Rows = new List<List<string>>();

        private readonly List<int> _rowIdxs = new List<int>();

        public GridExportObject(DataGrid grid)
        {
            grid.SelectAllCells();
            foreach (var cell in grid.SelectedCells)
            {
                if (cell.IsValid && cell.Column.Visibility == Visibility.Visible)
                {
                    var columnIndex = ((cell.Column as DataGridTemplateColumn).CellTemplateSelector as GridCellSelector).ColumnIdx;
                    var rowIndex = Convert.ToInt32(((Record)cell.Item).Item_RowSortOrder);

                    //If unique column, add it!
                    if (!Columns.Contains(grid.Columns[columnIndex].Header.ToString()))
                        Columns.Add(grid.Columns[columnIndex].Header.ToString());

                    //Get cell value                    
                    var content = ((Record)cell.Item).Properties[columnIndex].Value;
                    if (content.Contains(",")) content = "\"" + content + "\"";

                    //If we don't have a cell from this row yet, add the row.
                    if (!_rowIdxs.Contains(rowIndex))
                    {
                        Rows.Add(new List<string>());
                        _rowIdxs.Add(rowIndex);
                    }

                    Rows[_rowIdxs.IndexOf(rowIndex)].Add(content);
                }
            }
            grid.UnselectAllCells();
        }

    }

    public class ScrollSynchronizer : DependencyObject
    {
        public static readonly DependencyProperty ScrollGroupProperty =
            DependencyProperty.RegisterAttached(
            "ScrollGroup",
            typeof(string),
            typeof(ScrollSynchronizer),
            new PropertyMetadata(new PropertyChangedCallback(
            OnScrollGroupChanged)));

        public static void SetScrollGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(ScrollGroupProperty, scrollGroup);
        }

        public static string GetScrollGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(ScrollGroupProperty);
        }

        private static Dictionary<ScrollViewer, string> scrollViewers =
            new Dictionary<ScrollViewer, string>();

        private static Dictionary<string, double> horizontalScrollOffsets =
            new Dictionary<string, double>();

        private static Dictionary<string, double> verticalScrollOffsets =
            new Dictionary<string, double>();

        private static void OnScrollGroupChanged(DependencyObject d,
                    DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer != null)
            {
                if (!string.IsNullOrEmpty((string)e.OldValue))
                {
                    // Remove scrollviewer
                    if (scrollViewers.ContainsKey(scrollViewer))
                    {
                        scrollViewer.ScrollChanged -=
                          new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                        scrollViewers.Remove(scrollViewer);
                    }
                }

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    // If group already exists, set scrollposition of 
                    // new scrollviewer to the scrollposition of the group
                    if (horizontalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToHorizontalOffset(
                                      horizontalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        horizontalScrollOffsets.Add((string)e.NewValue,
                                                scrollViewer.HorizontalOffset);
                    }

                    if (verticalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToVerticalOffset(verticalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        verticalScrollOffsets.Add((string)e.NewValue, scrollViewer.VerticalOffset);
                    }

                    // Add scrollviewer
                    scrollViewers.Add(scrollViewer, (string)e.NewValue);
                    scrollViewer.ScrollChanged +=
                        new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);
                }
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0 || e.HorizontalChange != 0)
            {
                var changedScrollViewer = sender as ScrollViewer;
                Scroll(changedScrollViewer);
            }
        }

        private static void Scroll(ScrollViewer changedScrollViewer)
        {
            var group = scrollViewers[changedScrollViewer];
            verticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;
            horizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

            foreach (var scrollViewer in scrollViewers.Where((s) => s.Value ==
                                              group && s.Key != changedScrollViewer))
            {
                if (scrollViewer.Key.VerticalOffset != changedScrollViewer.VerticalOffset)
                {
                    scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
                }

                if (scrollViewer.Key.HorizontalOffset != changedScrollViewer.HorizontalOffset)
                {
                    scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
                }
            }
        }
    }

}