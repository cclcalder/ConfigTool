namespace ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Model;
    using Model.DataAccess;
    using ViewHelper;
    using WPF.Properties;

    public class InsightsViewModel : ViewModelBase
    {
        # region Constructor

        public InsightsViewModel()
        {
            IsDataLoading = true;

            Groups = InsightReportsAccess.GetInsightGroups();
            CurrentReport = _Groups.SelectMany(g => g.Reports).FirstOrDefault(r => r.IsDefault);

            IsDataLoading = false;
        }

        # endregion

        # region Properties

        private Report _CurrentReport;
        private List<Group> _Groups;
        private bool _isDataLoading;
        private Uri _selectedUri;

        public Uri SelectedUri
        {
            get { return _selectedUri; }
            set
            {
                if (_selectedUri != value)
                {
                    _selectedUri = value;
                    NotifyPropertyChanged(this, vm => vm.SelectedUri);
                }
            }
        }

        public IEnumerable<Group> Groups
        {
            get { return _Groups; }
            set
            {
                _Groups = value.ToList();
                NotifyPropertyChanged(this, vm => vm.Groups);
            }
        }

        public Report CurrentReport
        {
            get { return _CurrentReport; }
            set
            {
                _CurrentReport = value;
                NotifyPropertyChanged(this, vm => vm.CurrentReport);

                if (_CurrentReport != null)
                {
                    SelectedUri = string.IsNullOrWhiteSpace(Settings.Default.ReportServerUrlBase)
                                      ? new Uri(CurrentReport.Url)
                                      : new Uri(_CurrentReport.Url.Replace("http://localhost/",
                                                                           Settings.Default.
                                                                               ReportServerUrlBase));
                }
            }
        }

        public bool IsDataLoading
        {
            get { return _isDataLoading; }
            set
            {
                _isDataLoading = value;

                Mouse.OverrideCursor = _isDataLoading ? Cursors.Wait : null;

                NotifyPropertyChanged(this, vm => vm.IsDataLoading);
            }
        }

        # endregion

        #region Commands

        public ICommand NavigateCommand
        {
            get { return new ViewCommand(Navigate); }
        }

        # endregion

        # region Methods

        public void Navigate(object parameter)
        {
            SelectedUri = new Uri(parameter.ToString());
        }

        # endregion

        public Group GroupWithSelection
        {
            get { return Groups.FirstOrDefault(g => g.Reports.Any(r => r.IsDefault)); }
        }
    }
}