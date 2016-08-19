using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.CellsGrid;
using Exceedra.Common.GroupingMenu;
using Exceedra.Common.Utilities;
using Exceedra.Controls.DynamicRow.Models;
using Exceedra.Controls.DynamicRow.ViewModels;
using Model;
using Model.Entity.Canvas;
using ViewHelper;
using ViewModels;
using WPF.Navigation;

namespace WPF.ViewModels.Canvas
{
    public interface IRowViewModelProvider
    {
        RowViewModel Get(XElement xml, string selectedInsightId);
    }

    public class RowViewModelProvider : IRowViewModelProvider
    {
        public RowViewModel Get(XElement column, string selectedInsightId)
        {
            RowViewModel columnViewModel = new RowViewModel(column);
            LoadDropdowns(columnViewModel, selectedInsightId);
            return columnViewModel;
        }

        private void LoadDropdowns(RowViewModel columnViewModel, string selectedInsightId)
        {
            // If a column needs to have other columns values assigned in order to have options to choose from loaded - we call that column a dependent column
            // If a column assigned value is used to provide data for another column that is dependent on it - we call that column a responsible column
            // Example:
            // [RESPONSIBLE] -> [DEPENDENT]
            // PROMO_START_DATE -> PROMO_NAME
            // PROMO_END_DATE -> PROMO_NAME
            // PROMO_STATUS -> PROMO_NAME

            // BIG HINT: if you mess up the order of the filters (the "more" dependent filter will be before the "less" dependent) - you'll break the stuff, man.

            Dictionary<string, string> respToDepCols = new Dictionary<string, string>();

            foreach (var prop in columnViewModel.Records.SelectMany(x => x.Properties).Where(y => y.DependentColumns != null && y.DependentColumns.Any()))
                foreach (var dependentColumn in prop.DependentColumns)
                    if (!respToDepCols.ContainsKey(prop.ColumnCode))
                        respToDepCols.Add(prop.ColumnCode, dependentColumn);

            // loading options for filters that are dropdowns
            foreach (var record in columnViewModel.Records)
                foreach (var filter in record.Properties)
                    if (!string.IsNullOrEmpty(filter.DataSource) &&
                        filter.ControlType.Contains("down"))
                    {
                        #region initial dropdown loading for dependent columns
                        // if the filter is dependent from any other column
                        if (respToDepCols.ContainsValue(filter.ColumnCode))
                        {
                            // get all of that responsible columns
                            var responsibleColumns =
                                columnViewModel.Records.SelectMany(x => x.Properties)
                                    .Where(y => y.DependentColumns.Contains(filter.ColumnCode));

                            bool allResponsibleColumnsAssigned = true;

                            // check if all of them have value or values
                            foreach (var responsibleColumn in responsibleColumns)
                                if (!responsibleColumn.HasValue()) allResponsibleColumnsAssigned = false;

                            if (!allResponsibleColumnsAssigned) continue;
                        }
                        #endregion

                        record.InitialDropdownLoad(filter, new Dictionary<string, string> { { "Canvas_Report_Idx", selectedInsightId } });
                    }
        }
    }

    public class CanvasViewModel : ViewModelBase
    {
        #region private fields

        private readonly ICanvasAccessor _canvasAccessor;
        private readonly IRowViewModelProvider _rowViewModelProvider;

        private GroupingMenu _insights;
        private Insight _selectedInsight;
        private ObservableCollection<RowViewModel> _filtersColumns;
        private CellsGridViewModel _cellsGrid;

        #endregion

        #region ctors

        public CanvasViewModel(ICanvasAccessor dataAccessor, IRowViewModelProvider rowViewModelProvider)
        {
            _canvasAccessor = dataAccessor;
            _rowViewModelProvider = rowViewModelProvider;
        }

        #endregion

        #region events

        public delegate void InsightChanged(object insight);

        public event InsightChanged SelectedInsightChanged;

        #endregion

        #region properties

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

        public Insight SelectedInsight
        {
            get { return _selectedInsight; }
            set
            {
                if (_selectedInsight == value) return;

                _selectedInsight = value;
                NotifyPropertyChanged(this, vm => vm.SelectedInsight);
                NotifyPropertyChanged(this, vm => vm.FiltersVisibility);

                // clearing the cells grid
                CellsGrid = null;
            }
        }

        public ObservableCollection<RowViewModel> FiltersColumns
        {
            get
            {
                return _filtersColumns;
            }
            set
            {
                if (_filtersColumns == value) return;
                _filtersColumns = value;

                NotifyPropertyChanged(this, vm => vm.FiltersColumns);

                AllColumns = _filtersColumns == null ? new List<RowRecord>() : FiltersColumns.SelectMany(filterColumn => filterColumn.Records).ToList();
            }
        }

        private List<RowRecord> AllColumns { get; set; }

        public CellsGridViewModel CellsGrid
        {
            get
            {
                return _cellsGrid;
            }
            set
            {
                if (_cellsGrid == value) return;

                _cellsGrid = value;
                NotifyPropertyChanged(this, vm => vm.CellsGrid);
                NotifyPropertyChanged(this, vm => vm.CellsGridVisibility);
            }
        }

        public Visibility FiltersVisibility
        {
            get
            {
                return SelectedInsight != null && SelectedInsight.HasFilters ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility CellsGridVisibility
        {
            get
            {
                return CellsGrid != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ICommand LoadInsightsCommand
        {
            get { return new ViewCommand(LoadInsights); }
        }

        /// <summary>
        /// If the insight has filters those will be loaded.
        /// Otherwise it will load the insight controls immediately.
        /// </summary>
        public ICommand LoadInsightCommand
        {
            get { return new ViewCommand(LoadInsight); }
        }

        public ICommand LoadCellsGridCommand
        {
            get { return new ViewCommand(CanLoadCellsGrid, LoadCellsGrid); }
        }

        public ICommand SaveDefaultsCommand
        {
            get { return new ViewCommand(CanSaveDefaults, SaveDefaults); }
        }

        #endregion

        #region private methods
        /// <summary>
        /// Loads insights and group them by GroupId
        /// </summary>
        private void LoadInsights(object obj)
        {
            var insights = GetInsightsFromDb();
            Insights = new GroupingMenu { MenuItems = new List<IMenuItem>(insights) };
        }

        private IList<Insight> GetInsightsFromDb()
        {
            return _canvasAccessor.GetInsights().Result;
        }

        public void HyperlinkClicked(string type, string idx, string content)
        {
            if (type == null) return;

            type = type.ToLowerInvariant();
            string appType = string.Empty;

            if (type.Contains("rob"))
            {
                appType = type.Replace("rob", "");
                type = "rob";
            }

            RedirectMe.Goto(type, idx, content, appType, "", true);
        }

        private void LoadInsight(object selectedInsight)
        {
            Insight loadedInsight = selectedInsight as Insight;
            if (loadedInsight == null) return;

            if (!SelectedInsightUrlCheck()) return;

            // if the selected insight has any filter
            // the app will load it
            if (loadedInsight.HasFilters) LoadFilters();

            // otherwise the app will proceed to load the insight controls immediately
            else LoadCellsGrid(loadedInsight);

            if (SelectedInsightChanged != null)
            {
                SelectedInsightChanged(loadedInsight);
                if (SelectedInsight.HasValidUrl)
                {
                    SelectedInsight = null;
                }
            }
        }

        private void LoadFilters()
        {
            //TODO: when dividing filters into columns will be moved to the dynamic row control we'll get new-control-view-model returned from GetFiltersFromDb() and we'll just assigned it to FiltersColumns
            //TODO: after that, it's important to change LoadingFilters unit test in InsightsV2Tests accordingly
            XElement filters = GetFiltersFromDb();
            ObservableCollection<XElement> filtersGroupedInColumns = GroupFiltersByColumns(filters);
            ObservableCollection<RowViewModel> columnsViewModels = ConvertColumnsToViewModels(filtersGroupedInColumns);

            FiltersColumns = columnsViewModels.OrderBy(column => column.Records.FirstOrDefault().Item_RowSortOrder).ToObservableCollection();

            // Attach resolving dependencies between columns functionality
            foreach (var column in AllColumns)
            {
                column.OtherRecordsProperties = AllColumns.Where(col => column != col).SelectMany(col => col.Properties).ToList();
                column.ResolvingInterColumnDependencies += ResolvingIntercolumnDependenciesHandler;
            }
        }

        private void ResolvingIntercolumnDependenciesHandler(RowRecord sourceRecord, RowProperty sourceProperty)
        {
            foreach (var dependentColumn in AllColumns)
            {
                // Don't notify the source record to update itself; only the rest of records
                if (dependentColumn == sourceRecord) continue;

                // The second arg "false" is to not send the ResolvingInterColumnDependencies event again (to avoid a circular dependency)
                dependentColumn.LoadDependentDrops(sourceProperty, false);
            }
        }

        private XElement GetFiltersFromDb()
        {
            var filtersTask = _canvasAccessor.GetFilters(SelectedInsight.Id);
            if (filtersTask.Result == null) FiltersColumns = null;
            return filtersTask.Result;
        }

        private ObservableCollection<RowViewModel> ConvertColumnsToViewModels(ObservableCollection<XElement> columnsWithFilters)
        {
            ObservableCollection<RowViewModel> convertedColumns = new ObservableCollection<RowViewModel>();

            foreach (var column in columnsWithFilters)
            {
                var convertedColumn = ConvertColumnToViewModel(column);
                convertedColumns.Add(convertedColumn);
            }

            return convertedColumns;
        }

        private RowViewModel ConvertColumnToViewModel(XElement column)
        {
            return _rowViewModelProvider.Get(column, SelectedInsight.Id);
        }

        private static ObservableCollection<XElement> GroupFiltersByColumns(XElement filters)
        {
            if (filters == null) throw new Exception("Passed filters XElement is null");

            ObservableCollection<XElement> filterColumns = new ObservableCollection<XElement>();

            foreach (var filter in filters.Elements("RootItem"))
                filterColumns.Add(new XElement("Results", filter));

            return filterColumns;
        }

        private XElement GetFiltersWithSelectedOptionsOnly()
        {
            // in case of not having any filter the app will return an empty results xml
            if (FiltersColumns == null || !FiltersColumns.Any()) return new XElement("Results");

            var filters = new XElement(
                new XElement("Results"));

            foreach (var RowVM in FiltersColumns)
            {
                filters.Add(XElement.Parse(RowVM.ToAttributeXml().ToString()));
            }

            //from filterColumn in FiltersColumns.SelectMany(x => x.Records)
            //select new XElement("RootItem",
            //    new XElement("Item_Idx", filterColumn.Item_Idx),
            //    new XElement("Item_Type", filterColumn.Item_Type),
            //    new XElement("HeaderText", filterColumn.HeaderText),
            //    new XElement("Item_RowSortOrder", filterColumn.Item_RowSortOrder),
            //    new XElement("Attributes",
            //        from filter in filterColumn.Properties
            //        select new XElement("Attribute",
            //            new XElement("ColumnCode", filter.ColumnCode),
            //                    FixIfDate(filter.GetXmlSelectedItems())
            //)))));

            return filters;
        }

        private bool CanLoadCellsGrid(object obj)
        {
            return
                SelectedInsightUrlCheck(obj)
                && HaveFiltersValues()
                && AreFiltersValid();
        }

        private bool SelectedInsightUrlCheck(object obj = null)
        {
            if (SelectedInsight != null && SelectedInsight.HasValidUrl)
                return false;

            return true;
        }

        private bool CanSaveDefaults(object obj)
        {
            return HaveFiltersValues()
                   && AreFiltersValid();
        }

        private bool HaveFiltersValues()
        {
            if (FiltersColumns == null) return false;
            return FiltersColumns.All(col => col.AreRecordsFulfilled());
        }

        private bool AreFiltersValid()
        {
            if (FiltersColumns == null || !FiltersColumns.Any()) return true;

            return FiltersColumns.All(filter => filter.AreRecordsValid);
        }

        private void LoadCellsGrid(object cellsGridViewModel)
        {
            XElement filters = GetFiltersWithSelectedOptionsOnly();

            CellsGrid = CellsGridViewModel.GetEmptyCellsGrid(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID, SelectedInsight.Id);

            CellsGrid.NoHorizontalCells = SelectedInsight.HorizontalCells;
            CellsGrid.NoVerticalCells = SelectedInsight.VerticalCells;

            CellsGrid.LoadControlsData(SelectedInsight.Id, filters);
        }

        private void SaveDefaults(object obj)
        {
            XElement filters = GetFiltersWithSelectedOptionsOnly();
            var message = _canvasAccessor.SaveDefaults(SelectedInsight.Id, filters).Result;

            if (!message.ToLower().Contains("error occurred"))
                MessageBoxShow(message);
        }

        #endregion
    }
}