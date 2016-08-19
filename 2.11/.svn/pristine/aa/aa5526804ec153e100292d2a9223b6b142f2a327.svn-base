using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Exceedra.Controls.Chart.Controls;
using Exceedra.Controls.DynamicGrid.Controls;
using Exceedra.Controls.Messages;
using Exceedra.Documents.PDF;
using Exceedra.Pivot.Controls;
using Exceedra.Schedule.Controls;
using Model.Entity.Canvas;
using Telerik.Windows.Controls;
using WPF.ViewModels.Canvas;
using Exceedra.TreeGrid.Controls;
using WPF.Navigation;

namespace WPF.Pages.Canvas
{
    /// <summary>
    /// Interaction logic for InsightsV2.xaml
    /// </summary>
    public partial class Canvas
    {
        public Canvas()
        {
            ICanvasAccessor insightsAccessor = new CanvasAccessor();
            IRowViewModelProvider rowViewModelProvider = new RowViewModelProvider();
            DataContext = CanvasViewModel = new CanvasViewModel(insightsAccessor, rowViewModelProvider);

            InitializeComponent();

            CanvasViewModel.SelectedInsightChanged += delegate { FiltersPanelRow.Height = _formerHeight; };
        }

        public CanvasViewModel CanvasViewModel { get; set; }

        private GridLength _formerMenuInsightsWidth;

        private void insightsMenuBtnResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuInsightsColumn.Width == new GridLength(0))
            {
                MenuInsightsColumn.Width = _formerMenuInsightsWidth;
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/left.gif");
                var bitmap = new BitmapImage(uri);
                imgMenuInsightsResize.Source = bitmap;
            }
            else
            {
                _formerMenuInsightsWidth = MenuInsightsColumn.Width;
                MenuInsightsColumn.Width = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/right.gif");
                var bitmap = new BitmapImage(uri);
                imgMenuInsightsResize.Source = bitmap;
            }
        }

        private GridLength _formerHeight;

        private void btnResize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (FiltersPanelRow.Height == new GridLength(0))
            {
                FiltersPanelRow.Height = _formerHeight;
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/up.gif");
                var bitmap = new BitmapImage(uri);
                imgFiltersPanelResize.Source = bitmap;
            }
            else
            {
                _formerHeight = FiltersPanelRow.Height;
                FiltersPanelRow.Height = new GridLength(0);
                var uri = new Uri(App.SiteData.BaseURL + "/Images/caret/down.gif");
                var bitmap = new BitmapImage(uri);
                imgFiltersPanelResize.Source = bitmap;
            }
        }

        public void HyperlinkClicked(string type, string idx, string content, string robIdx, bool popUpNavigation)
        {
            CanvasViewModel.HyperlinkClicked(type, idx, content);
        }

        public void NavigationlinkClicked(string type, string idx, bool pop)
        {
            RedirectMe.Goto(type, idx, "", "", "", pop);
        }

        private void ExportCanvas()
        {
            var grids = this.ChildrenOfType<DynamicGridControl>().ToList();
            var charts = this.ChildrenOfType<ExceedraChartControl>().ToList();
            var pivots = this.ChildrenOfType<ExceedraRadPivotGrid>().ToList();
            var schedules = this.ChildrenOfType<ScheduleControl>().ToList();
            var treeGrids = this.ChildrenOfType<TreeGrid>().ToList();

            if (!grids.Any() && !charts.Any() && !pivots.Any() && !schedules.Any() && !treeGrids.Any())
            {
                CustomMessageBox.Show("Nothing to Export", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            var cleanFilename = CanvasViewModel.SelectedInsight.Header.Clean();

            dlg.FileName = cleanFilename + " - " + DateTime.Now.ToString("yyyMMdd-hhmmss"); // Default file name FileNames[0]
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx|PDF documents (.pdf)|*.pdf"; // Filter files by extension
            //"Excel documents (.xlsx)|*.xlsx|All files (*.*)|*.*"

            // Process save file dialog box results
            var showDialog = dlg.ShowDialog();
            if (showDialog != null && showDialog.Value)
            { 
                if (dlg.FileName.EndsWith("xlsx"))
                {
                    var chartList = GenerateImages(charts);
                    Exceedra.Documents.Excel.ExcelOutput.SaveToExcel(dlg.FileName, grids, chartList, pivots, schedules, treeGrids);
                }
                if (dlg.FileName.EndsWith("pdf"))
                {
                    var doc = new Exceedra.Documents.PDF.PdfDocument(dlg.FileName,
                    CanvasViewModel.SelectedInsight.Header, Model.User.CurrentUser.DisplayName, CanvasViewModel.CellsGrid.ControlsCollection.ToList(), DocumentType.Canvas);

                    doc.SavePDFDocument(string.Format("{0}/images/{1}", App.SiteData.BaseURL, "reportlogo.gif"));
                }
            }             
        }

        private List<System.Drawing.Image> GenerateImages(List<ExceedraChartControl> charts)
        {
            var images = new List<System.Drawing.Image>();
            int chartCounter = 0;

            foreach (var c in charts)
            {
                Stream s = new MemoryStream();
                Telerik.Windows.Media.Imaging.ExportExtensions.ExportToImage(c.GetBaseGrid(), s, new PngBitmapEncoder());
                var image = System.Drawing.Image.FromStream(s);
                image.Tag = c.RecordSource == null ? "Chart" + ++chartCounter : c.RecordSource.Chart.Title ?? "Chart" + ++chartCounter;
                images.Add(image);
            }

            return images;
        }

        private void ExcelExport_OnClick(object sender, RoutedEventArgs e)
        {
            ExportCanvas();
        }

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

        //    // string[] FileNames = FileName.Split('.');
        //    dlg.FileName = "Report " + DateTime.Now.ToString("yyyMMdd-hhmmss"); // Default file name FileNames[0]

        //    dlg.DefaultExt = ".pdf"; // Default file extension

        //    dlg.Filter = "PDF documents (.pdf)|*.pdf"; // Filter files by extension
            
            
        //    // Show save file dialog box
        //    //Nullable<bool> result = dlg.ShowDialog();

        //    // Process save file dialog box results
        //    if (dlg.ShowDialog().Value)
        //    {
        //        var doc = new Exceedra.Documents.PDF.PdfDocument(dlg.FileName,
        //            CanvasViewModel.SelectedInsight.Header, Model.User.CurrentUser.DisplayName,  CanvasViewModel.CellsGrid.ControlsCollection.ToList(), DocumentType.Canvas);

        //         doc.SavePDFDocument();

        //    }
        //}
    }
}
