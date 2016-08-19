using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exceedra.Controls.Messages;
using Telerik.Pivot.Core;
using WPF.Pages.Canvas.dummy; 
using Telerik.Pivot.Queryable;

namespace WPF.Pages.Canvas
{
    /// <summary>
    /// Interaction logic for Pivot1.xaml
    /// </summary>
    public partial class Pivot1 : Page
    {
        private PivotViewModel _viewModel;
        public Pivot1()
        {
            InitializeComponent();
            _viewModel = new PivotViewModel();


            try
            {
                (this.Resources["QueryableProvider"] as  QueryableDataProvider).Source = new PivotViewModel().Items.AsQueryable();
            }
            catch (Exception e)
            {

                CustomMessageBox.Show("Error " + e.InnerException);
                
            }


            //loadProvider();
        }

        public void loadProvider()
        {



            //QueryableDataProvider queryableDataProvider = new QueryableDataProvider();
            //queryableDataProvider.Source = _viewModel.Records.AsQueryable();
            //this.pg.DataProvider = queryableDataProvider;

            //queryableDataProvider.BeginInit();

            //var nameGroupDescription = new QueryablePropertyGroupDescription();
            //nameGroupDescription.PropertyName = "Customer";

            ////var volumeGroupDescription = new QueryableDoubleGroupDescription();
            ////volumeGroupDescription.PropertyName = "Volume"; ;

            //var customerGroupDescription = new QueryableDateTimeGroupDescription();
            //customerGroupDescription.PropertyName = "Name";

            //using (queryableDataProvider.DeferRefresh())
            //{
            //    queryableDataProvider.RowGroupDescriptions.Add(nameGroupDescription);
            //    //queryableDataProvider.RowGroupDescriptions.Add(volumeGroupDescription);
            //    //queryableDataProvider.RowGroupDescriptions.Add(customerGroupDescription);
            //};


            ////QueryableDoubleGroupDescription doubleGroupDescription = new QueryableDoubleGroupDescription();
            ////doubleGroupDescription.PropertyName = "Volume";
            ////queryableDataProvider.ColumnGroupDescriptions.Add(doubleGroupDescription);

            ////QueryableDateTimeGroupDescription dtGroupDescription = new QueryableDateTimeGroupDescription();
            ////dtGroupDescription.PropertyName = "Name";
            ////dtGroupDescription.Step = DateTimeStep.Day;
            ////queryableDataProvider.ColumnGroupDescriptions.Add(dtGroupDescription);

            //var freightAggregateDescription = new QueryablePropertyAggregateDescription();
            //freightAggregateDescription.PropertyName = "Volume";
            //freightAggregateDescription.StringFormat = "C";
            //freightAggregateDescription.AggregateFunction = QueryableAggregateFunction.Sum;

            //queryableDataProvider.EndInit();

            //pg.DataProvider = queryableDataProvider;
            //pl.DataProvider = queryableDataProvider;
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            loadProvider();
        }
    }
}
