using Model;
using Model.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Exceedra.Common;
using Exceedra.Controls.Messages;
using Exceedra.MultiSelectCombo.ViewModel;
using Model.DataAccess.Converters;
using Telerik.Pivot.Adomd;
using ViewHelper;
using ViewModels;
using WPF.TelerikHelpers;
using WPF.UserControls.Trees.ViewModels;

namespace WPF.ViewModels.Insights2
{
    public class AnalyticsViewModel : ViewModelBase
    {

        private readonly ICommand _deleteReportCommand;
        
        private MultiSelectViewModel _usersData = new MultiSelectViewModel();

        private static AdomdDataProvider _dataProvider;

        public AdomdDataProvider DataProvider
        {
            get { return _dataProvider; }
            set
            {
                _dataProvider = value;
                NotifyPropertyChanged(this, vm => vm.DataProvider);
            }
        }


        public AnalyticsViewModel()
        {
            _deleteReportCommand = new ViewCommand(DeleteReport);
           
            _analytics = new ClientConfigurationAccess().GetClientConfiguration().Analytics;

            Init();

        }

        public bool CanAddNew
        {
            get
            {
                return (                   
                    SelectedReport != null
                    );
            }
        }

        public bool CanSave
        {
            get
            {
                return (                
                    SelectedReport != null &&
                    !SelectedReport.IsReadOnlyForUser &&
                    SelectedUserIdxs != null &&
                    SelectedUserIdxs.Any()
                    );
            }
        }

        public ICommand DeleteReportCommand
        {
            get { return _deleteReportCommand; }
        }


        private void Init()
        {

            var tasks = new[]
            {
                LoadUserList()
            };
            
            GetUserReports();
          
        }

        private void LoadSelectedReport()
        {
            
        }


        private Task LoadUserList()
        {
            AnalyticsAccess insights2Access = new AnalyticsAccess();

            return insights2Access.GetUsers().ContinueWith(t =>
            {
                UsersData.SetItems(t.Result);
            }, App.Scheduler);

        }

        public void PopulateSelectedUsers()
        {
            if (UsersData == null || SelectedReport == null)
                return;
            if (SelectedReport.SelectedUsers == null)
                return;

            UsersData.Items.Do(i => i.IsSelected = false);
            foreach (var userData in UsersData.Items.Where(userData => SelectedReport.SelectedUsers.Contains(userData.Idx)))
            {
                userData.IsSelected = true;
            }
        }

        public MultiSelectViewModel UsersData
        {
            get
            {
                return _usersData;
            }
            set
            {
                _usersData = value;
                NotifyPropertyChanged(this, vm => vm.UsersData);
            }
        }

        public IEnumerable<string> SelectedUserIdxs
        {
            get
            {
                return UsersData.Items?.Where(user => user.IsSelected).Select(user => user.Idx);
            }

        }

        //private void LoadCubes()
        //{
        //    GetUserCubes();

        //}
 

        public void GetUserReport(string idx)
        {

            //var idx = ReportsTree.GetSingleSelectedNode().Idx;
            AnalyticsAccess _insights2Access = new AnalyticsAccess();
            
            var r = _insights2Access.GetUserReport(idx).Element("Item");
            SelectedReport = new AnalyticReport(r);

            //var cube = "";
            //var groupCatalog = "";// r.Element("Group_Catalog").MaybeValue();
            //XElement layoutXML = null;
            //var users = new List<string>();

            //foreach (var node in r.Elements())
            //{
            //    switch (node.Name.ToString())
            //    {
            //        case "Group_Cube":
            //            cube = node.Value;
            //            break;

            //        case "Group_Catalog":
            //            groupCatalog = node.Value;
            //            break;

            //        case "Report_Users":
            //            //users = node.Elements("").ToList();
            //            break;

            //        case "Report_LayoutXML":

            //              layoutXML = r.Element("Report_LayoutXML");
            //            try
            //            {
            //                layoutXML.Elements("User_Idx").Remove();
            //                layoutXML.Elements("Users").Remove();
            //                layoutXML.Elements("TypeCode").Remove();
            //                layoutXML.Elements("ReportName").Remove();
            //                layoutXML.Elements("ReportID").Remove();
            //                layoutXML.Elements("AnalyticsReport_Idx").Remove();
            //                layoutXML.Elements("AnalyticsReportGroup_Idx").Remove();
            //            }
            //            catch// (Exception ex)
            //            {
            //                CustomMessageBox.Show("Data format needs to be updated, please save this report to do this.");
            //            }

                      
            //            break;
            //    }
            //}


            try
            {
                AdomdProviderSerializer provider = new AdomdProviderSerializer();

                var url = Analytics.GetOrDefault("AnalyticsURL");
                //App.LogError(url);

                //App.LogError(database);

                DataProvider = new AdomdDataProvider();
                if (SelectedReport.LayoutXML != null)
                {
                    var x = SelectedReport.LayoutXML.Replace("OLAPGroupComparer", "OlapGroupComparer");

                    DataProvider = (AdomdDataProvider)provider.Deserialize(DataProvider, x);
                }

                var temp =
                    $"Provider=MSOLAP.5;Persist Security Info=True;Data Source={url};Initial Catalog={SelectedReport.Catalog};Timeout=300;CustomData={User.CurrentUser.ID};Roles={App.Configuration.AnalyticsRole};{App.Configuration.AnalyticsLogin}";

                DataProvider.ConnectionSettings = new AdomdConnectionSettings()
                {
                    Cube = SelectedReport.Cube,
                    Database = SelectedReport.Catalog,
                    ConnectionString = temp
                };

                NotifyPropertyChanged(this, vm => vm.DataProvider);
                //App.LogError(temp);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Saved report could not be loaded: " + ex.Message);
            } 

            PopulateSelectedUsers();
 
        }

        private void DeleteReport(object parameter)
        {

            MessageBoxResult messageBoxResult = CustomMessageBox.Show("Are you sure you want to delete this saved report?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                AnalyticsAccess _insights2Access = new AnalyticsAccess();
                _insights2Access.DeleteUserReport(SelectedReport.ID)
                   .ContinueWith(tg =>
                   {
                       if (tg.Result.Success)
                       {
                           SelectedReport = null;
                              NewReportName = "";
                           MessageBoxShow("Report Deleted", "Report ", MessageBoxButton.OK, MessageBoxImage.Information);
                       }
                       else
                       {
                           MessageBoxShow(tg.Result.Message, "Report ", MessageBoxButton.OK, MessageBoxImage.Warning);
                       }
                   }, App.Scheduler);

            }
            else
            {

            }


        }
        //private IEnumerable<AnalyticsCube> _cubes { get; set; }

        //public IEnumerable<AnalyticsCube> Cubes
        //{
        //    get { return _cubes; }
        //    set
        //    {
        //        if (_cubes == null)
        //        {
        //            _cubes = new List<AnalyticsCube>();
        //        }

        //        if (_cubes != value)
        //        {
        //            _cubes = value;
        //            NotifyPropertyChanged(this, vm => vm.Cubes);
        //            //if (SelectedCube == null)
        //            //{
        //            //    SelectedCube = (Cubes.First());
        //            //}

        //        }
        //    }
        //}

        private IEnumerable<AnalyticReport> _reports;
        //public IEnumerable<AnalyticReport> Reports { get { return _reports; } set { _reports = value; NotifyPropertyChanged(this, vm => vm.Reports); } }
        // private IEnumerable<AnalyticsGroup> _reportGroups { get; set; }

        //public IEnumerable<AnalyticsGroup> ReportGroups
        //{
        //    get { return _reportGroups; }
        //    set
        //    {
        //        if (_reportGroups != value)
        //        {
        //            _reportGroups = value;
        //            NotifyPropertyChanged(this, vm => vm.ReportGroups);

        //            Cubes = _reportGroups.Select(r => new AnalyticsCube(r.ReportCube, r.ReportCube, r.ReportCube, r.ReportCatalog, r.GroupID));

        //        }
        //    }
        //}

        private TreeViewModel _reportsTree;
        public TreeViewModel ReportsTree
        {
            get { return _reportsTree; }
            set
            {
                _reportsTree = value;
                NotifyPropertyChanged(this, vm => vm.ReportsTree);
            }
        }

        public void GetUserReports()
        {

            AnalyticsAccess _insights2Access = new AnalyticsAccess();
            var reps = _insights2Access.GetReportTree().ContinueWith(t =>
            {
                ReportsTree = new TreeViewModel(t.Result);
                
            });
             
        }

        public bool HasSelectedReport
        {
            get { return SelectedReport != null && SelectedReport.ID != null; }
        }

        private AnalyticReport _selectedReport;
        public AnalyticReport SelectedReport
        {
            get
            {
                return _selectedReport;
            }
            set
            {
                _selectedReport = value;

                NotifyPropertyChanged(this, vm => vm.SelectedReport);
                NotifyPropertyChanged(this, vm => vm.NewReportName);
                NotifyPropertyChanged(this, vm => vm.CanAddNew);

                PopulateSelectedUsers();

                NotifyPropertyChanged(this, vm => vm.CanExport);
                NotifyPropertyChanged(this, vm => vm.CanSave);

                NotifyPropertyChanged(this, vm => vm.IsAnyReportSelected);
                NotifyPropertyChanged(this, vm => vm.HasSelectedReport);
            }
        }

        public bool IsAnyReportSelected
        {
            get { return SelectedReport != null; }
        }

        public bool CanExport
        {
            get { return _selectedReport != null; }
        }

        private string _newReportName;
        public string NewReportName
        {
            get
            {
                return _newReportName;
            }
            set
            {
                _newReportName = value;
                NotifyPropertyChanged(this, vm => vm.NewReportName);
            }
        }


        //private static AnalyticsCube _selectedCube;
        //public AnalyticsCube SelectedCube
        //{
        //    get
        //    {
        //        return _selectedCube;
        //    }
        //    set
        //    {
        //        _selectedCube = value;
        //        NotifyPropertyChanged(this, vm => vm.SelectedCube);
        //        NotifyPropertyChanged(this, vm => vm.CanAddNew);
        //        NotifyPropertyChanged(this, vm => vm.CanSave);

        //        //  GetUserReports(); 
        //       // if (SelectedCube != null)
        //           // loadReportsForCube();

        //    }
        //}

        //private void loadReportsForCube(bool loadFirst = true)
        //{
        //    AnalyticsAccess _insights2Access = new AnalyticsAccess();
        //    //get the selected Cube reports
        //    AnalyticsGroup reps = _insights2Access.GetUserReportsList().Where(r => r.ReportCube == SelectedCube.Code).SingleOrDefault();
        //    if (reps != null)
        //    {

        //        if (reps.Reports != null)
        //        {
        //            Reports = reps.Reports;
        //            // SelectedReport = Reports.FirstOrDefault();                   
        //        }
        //        else
        //        {
        //            Reports = null;
        //            CustomMessageBox.Show("No reports have been found");
        //        }
        //    }
        //    else
        //    {
        //        Reports = null;
        //        CustomMessageBox.Show("No reports for this cube have been found");
        //    }

        //    //var rg = ReportGroups.Where(r => r.ReportCube == SelectedCube.Code);
        //    //   var newReps = new List<AnalyticReport>();
        //    //   foreach (var rr in rg)
        //    //   {
        //    //       newReps.AddRange(rr.Reports.Select(r => r));
        //    //   }
        //    //   if (loadFirst)
        //    //       SelectedReport = newReps.First();

        //    //   Reports = newReps;
        //}


        private Dictionary<string, string> _analytics;

        public Dictionary<string, string> Analytics
        {
            get
            { return _analytics; }
            set
            { _analytics = value; }
        }

        public void SaveSettings(string XML)
        {
            AnalyticsAccess _insights2Access = new AnalyticsAccess();
            var res = XElement.Parse(XML);

            res.FirstNode.AddAfterSelf(new XElement("User_Idx", Model.User.CurrentUser.ID));
            //res.FirstNode.AddAfterSelf(new XElement("TypeCode", SelectedCube.Code));
            res.FirstNode.AddAfterSelf(new XElement("ReportName", SelectedReport.Name));
            res.FirstNode.AddAfterSelf(new XElement("AnalyticsReportGroup_Idx", SelectedReport.GroupIdx));
            res.FirstNode.AddAfterSelf(new XElement("AnalyticsReport_Idx", SelectedReport.ID));
            res.FirstNode.AddAfterSelf(new XElement("IsReadOnlyChecked", SelectedReport.IsReadOnlyChecked ? "1" : "0"));
            res.Add(InputConverter.ToList("Users", "User_Idx", SelectedUserIdxs));

            res.Add(
            new XElement("TotalsSettings",
                new XElement("ColumnGrandTotal", SelectedReport.IsColumnGrandTotalChecked ? 1 : 0),
                new XElement("ColumnSubTotal", SelectedReport.IsColumnSubTotalChecked ? 1 : 0),
                new XElement("RowGrandTotal", SelectedReport.IsRowGrandTotalChecked ? 1 : 0),
                new XElement("RowSubTotal", SelectedReport.IsRowSubTotalChecked ? 1 : 0)));

            _insights2Access.SaveReport(res)
               .ContinueWith(tg =>
               {
                   if (tg.Result!=null)
                   {
                       var messageString = tg.Result.Element("Outcome").GetValue<string>("Msg");
              
                       //ReportsTree.ListTree.First(t=>t.Idx == SelectedReport.ID).Name = NewReportName;
                       NotifyPropertyChanged(this, vm => vm.ReportsTree);

                       MessageBoxShow(messageString, "Success");
                   }
                   else
                   {
                       const string message = "Warning";
                       MessageBoxShow(message, "View not saved", MessageBoxButton.OK, MessageBoxImage.Warning);
                   }
               }, App.Scheduler);//.ContinueWith(t => GetUserReports());

            SelectedReport.LayoutXML = XML;
            //var rep = Reports.FirstOrDefault(r => r.ID == SelectedReport.ID);
            //if (rep != null)
            //{
            //    rep.SelectedUsers = SelectedUserIdxs.ToList();
            //}
        }

        public void AddSettings(string XML)
        {
            AnalyticsAccess _insights2Access = new AnalyticsAccess();


            if (XML == "")
            {
                MessageBoxShow("There is no data attached to this report view, nothing has been saved.", "Cannot Add New Report");
                return;
            }

            var res = XElement.Parse(XML);
            if(NewReportName == "")
            {
                NewReportName = SelectedReport.Name + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() +
                            DateTime.Now.Day.ToString() + "_" + DateTime.Now.Ticks;
            }



            res.FirstNode.AddAfterSelf(new XElement("User_Idx", Model.User.CurrentUser.ID));
            //res.FirstNode.AddAfterSelf(new XElement("TypeCode", SelectedCube.Code));
            res.FirstNode.AddAfterSelf(new XElement("ReportName", NewReportName));
            res.FirstNode.AddAfterSelf(new XElement("AnalyticsReportGroup_Idx", SelectedReport.GroupIdx));
            res.FirstNode.AddAfterSelf(new XElement("AnalyticsReport_Idx", "-1"));
            res.Add(InputConverter.ToList("Users", "User_Idx", SelectedUserIdxs));

            _insights2Access.SaveReport(res)
               .ContinueWith(tg =>
               {
                   if (tg.Result!=null)
                   {
                       //Msg 
                       var messageString = tg.Result.Element("Outcome").GetValue<string>("Msg");
                       //var idx = tg.Result.Element("Outcome").GetValue<string>("AnalyticsReport_Idx");
                        
                       MessageBoxShow(messageString + " - " + NewReportName, "Success");                       
                       //GetUserReport(idx);
                       GetUserReports();

                      // ReportsTree.ListTree.First(t => t.Idx == idx).Name = NewReportName;
                       NotifyPropertyChanged(this, vm => vm.ReportsTree);

                       NewReportName = "";

                   }
                   else
                   {
                       const string message = "Report not saved.";
                       MessageBoxShow(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                   }
               }, App.Scheduler);//.ContinueWith(t => GetUserReports());
            //SelectedReport.LayoutXML = XML;

        }



    }
}
