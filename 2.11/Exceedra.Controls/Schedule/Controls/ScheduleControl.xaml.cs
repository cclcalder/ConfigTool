using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Exceedra.Common.Utilities;
using Exceedra.Schedule.Model;
using Exceedra.Schedule.ViewModels;
using Model;
using Model.Annotations;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Timeline;


namespace Exceedra.Schedule.Controls
{
    /// <summary>
    /// Interaction logic for ScheduleControl.xaml
    /// </summary>
    public partial class ScheduleControl : INotifyPropertyChanged
    {
        public ScheduleViewModel ViewModel { get; set; }
        public ScheduleControl()
        {
            ViewModel = new ScheduleViewModel();
            ViewModel.StartDate = DateTime.Now;
            ViewModel.EndDate = DateTime.Now.AddMonths(12);

            InitializeComponent();

            Loaded += ScheduleControl_Loaded;

        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {
            switchItems.SelectionChanged += Selector_OnSelectionChanged;
            ViewModel.SwitchVisibleItems(1);
        }

        public static readonly DependencyProperty ScheduleSourceProperty =
            DependencyProperty.Register("ScheduleSource", typeof(ScheduleViewModel),
            typeof(ScheduleControl),
            new FrameworkPropertyMetadata() { PropertyChangedCallback = OnDataChanged, BindsTwoWayByDefault = true }
        );

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScheduleControl)d).OnTrackerInstanceChanged(e);
        }

        protected virtual void OnTrackerInstanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((ScheduleViewModel)e.NewValue != null)
            {
                ViewModel = (ScheduleViewModel)e.NewValue;
                filterControl.Visibility = ViewModel.VisibleTimelineItems.Any(t => t.HasSellOut)
                    ? Visibility.Visible
                    : Visibility.Hidden;
                PropertyChanged.Raise(this, "ViewModel");
            }
        }

        public ScheduleViewModel ScheduleSource
        {
            get { return (ScheduleViewModel)GetValue(ScheduleSourceProperty); }
            set { SetValue(ScheduleSourceProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        private void LoadPanelFromParent(ScheduleItem scheduleItem, string flag)
        {
            //var scheduleItem = (ScheduleItem)((TimelineDataItem)((StackPanel)sender).DataContext).DataItem;

            var parentPage = VisualTreeHelper.GetParent(this);
            while (!(parentPage is Page))
            {
                parentPage = VisualTreeHelper.GetParent(parentPage);
            }


            //parentPage = VisualTreeHelper.GetParent(parentPage);
            //while (!(parentPage is Page))
            //{
            //    parentPage = VisualTreeHelper.GetParent(parentPage);
            //}

            var method = parentPage.GetType().GetMethod("Border_MouseRightButtonUp");
            if (method == null) return;
            object[] param = { scheduleItem.Idx, scheduleItem.ScheduleType, scheduleItem.CanEditItem, flag };

            method.Invoke(parentPage, param);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SwitchVisibleItems(Convert.ToInt32(switchItems.SelectedIndex));
        }

        private void RadMenuItem_OnClick(object sender, RadRoutedEventArgs e)
        {
            RadMenuItem item = sender as RadMenuItem;
            //implement the logic regarding the instance here.

            var scheduleItem = (ScheduleItem)((TimelineDataItem)((RadMenuItem)sender).DataContext).DataItem;

            switch (item.Tag.ToString())
            {
                case "C":
                    LoadPanelFromParent(scheduleItem, "Copy");
                    break;

                case "QE":
                    LoadPanelFromParent(scheduleItem, "Edit");
                    break;

                case "E":
                    Navigate(scheduleItem);
                    break;

                case "CF":
                    LoadPanelFromParent(scheduleItem, "CopyForwards");
                    break;

                case "S":
                    var toggle = !scheduleItem.IsSelected;
                    scheduleItem.IsSelected = toggle;
                    break;
            }
        }

        private void Navigate(ScheduleItem scheduleItem)
        {
            var navPath = scheduleItem.ScheduleType;
            var idx = scheduleItem.Idx;
            var content = scheduleItem.Name;
            var appTypeIdx = scheduleItem.AppTypeIdx;

            var parentPage = VisualTreeHelper.GetParent(this);
            while (!(parentPage is Page))
            {
                parentPage = VisualTreeHelper.GetParent(parentPage);
            }

            //Special case: ROBs
            if (((Page)parentPage).GetType().ToString().ToLower().Contains("eventspage"))
            {
                appTypeIdx = ((Page)parentPage).GetType().GetProperty("AppTypeID").GetValue(parentPage, null).ToString();
            }


            parentPage = VisualTreeHelper.GetParent(parentPage);
            while (!(parentPage is Page))
            {
                parentPage = VisualTreeHelper.GetParent(parentPage);
            }

            var method = parentPage.GetType().GetMethod("HyperlinkClicked");
            object[] param = { navPath, idx, content, appTypeIdx, false };

            method.Invoke(parentPage, param);
        }
    }
}
