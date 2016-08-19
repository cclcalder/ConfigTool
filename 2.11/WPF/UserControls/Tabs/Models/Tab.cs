using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Schedule.ViewModels;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using ViewHelper;
using ViewModels;

namespace WPF.UserControls.Tabs.Models
{
    public class Tab : ViewModelBase
    {
        public Tab()
        {
            TabChartItems.PropertyChanged += TabChartItemsOnPropertyChanged;
        }

        public string TabName { get; set; }
        public string TabTitleWithCount { get { return "  " + TabTitle + " (" + GetContentCount() + ")  "; } }
        public string TabTitle { get; set; }
        public string TabType { get; set; }
        public string TabMainContentProc { get; set; }
        public string ApplyRootXml { get; set; }

        public XElement AdditionalInputXml { get; set; }

        private object _tabMainContent;
        public object TabMainContent
        {
            get
            {
                return _tabMainContent;
            }
            set
            {
                _tabMainContent = value;
                NotifyPropertyChanged(this, vm => vm.TabMainContent);
                NotifyPropertyChanged(this, vm => vm.TabTitleWithCount);
            }
        }

        public bool AreItemsSelected(object o)
        {
            return GetSelectedItems().Count > 0;
        }

        public List<string> GetSelectedItems()
        {
            if (TabMainContent != null)
            {
                switch (TabType)
                {
                    case "HorizontalGrid":
                        if (((RecordViewModel)TabMainContent).Records == null) return new List<string>();
                        return ((RecordViewModel)TabMainContent).Records.Select(r => r.SelectedItems).Where(r => r.HasValue()).ToList();
                }
            }

            return new List<string>();  
        }

        private int GetContentCount()
        {
            var count = 0;

            if (TabMainContent != null)
            {
                switch (TabType.ToLower())
                {
                    case "horizontalgrid":
                        if (((RecordViewModel) TabMainContent).Records == null) return 0;
                        count = ((RecordViewModel)TabMainContent).Records.Count(r => r.Properties.Count(p => !String.IsNullOrWhiteSpace(p.Value)) > 2);
                        break;
                    case "schedulegrid":
                        count = ((ScheduleViewModel) TabMainContent).TimelineItems.Count;
                        break;
                    case "chart":
                        if (((Exceedra.Chart.ViewModels.RecordViewModel)TabMainContent).Chart.Series == null) return 0;
                        count = ((Exceedra.Chart.ViewModels.RecordViewModel)TabMainContent).Chart.Series.SelectMany(s => s.Datapoints).Count();
                        break;
                }
            }

            return count;            
        }

        public void ReloadCount()
        {
            NotifyPropertyChanged(this, vm => vm.TabTitleWithCount);
        }
        
        public string TabChartListProc { get; set; }

        private SingleSelectViewModel _tabChartItems = new SingleSelectViewModel();
        public SingleSelectViewModel TabChartItems
        {
            get { return _tabChartItems; }
            set { _tabChartItems = value; }
        }

        public string SelectedItemIdx { get { return TabChartItems.SelectedItem.Idx; } }

        private void TabChartItemsOnPropertyChanged(object s, PropertyChangedEventArgs p)
        {
            if (p.PropertyName == "SelectedItem" && TabChartItems.SelectedItem != null)
            {
                ((Exceedra.Chart.ViewModels.RecordViewModel)TabMainContent).IsLoading = true;
                NotifyPropertyChanged(this, vm => vm.SelectedItemIdx);
            }
        }

        #region Comments

        public bool CommentVisibility { get { return !string.IsNullOrEmpty(AddCommentProc); } }

        private string _addCommentProc;
        public string AddCommentProc
        {
            get
            {
                return _addCommentProc;
            }
            set
            {
                _addCommentProc = value;
                NotifyPropertyChanged(this, vm => vm.AddCommentProc, vm => vm.CommentVisibility);
            }
        }

        private XElement _addCommentArguments;
        public XElement AddCommentArguments
        {
            get
            {
                return _addCommentArguments;
            }
            set
            {
                _addCommentArguments = value;
                NotifyPropertyChanged(this, vm => vm.AddCommentArguments);
            }
        }

        private string _commentText;
        public string CommentText
        {
            get
            {
                return _commentText;
            }
            set
            {
                _commentText = value;
                NotifyPropertyChanged(this, vm => vm.CommentText);
            }
        }

        public ICommand AddCommentCommand
        {
            get { return new ViewCommand(CanAddComment, AddComment); }
        }

        public bool CanAddComment(object o)
        {
            return !string.IsNullOrEmpty(CommentText) && !string.IsNullOrEmpty(AddCommentProc);
        }

        public void AddComment(object o)
        {
            var args = AddCommentArguments != null ? XElement.Parse(AddCommentArguments.ToString()) : CommonXml.GetBaseArguments();
            args.AddElement("Comment", CommentText);
            if (MessageConverter.DisplayMessage(DynamicDataAccess.GetDynamicData(AddCommentProc, args)))
            {
                CommentText = null;
                NotifyPropertyChanged(this, vm => vm.CommentAdded);
            }
        }

        public bool CommentAdded { get; set; }

        #endregion
    }
}