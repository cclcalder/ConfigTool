using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Exceedra.Common.Utilities;
using WPF.ViewModels.Generic;
using WPF.ViewModels.UserSettings;
using Model;
using Model.Annotations;
using Model.Entity;
using WPF.ViewModels;

namespace WPF.Pages.UserSettings
{
    /// <summary>
    /// Interaction logic for Default.xaml
    /// </summary>
    public partial class Default : INotifyPropertyChanged
    {
        private ObservableCollection<Screen> _myscreens = new ObservableCollection<Screen>(App.Configuration.GetScreens());
        public SettingsViewModel ViewModel { get; set; }
        public Default()
        {
            InitializeComponent();

            DataContext = ViewModel = new SettingsViewModel();

            ct.Text = string.Format("Version {0}", App.VersionMinorInfo);
            username.Text = User.CurrentUser.ID + " - " + User.CurrentUser.DisplayName;
            ScreenList.ItemsSource = _myscreens;
            PropertyChanged.Raise(this, "ViewModel");
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            // Logout
            User.Logout();
            //PromotionAccessBase.ForceCustomerCacheRefresh();
            App.AppCache = new ApplicationCache();
            if (NavigationService != null)
            {
                App.AutomaticLoginSuppressed = true;
                NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.RelativeOrAbsolute));
            }

        }

        private void OldPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.OldPassword = OldPassword.Password;
        }

        private void SaveNewPassword_OnClick(object sender, RoutedEventArgs e)
        {
            OldPassword.Password = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if(PropertyChanged != null)
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void up_click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.ScreenList.SelectedIndex;

            if (selectedIndex > 0)
            {
                var itemToMoveUp = this._myscreens[selectedIndex];
                this._myscreens.RemoveAt(selectedIndex);
                this._myscreens.Insert(selectedIndex - 1, itemToMoveUp);
                this.ScreenList.SelectedIndex = selectedIndex - 1;
            }
        }

        private void down_click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.ScreenList.SelectedIndex;

            if (selectedIndex + 1 < this._myscreens.Count)
            {
                var itemToMoveDown = this._myscreens[selectedIndex];
                this._myscreens.RemoveAt(selectedIndex);
                this._myscreens.Insert(selectedIndex + 1, itemToMoveDown);
                this.ScreenList.SelectedIndex = selectedIndex + 1;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var i = 0;
            foreach (var screen in _myscreens)
            {
                screen.SortOrder = i;
                i++;
            }

            App.MainPV.Screens = new ObservableCollection<Screen>(_myscreens);
            PropertyChanged.Raise(App.MainPV, "Screens");

            ViewModel.SaveScreens(_myscreens);
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SwapUser(Uid.Text);
        }
    }
}
