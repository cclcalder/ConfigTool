using System;
using System.ComponentModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Exceedra.Common.Utilities;
using Exceedra.Controls.Messages;
using Model;
using Model.Annotations;
using Model.Entity;
using Model.Entity.Listings;
using Telerik.Windows;
using Telerik.Windows.Controls;
using WPF.UserControls.Trees.Controls;
using WPF.UserControls.Trees.ViewModels;
using WPF.ViewModels;
using WPF.ViewModels.RobContracts;

namespace WPF.Pages.RobContracts
{
    /// <summary>
    /// Interaction logic for RobContractsEditor.xaml
    /// </summary>
    public partial class RobContractsEditorView : INotifyPropertyChanged
    {
        public RobContractsEditorView(RobContractsEditorViewModel viewModel)
        {
            InitializeComponent();

            ViewModel = viewModel;

            var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == _viewModel._robScreenId);

            if (screen != null && screen.Key != null)
            {
                UploadFileTab.Visibility = ClientConfiguration.IsFeatureVisible("UploadFilesTab_" + screen.Key + "_Contracts");
                UploadFile.Load(screen.Key + "$GROUP", _viewModel.ContractId, App.Configuration.StorageDetails);

                ViewModel.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(ViewModel.ContractId))
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            UploadFile.Load(screen.Key + "$GROUP", _viewModel.ContractId, App.Configuration.StorageDetails);
                        }));
                };
            }
        }

        private RobContractsEditorViewModel _viewModel;
        public RobContractsEditorViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value; 
                PropertyChanged.Raise(this, "ViewModel");
            }
        }

        /// <summary>
        /// If a user wants to make a change in the customers selection (or turn the toggle planning of an item on or off) and has already some terms added in the contract these terms may no longer be consistent with the new customers selection (i.e. their selected customers etc.).
        /// Therefore, a user will be asked for the confirmation of the selection (or toggling) being warned that if it will be confirmed the terms will be erased. Otherwise the action will be cancelled.
        /// </summary>
        private void TreeControl_OnUnsafeCheck(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult =
                CustomMessageBox.Show(
                    "The change you are about to make may affect the existing terms therefore all of them will be deleted.\n" +
                    "Do you want to proceed?", "Warning", MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                ViewModel.PurgeTerms();
                ViewModel.SetIsCheckSafe();
            }
            else e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
