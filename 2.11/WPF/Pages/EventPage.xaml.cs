using System.Linq;
using System.Windows;
using Model.Entity;

namespace WPF.Pages
{
    using global::ViewModels;

    /// <summary>
    /// Interaction logic for EventPage.xaml
    /// </summary>
    public partial class EventPage
    {
        public string AppTypeID;
        private readonly EventViewModel _viewModel;

        public EventPage(EventViewModel viewModel)
        {
            InitializeComponent();
            DataContext = _viewModel = viewModel;
            AppTypeID = viewModel.AppTypeID;
            FileEntryControl.IsReadOnlyMode = true;
            viewModel.PropertyChanged += viewModel_PropertyChanged;

            var screen = App.Configuration.GetScreens().FirstOrDefault(rs => rs.RobAppType == AppTypeID);
            if (screen != null && screen.Key != null)
            {
                UploadFileTab.Visibility = ClientConfiguration.IsFeatureVisible("UploadFilesTab_" + screen.Key);
                UploadFile.Load(screen.Key + "$SINGLE", _viewModel._originalRob.ID, App.Configuration.StorageDetails);
            }
        }

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "RobInformation")
            //{
            //    if (_viewModel.HasRobInformation)
            //        InfoGrid.Width = new GridLength(1, GridUnitType.Star);
            //}
        }

        private void EndDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (d2.Text == "")
            {
                d2.Text = ((EventViewModel)DataContext).End.ToString();
            }
        }

        private void StartDatePicker_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (d1.Text == "")
            {
                d1.Text = ((EventViewModel)DataContext).Start.ToString();
            }
        }

        //private void Root_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ModalPresenter.Visibility = Visibility.Visible;
        //}

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ModalPresenter.Visibility = Visibility.Hidden;
        //}
    }
}
