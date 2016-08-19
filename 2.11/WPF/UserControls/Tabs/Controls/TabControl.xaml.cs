using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Chart.Controls;
using Exceedra.Controls.DynamicGrid.Controls;
using Exceedra.Documents.PDF;
using Exceedra.Pivot.Controls;
using Exceedra.Schedule.Controls;
using Model;
using Model.Annotations;
using Telerik.Windows.Controls;
using WPF.UserControls.Tabs.Models;
using WPF.UserControls.Tabs.ViewModels;
using WPF.ViewModels.Canvas;
using Button = System.Windows.Controls.Button;
using Exceedra.TreeGrid.Controls;
using Model.Entity.Calendar;

namespace WPF.UserControls.Tabs.Controls
{
    /// <summary>
    /// Interaction logic for TabControl.xaml
    /// </summary>
    public partial class TabControl : INotifyPropertyChanged
    {
        public TabControl()
        {
            InitializeComponent();
            BottomRowResizeTimer.Tick += ResizeBottomRow;
        }

        public TabViewModel TabDataSource
        {
            get { return (TabViewModel)GetValue(TabDataSourceProperty); }
            set { SetValue(TabDataSourceProperty, value); }
        }

        public static readonly DependencyProperty TabDataSourceProperty =
            DependencyProperty.Register("TabDataSource", typeof(TabViewModel),
            typeof(TabControl),
            new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true }
            );

        #region Filter Textboxes

        private const string FilterWatermark = "Filter...";

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

            if (string.IsNullOrWhiteSpace(filterTextBox.Text))
            {
                TabDataSource.FilterAllTabs(filterTextBox.Text = FilterWatermark);
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        private void FilterTextBox_TextChanged(object sender, KeyEventArgs e)
        {
            var filterTextBox = (RadWatermarkTextBox)sender;

            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                TabDataSource.FilterAllTabs(filterTextBox.Text);
                filterTextBox.Foreground = Brushes.Gray;
            }
        }

        #endregion

        #region Export

        private void ExportTab()
        {
            var grids = this.ChildrenOfType<DynamicGridControl>().ToList();
            var charts = this.ChildrenOfType<ExceedraChartControl>().ToList();
            var pivots = this.ChildrenOfType<ExceedraRadPivotGrid>().ToList();
            var schedules = this.ChildrenOfType<ScheduleControl>().Where(t => t.IsVisible).ToList();
            var treeGrids = this.ChildrenOfType<TreeGrid>().ToList();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = ((Tab)MainTabControl.SelectedItem).TabTitle; // Default file name FileNames[0]
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx|PDF documents (.pdf)|*.pdf"; // Filter files by extension
            //"Excel documents (.xlsx)|*.xlsx|All files (*.*)|*.*"

            // Process save file dialog box results
            var showDialog = dlg.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {


                if (dlg.FileName.EndsWith("xlsx"))
                {
                    var periods = new List<Period>();

                    periods.Add(new Period()
                    {
                        StartDate = new DateTime(2014, 4, 1),
                        Name = "FY 2014",
                        EndDate = new DateTime(2015, 3, 31)
                    });

                    periods.Add(new Period()
                    {
                        StartDate = new DateTime(2015, 4, 1),
                        Name = "FY 2015",
                        EndDate = new DateTime(2016, 3, 31)
                    });

                    periods.Add(new Period()
                    {
                        StartDate = new DateTime(2016, 4, 1),
                        Name = "FY 2016",
                        EndDate = new DateTime(2017, 3, 31)
                    });

                    periods.Add(new Period()
                    {
                        StartDate = new DateTime(2017, 3, 31),
                        Name = "FY 2017",
                        EndDate = new DateTime(2018, 4, 1)
                    });

                    var chartList = GenerateImages(charts);
                    Exceedra.Documents.Excel.ExcelOutput.SaveToExcel(dlg.FileName, grids, chartList, pivots, schedules, treeGrids, periods);
                }

                if (dlg.FileName.EndsWith("pdf"))
                {
                    var p = new Exceedra.Documents.PDF.PdfDocument(dlg.FileName, ((Tab)MainTabControl.SelectedItem).TabTitle, Model.User.CurrentUser.DisplayName, grids.Select(t => t.ItemDataSource).ToList(), DocumentType.DynamicGrids);
                    p.SavePDFDocument(string.Format("{0}/images/{1}", App.SiteData.BaseURL, "reportlogo.gif"));
                }
            }



        }




        public List<System.Drawing.Image> GenerateImages(List<ExceedraChartControl> charts)
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

        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag.ToString() == "Export")
            {
                ExportTab();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }



        private static readonly DispatcherTimer BottomRowResizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250), IsEnabled = false };

        private void PlReviewPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BottomRowResizeTimer.IsEnabled = true;
            BottomRowResizeTimer.Stop();
            BottomRowResizeTimer.Start();
        }

        private void ResizeBottomRow(object sender, EventArgs e)
        {
            BottomRowResizeTimer.IsEnabled = false;

            if (FilterBox.ActualWidth + Dropdowns.ActualWidth + Buttons.ActualWidth > MainGrid.ActualWidth)
            {
                Grid.SetRow(Buttons, 1);
                Grid.SetColumn(Buttons, 0);
                Grid.SetColumnSpan(Buttons, 3);
                Buttons.HorizontalAlignment = HorizontalAlignment.Left;
                Dropdowns.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                Grid.SetRow(Buttons, 0);
                Grid.SetColumn(Buttons, 2);
                Grid.SetColumnSpan(Buttons, 1);
                Buttons.HorizontalAlignment = HorizontalAlignment.Right;
                Dropdowns.HorizontalAlignment = HorizontalAlignment.Center;
            }
            PropertyChanged.Raise(this, "DropdownOrientation");
        }

    }
}
