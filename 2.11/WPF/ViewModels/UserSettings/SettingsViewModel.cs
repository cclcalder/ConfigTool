using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Messages;
using Exceedra.MultiSelectCombo.ViewModel;
using Exceedra.SingleSelectCombo.ViewModel;
using Model;
using Model.DataAccess;
using Model.DataAccess.Converters;
using Model.DataAccess.Generic;
using Model.DataAccess.Listings;
using Model.Entity;
using Model.Entity.Generic;
using Model.Entity.Listings;
using Model.Entity.UserSettings;
using Telerik.Windows.Controls;
using ViewHelper;
using Model.Language;
using WPF.UserControls.Listings;
using WPF.ViewModels.Scenarios;
using WPF.ViewModels.Generic;
using WPF.ViewModels.Cache;
using ViewModelBase = ViewModels.ViewModelBase;

namespace WPF.ViewModels.UserSettings
{
    public class SettingsViewModel : ViewModelBase
    {
        readonly SettingsAccess _settingsAccess = new SettingsAccess();
        private readonly ListingsAccess _listingsAccess = new ListingsAccess();


        public SettingsViewModel()
        {
            _changeSalesOrgCommand = new ViewCommand(ChangeSalesOrg);
            _clearLocalDataCommand = new ViewCommand(ClearLocalData);
            _savePasswordCommand = new ViewCommand(CanSavePassword, SavePassword);
            _saveStartScreenCommand = new ViewCommand(CanSaveStartScreen, SaveStartScreen);
            GetSalesOrg();

            ListingGroups.PropertyChanged += ListingGroupsOnPropertyChanged;

            if (App.CurrentLang == null || String.IsNullOrWhiteSpace(Model.User.CurrentUser.LanguageCode))
            {
                SelectedCulture = Thread.CurrentThread.CurrentUICulture;
            }
            else
            {
                SelectedCulture = new CultureInfo(Model.User.CurrentUser.LanguageCode);
            }

            Languages = CollectionViewSource.GetDefaultView(TranslationManager.Instance.Languages);
            //Languages.CurrentChanged += (ss, e) => TranslationManager.Instance.CurrentLanguage = SelectedCulture;

            //LoadScreensList();
            LoadUser();
            LoadListings();

            UserOsVersion = RegistryReader.GetOsVersion();
            UserNetFrameworkVersion = RegistryReader.GetNetFrameworkVersion();
            UserIeVersion = RegistryReader.GetIeVersion();

            if (AreProceduresVisible)
                Procedures = GetProcedures();

            var passwordPolicy = LoginAccess.GetPasswordPolicy(UserName);
            if (passwordPolicy != null) PasswordValidator = new PasswordValidator(passwordPolicy, UserName);
            else PasswordValidator = new PasswordValidator(XElement.Parse("<Results/>"));
        }

        #region "password management"

        public string OldPassword { get; set; }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged(this, vm => vm.Password);
            }
        }

        private string _password2;
        public string Password2
        {
            get { return _password2; }
            set
            {
                _password2 = value;
                NotifyPropertyChanged(this, vm => vm.Password2);
            }
        }

        private PasswordValidator _passwordValidator;
        public PasswordValidator PasswordValidator
        {
            get { return _passwordValidator; }
            set
            {
                _passwordValidator = value;
                NotifyPropertyChanged(this, vm => vm.PasswordValidator);
            }
        }

        private bool CanSavePassword(object obj)
        {
            return !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Password2) &&
                   Password == Password2 && !string.IsNullOrEmpty(OldPassword);
        }

        private void SavePassword(object obj)
        {
            var res = _settingsAccess.SaveNewPassword(OldPassword, Password);
            if (res.MaybeElement("Message") != null)
            {
                MessageBoxShow(res.Element("Message").Value);
            }
            else
            {
                MessageBoxShow(res.Element("Error").Value, null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        private void LoadUser()
        {
            try
            {
                var user = User.GetRememberedUser(App.VersionInfo);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Accent) && user.Accent.Contains('#'))
                    {
                        SelectedColor = (Color)ColorConverter.ConvertFromString(user.Accent);
                    }

                    UserName = UserSecurity.Decrypt(user.Hash1);

                }
            }
            catch (Exception)
            {

            }


        }
        private Color _selectedColor;
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;

                    User.CurrentUser.Accent = _selectedColor.ToString();

                    LoginAccess.SaveUser();

                    if (User.CurrentUser.RememberMe != RememberMe.Dont)
                        User.Remember(App.VersionInfo);

                    Windows8Palette.Palette.AccentColor = _selectedColor;
                }

            }
        }

        private void ClearLocalData(object obj)
        {
            MessageBoxResult messageBoxResult = CustomMessageBox.Show("Are you sure?",
                "Clear Local Data", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //PromotionAccessBase.ForceCustomerCacheRefresh();
                App.AppCache = new ApplicationCache();

                //reload the lot again
                Loader.LoadAll(App.Configuration.GetScreens().ToList());
            }
        }

        private CultureInfo _selectedCulture;
        public CultureInfo SelectedCulture
        {

            get
            {
                return _selectedCulture;
            }
            set
            {
                _selectedCulture = value;

                if (Model.User.CurrentUser.LanguageCode != value.IetfLanguageTag)
                {
                    Model.User.CurrentUser.LanguageCode = _selectedCulture.Name;

                    LoginAccess.SaveUser();

                    if (User.CurrentUser.RememberMe != RememberMe.Dont)
                        User.Remember(App.VersionInfo);

                    if (CultureInfo.CurrentCulture.Name != _selectedCulture.Name)
                    {
                        App.SetDefaultCulture(new CultureInfo(_selectedCulture.Name));
                        App.LoadLanguage(_selectedCulture.Name);

                        TranslationManager.Instance.CurrentLanguage = _selectedCulture;
                    }
                }

                LoadScreensList();

                NotifyPropertyChanged(this, vm => vm.SelectedCulture);
            }
        }

        public Dictionary<string, string> Config
        {
            get { return App.Configuration.Configuration; }

        }

        private ObservableCollection<LanguageSet> _languages;
        public ICollectionView Languages { get; private set; }

        public bool ShowKeys
        {
            get
            {
                return App.ShowKeys;
            }
            set
            {
                App.ShowKeys = value;
                Languages.Refresh();
            }
        }

        public bool Diagnostics
        {
            get
            {
                return App.Configuration.IsDiagnosticsTab;
            }

        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged(this, vm => vm.UserName);
            }
        }

        public RememberMe RememberMe
        {
            get
            {
                return User.CurrentUser.RememberMe;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(UserName))
                {

                    User.CurrentUser.RememberMe = value;

                    if (User.CurrentUser.RememberMe != RememberMe.Dont)
                    {
                        User.CurrentUser.HashData(UserName);
                        User.Remember(App.VersionInfo);

                    }
                    NotifyPropertyChanged(this, vm => vm.RememberMe);
                }
                else
                {
                    MessageBoxShow("Username is required when changing login type", "Error changing login type");
                }

            }
        }

        private readonly ViewCommand _clearLocalDataCommand;

        public ICommand ClearLocalDataCommand
        {
            get { return _clearLocalDataCommand; }
        }

        private readonly ViewCommand _savePasswordCommand;
        public ICommand SavePasswordCommand
        {
            get { return _savePasswordCommand; }
        }

        #region Sales Org

        private SalesOrgDataViewModel _selectedSalesOrg;
        private ObservableCollection<SalesOrgDataViewModel> _salesOrgDataList;

        private readonly ViewCommand _changeSalesOrgCommand;

        public ICommand ChangeSalesOrgCommand
        {
            get { return _changeSalesOrgCommand; }
        }

        private void ChangeSalesOrg(object obj)
        {
            MessageBoxResult messageBoxResult = CustomMessageBox.Show("Are you sure?", "Change Sales Organisation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SetUserSalesOrg();

                if (User.CurrentUser.RememberMe != RememberMe.Dont)
                    User.Remember(App.VersionInfo);

                LoginAccess.SaveUser();

                Loader.ClearSalesOrgRelatedCache();
            }
        }

        private void GetSalesOrg()
        {
            var salesOrgViewModels = new List<SalesOrgDataViewModel>();

            var so = App.AppCache.GetItem("SalesOrganisations");
            if (so == null)
            {
                var res = LoginAccess.GetSalesOrgs();
                var salesOrgs = res.OrderBy(s => s.SortIndex).ToList();
                salesOrgViewModels = salesOrgs.Select(s => new SalesOrgDataViewModel(s, s.SortIndex == 0)).ToList();
                SalesOrgDataList = new ObservableCollection<SalesOrgDataViewModel>(salesOrgViewModels);
            }
            else
            {
                salesOrgViewModels = (List<SalesOrgDataViewModel>)App.AppCache.GetItem("SalesOrganisations").obj;
                SalesOrgDataList = new ObservableCollection<SalesOrgDataViewModel>(salesOrgViewModels);
            }

            foreach (var s in salesOrgViewModels.Where(s => s.SalesOrgData.ID == User.CurrentUser.SalesOrganisationID.ToString()))
            {
                SelectedSalesOrg = s;
            }

            if (SelectedSalesOrg == null)
            {
                SelectedSalesOrg = SalesOrgDataList.First();
            }
        }

        public SalesOrgDataViewModel SelectedSalesOrg
        {
            get
            {
                return _selectedSalesOrg;
            }
            set
            {
                _selectedSalesOrg = value;

                foreach (var salesOrg in _salesOrgDataList)
                    salesOrg.IsSelected = (_selectedSalesOrg != null
                        && _selectedSalesOrg.SalesOrgData != null
                        && salesOrg.SalesOrgData.ID == _selectedSalesOrg.SalesOrgData.ID);

                NotifyPropertyChanged(this, vm => vm.SelectedSalesOrg);

                /* ACH - For fast loading the db requires the selected sales org to be the defualt. Hence, don't reload/set on change, only on Apply */
                //SetUserSalesOrg()
            }
        }

        private void SetUserSalesOrg()
        {
            if (User.CurrentUser.SalesOrganisationID.ToString() != SelectedSalesOrg.SalesOrgData.ID)
            {
                User.CurrentUser.SalesOrganisationID = Convert.ToInt32(SelectedSalesOrg.SalesOrgData.ID);

                _listingsAccess.ResetListingsData();
            }
        }

        public ObservableCollection<SalesOrgDataViewModel> SalesOrgDataList
        {
            get
            {
                return _salesOrgDataList;
            }
            set
            {
                if (_salesOrgDataList != value)
                {
                    _salesOrgDataList = value;
                    NotifyPropertyChanged(this, vm => vm.SalesOrgDataList);
                }
            }
        }

        #endregion

        private List<ScreenComboBoxItem> _screens;
        public List<ScreenComboBoxItem> Screens
        {
            get { return _screens; }
            set
            {
                _screens = value;
                SelectedScreen = Screens.FirstOrDefault(s => s.IsSelected);
                NotifyPropertyChanged(this, vm => vm.Screens);
            }
        }

        private ScreenComboBoxItem _selectedScreen;
        public ScreenComboBoxItem SelectedScreen
        {
            get { return _selectedScreen; }
            set
            {
                _selectedScreen = value;
                NotifyPropertyChanged(this, vm => vm.SelectedScreen);
            }
        }

        private void LoadScreensList()
        {
            var cache = App.AppCache.GetItem("Settings_ScreenList_" + SelectedCulture.IetfLanguageTag);

            if (cache == null)
            {
                Screens = _settingsAccess.GetScreens().Result.OrderBy(a => a.SortOrder).ToList();
                App.AppCache.Upsert("Settings_ScreenList_" + SelectedCulture.IetfLanguageTag, Screens);
            }
            else
                Screens = (List<ScreenComboBoxItem>)cache.obj;

        }

        private readonly ViewCommand _saveStartScreenCommand;
        public ViewCommand SaveStartScreenCommand
        {
            get { return _saveStartScreenCommand; }
        }

        private void SaveStartScreen(object obj)
        {
            _settingsAccess.SaveStartScreen(SelectedScreen);

            //Reassert the selected screen into the list
            Screens.Do(s => s.IsSelected = false);
            Screens.Where(screen => screen.Idx == SelectedScreen.Idx).Do(s => s.IsSelected = true);
            App.AppCache.Upsert("Settings_ScreenList", Screens);
        }

        private bool CanSaveStartScreen(object obj)
        {
            return (SelectedScreen != null);
        }

        #region Listings Groups

        private ListingsViewModel _listingsVM;
        public ListingsViewModel ListingsVM
        {
            get { return _listingsVM; }
            set
            {
                _listingsVM = value;
                NotifyPropertyChanged(this, vm => vm.ListingsVM);
            }
        }

        public Task LoadListings()
        {
            return Task.Factory.StartNew(() =>
            {
                ListingsVM = new ListingsViewModel(ListingsAccess.GetFilterCustomers().Result, ListingsAccess.GetFilterProducts().Result, ScreenKeys.SETTINGS.ToString(),true, true);
                LoadListingsGroups();
            });
        }

        public void LoadListingsGroups(string selectedName = null)
        {
            DynamicDataAccess.GetGenericEnumerableAsync<ComboboxItem>(StoredProcedure.Shared.GetListingsGroups,
                CommonXml.GetBaseScreenArguments(ScreenKeys.SETTINGS.ToString(), "GetData")).ContinueWith(t =>
                {
                    var groups = t.Result;

                    if (selectedName != null)
                    {
                        groups.Where(g => g.IsSelected).Do(g => g.IsSelected = false);
                        groups.First(g => g.Name == selectedName).IsSelected = true;
                    }

                    ListingGroups.SetItems(groups);
                });
        }

        private void ListingGroupsOnPropertyChanged(object o, PropertyChangedEventArgs p)
        {
            if (p.PropertyName == "SelectedItem")
            {
                var vm = (SingleSelectViewModel)o;

                if (vm.SelectedItem == null) return;

                LoadListingsData(vm.SelectedItem.Idx);
            }
        }

        public void LoadListingsData(string groupIdx)
        {
            var args = CommonXml.GetBaseScreenArguments(ScreenKeys.SETTINGS.ToString(), "GetData");
            args.AddElement("ListingsGroup_Idx", ListingGroups.SelectedItem.Idx);

            DynamicDataAccess.GetDynamicDataAsync(StoredProcedure.Shared.GetListingsGroup, args).ContinueWith(t =>
            {
                var users = t.Result.Element("Users").Elements().Select(u => new ComboboxItem(u));
                users.Do(u => u.IsEnabled = true);
                Users.SetItems(users);

                ListingsVM.SetSelections(new UserSelectedDefaults(t.Result));
                ListingsVM.Customers.Listings.ClearSearch();
                ListingsVM.VisibleProducts.Listings.ClearSearch();

                LoadedGroupSerialized = SerializeGroup();
            });
        }


        private SingleSelectViewModel _listingGroups = new SingleSelectViewModel();
        public SingleSelectViewModel ListingGroups
        {
            get
            {
                return _listingGroups;
            }
            set
            {
                //_listingGroups = value;
                NotifyPropertyChanged(this, vm => vm.ListingGroups);
            }
        }

        private MultiSelectViewModel _users = new MultiSelectViewModel();
        public MultiSelectViewModel Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyPropertyChanged(this, vm => vm.Users);
            }
        }

        private string _newGroupName;
        public string NewGroupName
        {
            get
            {
                return _newGroupName;
            }
            set
            {
                _newGroupName = value;
                NotifyPropertyChanged(this, vm => vm.NewGroupName);
            }
        }

        #region Commands

        private XElement LoadedGroupSerialized { get; set; }

        public ViewCommand SaveGroupChangesCommand
        {
            get { return new ViewCommand(CanSaveGroupChanges, SaveGroupChanges); }
        }

        private bool CanSaveGroupChanges(object o)
        {
            if (!IsValidSelection() || LoadedGroupSerialized == null) return false;

            return !XNode.DeepEquals(LoadedGroupSerialized, SerializeGroup());
        }

        private void SaveGroupChanges(object o)
        {
            var args = SerializeGroup();
            args.AddElement("ListingsGroup_Idx", ListingGroups.SelectedItem.Idx);
            args.AddElement("ListingsGroup_Name", ListingGroups.SelectedItem.Name);

            DynamicDataAccess.SaveDynamicData(StoredProcedure.Settings.SaveListingsGroup, args);
        }

        public ViewCommand SaveNewGroupCommand
        {
            get { return new ViewCommand(CanSaveNewGroup, SaveNewGroup); }
        }

        private bool CanSaveNewGroup(object o)
        {
            return IsValidSelection() && !string.IsNullOrEmpty(NewGroupName);
        }

        private void SaveNewGroup(object o)
        {
            var args = SerializeGroup();
            args.AddElement("ListingsGroup_Name", NewGroupName);

            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.Settings.SaveListingsGroup, args))
            {
                LoadListingsGroups(NewGroupName);

                NewGroupName = "";
            }
        }

        public ViewCommand DeleteGroupCommand
        {
            get { return new ViewCommand(CanDeleteGroup, DeleteGroup); }
        }

        private bool CanDeleteGroup(object o)
        {
            return ListingGroups != null && ListingGroups.SelectedItem != null;
        }

        private void DeleteGroup(object o)
        {
            var args = CommonXml.GetBaseSaveAttributeArguments();
            var groups = new XElement("ListingsGroups");
            groups.AddElement("Idx", ListingGroups.SelectedItem.Idx);
            args.Add(groups);

            if (DynamicDataAccess.SaveDynamicData(StoredProcedure.Settings.DeleteListingsGroup, args))
            {
                LoadListingsGroups();
            }
        }

        private XElement SerializeGroup()
        {
            var args = CommonXml.GetBaseSaveAttributeArguments();
            args.Add(InputConverter.ToCustomers(ListingsVM.CustomerIDsList));
            args.Add(InputConverter.ToProducts(ListingsVM.ProductIDsList));
            args.Add(InputConverter.ToIdxList("Users", Users.SelectedItemIdxs));

            return args;
        }

        private bool IsValidSelection()
        {
            return ListingsVM != null
                && ListingsVM.CustomerIDsList.Any()
                && ListingsVM.ProductIDsList.Any()
                && Users.SelectedItemIdxs.Any();
        }

        #endregion

        #endregion

        public void SaveScreens(ObservableCollection<Screen> myscreens)
        {
            //todo: https://trello.com/c/2lHbDbii        
            _settingsAccess.SaveScreenOrder(myscreens.ToList());

        }

        #region Diagnostics

        public string UserOsVersion { get; set; }
        public string UserNetFrameworkVersion { get; set; }
        public string UserIeVersion { get; set; }

        public bool AreProceduresVisible
        {
            get { return App.Configuration.IsSettingsAppDetailsVisible; }
        }
        public List<Procedure> Procedures { get; set; }

        private List<Procedure> GetAppProcedures()
        {
            var appProcedures = new List<Procedure>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Convert "WPF, Version=2.10.0.21960, Culture=neutral, PublicKeyToken=null" to "WPF" etc.
                var assemblyNameParts = assembly.FullName.Split(',');
                if (!assemblyNameParts.Any()) continue;

                var assemblyShortName = assemblyNameParts[0];

                // The AppDomain.CurrentDomain.GetAssemblies() method gets all the assemblies loaded into our application
                // but actually we want to search only through the projects we have created
                if (!assemblyShortName.Contains("Exceedra")
                    && assemblyShortName != "WPF"
                    && assemblyShortName != "Coder.UI.WPF"
                    && assemblyShortName != "Model")
                    continue;

                try
                {
                    foreach (var classType in assembly.GetTypes().Where(t => t.IsClass))
                    {
                        var staticStringFields = classType.GetFields().Where(field => field.IsStatic && field.FieldType == typeof(string));

                        foreach (var field in staticStringFields)
                        {
                            var fieldValue = field.GetValue(null);
                            if (fieldValue == null) continue;

                            var fieldValueString = fieldValue.ToString();
                            if (fieldValueString.Contains("app.") || fieldValueString.Contains("[app]."))
                            {
                                Procedure appProcedure = new Procedure
                                {
                                    // Even if the name of an app proc is assigned with brackets those are removed so it can match the db proc
                                    // (the procs in the db never have brackets)
                                    Name = fieldValueString.Replace("[", "").Replace("]", ""),
                                    Source = classType.FullName
                                };

                                appProcedures.Add(appProcedure);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                   
                }
            }

            return appProcedures;
        }

        private List<Procedure> GetProcedures()
        {
            var appProcedures = GetAppProcedures();
            var dbProcedures = _settingsAccess.GetDbProcedures().Result;

            foreach (var appProcedure in appProcedures)
            {
                var correspondingDbProcedure = dbProcedures.FirstOrDefault(proc => proc.Name.ToLower() == appProcedure.Name.ToLower());

                appProcedure.HasCorrespondentInDb = correspondingDbProcedure != null;
                appProcedure.HasClientVersion = correspondingDbProcedure != null && correspondingDbProcedure.HasClientVersion;
            }

            return appProcedures;
        }

        #endregion

        public void SwapUser(string text)
        {
            Model.User.CurrentUser.ID = text;
            _listingsAccess.ResetListingsData();
            App.AppCache  = new ApplicationCache();
            App.Configuration = new ClientConfigurationAccess().GetClientConfiguration(true);
            CustomMessageBox.Show("You are now impersonating user #" + text + "/n Your app may still look like it is you, but all data calls are now uinsg the new user ID", "Impersonation complete");
        }
    }
}
