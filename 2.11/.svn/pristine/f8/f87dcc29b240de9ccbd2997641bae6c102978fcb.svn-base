
using Exceedra.Controls.Messages;
using Model.Entity.Listings;
using Telerik.Pivot.Core;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;


namespace WPF
{
    using System;

    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Model;

    using global::ViewModels;

    using WPF.TelerikHelpers;
    using WPF.ViewModels.Insights2;
    using Telerik.Pivot.Adomd;
    using Telerik.Windows.Documents.Spreadsheet.Model;
    using System.Windows.Media;
    using Telerik.Windows.Controls.Pivot.Export;
    using System.IO;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for Insights.xaml
    /// </summary>
    public partial class AnalyticsV2
    {

        private Telerik.Pivot.Adomd.AdomdDataProvider _provider { get; set; }
        public AnalyticsViewModel _viewModel { get; set; }
        public AnalyticsV2()
        {
            DataContext = _viewModel = new AnalyticsViewModel();
            InitializeComponent();
            if (_provider == null)
            {
                _provider = new AdomdDataProvider();
            }
            _viewModel.DataProvider = _provider;
            loadProvider();
            leftCol.Width = new GridLength(300);
            pivotCol.Width = new GridLength(300);


            var chartViewModel = new PivotChartViewModel();
            chartViewModel.DataProvider = _provider;
            AnalyticsChart.DataContext = chartViewModel;
            ChartSelector.DataContext = chartViewModel;

            AnalyticsChart.SeriesProvider.SeriesDescriptors.First().Style = Resources["barCategoricalSeriesDescriptorStyle"] as Style;
        }

        private void btnResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (leftCol.Width == new GridLength(0))
            {
                leftCol.Width = new GridLength(300);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                btnResize.Source = bitmap;
            }
            else
            {
                leftCol.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                btnResize.Source = bitmap;
            }
        }


        private void btnResize_MouseDown2(object sender, MouseButtonEventArgs e)
        {
            if (pivotCol.Width == new GridLength(0))
            {
                pivotCol.Width = new GridLength(300);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                btnResize2.Source = bitmap;
            }
            else
            {
                pivotCol.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                btnResize2.Source = bitmap;
            }
        }

       // private GridLength _hidePivotRowHeight;
        //private void HidePivot_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (PivotRow.Height == new GridLength(0))
        //    {
        //        PivotRow.Height = _hidePivotRowHeight;
        //        var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/up.gif");
        //        var bitmap = new BitmapImage(uri);
        //        HidePivot.Source = bitmap;
        //    }
        //    else
        //    {
        //        _hidePivotRowHeight = PivotRow.Height;
        //        PivotRow.Height = new GridLength(0);
        //        var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/down.gif");
        //        var bitmap = new BitmapImage(uri);
        //        HidePivot.Source = bitmap;
        //    }
        //}

        private void HideLegend_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LegendColumn.Width == new GridLength(0))
            {
                LegendColumn.Width = new GridLength();
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                HideLegend.Source = bitmap;
            }
            else
            {
                LegendColumn.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                HideLegend.Source = bitmap;
            }
        }

        //private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var clickedReport = (sender as TextBlock).DataContext as Report;
        //    (DataContext as InsightsViewModel).CurrentReport = clickedReport;
        //}

        public void loadProvider()
        {             
            var layout = _viewModel.DataProvider;

            // Setting default sorting for new rows and columns to "Sort by keys"
            layout.PrepareDescriptionForField += (sender, args) =>
            {
                if (args.DescriptionType == DataProviderDescriptionType.Group)
                {
                    var propertyGroupDescriptionBase = args.Description as AdomdGroupDescription;
                    if (propertyGroupDescriptionBase != null)
                        propertyGroupDescriptionBase.GroupComparer = new OlapGroupComparer();
                }
            };

            //if (_viewModel.SelectedReport != null)
            //{

            layout.BeginInit();
            layout.StatusChanged += Layout_StatusChanged;

            radPivotGrid.DataProvider = layout;
            radPivotFieldList.DataProvider = layout;


            var chartViewModel = new PivotChartViewModel();
            chartViewModel.DataProvider = radPivotFieldList.DataProvider;
            AnalyticsChart.DataContext = chartViewModel;
            ChartSelector.DataContext = chartViewModel;

            layout.EndInit();

            _provider = layout;

            //}
            //else
            //{
            //    radPivotGrid.DataProvider = new AdomdDataProvider();
            //    radPivotFieldList.DataProvider = new AdomdDataProvider();


            //    var chartViewModel = new PivotChartViewModel();
            //    chartViewModel.DataProvider = radPivotFieldList.DataProvider;
            //    AnalyticsChart.DataContext = chartViewModel;
            //    ChartSelector.DataContext = chartViewModel;
            //}


        }

        private void Layout_StatusChanged(object sender, DataProviderStatusChangedEventArgs e)
        {
            if (e.NewStatus == DataProviderStatus.RetrievingData)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    radPivotGrid.ColumnGrandTotalsPosition = (bool)ColumnGrand.IsChecked ? Telerik.Windows.Controls.ColumnTotalsPosition.Right : Telerik.Windows.Controls.ColumnTotalsPosition.None;
                    radPivotGrid.ColumnSubTotalsPosition = (bool)ColumnSub.IsChecked ? Telerik.Windows.Controls.ColumnTotalsPosition.Right : Telerik.Windows.Controls.ColumnTotalsPosition.None;
                    radPivotGrid.RowGrandTotalsPosition = (bool)RowGrand.IsChecked ? Telerik.Windows.Controls.RowTotalsPosition.Bottom : Telerik.Windows.Controls.RowTotalsPosition.None;
                    radPivotGrid.RowSubTotalsPosition = (bool)RowSub.IsChecked ? Telerik.Windows.Controls.RowTotalsPosition.Bottom : Telerik.Windows.Controls.RowTotalsPosition.None;
                });

            }
        }

        private string lastSerializadProvider;

       
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    radPivotFieldList.Visibility = (radPivotFieldList.Visibility == System.Windows.Visibility.Visible ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);
        //}

        private void ExportToExcel()
        {

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xlsx";
            dialog.Filter = "Excel Workbook (xlsx) | *.xlsx |All Files (*.*) | *.*";

            var result = dialog.ShowDialog();
            if ((bool)result)
            {
                try
                {
                    var workbook = GenerateWorkbook();

                    using (var stream = dialog.OpenFile())
                    {
                        XlsxFormatProvider provider = new XlsxFormatProvider();
                        provider.Export(workbook, stream);
                    }
                }
                catch (IOException ex)
                {
                    CustomMessageBox.Show(ex.Message);
                }
                catch
                {
                    CustomMessageBox.Show("Unable to export data.", "Data Export", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }

        private Workbook GenerateWorkbook()
        {

            var export = radPivotGrid.GenerateExport();

            Workbook workbook = new Workbook();
            workbook.History.IsEnabled = false;

            var worksheet = workbook.Worksheets.Add();

            workbook.SuspendLayoutUpdate();
            int rowCount = export.RowCount;
            int columnCount = export.ColumnCount;

            var allCells = worksheet.Cells[0, 0, rowCount - 1, columnCount - 1];
            allCells.SetFontFamily(new ThemableFontFamily(radPivotGrid.FontFamily));
            allCells.SetFontSize(12);
            allCells.SetFill(GenerateFill(radPivotGrid.Background));

            foreach (var cellInfo in export.Cells)
            {
                int rowStartIndex = cellInfo.Row;
                int rowEndIndex = rowStartIndex + cellInfo.RowSpan - 1;
                int columnStartIndex = cellInfo.Column;
                int columnEndIndex = columnStartIndex + cellInfo.ColumnSpan - 1;

                CellSelection cellSelection = worksheet.Cells[rowStartIndex, columnStartIndex];

                var value = cellInfo.Value;
                if (value != null)
                {
                    cellSelection.SetFormat(new CellValueFormat("@"));
                    cellSelection.SetValue(Convert.ToString(value));
                    cellSelection.SetVerticalAlignment(RadVerticalAlignment.Center);
                    cellSelection.SetHorizontalAlignment(GetHorizontalAlignment(cellInfo.TextAlignment));
                    int indent = cellInfo.Indent;
                    if (indent > 0)
                    {
                        cellSelection.SetIndent(indent);
                    }
                }

                cellSelection = worksheet.Cells[rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex];

                SetCellProperties(cellInfo, cellSelection);
            }

            for (int i = 0; i < columnCount; i++)
            {
                var columnSelection = worksheet.Columns[i];
                columnSelection.AutoFitWidth();

                //NOTE: workaround for incorrect autofit.
                var newWidth = worksheet.Columns[i].GetWidth().Value.Value + 15;
                columnSelection.SetWidth(new ColumnWidth(newWidth, false));
            }

            workbook.ResumeLayoutUpdate();
            return workbook;
        }
        private static IFill GenerateFill(Brush brush)
        {
            if (brush != null)
            {
                SolidColorBrush solidBrush = brush as SolidColorBrush;
                if (solidBrush != null)
                {
                    return PatternFill.CreateSolidFill(solidBrush.Color);
                }
            }

            return null;
        }

        private RadHorizontalAlignment GetHorizontalAlignment(TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case TextAlignment.Center:
                    return RadHorizontalAlignment.Center;

                case TextAlignment.Left:
                    return RadHorizontalAlignment.Left;

                case TextAlignment.Right:
                    return RadHorizontalAlignment.Right;

                case TextAlignment.Justify:
                default:
                    return RadHorizontalAlignment.Justify;
            }
        }

        private static void SetCellProperties(PivotExportCellInfo cellInfo, CellSelection cellSelection)
        {
            var fill = GenerateFill(cellInfo.Background);
            if (fill != null)
            {
                cellSelection.SetFill(fill);
            }

            SolidColorBrush solidBrush = cellInfo.Foreground as SolidColorBrush;
            if (solidBrush != null)
            {
                cellSelection.SetForeColor(new ThemableColor(solidBrush.Color));
            }

            if (cellInfo.FontWeight.HasValue && cellInfo.FontWeight.Value != FontWeights.Normal)
            {
                cellSelection.SetIsBold(true);
            }

            SolidColorBrush solidBorderBrush = cellInfo.BorderBrush as SolidColorBrush;
            if (solidBorderBrush != null && cellInfo.BorderThickness.HasValue)
            {
                var borderThickness = cellInfo.BorderThickness.Value;
                var color = new ThemableColor(solidBorderBrush.Color);
                //var leftBorder = new CellBorder(GetBorderStyle(borderThickness.Left), color);
                //var topBorder = new CellBorder(GetBorderStyle(borderThickness.Top), color);
                var rightBorder = new CellBorder(GetBorderStyle(borderThickness.Right), color);
                var bottomBorder = new CellBorder(GetBorderStyle(borderThickness.Bottom), color);
                var insideBorder = cellInfo.Background != null ? new CellBorder(CellBorderStyle.None, color) : null;
                cellSelection.SetBorders(new CellBorders(null, null, rightBorder, bottomBorder, insideBorder, insideBorder, null, null));
            }
        }
        private static CellBorderStyle GetBorderStyle(double thickness)
        {
            if (thickness < 1)
            {
                return CellBorderStyle.None;
            }
            else if (thickness < 2)
            {
                return CellBorderStyle.Thin;
            }
            else if (thickness < 3)
            {
                return CellBorderStyle.Medium;
            }
            else
            {
                return CellBorderStyle.Thick;
            }
        }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedReport != null)
                if (_viewModel.SelectedReport.Name.Trim() != "")
                {
                    AdomdProviderSerializer provider = new AdomdProviderSerializer();
                    this.lastSerializadProvider = provider.Serialize(this.radPivotGrid.DataProvider);
                    _viewModel.SaveSettings(this.lastSerializadProvider);
                }
                else
                {
                    MessageBox.Show("Report name is invalid. Save Failed.", "Save Report", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
        }

        //private void RadListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    _viewModel.GetUserReport();
        //    loadProvider();

        //    //PivotRow.MaxHeight = radPivotGrid.MaxHeight =
        //    //    AnalyticsMainGrid.ActualHeight - HideLeftMenuResizer.ActualHeight - HidePivotResizer.ActualHeight;
        //}

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            
                AdomdProviderSerializer provider = new AdomdProviderSerializer();
                this.lastSerializadProvider = provider.Serialize(this.radPivotGrid.DataProvider);
                _viewModel.AddSettings(this.lastSerializadProvider);
       
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }
 
        private void ChartType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var item = listBox.SelectedItems[0].ToString();
            //ChartType type = ((ChartType)listBox.SelectedItem);

            if (item.Contains("Bar"))
            {
                RefreshChartType(Resources["barCategoricalSeriesDescriptorStyle"] as Style);
            }
            else
            {
                if (item.Contains("Area"))
                {
                    RefreshChartType(Resources["areaCategoricalSeriesDescriptorStyle"] as Style);
                }
                else
                {
                    RefreshChartType(Resources["lineCategoricalSeriesDescriptorStyle"] as Style);
                }

            }
        }

        private void RefreshChartType(Style style)
        {
            if (AnalyticsChart != null)
            {
                AnalyticsChart.SeriesProvider.SeriesDescriptors.First().Style = style;
                AnalyticsChart.SeriesProvider.RefreshAttachedCharts();
            }
        }
 

        private void MenuItem2_OnClick(object sender, RoutedEventArgs e)
        {
            AdomdProviderSerializer provider = new AdomdProviderSerializer();
            this.lastSerializadProvider = provider.Serialize(this.radPivotGrid.DataProvider);
            _viewModel.AddSettings(this.lastSerializadProvider);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            loadProvider();
        }

        private void MainRadTreeViewNew_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems[0] as TreeViewHierarchy;
            LoadSelectedReport(item);
        }

        private void LoadSelectedReport(TreeViewHierarchy item)
        {
           
            if (item.HasChildren == false)
            {
                _viewModel.GetUserReport(item.Idx.Replace("REPORT$", ""));
                loadProvider();
            }
        }
    }

    public class ChartType
    {
        public ChartSeriesType ChartSeriesType { get; set; }
    }

    public enum ChartSeriesType
    {
        Bar,
        Area,
        Line
    }
}
