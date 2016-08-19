using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Exceedra.TreeGrid.ViewModels;
using Model.Annotations;
using Telerik.Windows.Controls;
using System.Windows.Controls;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Exceedra.TreeGrid.Controls
{
    /// <summary>
    /// Interaction logic for TreeGrid.xaml
    /// </summary>
    public partial class TreeGrid : INotifyPropertyChanged
    {
        public TreeGrid()
        {
            InitializeComponent();
        }

        public TreeGridViewModel DataSource
        {
            get { return (TreeGridViewModel)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(TreeGridViewModel),
                typeof(TreeGrid),
                new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true, PropertyChangedCallback = OnDataChanged }
                );
                
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeGrid)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((TreeGridViewModel)e.NewValue != null)
            {
                MainDataColumn.Header = HeaderStack;
            }
        }

        public bool ShowChartByDefault
        {
            get { return (bool)GetValue(ShowChartByDefaultProperty); }
            set { SetValue(ShowChartByDefaultProperty, value); }
        }

        public static readonly DependencyProperty ShowChartByDefaultProperty =
            DependencyProperty.Register("ShowChartByDefault", typeof(bool),
                typeof(TreeGrid),
                new UIPropertyMetadata(false)
                );


        public StackPanel HeaderStack
        {
            get
            {
                var newStack = new StackPanel
                {
                    Margin = new Thickness(8, 0, -12, 0),
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                DataSource.Headers.Do(h => newStack.Children.Add(h));

                return newStack;
            }
        }

        public static readonly DependencyProperty CanAddCommentsProperty =
            DependencyProperty.Register("CanAddComments", typeof(bool),
                typeof(TreeGrid)
                );

        public bool CanAddComments
        {
            get { return (bool)GetValue(CanAddCommentsProperty); }
            set { SetValue(CanAddCommentsProperty, value); }
        }

        private void DataControl_OnSelectionChanging(object sender, SelectionChangingEventArgs e)
        {
            if (!IsSelectionEnabled)
                e.Handled = true;
        }

        private void DataControl_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {

        }

        public bool IsSelectionEnabled
        {
            get { return (bool)GetValue(IsSelectionEnabledProperty); }
            set { SetValue(IsSelectionEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsSelectionEnabledProperty =
            DependencyProperty.Register("IsSelectionEnabled", typeof(bool),
                typeof(TreeGrid),
                new UIPropertyMetadata(false)
                );

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var treeGrids = new List<TreeGrid> { this };

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = "PlanningExport"; //((Tab)MainTabControl.SelectedItem).TabTitle; // Default file name FileNames[0]
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension
            //"Excel documents (.xlsx)|*.xlsx|All files (*.*)|*.*"

            // Process save file dialog box results
            var showDialog = dlg.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {

                if (dlg.FileName.EndsWith("xlsx"))
                {
                    //Use reflection to load in the exceedra.documents.dll so we can access the ExcelOutput method.
                    var dllPath = Assembly.GetExecutingAssembly().CodeBase.Replace("Exceedra.Controls.DLL", "").Replace("file:///", "").Replace("/", "\\") + "Exceedra.Documents.dll";
                    var documentsdll = Assembly.LoadFrom(dllPath);
                    var excelOutput = documentsdll.GetTypes().First(a => a.Name == "ExcelOutput");

                    excelOutput.GetMethod("SaveToExcel").Invoke(null, new object[] { dlg.FileName, null, null, null, null, treeGrids });
                }
            }
        }
    }
}
