using Exceedra.Common.Utilities;

namespace WPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Model.DataAccess;
    using Model.Entity;
    using Exceedra.Common.Mvvm;
    using ViewHelper;
    using global::ViewModels;
    using Model.Utilities;
    using System.ComponentModel;

    public class ManagePhasingViewModel : ViewModelBase
    {
        public static readonly DependencyProperty IsWeekSelectedProperty =
            DependencyProperty.Register("IsWeekSelected", typeof(bool), typeof(ManagePhasingViewModel),
                                        new PropertyMetadata(true, OnIsWeekSelectedPropertyChanged));

        public static readonly DependencyProperty IsDaySelectedProperty =
            DependencyProperty.Register("IsDaySelected", typeof(bool), typeof(ManagePhasingViewModel),
                                        new PropertyMetadata(default(bool), OnIsDaySelectedPropertyChanged));

        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register("SelectedProfile", typeof(PhasingProfileViewModel),
                                        typeof(ManagePhasingViewModel),
                                        new PropertyMetadata(default(PhasingProfileViewModel),
                                                             OnSelectedProfilePropertyChanged));

        public static readonly DependencyProperty IsNewProfileProperty =
            DependencyProperty.Register("IsNewProfile", typeof(bool), typeof(ManagePhasingViewModel),
                                        new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsProfileSelectedProperty =
    DependencyProperty.Register("IsProfileSelected", typeof(bool), typeof(ManagePhasingViewModel),
                        new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty NewProfileNameProperty =
            DependencyProperty.Register("NewProfileName", typeof(string), typeof(ManagePhasingViewModel),
                                        new PropertyMetadata(default(string), OnNewProfileNameChanged));

        private readonly IPhasingAccess _access;

        private readonly RangeObservableCollection<PhasingProfileViewModel> _dayProfiles =
            new RangeObservableCollection<PhasingProfileViewModel>();

        private readonly ActionCommand _deleteProfile;
        private readonly ActionCommand _newProfile;
        private readonly RangeObservableCollection<PhasingProfileViewModel> _profiles = new RangeObservableCollection<PhasingProfileViewModel>();
        private readonly ActionCommand _save;

        private readonly RangeObservableCollection<PhasingProfileViewModel> _weekProfiles =
            new RangeObservableCollection<PhasingProfileViewModel>();

        public ManagePhasingViewModel()
            : this(new PhasingAccess())
        {
        }

        public ManagePhasingViewModel(IPhasingAccess access)
        {
            _newProfile = new ActionCommand(NewProfileImpl);
            _deleteProfile = new ActionCommand(DeleteProfileImpl);
            _save = new ActionCommand(SaveImpl);

            _access = access;

            LoadProfiles();
        }

        public ActionCommand Save
        {
            get { return _save; }
        }

        public ActionCommand DeleteProfile
        {
            get { return _deleteProfile; }
        }

        public ActionCommand NewProfile
        {
            get { return _newProfile; }
        }

        public string NewProfileName
        {
            get { return (string)GetValue(NewProfileNameProperty); }
            set { SetValue(NewProfileNameProperty, value); }
        }

        public bool IsNewProfile
        {
            get { return (bool)GetValue(IsNewProfileProperty); }
            set { SetValue(IsNewProfileProperty, value); }
        }

        public bool IsWeekSelected
        {
            get { return (bool)GetValue(IsWeekSelectedProperty); }
            set { SetValue(IsWeekSelectedProperty, value); }
        }

        public bool IsDaySelected
        {
            get { return (bool)GetValue(IsDaySelectedProperty); }
            set { SetValue(IsDaySelectedProperty, value); }
        }

        public bool IsProfileSelected
        {
            get { return (bool)GetValue(IsProfileSelectedProperty); }
            set { SetValue(IsProfileSelectedProperty, value); }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public PhasingProfileViewModel SelectedProfile
        {
            get { return (PhasingProfileViewModel)GetValue(SelectedProfileProperty); }
            set { SetValue(SelectedProfileProperty, value); }
        }

        public ObservableCollection<PhasingProfileViewModel> Profiles
        {
            get { return IsWeekSelected ? _weekProfiles : _dayProfiles; }
        }

        private Task LoadProfiles()
        {
            TaskScheduler scheduler = TaskScheduler.Default;
            Dispatcher.Invoke(new Action(() => { scheduler = App.Scheduler; }));
            var tasks = new Task[] { _access.GetDayProfilesAsync(Enumerable.Empty<string>()), _access.GetWeekProfilesAsync(Enumerable.Empty<string>()) };
            return Task.Factory.ContinueWhenAll(tasks, GetProfilesContinuation, CancellationToken.None,
                                                TaskContinuationOptions.None, scheduler);
        }

        private void SaveImpl()
        {
            if (SelectedProfile == null) return;

            if (SelectedProfile.Total != 100m)
            {
                Messages.Instance.PutError("Total must be 100%.");
                return;
            }

            SelectedProfile.SaveAsync(_access)
                .ContinueWith(SaveContinuation, CancellationToken.None, TaskContinuationOptions.None,
                              App.Scheduler);
        }

        private void SaveContinuation(Task task)
        {
            if (task.IsFaulted)
            {
                if (task.Exception != null && task.Exception.InnerException != null)
                {
                    Messages.Instance.PutError(task.Exception.InnerException.Message);
                }
                else
                {
                    Messages.Instance.PutError("Save failed. No further information is available.");
                }
                return;
            }

            string selectedId = SelectedProfile.ID;

            LoadProfiles();
                //.ContinueWith(_ => SelectedProfile = Profiles.FirstOrDefault(p => p.ID == selectedId));

            ChangesSaved.Raise(this);
        }

        public event EventHandler ChangesSaved;

        private void DeleteProfileImpl()
        {
            if (SelectedProfile == null) return;

            if (SelectedProfile.ID == "0")
            {
                Profiles.Remove(SelectedProfile);
                SelectedProfile = null;
            }
            else
            {
                SelectedProfile.DeleteAsync(_access)
                               .ContinueWith(t =>
                                                 {
                                                     if (!(t.IsFaulted || t.IsCanceled))
                                                     {
                                                         Profiles.Remove(SelectedProfile);
                                                         SelectedProfile = null;
                                                         ChangesSaved.Raise(this);
                                                     }
                                                 }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void NewProfileImpl()
        {
            PhasingProfileViewModel viewModel;

            const string newProfileName = "New profile";

            if (IsWeekSelected)
            {
                var profile = new WeekPhasingProfile("0", newProfileName, _ => Enumerable.Repeat<decimal>(0, 4));
                viewModel = PhasingProfileViewModel.Create(profile);
            }
            else
            {
                var profile = new DayPhasingProfile("0", newProfileName, _ => Enumerable.Repeat<decimal>(0, 7));
                viewModel = PhasingProfileViewModel.Create(profile);
            }

            //Profiles.Add(viewModel);
            SelectedProfile = viewModel;
            NewProfileName = newProfileName;
        }

        private void GetProfilesContinuation(Task[] tasks)
        {
            _profiles.Clear();
            _weekProfiles.Clear();
            _dayProfiles.Clear();

            Task<IEnumerable<DayPhasingProfile>> dayTask = tasks.OfType<Task<IEnumerable<DayPhasingProfile>>>().Single();
            _profiles.AddRange(dayTask.Result.Select(PhasingProfileViewModel.Create));

            Task<IEnumerable<WeekPhasingProfile>> weekTask =
                tasks.OfType<Task<IEnumerable<WeekPhasingProfile>>>().Single();
            _profiles.AddRange(weekTask.Result.Select(PhasingProfileViewModel.Create));

            _weekProfiles.AddRange(_profiles.OfType<PhasingProfileViewModel<WeekPhasingProfile>>());
            _dayProfiles.AddRange(_profiles.OfType<PhasingProfileViewModel<DayPhasingProfile>>());
        }

        private static void OnNewProfileNameChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs
                                                        dependencyPropertyChangedEventArgs)
        {
            var viewModel = (ManagePhasingViewModel)dependencyObject;
            if (viewModel.IsProfileSelected)
            {
                viewModel.SelectedProfile.Name = dependencyPropertyChangedEventArgs.NewValue.ToString();
            }
        }

        private static void OnSelectedProfilePropertyChanged(DependencyObject dependencyObject,
                                                             DependencyPropertyChangedEventArgs
                                                                 dependencyPropertyChangedEventArgs)
        {
            var viewModel = ((ManagePhasingViewModel)dependencyObject);
            viewModel.SelectedProfileChanged.Raise(dependencyObject);
            viewModel.IsNewProfile = viewModel.SelectedProfile != null && viewModel.SelectedProfile.ID.Equals("0");
            viewModel.IsProfileSelected = viewModel.SelectedProfile != null;
            viewModel.NewProfileName = (viewModel.IsProfileSelected) ? viewModel.SelectedProfile.Name : "";
        }

        public event EventHandler SelectedProfileChanged;

        private static void OnIsWeekSelectedPropertyChanged(DependencyObject dependencyObject,
                                                            DependencyPropertyChangedEventArgs
                                                                dependencyPropertyChangedEventArgs)
        {
            ResetProfiles(dependencyObject);
        }

        private static void ResetProfiles(DependencyObject dependencyObject)
        {
            var viewModel = ((ManagePhasingViewModel)dependencyObject);
            viewModel.SelectedProfile = null;
            viewModel.NotifyPropertyChanged(viewModel, vm => vm.Profiles);
        }

        private static void OnIsDaySelectedPropertyChanged(DependencyObject dependencyObject,
                                                           DependencyPropertyChangedEventArgs
                                                               dependencyPropertyChangedEventArgs)
        {
            ResetProfiles(dependencyObject);
        }
    }
}