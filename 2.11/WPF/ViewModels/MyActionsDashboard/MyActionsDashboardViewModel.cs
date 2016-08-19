using Exceedra.CellsGrid;
using Exceedra.Controls.Messages;
using Model;
using Model.Entity;
using Telerik.Windows.Controls;

namespace WPF.ViewModels.MyActionsDashboard

{
    public class MyActionsDashboardViewModel : ViewModelBase
    {
        #region ctor
        
        public MyActionsDashboardViewModel()
        {

            if (string.IsNullOrEmpty(App.Configuration.DefaultDashboardCanvasId))
            {
                CustomMessageBox.ShowOK("No dashboard found in user config", "Information", "OK");
            }
            else
            {
                CellsGrid = CellsGridViewModel.GetEmptyCellsGrid(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID, 
                    App.Configuration.DefaultDashboardCanvasId, ScreenKeys.DASHBOARD.ToString());

                CellsGrid.LoadControlsData("dashboard", null);
            }

        }


        public MyActionsDashboardViewModel(string idx)
        {

            if (string.IsNullOrEmpty(idx))
            {
                CustomMessageBox.ShowOK("No dashboard found for this reference", "Information", "OK");
            }
            else
            {
                CellsGrid = CellsGridViewModel.GetEmptyCellsGrid(User.CurrentUser.ID, User.CurrentUser.SalesOrganisationID,
                    idx, ScreenKeys.DASHBOARD.ToString());

                CellsGrid.LoadControlsData("dashboard", null);
            }

        }

        #endregion

        #region properties

        private CellsGridViewModel _cellsGrid;
        public CellsGridViewModel CellsGrid
        {
            get { return _cellsGrid; }
            set
            {
                _cellsGrid = value;
                OnPropertyChanged("CellsGrid");
            }
        }

        #endregion
    }
}
