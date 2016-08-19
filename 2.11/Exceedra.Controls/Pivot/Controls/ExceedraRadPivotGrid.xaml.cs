using System.Windows;
using Exceedra.Pivot.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Pivot.Export;

namespace Exceedra.Pivot.Controls
{
    /// <summary>
    /// Interaction logic for RadPivotGridControl.xaml
    /// </summary>
    public partial class ExceedraRadPivotGrid
    {
        public ExceedraRadPivotGrid()
        {
            InitializeComponent();
        }

        #region DataSource Dependency Property

        public ExceedraRadPivotGridViewModel ViewModel
        {
            get { return (ExceedraRadPivotGridViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
    DependencyProperty.Register("ViewModel", typeof(ExceedraRadPivotGridViewModel),
        typeof(ExceedraRadPivotGrid),
        new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true }
        );

        #endregion

        #region IsEmpty message

        public static readonly DependencyProperty IsEmptyMessageProperty = DependencyProperty.Register(
            "IsEmptyMessage", typeof(string), typeof(ExceedraRadPivotGrid), new PropertyMetadata(default(string)));

        /// <summary>
        /// Defines a message shown to a user when the pivot grid is empty (doesn't have any rows / fields)
        /// </summary>
        public string IsEmptyMessage
        {
            get { return (string)GetValue(IsEmptyMessageProperty); }
            set { SetValue(IsEmptyMessageProperty, value); }
        }

        #endregion

        public PivotExportModel GetPivotGrid()
        {
            return PivotGrid.GenerateExport();
        }
    }
}